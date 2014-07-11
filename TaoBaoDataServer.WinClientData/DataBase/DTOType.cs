using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TaoBaoDataServer.WinClientData
{
    internal class DTOType
    {
        public string Name { get; set; }
        public Guid TypeGuid { get; private set; }
        //  public Type Type { get; set; }
        public FastCreateInstanceHandler Handler { get; set; }
        public DateTime LastCallDateTime { get; set; }
        public int CallCount { get; set; }

        public SortedList<string, DTOProperty> Properties { get; private set; }

        public DTOType()
        {
            Properties = new SortedList<string, DTOProperty>();
        }

        public DTOType(Type type, FastCreateInstanceHandler handler)
            : this()
        {
            // Type = type;
            Name = type.Name;
            TypeGuid = type.GUID;
            Handler = handler;
            LastCallDateTime = DateTime.Now;
            CallCount = 1;
        }

        public override string ToString()
        {
            return Name;
        }
    }


    public class DTOTypeCache
    {
        private static readonly object syncLocker = new object();
        private static readonly System.Collections.Generic.SortedDictionary<Guid, DTOType> Cache = new SortedDictionary<Guid, DTOType>();
        private static Thread CleanThread;
        private static readonly object padlock = new object();
        private static volatile bool stopping = false;
        ~DTOTypeCache()
        {
            Stop();
            if (CleanThread != null)
            {
                CleanThread.Abort();
                CleanThread = null;
            }
        }

        static DTOTypeCache()
        {
            CleanThread = new Thread(ThreadedWork)
            {
                IsBackground = true,
                Name = "DTOTypeCleanThread",
                Priority = ThreadPriority.Lowest
            };
            CleanThread.Start();
        }

        public static void Stop() // Could make this Dispose if you want 
        {
            stopping = true;
            lock (padlock)
            {
                Monitor.Pulse(padlock);
            }
        }
        private static void ThreadedWork()
        {
            while (!stopping)
            {
                CleanCache();
                lock (padlock)
                {
                    Monitor.Wait(padlock, TimeSpan.FromMinutes(10));
                }
            }
        }
        /// <summary>
        /// 定时清除无效的缓存
        /// </summary>
        private static void CleanCache()
        {
            lock (syncLocker)
            {
                var oldCache = Cache.Values.OrderBy(o => o.LastCallDateTime)
                    .ThenBy(o => o.CallCount)
                    .Take(20)
                    .Where(o => (DateTime.Now - o.LastCallDateTime).TotalMinutes > 5).ToArray();

                foreach (var type in oldCache)
                {
                    Cache.Remove(type.TypeGuid);
                }
            }

        }

        internal static bool ContainKeys(Type type)
        {
            if (type == null)
                return false;
            return Cache.ContainsKey(type.GUID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static FastCreateInstanceHandler TryGetValue(Type type)
        {
            if (type == null)
                return null;
            DTOType dtoType;
            if (!Cache.TryGetValue(type.GUID, out dtoType))
                return null;

            return dtoType.Handler;
        }

        internal static FastCreateInstanceHandler GetValue(Type type)
        {
            if (type == null)
                return null;
            return Cache[type.GUID].Handler;
        }

        internal static DTOType GetDTOType(Type type)
        {
            if (type == null)
                return null;
            DTOType dtoType;
            if (!Cache.TryGetValue(type.GUID, out dtoType))
                return null;

            dtoType.LastCallDateTime = DateTime.Now;
            unchecked
            {
                dtoType.CallCount++;
            }
            return dtoType;
        }

        internal static void Add(Type type, FastCreateInstanceHandler handler)
        {
            if (type == null)
                return;
            lock (syncLocker)
            {
                Cache.Add(type.GUID, new DTOType(type, handler));
            }
        }
        internal static bool Remove(Type type)
        {
            if (type == null)
                return false;
            lock (syncLocker)
            {
                return Cache.Remove(type.GUID);
            }
        }
    }

}

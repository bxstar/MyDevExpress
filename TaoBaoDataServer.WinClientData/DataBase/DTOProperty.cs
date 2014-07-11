using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TaoBaoDataServer.WinClientData
{

    public class DTOProperty
    {
        public string Name { get; private set; }
        public PropertyInfo Info { get; private set; }
        public FastPropertySetHandler Handler { get; private set; }
        public FastPropertyGetHandler GetHandler { get; private set; }

        public DTOProperty()
        {
        }

        public DTOProperty(PropertyInfo info, FastPropertySetHandler handler)
            : this()
        {
            Info = info;
            Handler = handler;
            Name = info.Name;
        }

        public DTOProperty(PropertyInfo info, FastPropertySetHandler handler, FastPropertyGetHandler getHandler)
            : this(info, handler)
        {
            GetHandler = getHandler;
        }
    }
}

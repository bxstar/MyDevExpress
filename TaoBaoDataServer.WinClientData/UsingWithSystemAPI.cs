using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;

namespace TaoBaoDataServer.WinClientData
{
    public class UsingWithSystemAPI
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 验证当前程序是否是出于活动状态
        /// processName是进程的名称，比如淘快车目前就是Taokuaiche
        /// </summary>
        /// <returns></returns>
        bool checkActive(string processName)
        {
            bool returnInfo = false;
            foreach (System.Diagnostics.Process myproc in System.Diagnostics.Process.GetProcessesByName(processName))
            {
                if (myproc.MainWindowHandle.ToInt32() == GetForegroundWindow().ToInt32())
                {
                    returnInfo = true;
                    break;
                }
            }
            return returnInfo;
        }

        /* All of the code below can optionally be put in a class library and reused with all your applications. */

        /*
        *	SingeInstance
        *
        *	This is where the magic happens.
        *
        *	Start() tries to create a mutex.
        *	If it detects that another instance is already using the mutex, then it returns FALSE.
        *	Otherwise it returns TRUE.
        *	(Notice that a GUID is used for the mutex name, which is a little better than using the application name.)
        *
        *	If another instance is detected, then you can use ShowFirstInstance() to show it
        *	(which will work as long as you override WndProc as shown above).
        *
        *	ShowFirstInstance() broadcasts a message to all windows.
        *	The message is WM_SHOWFIRSTINSTANCE.
        *	(Notice that a GUID is used for WM_SHOWFIRSTINSTANCE.
        *	That allows you to reuse this code in multiple applications without getting
        *	strange results when you run them all at the same time.)
        *
        */

        // using System.Threading;

        static public class SingleInstance
        {
            public static readonly int WM_SHOWFIRSTINSTANCE =
                WinApi.RegisterWindowMessage("WM_SHOWFIRSTINSTANCE|{0}", ProgramInfo.AssemblyGuid);
            static Mutex mutex;
            static public bool Start()
            {
                bool onlyInstance = false;
                string mutexName = String.Format("Local\\{0}", ProgramInfo.AssemblyGuid);

                // if you want your app to be limited to a single instance
                // across ALL SESSIONS (multiple users & terminal services), then use the following line instead:
                // string mutexName = String.Format("Global\\{0}", ProgramInfo.AssemblyGuid);

                mutex = new Mutex(true, mutexName, out onlyInstance);
                return onlyInstance;
            }
            static public void ShowFirstInstance()
            {
                WinApi.PostMessage(
                    (IntPtr)WinApi.HWND_BROADCAST,
                    WM_SHOWFIRSTINSTANCE,
                    IntPtr.Zero,
                    IntPtr.Zero);
            }
            static public void Stop()
            {
                try
                {
                    mutex.ReleaseMutex();
                }
                catch { }
            }
        }

        /*
        *	WinApi
        *
        *	This class is just a wrapper for your various WinApi functions.
        *
        *	In this sample only the bare essentials are included.
        *	In my own WinApi class, I have all the WinApi functions that any
        *	of my applications would ever need.
        *
        */

        // using System.Runtime.InteropServices;

        static public class WinApi
        {
            [DllImport("user32")]
            public static extern int RegisterWindowMessage(string message);

            public static int RegisterWindowMessage(string format, params object[] args)
            {
                string message = String.Format(format, args);
                return RegisterWindowMessage(message);
            }

            public const int HWND_BROADCAST = 0xffff;
            public const int SW_SHOWNORMAL = 1;

            [DllImport("user32")]
            public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

            [DllImportAttribute("user32.dll")]
            public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            [DllImportAttribute("user32.dll")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            public static void ShowToFront(IntPtr window)
            {
                ShowWindow(window, SW_SHOWNORMAL);
                SetForegroundWindow(window);
            }
        }

        /*
        *	ProgramInfo
        *
        *	This class is just for getting information about the application.
        *	Each assembly has a GUID, and that GUID is useful to us in this application,
        *	so the most important thing in this class is the AssemblyGuid property.
        *
        *	GetEntryAssembly() is used instead of GetExecutingAssembly(), so that you
        *	can put this code into a class library and still get the results you expect.
        *	(Otherwise it would return info on the DLL assembly instead of your application.)
        */

        // using System.Reflection;

        static public class ProgramInfo
        {
            static public string AssemblyGuid
            {
                get
                {
                    object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false);
                    if (attributes.Length == 0)
                    {
                        return String.Empty;
                    }
                    return ((System.Runtime.InteropServices.GuidAttribute)attributes[0]).Value;
                }
            }
            static public string AssemblyTitle
            {
                get
                {
                    object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                    if (attributes.Length > 0)
                    {
                        AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                        if (titleAttribute.Title != "")
                        {
                            return titleAttribute.Title;
                        }
                    }
                    return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().CodeBase);
                }
            }
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetGetCookie(string lpszUrlName, string lbszCookieName, StringBuilder lpszCookieData, ref int lpdwSize);

        [DllImport("kernel32.dll")]
        public static extern Int32 GetLastError();

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);


        [DllImport("User32.DLL")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImport("User32.DLL")]
        public static extern bool ReleaseCapture();
        [DllImport("User32.DLL")]
        public static extern bool SetCapture();
        public const uint WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 61456;
        public const int HTCAPTION = 2;

        private const uint WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_STYLE = (-16);
        private const int GWL_EXSTYLE = (-20);
        private const int LWA_ALPHA = 0x2;
        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong(
        IntPtr hwnd,
        int nIndex,
        uint dwNewLong
        );

        [DllImport("user32", EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLong(
        IntPtr hwnd,
        int nIndex
        );

        [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
        private static extern int SetLayeredWindowAttributes(
        IntPtr hwnd,
        int crKey,
        int bAlpha,
        int dwFlags
        );
        /// <summary>
        /// 使窗体/控件对象有鼠标穿透功能
        /// </summary>
        public static void CanPenetrate(IntPtr handle)
        {
            try
            {
                uint intExTemp = GetWindowLong(handle, GWL_EXSTYLE);
                uint oldGWLEx = SetWindowLong(handle, GWL_EXSTYLE, WS_EX_TRANSPARENT | WS_EX_LAYERED);
                SetLayeredWindowAttributes(handle, 0, 100, LWA_ALPHA);
            }
            catch { }
        }
    }
}

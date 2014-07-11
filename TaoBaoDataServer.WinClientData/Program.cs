using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TaoBaoDataServer.WinClientData
{
	static class Program
	{
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main()
		{
			log4net.Config.XmlConfigurator.Configure();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);


            //Application.Run(new FrmReport());
            Application.Run(new MainForm());
			//Application.Run(new FrmWord());
			//Application.Run(new FrmFindWord());
            //Application.Run(new FrmAddKeyword());
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace TaoBaoDataServer.WinClientData
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.Text = Config.App_Title;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            FrmOutPut frmOutPut = new FrmOutPut();
            FrmDataBaseTest frmDB = new FrmDataBaseTest();
            FrmAPITest frmContent1 = new FrmAPITest(frmOutPut, this);
            FrmFindWordByAdgroup frmContent2 = new FrmFindWordByAdgroup(frmOutPut);
            FrmFindWord frmContent3 = new FrmFindWord();
            FrmWord frmContent4 = new FrmWord(frmOutPut);
            FrmReport frmContent5 = new FrmReport();
            Form1 frmTool = new Form1();

            frmOutPut.Show(dockPanel1, DockState.DockLeftAutoHide);
            frmContent1.Show(dockPanel1);
            frmContent2.Show(dockPanel1);
            frmContent3.Show(dockPanel1);
            frmContent4.Show(dockPanel1);
            frmContent5.Show(dockPanel1);
            frmTool.Show(dockPanel1);
            frmDB.Show(dockPanel1);
            frmContent4.BringToFront();
        }

        public void SetMainTitle(string title)
        {
            this.Text = title;
        }
    }
}

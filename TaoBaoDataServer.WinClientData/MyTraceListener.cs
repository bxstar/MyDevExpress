using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace TaoBaoDataServer.WinClientData
{
    /// <summary>
    /// http://www.cnblogs.com/swnuwangyun/archive/2007/03/24/686289.html
    /// </summary>
    public class MyTraceListener : TraceListener
    {
        private RichTextBox _richTextBox = null;

        public MyTraceListener(RichTextBox richTextBox)
        {
            this._richTextBox = richTextBox;
            this.NeedIndent = true;
        }

        private delegate void WriteDelegate(string message);
        private void WriteImpl(string message)
        {
            if (this.NeedIndent)
            {
                this.WriteIndent();
                this.NeedIndent = true;
            }
            this._richTextBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  " + message);
            this._richTextBox.Select(this._richTextBox.Text.Length, 0);
            this._richTextBox.ScrollToCaret();
            //必须加上，否则会在方法完全执行完之后呈现UI，不能及时看到日志输出
            Application.DoEvents();

            //string sInput = tbInput.Text.Trim() + '\r' + '\n';
            //tbInput.Clear();
            //tbInput.Focus();
            //rtbOutput.AppendText(sInput);
            //rtbOutput.SelectionStart = rtbOutput.Text.Length;
            //rtbOutput.ScrollToCaret();
        }

        public override void Write(string message)
        {
            //This is for thread safety
            this._richTextBox.Invoke(new WriteDelegate(this.WriteImpl), new object[] { message });
        }

        public override void WriteLine(string message)
        {
            this.Write(message + Environment.NewLine);
        }
    }
}

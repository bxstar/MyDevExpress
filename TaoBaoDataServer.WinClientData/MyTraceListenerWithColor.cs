/*
http://www.cnblogs.com/swnuwangyun/archive/2007/03/26/688550.html
调用示例：
for (int i = 0; i < 10; i++)
{
    Debug.WriteLine("aa");
    Thread.Sleep(200);
}

Debug.IndentLevel = 1;
Debug.WriteLine("bb"); 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace TaoBaoDataServer.WinClientData
{
    /// <summary>
    /// TraceListener自定义
    /// </summary>
    public class MyTraceListenerWithColor : TraceListener
    {
        private RichTextBox _richTextBox = null;

        public MyTraceListenerWithColor(RichTextBox richTextBox)
        {
            this._richTextBox = richTextBox;
        }

        private delegate void WriteDelegate(string message);
        private void WriteImpl(string message)
        {
            if (this.NeedIndent)
            {
                this.WriteIndent();
                this.NeedIndent = true;
            }
            Color color = new Color();
            switch (this.IndentLevel)
            {
                case 0:
                    color = Color.FromArgb(255, 0, 0);
                    break;
                case 1:
                    color = Color.FromArgb(255, 255, 255);
                    break;
                case 2:
                    color = Color.FromArgb(100, 150, 0);
                    break;
                case 3:
                    color = Color.FromArgb(150, 100, 0);
                    break;
                case 4:
                    color = Color.FromArgb(200, 50, 0);
                    break;
                default:
                    color = Color.FromArgb(250, 0, 0);
                    break;
            }
            this._richTextBox.Select(this._richTextBox.Text.Length, 0);
            this._richTextBox.SelectionBackColor = color;
            this._richTextBox.AppendText(message);
            this._richTextBox.Select(this._richTextBox.Text.Length, 0);
            this._richTextBox.ScrollToCaret();

            //必须加上，否则会在方法完全执行完之后呈现UI，不能及时看到日志输出
            Application.DoEvents();
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

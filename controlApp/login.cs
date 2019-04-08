using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace controlApp
{
    public partial class login : Form
    {
        private string SPort = "";
        private SerialPort isp = null;
        public login()
        {
            InitializeComponent();
        }
        private void getDeviceInfo()
        {
            string []names = SerialPort.GetPortNames();
            if (names.Length == 0)
            {
                return;                
            }
            foreach(var name in names)
            {
                SerialPort sp = new SerialPort(name, 115200);
                try
                {
                    toolStripStatusLabel3.Text = "当前串口:" + name;
                    sp.Open();
                    sp.Write("begin");
                    Thread.Sleep(1000);
                    if(sp.BytesToRead != 0)
                    {
                        byte[] bytebuff = new byte[sp.BytesToRead+1];
                        sp.Read(bytebuff, 0, sp.BytesToRead);
                        //if(bytebuff[0] == 0x01)
                        {
                            isp = sp;
                            SPort = name;
                            return;
                        }
                    }
                    sp.Close();
                }
                catch (Exception)
                {                    
                }
            }                           
        }
        private void login_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(0, 25, 47);
            statusStrip1.BackColor = Color.FromArgb(0, 25, 47);
            label1.ForeColor = Color.FromArgb(4, 254, 255);
            label2.ForeColor = Color.FromArgb(4, 254, 255);
            label3.ForeColor = Color.FromArgb(4, 254, 255);

            timer1.Start();
            getDeviceInfo();         
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(SPort != "")
            {
                timer1.Stop();
                Form1 ff = new Form1(isp);
                this.Hide();
                ff.ShowDialog();
                this.Close();
            }
            else
            {
                getDeviceInfo();
            }
        }
    }
}

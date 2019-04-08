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
    public partial class Form1 : Form
    {
        public const byte RUNSPEED = 0x00;
        public const byte RUNMODE = 0x01;
        public const byte STOPSPEED = 0x02;
        public const byte RUNMAXCOUNT = 0x03;
        public const byte RETREATMAXCOUNT = 0x04;
        public const byte RETREATSPEED = 0x05;
        public const byte INCLINE = 0x06;
        public Color sysBlackColor = Color.FromArgb(4, 254, 255);
        public Color picBlackColor;
        private bool SerialConnectStatus = false;
        private SerialPort isp = new SerialPort();
        private serialData serialControl = null;
        public Form1()
        {
            InitializeComponent();
        }
        public Form1(SerialPort sport)
        {
            InitializeComponent();
            isp = sport;
            SerialConnectStatus = true;
        }        
        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(0, 25, 47);
            label1.ForeColor = sysBlackColor;
            label6.ForeColor = sysBlackColor;
            label7.ForeColor = sysBlackColor;
            label8.ForeColor = sysBlackColor;


            picBlackColor = picRunMode1.BackColor;
            //label9.ForeColor = Color.FromArgb(4, 254, 255);
            //label10.ForeColor = Color.FromArgb(4, 254, 255);
            //label11.ForeColor = Color.FromArgb(4, 254, 255);
            //label18.ForeColor = Color.FromArgb(4, 254, 255);

            //get serialData object
            serialControl = new serialData(isp);
            serialControl.initSerailData();
            // start status timer
            timerCheckStatus.Start();

            //get data for device
            button1_Click(this,null);

        }
        private void showMessage(string str)
        {
            richTextBox1.AppendText(str + "\r\n");
        }
        private void pictureBox4_Paint(object sender, PaintEventArgs e)
        {
            if (SerialConnectStatus == false)
            {
                Rectangle ret = new Rectangle(210, 150, 10, 10);
                e.Graphics.DrawRectangle(new Pen(Color.Red, 20), ret);
            }
            else
            {
                Rectangle ret = new Rectangle(210, 150, 10, 10);
                e.Graphics.DrawRectangle(new Pen(Color.Blue, 20), ret);
            }
        }

        private void timerCheckStatus_Tick(object sender, EventArgs e)
        {
            try
            {
                if (isp.IsOpen == false)
                {
                    SerialConnectStatus = false;
                    pictureBox4_Paint(this, null);
                }
            }
            catch (Exception)
            {
            }

        }
        public void testFace()
        {
            picRunMode1.BackColor = Color.Blue;
        }
        /// <summary>
        /// 显示界面信息，通过一个list
        /// </summary>
        /// <param name="temp"></param>
        private void showFaceList(List<int> temp)
        {
            try
            {
                //速度，
                tbSpeed.Text = (temp[0] / 10).ToString();
                //模式
                if (temp[1] == 0)
                {
                    picRunMode1.BackColor = Color.Blue;
                    picRunMode2.BackColor = picBlackColor;
                }
                else
                {
                    picRunMode1.BackColor = picBlackColor;
                    picRunMode2.BackColor = Color.Blue;
                }
                //静止数据区间
                textBox1.Text = ((temp[2] & 0xFF00) >> 8).ToString();
                textBox2.Text = (temp[2] & 0x00FF).ToString();

                //前进最大角度
                textBox8.Text = temp[3].ToString();
                //后退最大角度
                textBox9.Text = temp[4].ToString();
                //后退最大速度
                textBox14.Text = temp[5].ToString();
                //计算总里程

                //计算卡路里
            }
            catch (Exception)
            {
                                
            }
            

        }
        private void ansySerialDataAll(byte[] revData)
        {
            List<int> tempShowFace = new List<int>();
            int dataCount = revData[2];
            for(int i = 3; i < dataCount + 3; i+=2 )
            {
                tempShowFace.Add(revData[i] << 8 | revData[i + 1]);
            }
            showFaceList(tempShowFace);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // 刷新界面，
            try
            {
                if (serialControl.getRefreshData() == true)
                {
                    //获取和解析接收到的数据
                    byte[] revData = serialControl.getSerialAllData();
                    //解析数据
                    ansySerialDataAll(revData);
                }
                else
                {
                    showMessage("初始化数据发送失败，请检查设备链接");
                }
            }
            catch (Exception)
            {
                showMessage("初始化数据发送失败，请检查设备链接");
            }
            
        }
        /// <summary>
        /// 设置左倾角度值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            byte data1;
            byte data2;
            try
            {
                data1 = Convert.ToByte(textBox10.Text);
                data2 = Convert.ToByte(textBox11.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("输入的参数错误");
                return;
            }

            if (serialControl.setDataCmd(INCLINE, data1, data2))
            {
                MessageBox.Show("设置左右倾斜成功");
                button2.BackColor = Color.Blue;
            }
            else
            {
                MessageBox.Show("设置左右倾斜失败");
                button2.BackColor = Color.Red;
            }
        }
        /// <summary>
        /// picturebox 1 check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picRunMode1_Click(object sender, EventArgs e)
        {
            //check run mode 
            //1.5km
            //15
            byte data1 = (15 & 0xFF00) >> 8;
            byte data2 = (15 & 0x00FF);
            picRunMode1.BackColor = picBlackColor;
            picRunMode2.BackColor = picBlackColor;
            if (serialControl.setDataCmd(RUNMODE, data1, data2))
            {
                MessageBox.Show("设置VR模式成功");
                picRunMode1.BackColor = Color.Blue;
            }
            else
            {
                MessageBox.Show("设置VR模式失败");
                picRunMode1.BackColor = Color.Red;
            }
        }
        /// <summary>
        /// picturebox 2 check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picRunMode2_Click(object sender, EventArgs e)
        {
            //check run mode  
            //6.0km
            byte data1 = (60 & 0xFF00) >> 8;
            byte data2 = (60 & 0x00FF);
            picRunMode1.BackColor = picBlackColor;
            picRunMode2.BackColor = picBlackColor;
            if (serialControl.setDataCmd(RUNMODE, data1, data2))
            {
                MessageBox.Show("设置PC模式成功");
                picRunMode2.BackColor = Color.Blue;
            }
            else
            {
                MessageBox.Show("设置PC模式失败");
                picRunMode2.BackColor = Color.Red;
            }
        }
        /// <summary>
        /// 设置静止区间的值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox8_Click(object sender, EventArgs e)
        {
            byte data1;
            byte data2;
            try
            {
                data1 = Convert.ToByte(textBox1.Text);
                data2 = Convert.ToByte(textBox2.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("输入的参数错误");
                return;       
            }

            if (serialControl.setDataCmd(STOPSPEED, data1, data2))
            {
                MessageBox.Show("设置静止区间成功");
                pictureBox8.BackColor = Color.Blue;
            }
            else
            {
                MessageBox.Show("设置静止区间失败");
                pictureBox8.BackColor = Color.Red;
            }
        }
        /// <summary>
        /// 设置最大速度，最小速度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox11_Click(object sender, EventArgs e)
        {
            byte data1;
            byte data2;
            try
            {
                int data = (int)Convert.ToDouble(textBox8.Text)*10;
                data1 = (byte)((data & 0xFF00) >> 8);
                data2 = (byte)(data & 0x00FF);
            }
            catch (Exception)
            {
                MessageBox.Show("输入的参数错误");
                return;
            }

            if (serialControl.setDataCmd(RUNMAXCOUNT, data1, data2))
            {
                MessageBox.Show("设置最大速度成功");
                pictureBox11.BackColor = Color.Blue;
            }
            else
            {
                MessageBox.Show("设置最大速度失败");
                pictureBox11.BackColor = Color.Red;
            }
        }
        /// <summary>
        /// 设置后退的角度和速度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            byte data1;
            byte data2;
            try
            {
                data1 = Convert.ToByte(textBox9.Text);
                double dd = Convert.ToDouble(textBox14.Text);
                data2 = (byte)(dd * 10);
            }
            catch (Exception)
            {
                MessageBox.Show("输入的参数错误");
                return;
            }

            if (serialControl.setDataCmd(RETREATSPEED, data1, data2))
            {
                MessageBox.Show("设置后退设置成功");
                button3.BackColor = Color.Blue;
            }
            else
            {
                MessageBox.Show("设置后退设置失败");
                button3.BackColor = Color.Red;
            }
        }

        private void 设置ccidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setDeviceCCid sdc = new setDeviceCCid();
            sdc.Show();
            while(sdc.ccid.Length != 0)
            {
                isp.Write("ccid:" + sdc.ccid);
                Thread.Sleep(1000);
                if(isp.BytesToRead > 5)
                {
                    sdc.setCmdRpy(true);
                }
                else
                {
                    sdc.setCmdRpy(false);
                }
            }
            //sdc.ccid
        }
    }
}

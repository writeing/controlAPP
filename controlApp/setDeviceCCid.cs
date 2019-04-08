using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace controlApp
{
    public partial class setDeviceCCid : Form
    {
        public string ccid;

        public setDeviceCCid()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ccid = textBox1.Text;                                
            }
            catch (Exception)
            {                
            }
            
        }
        public void setCmdRpy(bool status)
        {
            if(status)
            {
                button1.BackColor = Color.Blue;
            }
            else
            {
                button1.BackColor = Color.Red;
            }
        }
    }
}

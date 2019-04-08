using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace controlApp
{
    class serialData
    {
        public SerialPort isp;
        public byte[] sendBuff = new byte[20];
        public serialData(SerialPort isp)
        {
            this.isp = isp;                  
        }
        /// <summary>
        /// 初始化要发送的数据基础格式
        /// </summary>
        public void initSerailData()
        {
            sendBuff[0] = 0x01;
            sendBuff[1] = 0x03;
            sendBuff[6] = 0x00;
            sendBuff[7] = 0x00;

        }
        /// <summary>
        /// 发送数据到设备，这个函数
        /// </summary>
        /// <returns></returns>
        private bool _sendSerialData()
        {
            try
            {
                isp.Write(sendBuff, 0, 8);
                Thread.Sleep(500);           
            }
            catch (Exception)
            {
                return false;            
            }
            return true;
        }

        /// <summary>
        /// 刷新界面信息
        /// </summary>
        /// <returns></returns>
        public bool getRefreshData()
        {            
            sendBuff[1] = 0x03;
            sendBuff[2] = 0x00;
            sendBuff[3] = 0x07;
            sendBuff[4] = 0x00;
            sendBuff[5] = 0x00;
            return _sendSerialData();
        }
        /// <summary>
        /// 发送数据，根据寄存器和数字位
        /// </summary>
        /// <param name="reg2"></param>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <returns></returns>
        public bool setDataCmd(byte reg2,byte data1, byte data2)
        {
            int i = 1;
            sendBuff[i++] = 0x03;
            sendBuff[i++] = 0x00;
            sendBuff[i++] = reg2;
            sendBuff[i++] = data1;
            sendBuff[i++] = data2;
            if(_sendSerialData() == false)
            {
                return false;
            }
            int data = 0;
            if(getSerialDataForDevice(ref data) > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 检查接收到的数据，是否正常
        /// </summary>
        /// <param name="readbuff"></param>
        /// <returns></returns>
        private bool checkBuff(byte[] readbuff)
        {
            if(readbuff[0] == 0x01 && readbuff[1] == 0x03)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }
        /// <summary>
        /// 获取设备的数据，显示单个数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int getSerialDataForDevice(ref int data)
        {
            int len = isp.BytesToRead;
            if (len > 0)
            {
                try
                {
                    byte[] readBuff = new byte[len];
                    int readLen = isp.Read(readBuff, 0, len);
                    if(checkBuff(readBuff))
                    {
                        data = readBuff[3] << 8 | readBuff[4];
                        return readLen;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取刷新的所有数据
        /// </summary>
        /// <returns></returns>
        public byte[] getSerialAllData()
        {
            int len = isp.BytesToRead;
            if (len > 0)
            {
                try
                {
                    byte[] readBuff = new byte[len];
                    int readLen = isp.Read(readBuff, 0, len);
                    if (checkBuff(readBuff))
                    {
                        return readBuff;
                    }
                    else
                    {
                        return null;
                    }                    
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}

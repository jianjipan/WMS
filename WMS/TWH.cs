using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace WMS
{
    class TWH
    {
        public delegate void TWHThreadCallBackDelegate(byte[] msg);
        public static Socket TWHSocket;
       // Thread t;
        public TWHThreadCallBackDelegate callback;       
        
        public static byte[] ExecuteState = new byte[4];    //执行状态信息   
       // private volatile bool CanStop = false;    //当客户端断开连接时，置位true


        public static int OutlibCount = 0;   //出库次数
        public static int InlibCount = 0;    //入库次数

        public static bool IsSendFlag = true;    //是否发送socket数据的开关

        public TWH(Socket s)
        {
            //构造函数
            TWHSocket = s;
                 
        }            
//接收立库发来的消息      
        public void recdata()   //接收消息
        {
            while (true)
            {
                byte[] buffer = new byte[21];  //数据接收容器
                try
                {
                    int r = TWHSocket.Receive(buffer);
                    //ModifyTxt.PrintReceiveTxt(buffer);
                    if ((buffer[0] == 0x05) && (buffer[1] == 0x0A) && (buffer[2] == 0x05) && (buffer[3] == 0x0A))  //头校验通过
                    {                     
                            if ((buffer[5] == 0x02) && (buffer[6] == 0x0B) && (buffer[7] == 0x00) && (buffer[8] == 0x01))  //执行状态回复信息 
                            {
                                ExecuteState[0] = 0x07;
                                ExecuteState[1] = buffer[20];
                                callback(ExecuteState);
                                if (ExecuteState[1] == 0x01)
                                {
                                    
                                }
                                if (ExecuteState[1] == 0x02)
                                {
                                    test1.OutlibCompleted = true;
                                    CJClass.OutlibCompleted = true;
                                }
                                if (ExecuteState[1] == 0x03)
                                {
                                    CJClass.InlibAssemblyCompleted = true;
                                    
                                }
                                if (ExecuteState[1] == 0x04)
                                {
                                    test1.AssemblyOutlibCompleted = true;
                                    CJClass.OutlibAssemblyCompleted = true;
                                }
                               Array.Clear(buffer, 0, 21);                               
                            }                       
                    }
                }
                catch
                {
                    WMS.MainWindow.InterruptMsg(6, "立库软件断开连接");
                 //   CanStop = true;
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    TWHSocket.Dispose();
                    TWHSocket.Close();
                    MessageBox.Show("立库软件断开连接");
                }
            }
        }
//向立库发送消息
        public static void sendRequestcmd(byte[] cmdFlag)
        {
            if (IsSendFlag==true)
            {
                try
                {
                    CMD.sendDataE0(cmdFlag, TWHSocket);
                }
                catch
                {
                    MessageBox.Show("向立库软件发送命令失败，请检查通讯是否断开");
                }
            }
        }
//心跳
        private void SendHeartBeatcmd()
        {           
                byte[] cmd = new byte[4];
                cmd[0] = 0xFF;
                cmd[1] = 0x29;
                cmd[2] = 0x00;
                cmd[3] = 0x00;
                CMD.sendDataE0(cmd, TWHSocket);//发送判断心跳的命令         
        }      
    }
}

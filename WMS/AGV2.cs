using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace WMS
{
    class AGV2
    {         
        public delegate void PAGV2ThreadCallBackDelegate(byte[] msg);  //定义一个AGV2的线程回调委托
        public static Socket Pagv2Socket;
        Thread t;
        public PAGV2ThreadCallBackDelegate callback;
        public byte[] HeartBeatByteArray = new byte[2];   //回调的 心跳 字节数组
        public static byte[] PlatfromStateArray=new byte[17];  //回调的 平台主动反馈当前执行状态的 字节数组
      

        private volatile bool CanStop = false;    //当客户端断开连接时，置位true      

        public AGV2(Socket s)
        {
            //构造函数
            Pagv2Socket = s;                    
            t = new Thread(new ThreadStart(HeartBeatcmd));
            t.Start();    
        }
        public void HeartBeatcmd()
        {
            while (!CanStop)
            {
                SendHeartBeatcmd();  //发送心跳命令                              
                Thread.Sleep(TimeSpan.FromSeconds(1));              
            }
            CanStop = false;
        }
//接收AGV2消息               
        public void recdata()  
        {          
            while (true)
            {
                byte[] buffer = new byte[36];  //数据接收容器
                if (Pagv2Socket.Connected)
                {
                    try
                    {
                        int r = Pagv2Socket.Receive(buffer);
                        //   ModifyTxt.PrintReceiveTxt(buffer);   //打印
                        if ((buffer[0] == 0x05) && (buffer[1] == 0x0A) && (buffer[2] == 0x05) && (buffer[3] == 0x0A))  //头校验通过
                        {
                            if (DataCheck.CheckData(buffer))   //校验通过
                            {
                                if ((buffer[5] == 0xFF) && (buffer[6] == 0x30) && (buffer[7] == 0x00) && (buffer[8] == 0x00))   //0x30FF 平台AGV2主动反馈当前状态信息
                                {
                                    PlatfromStateArray[0] = 0x05;
                                    PlatfromStateArray[1] = buffer[20];   //应答状态
                                    PlatfromStateArray[2] = buffer[21];   //平台位置
                                    PlatfromStateArray[3] = buffer[22];  //是否有料盘
                                    PlatfromStateArray[4] = buffer[23];   //作业执行状态
                                    PlatfromStateArray[5] = buffer[24];
                                    PlatfromStateArray[6] = buffer[25];
                                    PlatfromStateArray[7] = buffer[26];
                                    PlatfromStateArray[8] = buffer[27];   //X坐标
                                    PlatfromStateArray[9] = buffer[28];
                                    PlatfromStateArray[10] = buffer[29];
                                    PlatfromStateArray[11] = buffer[30];
                                    PlatfromStateArray[12] = buffer[31];   //Y坐标
                                    PlatfromStateArray[13] = buffer[32];
                                    PlatfromStateArray[14] = buffer[33];
                                    PlatfromStateArray[15] = buffer[34];
                                    PlatfromStateArray[16] = buffer[35];   //theta坐标   
                                    callback(PlatfromStateArray);
                                    Array.Clear(buffer, 0, 36);
                                }
                            }
                        }
                    }
                    catch
                    {
                        WMS.MainWindow.InterruptMsg(2, "AGV2断开连接");
                        CanStop = true;
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        Pagv2Socket.Dispose();
                        Pagv2Socket.Close();
                        MessageBox.Show("AGV2断开连接");
                    }
                }
                else
                {
 
                }
            }            
        }
//向AGV2发送消息
        public static void sendRequestcmd(byte[] cmd)
        {
            try
            {
                CMD.sendDataE0(cmd, Pagv2Socket);
            }
            catch
            {
                MessageBox.Show("向AGV2发送命令失败，请检查通讯是否断开");
            }
        }       
//发送心跳命令
        private void SendHeartBeatcmd()
        {
            try
            {
                byte[] cmd = new byte[4];
                cmd[0] = 0xFF;
                cmd[1] = 0x29;
                cmd[2] = 0x00;
                cmd[3] = 0x00;
                CMD.sendDataE0(cmd, Pagv2Socket);//发送判断心跳的命令   
            }
            catch
            {
 
            }
        }

    }    
}

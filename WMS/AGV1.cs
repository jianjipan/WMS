using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace WMS
{
    class AGV1
    {
        public delegate void PAGV1ThreadCallBackDelegate(byte[] msg);   //定义一个AGV1线程的回调委托
        public static Socket PAGV1Socket;
        Thread t;
        public PAGV1ThreadCallBackDelegate callback;

              
        public static byte[] PlatfromStateArray = new byte[17];   //回调的 平台主动反馈当前执行状态 的数组
        private volatile bool CanStop = false;    //当客户端断开连接时，置位true
       
      
        public AGV1(Socket s)   
        {
            PAGV1Socket = s;           
            t = new Thread(new ThreadStart(HeartBeatcmd));
            t.Start();
        }
        public void HeartBeatcmd()
        {
            while (!CanStop)
            {
                SendHeartBeatcmd();  //发送心跳命令                
                Thread.Sleep(TimeSpan.FromSeconds(1));   //保证大约1s发送一次应答命令                       
            }
            CanStop = false;  //偏于下次使用
        }      
        public void recdata()   //接收消息
        {
            while (true)
            {
                byte[] buffer = new byte[36];  //数据接收容器(共36个数据)
                if (PAGV1Socket.Connected)
                {
                    try
                    {
                        int r = PAGV1Socket.Receive(buffer);
                        //  ModifyTxt.PrintReceiveTxt(buffer);     //打印
                        if ((buffer[0] == 0x05) && (buffer[1] == 0x0A) && (buffer[2] == 0x05) && (buffer[3] == 0x0A))  //头校验通过
                        {
                            if (DataCheck.CheckData(buffer))
                            {
                                if ((buffer[5] == 0xFF) && (buffer[6] == 0x30) && (buffer[7] == 0x00) && (buffer[8] == 0x00))   //0x30FF 平台AGV主动反馈当前状态信息
                                {
                                    PlatfromStateArray[0] = 0x04;
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
                        WMS.MainWindow.InterruptMsg(1,"AGV1断开连接");
                        CanStop = true;
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        PAGV1Socket.Dispose();
                        PAGV1Socket.Close();
                        MessageBox.Show("AGV1断开连接");
                    }
                }
                else
                {
                     
                }

            }
        }
        public static void sendRequestcmd(byte[] cmd)
        {
            try
            {
                CMD.sendDataE0(cmd, PAGV1Socket);
            }
            catch
            {
                MessageBox.Show("向AGV1发送命令失败，请检查通讯是否断开");
            }
        }
        //public static void sendRequestcmd(byte[] cmdcode, byte[] cmdMsg)
        //{
        //    CMD.sendDataE4(cmdcode, cmdMsg, PAGV1Socket);
        //}
        private void SendHeartBeatcmd()
        {
            try
            {
                byte[] cmd = new byte[4];
                cmd[0] = 0xFF;
                cmd[1] = 0x29;
                cmd[2] = 0x00;
                cmd[3] = 0x00;
                CMD.sendDataE0(cmd, PAGV1Socket);//发送判断心跳的命令
            }
            catch
            {
 
            }
        }
         
    }
}

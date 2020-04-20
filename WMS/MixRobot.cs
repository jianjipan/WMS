using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;

namespace WMS
{
    class MixRobot
    {
        public delegate void MixRobotThreadCallBackDelegate(byte[] msg);  //定义一个MixRobot的线程回调委托
        public static Socket MixSocket;
        Thread t;
        public MixRobotThreadCallBackDelegate callback;      
        public static byte[] PlatfromStateArray = new byte[16];  //回调的 平台主动反馈当前执行状态的 字节数组
  
       
        private volatile bool CanStop = false;    //当客户端断开连接时，置位true

        public MixRobot(Socket s)
        {
            //构造函数
            MixSocket = s;
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

        //接收复合发来的消息         
        public void recdata()   //接收消息
        {
            while (true)
            {
                byte[] buffer = new byte[35];  //数据接收容器
                if (MixSocket.Connected)
                {
                    try
                    {
                        int r = MixSocket.Receive(buffer);
                       // ModifyTxt.PrintReceiveTxt(buffer);   //打印
                        if ((buffer[0] == 0x05) && (buffer[1] == 0x0A) && (buffer[2] == 0x05) && (buffer[3] == 0x0A))  //头校验通过
                        {
                            if (DataCheck.CheckData(buffer))   //异或校验通过
                            {
                                if ((buffer[5] == 0xFF) && (buffer[6] == 0x30) && (buffer[7] == 0x00) && (buffer[8] == 0x00))  //0x30FF  复合反馈当前状态信息
                                {
                                    PlatfromStateArray[0] = 0x06;
                                    PlatfromStateArray[1] = buffer[20];   //应答状态
                                    PlatfromStateArray[2] = buffer[21];   //平台位置
                                    PlatfromStateArray[3] = buffer[22];    //动作完成状态
                                    PlatfromStateArray[4] = buffer[23];  //X轴
                                    PlatfromStateArray[5] = buffer[24];
                                    PlatfromStateArray[6] = buffer[25];
                                    PlatfromStateArray[7] = buffer[26];
                                    PlatfromStateArray[8] = buffer[27];  //Y轴
                                    PlatfromStateArray[9] = buffer[28];
                                    PlatfromStateArray[10] = buffer[29];
                                    PlatfromStateArray[11] = buffer[30];
                                    PlatfromStateArray[12] = buffer[31];  //theta轴
                                    PlatfromStateArray[13] = buffer[32];
                                    PlatfromStateArray[14] = buffer[33];
                                    PlatfromStateArray[15] = buffer[34];
                                    callback(PlatfromStateArray);
                                    Array.Clear(buffer, 0, 35);
                                }
                            }
                        }
                    }
                    catch
                    {
                        WMS.MainWindow.InterruptMsg(3, "复合断开连接");
                        CanStop = true;
                        Thread.Sleep(TimeSpan.FromSeconds(1));  //延时1秒
                        MixSocket.Dispose();
                        MixSocket.Close();
                        MessageBox.Show("复合断开连接");
                    }
                }
            }
        }
        //向复合发送请求命令
        public static void sendRequestcmd(byte[] cmd)
        {
            try
            {
                CMD.sendDataT0(cmd, MixSocket);
            }
            catch
            {
                MessageBox.Show("向复合机器人发送命令失败，请检查通讯是否断开");
            }
        }
        //心跳
        private void SendHeartBeatcmd()
        {
            try
            {
                byte[] cmd = new byte[4];
                cmd[0] = 0xFF;
                cmd[1] = 0x29;
                cmd[2] = 0x00;
                cmd[3] = 0x00;
                CMD.sendDataT0(cmd, MixSocket);//发送判断心跳的命令     
            }
            catch
            {
 
            }
        }
    }
    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows.Forms;

namespace WMS
{
    class FAGV
    {
        public delegate void FAGVThreadCallBackDelegate(byte[] msg);  //定义一个FAGV的线程回调委托
        public static Socket FagvSocket;
        Thread t;
        public FAGVThreadCallBackDelegate callback;        
        public static byte[] ExecuteState = new byte[7];   //执行状态       
        private volatile bool CanStop = false;    //当客户端断开连接时，置位true        
        public FAGV(Socket s)
        {
            //构造函数
            FagvSocket = s;
            ConnectServer();
            t = new Thread(new ThreadStart(SendHeatCmd));
            t.Start();
        }
        private void ConnectServer()
        {
            try
            {
                IPAddress ip = IPAddress.Parse("192.168.2.151");
                //IPAddress ip = IPAddress.Parse("192.168.1.201");
                IPEndPoint ipe = new IPEndPoint(ip, 3000);
                //定义一个套接字监听
                FagvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    FagvSocket.Connect(ipe);
                    WMS.MainWindow.InterruptMsg(5, "FAGV已连接");
                }
                catch (Exception e)
                {
                    throw new Exception(e.ToString());
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
        private void SendHeatCmd()
        {
            while (!CanStop)
            {
                CMD.sendDataE3(13000,FagvSocket);  //发送心跳命令                 
                Thread.Sleep(TimeSpan.FromSeconds(1));                
            }
            CanStop = false;
        }
//接收FAGV发来的消息
        public void recdata()   //接收消息
        {
            while (true)
            {
                byte[] buffer = new byte[13];  //数据接收容器
                try
                {
                    int r = FagvSocket.Receive(buffer);
                    //   ModifyTxt.PrintReceiveTxt(buffer);    //打印
                    if (buffer[0] == 0x1B && buffer[1] == 0x27)
                    {
                        if (DataCheck.CheckDataE2(buffer))
                        {
                            ExecuteState[0] = 0x07;
                            ExecuteState[1] = buffer[6]; //表示是执行状态信息
                            ExecuteState[2] = buffer[7];  //终点
                            ExecuteState[3] = buffer[8];  //工作状态
                            ExecuteState[4] = buffer[9];
                            ExecuteState[5] = buffer[10];
                            ExecuteState[6] = buffer[11]; //故障代码
                            callback(ExecuteState);
                            if (ExecuteState[3] == 0x06)   //叉车AGV在安全位置
                            {
                                test1.FAGVInSafeArea = true;
                                CJClass.FAGVInSafeArea = true;
                            }
                            Array.Clear(buffer, 0, 13);                        
                        }
                    }
                }
                catch
                {
                    WMS.MainWindow.InterruptMsg(5, "叉车AGV断开连接");
                    CanStop = true;
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    FagvSocket.Dispose();
                    FagvSocket.Close();
                    MessageBox.Show("叉车AGV断开连接");
                }
            }
        }
//向FAGV发送消息
        public static void sendRequestcmd(byte  cmd)
        {
            try
            {
                CMD.sendDataE2(cmd, FagvSocket);
            }
            catch
            {
                MessageBox.Show("向FAGV发送命令失败，请检查通讯是否断开");
            }
        }     
    }
}

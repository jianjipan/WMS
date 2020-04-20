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
    class TwoRobot
    {
        public delegate void TWORobotThreadCallBackDelegate(byte[] msg);  //定义一个TWORobot的线程回调委托
        Socket TwoSocket;
        Thread t;
        public TWORobotThreadCallBackDelegate callback;
        public static byte[] answerinfo = new byte[8];    //答复数据数组
        public int[] RequestId=new int[6]{0,0,0,0,0,0};
        private volatile bool CanStop = false;    //当客户端断开连接时，置位true

        public static int Count1 = 0;
        public static int Count2 = 0;

        public bool OnlyOne1 = false;
        public bool OnlyOne2 = false;
        
        public TwoRobot(Socket s)       
        {
            //构造函数
            TwoSocket = s;                    
            t = new Thread(new ThreadStart(Sendcmd));
            t.Start();
        }
        public void Sendcmd()
        {
            while(!CanStop)
            {  
                //这里缺一个心跳命令
                if (MainWindow.TwoRobotActionFlag0 == true)
                {                   
                    byte[] cmd=new byte[6];
                    cmd=AcquireCmd(0);
                    CMD.sendDataE1(cmd, TwoSocket);
                    Thread.Sleep(TimeSpan.FromSeconds(3));   //每3秒询问一次
                }
                if (MainWindow.TwoRobotActionFlag1 == true)
                {                   
                    byte[] cmd = new byte[6];
                    cmd = AcquireCmd(1);
                    CMD.sendDataE1(cmd, TwoSocket);
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
                if (MainWindow.TwoRobotActionFlag2 == true)
                {
                    byte[] cmd = new byte[6];
                    cmd = AcquireCmd(2);
                    CMD.sendDataE1(cmd, TwoSocket);
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
                if (MainWindow.TwoRobotActionFlag3 == true)
                {
                    byte[] cmd = new byte[6];
                    cmd = AcquireCmd(3);
                    CMD.sendDataE1(cmd, TwoSocket);
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
                if (MainWindow.TwoRobotActionFlag4 == true)
                {
                    byte[] cmd = new byte[6];
                    cmd = AcquireCmd(4);
                    CMD.sendDataE1(cmd, TwoSocket);
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
                if (MainWindow.TwoRobotActionFlag5 == true)
                {
                    byte[] cmd = new byte[6];
                    cmd = AcquireCmd(5);
                    CMD.sendDataE1(cmd, TwoSocket);
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            }
            CanStop = false;
        }
        public byte[] AcquireCmd(int ActionId)
        {
            byte[] cmd=new byte[6];                       
            cmd[0] = (byte)ActionId;   //动作1：双臂机器人从零件托盘取出零件
            cmd[1] = 0x00;    //立即应答
            RequestId[ActionId]++;
            for (int i = 3; i >= 0; i--)
            {
                cmd[2 + i] = (byte)(RequestId[ActionId] << (8 * i));
            }              
            return cmd;
        }       
        public void recdata()   //接收消息
        {
            while (true)
            {
                byte[] buffer = new byte[27];  //数据接收容器
                if (TwoSocket.Connected)
                {
                    try
                    {
                        int r = TwoSocket.Receive(buffer);
                        //  ModifyTxt.PrintReceiveTxt(buffer);   //打印
                        if ((buffer[0] == 0x05) && (buffer[1] == 0x0A) && (buffer[2] == 0x05) && (buffer[3] == 0x0A))  //头校验通过
                        {
                            if (DataCheck.CheckData(buffer))   //校验通过
                            {
                                if ((buffer[5] == 0x00) && (buffer[6] == 0x10) && (buffer[7] == 0x00) && (buffer[8] == 0x00))   //0x1000 双臂机器人请求命令回复信息
                                {
                                    answerinfo[0] = 0x00;
                                    answerinfo[1] = buffer[20];   //启动信号
                                    answerinfo[2] = buffer[21];   //请求动作是否完成
                                    answerinfo[3] = buffer[22];
                                    answerinfo[4] = buffer[23];
                                    answerinfo[5] = buffer[24];
                                    answerinfo[6] = buffer[25];    //前四位为应答号（与请求号一致）
                                    answerinfo[7] = buffer[26];    //执行状态  0：正常  1：异常
                                    callback(answerinfo);   //回调                             
                                    if (buffer[20] == 0x00 && (buffer[21] == 0x00))   //动作0完成
                                    {
                                        MainWindow.TwoRobotActionFlag0 = false;   //停止发送动作0消息
                                        test1.TwoRobotActionComplete0 = true;     //通知自动化类test1动作0完成
                                        RequestId[0] = 0;
                                    }
                                    if (buffer[20] == 0x01 && buffer[21] == 0x00)   //动作1完成
                                    {
                                        MainWindow.TwoRobotActionFlag1 = false;   //停止动作1：装配的发送
                                        RequestId[1] = 0;
                                        test1.AssemblyCompleted = true;   //装配完成
                                    }
                                    if (buffer[20] == 0x02 && buffer[21] == 0x00)   //动作2完成
                                    {
                                        MainWindow.TwoRobotActionFlag2 = false;                                       
                                        RequestId[2] = 0;
                                        test1.TwoRobotActionComplete2 = true;
                                    }
                                    if (buffer[20] == 0x03 && buffer[21] == 0x00)   //动作3完成
                                    {
                                        MainWindow.TwoRobotActionFlag3 = false;
                                        RequestId[3] = 0;
                                        CJClass.TwoRobotActionComplete3 = true;    //通知拆解类，双臂动作3完成：从成品料盘取出成品
                                    }
                                    if (buffer[20] == 0x04 && buffer[21] == 0x00)   //动作4完成
                                    {
                                        MainWindow.TwoRobotActionFlag4 = false;
                                        RequestId[4] = 0;
                                        CJClass.TwoRobotActionComplete4 = true;     //通知拆解类，双臂动作4完成：拆解
                                    }
                                    if (buffer[20] == 0x05 && buffer[21] == 0x00)   //动作5完成
                                    {
                                        MainWindow.TwoRobotActionFlag5 = false;
                                        RequestId[5] = 0;
                                        CJClass.TwoRobotActionComplete5 = true;     //通知拆解类，双臂动作5完成：将零件放到零件料盘
                                    }
                                    Array.Clear(buffer, 0, 27);
                                }
                            }
                        }
                    }
                    catch
                    {
                        WMS.MainWindow.InterruptMsg(4, "双臂断开连接");
                        CanStop = true;
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        TwoSocket.Dispose();
                        TwoSocket.Close();
                        MessageBox.Show("双臂断开连接");
                    }
                }
            }
        }      
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;

namespace WMS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public delegate void WriteWrongMsg(int id,string msg);
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            InterruptMsg = InterruptMsgShow;
        }
        public static WriteWrongMsg InterruptMsg;
       
        public static bool AutoMode1 = false;   //自动模式1：装配
        public static bool AutoMode2 = false;    //自动模式2：拆解
        public static bool PrintLog = false;    //加不加打印日志
       
        public static bool TwoRobotActionFlag0 = false;   //双臂机器人动作0：  双臂机器人从零件托盘取出零件
        public static bool TwoRobotActionFlag1 = false;    //双臂机器人动作1： 双臂机器人装配
        public static bool TwoRobotActionFlag2 = false;    //双臂机器人动作2：  双臂机器人将成品放到成品托盘
        public static bool TwoRobotActionFlag3 = false;    //双臂机器人动作3：  双臂机器人从成品托盘取出成品
        public static bool TwoRobotActionFlag4 = false;   //双臂机器人动作4：  双臂机器人拆解成品
        public static bool TwoRobotActionFlag5 = false;    //双臂机器人动作5：双臂机器人将零件放到零件托盘

        public static bool CLkOutlibSignal1 = false;      //立库出库信号1   //装配启动信号
        public static bool CJStartSignal = false;         //拆解启动信号

        public static bool ZPMode = true;    //装配模式

        private DispatcherTimer showTimer = new DispatcherTimer();   //定时器，使控件内容显示一段时间后消失
        private DispatcherTimer showTimer1 = new DispatcherTimer();   //定时器1，使控件内容显示一段时间后消失

        Socket socketServer;
        Thread threadListen;
        Thread AGV1thread = null;  //接受AGV1消息的线程
        Thread AGV2thread = null;  //接收AGV2消息的线程
        Thread Mixthread = null;   //接收复合机器人消息的线程
        Thread TWOthread = null;   //接收双臂机器人消息的线程
        Thread TWHthread = null;    //接收立库消息的线程
        
        Socket FAGVSocket = null;   //FAGV做客户端
        Thread FAGVthread = null;   //接收FAGV消息的线程
       // Socket TWHSocket = null;    //TWH做客户端
       

        Thread watchZPthread = null;   //自动化装配监听线程
        Thread delayZPthread = null;    //延时发送线程（装配） 
        Thread watchCJthread = null;    //自动化拆解监听线程
        Thread delayCJthread = null;     //延时发送线程（拆解）

        # region  通信断开信息处理函数
        public void InterruptMsgShow(int id,string msg)
        {
            try
            {
                if (id == 1)
                {
                    this.AGV1_comState.Dispatcher.Invoke(new Action(() =>
                        {
                            AGV1_comState.Content = msg;
                        }));
                }
                if (id == 2)
                {
                    this.AGV2_comState.Dispatcher.Invoke(new Action(() =>
                    {
                        AGV2_comState.Content = msg;
                    }));
                }
                if (id == 3)
                {
                    this.Mix_comState.Dispatcher.Invoke(new Action(() =>
                    {
                        Mix_comState.Content = msg;
                    }));
                }
                if (id == 4)
                {
                    this.Two_comState.Dispatcher.Invoke(new Action(() =>
                    {
                        Two_comState.Content = msg;
                    }));
                }
                if (id == 5)
                {
                    this.FAGV_comState.Dispatcher.Invoke(new Action(() =>
                    {
                        FAGV_comState.Content = msg;
                    }));
                }
                if (id == 6)
                {
                    this.TWH_comState.Dispatcher.Invoke(new Action(() =>
                    {
                        TWH_comState.Content = msg;
                    }));
                }
            }
            catch
            {
                        //什么都不做，这里程序关掉后，会出现异常，//现在本帅哥还不知道为啥
            }
        }
        # endregion


        # region 通信信息处理函数（委托函数）
        private delegate void OutputDelegate(string msg);
        private void OutputdelAGV2(string msg)
        {
            this.AGV2_comState.Dispatcher.Invoke(new OutputDelegate(OutputAGV2), msg);
        }
        private void OutputAGV2(string msg)
        {
            this.AGV2_comState.Content = msg;
        }
        private void OutputdelAGV1(string msg)
        {
            this.AGV1_comState.Dispatcher.Invoke(new OutputDelegate(OutputAGV1), msg);
        }
        private void OutputAGV1(string msg)
        {
            this.AGV1_comState.Content = msg;
        }
        private void OutputdelFAGV(string msg)
        {
            this.FAGV_comState.Dispatcher.Invoke(new OutputDelegate(OutputFAGV), msg);
        }
        private void OutputFAGV(string msg)
        {
            this.FAGV_comState.Content = msg;
        }
        private void OutputdelTWH(string msg)
        {
            this.TWH_comState.Dispatcher.Invoke(new OutputDelegate(OutputTWH), msg);
        }
        private void OutputTWH(string msg)
        {
            this.TWH_comState.Content = msg;
        }
        private void OutputdelMix(string msg)
        {
            this.Mix_comState.Dispatcher.Invoke(new OutputDelegate(OutputMix), msg);
        }
        private void OutputMix(string msg)
        {
            this.Mix_comState.Content = msg;
        }
        private void OutputdelTwo(string msg)
        {
            this.Two_comState.Dispatcher.Invoke(new OutputDelegate(OutputTwo), msg);
        }
        private void OutputTwo(string msg)
        {
            this.Two_comState.Content = msg;
        }

        # endregion

        # region 服务器Socket创建
        private void serverSocketConnect()    //服务器端Socket创建
        {
            try
            {
                socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = new IPAddress(new byte[] { 192, 168, 1,101});
                EndPoint point = new IPEndPoint(ip, 6000);
                socketServer.Bind(point);
                socketServer.Listen(10);
                //////监听线程
                threadListen = new Thread(new ParameterizedThreadStart(Listen));
                threadListen.IsBackground = true;
                threadListen.Start(socketServer);               
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接服务端出错" + ex.ToString());
            }
        }
        # endregion

        # region FAGV的连接
        private void FAGVStart()  //连接FAGV，开启线程
        {
            FAGV m = new FAGV(FAGVSocket);
            m.callback = new FAGV.FAGVThreadCallBackDelegate(FAGVThreadCallBack);
            FAGVthread = new Thread(new ThreadStart(m.recdata));
            FAGVthread.Start();
            FAGVthread.IsBackground = true;
        }
        # endregion

        # region 等待其他机器客户端的连接并分配连接通信的socket
        private void Listen(object obj)
        {
            Socket watchSocket = obj as Socket;
            while (true)
            {
                Socket ClientSocket = watchSocket.Accept();  //等待客户端连接，并创建一个用于通信的Socket
                IPEndPoint Clientipe = (IPEndPoint)ClientSocket.RemoteEndPoint;
                if (Clientipe.Address.ToString() == "192.168.1.103")
                {
                    OutputdelAGV1("AGV1连接成功");
                    AGV1 m = new AGV1(ClientSocket);
                    m.callback = new AGV1.PAGV1ThreadCallBackDelegate(PAGV1ThreadCallBack);
                    AGV1thread = new Thread(new ThreadStart(m.recdata));
                    AGV1thread.Start();
                    AGV1thread.IsBackground = true;
                }
                if (Clientipe.Address.ToString() == "192.168.1.104") 
                {
                    OutputdelAGV2("AGV2连接成功");
                    AGV2 m = new AGV2(ClientSocket);
                    m.callback = new AGV2.PAGV2ThreadCallBackDelegate(PAGV2ThreadCallBack);
                    AGV2thread = new Thread(new ThreadStart(m.recdata));
                    AGV2thread.Start();
                    AGV2thread.IsBackground = true;
                }
                if (Clientipe.Address.ToString() == "192.168.1.122") 
                {
                    OutputdelMix("MixRobot连接成功");
                    MixRobot m = new MixRobot(ClientSocket);
                    m.callback = new MixRobot.MixRobotThreadCallBackDelegate(MixRobotThreadCallBack);
                    Mixthread = new Thread(new ThreadStart(m.recdata));
                    Mixthread.Start();
                    Mixthread.IsBackground = true;
                }
                if (Clientipe.Address.ToString() == "192.168.1.105")
                {
                    OutputdelTwo("TwoRobot连接成功");          
                    TwoRobot m = new TwoRobot(ClientSocket);
                    m.callback = new TwoRobot.TWORobotThreadCallBackDelegate(TWORobotThreadCallBack);
                    TWOthread = new Thread(new ThreadStart(m.recdata));
                    TWOthread.Start();
                    TWOthread.IsBackground = true;
                }
                if (Clientipe.Address.ToString() == "192.168.1.101")
                {
                    OutputdelTWH("TWH连接成功");
                    TWH m = new TWH(ClientSocket);
                    m.callback = new TWH.TWHThreadCallBackDelegate(TWHThreadCallBack);
                    TWHthread = new Thread(new ThreadStart(m.recdata));
                    TWHthread.Start();
                    TWHthread.IsBackground = true;
                }
            }
        }

        # endregion

        # region 将叉车AGV发来的信息可视化
        private void FAGVThreadCallBack(byte[] msg)   //处理FAGV线程回调的信息
        {                       
            if (msg[0] == 0x07)
            {
                if (Dispatcher.Thread != Thread.CurrentThread)
                {
                    //this.FAGV_FinalPoint.Dispatcher.Invoke(new Action(() =>
                    //{
                    //    FAGV_FinalPoint.Content = BitConverter.ToUInt16(msg, 1);
                    //}));
                    this.FAGV_executeState.Dispatcher.Invoke(new Action(() =>
                    {
                        FAGV_executeState.Content = Decode.FAGVExecuteState[msg[3]];
                        showTimer.Tick += new EventHandler(FAGV_SetNull);
                        showTimer.Interval = new TimeSpan(0, 0, 0, 18);
                        showTimer.Start();  
                    }));
                    //this.FAGV_ErrorCode.Dispatcher.Invoke(new Action(() =>
                    //{
                    //    FAGV_ErrorCode.Content = BitConverter.ToUInt16(msg, 5);
                    //}));
                }
            }
        }

        # endregion

        # region 将AGV1发来的信息可视化
        private void PAGV1ThreadCallBack(byte[] msg)   //处理AGV1线程回调的信息
        {                      
            if (msg[0] == 0x04)
            {
                RefleshAGV1_Anserinfo("AGV1", msg[1]);
                if (Dispatcher.Thread != Thread.CurrentThread)
                {
                    this.AGV1_executeState.Dispatcher.Invoke(new Action(() =>
                    {
                        AGV1_executeState.Content = "AGV1位置：" + Decode.platformLocationStr[msg[2]] + "且" + Decode.isTrayStr[msg[3]];
                    }));
                    this.AGV1_WorkState.Dispatcher.Invoke(new Action(()=>
                    {
                        AGV1_WorkState.Content = "AGV1:" + Decode.WorkStateStr[msg[4]];
                    }));
                }
                /////还有浮点数(位置坐标)未处理
            }           
        }
        # endregion

        # region 将AGV2发来的信息可视化
        private void PAGV2ThreadCallBack(byte[] msg)   //处理AGV2线程回调的信息
        {                       
            if (msg[0] == 0x05)
            {
                RefleshAGV2_Anserinfo("AGV2", msg[1]);
                if (Dispatcher.Thread != Thread.CurrentThread)
                {
                    this.AGV2_executeState.Dispatcher.Invoke(new Action(() =>
                    {
                        AGV2_executeState.Content = "AGV2位置：" + Decode.platformLocationStr[msg[2]] + "且" + Decode.isTrayStr[msg[3]];
                    }));
                    this.AGV2_WorkState.Dispatcher.Invoke(new Action(() =>
                    {
                        AGV2_WorkState.Content = "AGV2" + Decode.WorkStateStr[msg[4]];
                    }));
                }
                /////还有浮点数(位置坐标)未处理
            }            
        }

        # endregion

        #region 将复合机器人发来的信息可视化
        private void MixRobotThreadCallBack(byte[] msg)   //处理MixRobot线程回调的信息
        {        
            if (msg[0] == 0x06)
            {
                RefleshMix_Anserinfo("复合机器人：", msg[1]);
                if (Dispatcher.Thread != Thread.CurrentThread)
                {
                    this.Mix_executeState.Dispatcher.Invoke(new Action(() =>
                    {
                        if (msg[2] == 0 || msg[2] == 1 || msg[2] == 5 || msg[2] == 6 || msg[2] == 7 || msg[2] == 8||msg[2]==9)
                        {
                            Mix_executeState.Content = "平台：" + Decode.MixplatformLocationStr[msg[2]];
                        }
                        if (msg[2] == 2)
                        {
                            Mix_executeState.Content = Decode.MixplatformLocationStr[msg[2]] + Decode.MixLocationStateStr1[msg[3]];
                        }
                        if (msg[2] == 3)
                        {
                            Mix_executeState.Content =  Decode.MixplatformLocationStr[msg[2]] + Decode.MixLocationStateStr2[msg[3]];
                        }
                        if (msg[2] == 4)
                        {
                            Mix_executeState.Content = Decode.MixplatformLocationStr[msg[2]] + Decode.MixLocationStateStr3[msg[3]];
                        }

                        ////还有坐标未处理
                    }));
                }
            }
        }

        # endregion

        #region 将双臂发来的信息可视化
        private void TWORobotThreadCallBack(byte[] msg)   //处理TWORobot线程回调的信息
        {
            if (msg[0] == 0x00)
            {
                if (Dispatcher.Thread != Thread.CurrentThread)
                {
                    this.TWO_StartSignal.Dispatcher.Invoke(new Action(() =>
                    {
                        this.TWO_StartSignal.Content = Decode.TWOStartSignal[msg[1]];
                    }));
                    this.TWO_IsComplete.Dispatcher.Invoke(new Action(() =>
                    {
                        this.TWO_IsComplete.Content = Decode.TWOIscomplete[msg[2]];
                    }));
                    this.TWO_ExecuteState.Dispatcher.Invoke(new Action(() =>
                    {
                        this.TWO_ExecuteState.Content = Decode.TWOExecuteState[msg[7]];
                    }));
                    this.TWO_ResponseID.Dispatcher.Invoke(new Action(() =>
                    {
                        this.TWO_ResponseID.Content = bytesToInt(msg, 3).ToString();
                    }));
                }
            }
        }
        # endregion

        # region 将立库发来的信息可视化

        private void TWHThreadCallBack(byte[] msg)   //处理TWH线程回调的信息
        {
            if (msg[0] == 0x07)    //对相应的执行状态执行相应的回复   
            {
                if (Dispatcher.Thread != Thread.CurrentThread)//TODO
                {
                   // RefleshTWH_Anserinfo("立库操作机", msg[1]);
                    this.TWH_executeState.Dispatcher.Invoke(new Action(() =>
                    {
                        this.TWH_executeState.Content = "立库操作机: " + Decode.TWHState[msg[1]];
                        showTimer1.Tick += new EventHandler(TWH_SetNull);
                        showTimer1.Interval = new TimeSpan(0, 0, 0, 18);
                        showTimer1.Start();  
                    }));
                }
            }
        }

        #endregion

        #region 可视化辅助函数

        
        private void FAGV_SetNull(object sender, EventArgs e)           //使FAGV上控件内容显示一段时间后消失
        {
            int i = 0;    //保证定时器只执行一次
            if (i == 0)
            {
                FAGV_executeState.Content = "正在执行当前操作";
                i++;
            }
            if (i == 1)
            {
                showTimer.Stop();    //关闭定时器
            }
        }
        private void TWH_SetNull(object sender, EventArgs e)           //使FAGV上控件内容显示一段时间后消失
        {
            int i = 0;    //保证定时器只执行一次
            if (i == 0)
            {
                TWH_executeState.Content = "正在执行当前操作";
                i++;
            }
            if (i == 1)
            {
                showTimer1.Stop();    //关闭定时器
            }
        }
        private void RefleshAGV1_Anserinfo(string str, byte m)  //刷新AGV1应答信息控件信息   
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                this.AGV1_answerState.Dispatcher.Invoke(new Action(() =>
                {
                    this.AGV1_answerState.Content = str + ":" + Decode.comStateStr[m];
                }));
            }
            else
            {
                this.AGV1_answerState.Content = str + ":" + Decode.comStateStr[m];
            }
        }
        private void RefleshAGV2_Anserinfo(string str, byte m)  //刷新AGV2应答信息控件信息   
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                this.AGV2_answerState.Dispatcher.Invoke(new Action(() =>
                {
                    this.AGV2_answerState.Content = str + ":" + Decode.comStateStr[m];
                }));
            }
            else
            {
                this.AGV2_answerState.Content = str + ":" + Decode.comStateStr[m];
            }
        }
        private void RefleshMix_Anserinfo(string str, byte m)  //刷新AGV1应答信息控件信息   
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                this.Mix_answerState.Dispatcher.Invoke(new Action(() =>
                {
                    this.Mix_answerState.Content = str + ":" + Decode.comStateStr[m];
                }));
            }
            else
            {
                this.Mix_answerState.Content = str + ":" + Decode.comStateStr[m];
            }
        }
        private void RefleshFAGV_Answerinfo(string str, byte m)   //刷新FAGV应答信息控件信息
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                this.FAGV_answerState.Dispatcher.Invoke(new Action(() =>
                {
                    this.FAGV_answerState.Content = str + ":" + Decode.FAGVcomStateStr[m];
                }));
            }
            else
            {
                this.FAGV_answerState.Content = str + ":" + Decode.FAGVcomStateStr[m];
            }
        }
        private void RefleshTWH_Anserinfo(string str, byte m)  //刷新立库应答信息控件信息   
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                this.TWH_answerState.Dispatcher.Invoke(new Action(() =>
                {
                    this.TWH_answerState.Content = str + ":" + Decode.comStateStr[m];
                }));
            }
            else
            {
                this.TWH_answerState.Content = str + ":" + Decode.comStateStr[m];
            }
        }
        public static int bytesToInt(byte[] src, int offset)
        {
            int value;
            value = (int)((src[offset] & 0xFF)
                    | ((src[offset + 1] & 0xFF) << 8)
                    | ((src[offset + 2] & 0xFF) << 16)
                    | ((src[offset + 3] & 0xFF) << 24));
            return value;
        }
        # endregion


        /// <summary>
        /// //////////////////////////////////按钮事件/////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
   
    
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {         
            serverSocketConnect();                             
        }

        # region 手动界面按钮事件
        private void AGV1_button1_Click(object sender, RoutedEventArgs e)
        {            
            byte[] cmdcode=new byte[]{0x05,0x2A,0x00,0x00};   //平台AGV前往立库料台取料盘          
            AGV1.sendRequestcmd(cmdcode);
        }
        private void AGV1_button4_Click(object sender, RoutedEventArgs e)
        {
            byte[] cmdcode = new byte[] { 0x04, 0x2A, 0x00, 0x00 };   //平台AGV前往立库料台处放料盘
            AGV1.sendRequestcmd(cmdcode);
        }
        private void AGV1_button2_Click(object sender, RoutedEventArgs e)
        {
            byte[] cmd = new byte[] { 0x02, 0x2A, 0x00, 0x00};    //平台AGV前往零件接转区
            AGV1.sendRequestcmd(cmd);
        }
        private void AGV1_button5_Click(object sender, RoutedEventArgs e)  //平台AGV1前往立库料台处
        {
            byte[] cmd = new byte[] { 0x01, 0x2A, 0x00, 0x00 };
            AGV1.sendRequestcmd(cmd);
        }
        private void AGV1_button3_Click(object sender, RoutedEventArgs e)
        {
            byte[] cmd = new byte[] { 0x03, 0x2A, 0x00, 0x00 };   //平台AGV前往初始位置
            AGV1.sendRequestcmd(cmd);
        }

        private void AGV2_button1_Click(object sender, RoutedEventArgs e)
        {
            byte[] cmdcode = new byte[] { 0x05, 0x2A, 0x00, 0x00 };   //平台AGV2前往立库料台处取料盘
            byte[] cmdMsg = new byte[] { 0x00 };
            AGV2.sendRequestcmd(cmdcode);
        }
        private void AGV2_button4_Click(object sender, RoutedEventArgs e)
        {
            byte[] cmdcode = new byte[] { 0x04, 0x2A, 0x00, 0x00 };   //平台AGV2前往立库料台处送料盘           
            AGV2.sendRequestcmd(cmdcode);
        }
        private void AGV2_button2_Click(object sender, RoutedEventArgs e)
        {
            byte[] cmd = new byte[] { 0x02, 0x2A, 0x00, 0x00 };   //平台AGV前往零件接转区
            AGV2.sendRequestcmd(cmd);
        }
       
        private void AGV2_button5_Click(object sender, RoutedEventArgs e)   //平台AGV2前往立库料台处
        {
            byte[] cmd = new byte[] { 0x01, 0x2A, 0x00, 0x00 };
            AGV2.sendRequestcmd(cmd);
        }
        private void AGV2_button3_Click(object sender, RoutedEventArgs e)
        {
            byte[] cmd = new byte[] { 0x03, 0x2A, 0x00, 0x00 };   //平台AGV2前往初始位置
            AGV2.sendRequestcmd(cmd);
        }

        private void Mix_button1_Click(object sender, RoutedEventArgs e)   //复合前往零件接转区
        {
            byte[] cmd = new byte[] { 0x01, 0x2A, 0x00, 0x00 };   
            MixRobot.sendRequestcmd(cmd);
        }

        private void Mix_button2_Click(object sender, RoutedEventArgs e)   //复合前往双臂装配区
        {
            byte[] cmd = new byte[] { 0x02, 0x2A, 0x00, 0x00 };   
            MixRobot.sendRequestcmd(cmd);
        }

        private void Mix_button3_Click(object sender, RoutedEventArgs e)   //复合前往成品接转区
        {
            byte[] cmd = new byte[] { 0x03, 0x2A, 0x00, 0x00};   
            MixRobot.sendRequestcmd(cmd);
        }

        private void Mix_button4_Click(object sender, RoutedEventArgs e)   //复合回到初始位置
        {
            byte[] cmd = new byte[] { 0x04, 0x2A, 0x00, 0x00 };  
            MixRobot.sendRequestcmd(cmd);
        }

        private void Mix_button5_Click(object sender, RoutedEventArgs e)  //复合在接转区取/放零件料盘
        {
            byte[] cmd = new byte[] { 0x05, 0x2A, 0x00, 0x00 };   
            MixRobot.sendRequestcmd(cmd);
        }

        private void Mix_button6_Click(object sender, RoutedEventArgs e)   //复合在装配区取/放零件料盘
        {
            byte[] cmd = new byte[] { 0x06, 0x2A, 0x00, 0x00 };   
            MixRobot.sendRequestcmd(cmd);
        }

        private void Mix_button7_Click(object sender, RoutedEventArgs e)  //复合在成品区取/放零件料盘
        {
            byte[] cmd = new byte[] { 0x07, 0x2A, 0x00, 0x00 };  
            MixRobot.sendRequestcmd(cmd);
        }         
        private void TWH_button1_Click(object sender, RoutedEventArgs e)   //零件入库操作
        {
            DB.ResetDB_cmd();
            byte[] cmd = new byte[] { 0x02, 0x0A, 0x00, 0x01 };
            TWH.sendRequestcmd(cmd);
        }

        private void TWH_button2_Click(object sender, RoutedEventArgs e)   //零件出库操作
        {
            DB.ResetDB_cmd();
            byte[] cmd = new byte[] { 0x02, 0x0A, 0x00, 0x02 };
            TWH.sendRequestcmd(cmd);
        }      

        private void TWH_button4_Click(object sender, RoutedEventArgs e)   //成品入库操作
        {
            DB.ResetDB_cmd();
            byte[] cmd = new byte[] { 0x02, 0x0A, 0x00, 0x04 };
            TWH.sendRequestcmd(cmd);
        }

        private void TWH_button5_Click(object sender, RoutedEventArgs e)  //成品出库操作
        {
            DB.ResetDB_cmd();
            byte[] cmd = new byte[] { 0x02, 0x0A, 0x00, 0x05 };
            TWH.sendRequestcmd(cmd);

        }
        private void TWO_button1_Click(object sender, RoutedEventArgs e)   //双臂机器人从零件托盘取出零件
        {
            TwoRobotActionFlag0 = true;
        }

        private void TWO_button2_Click(object sender, RoutedEventArgs e)  //双臂机器人装配
        {
            TwoRobotActionFlag1 = true;
        }

        private void TWO_button3_Click(object sender, RoutedEventArgs e)   //双臂机器人将成品放到成品托盘
        {
            TwoRobotActionFlag2 = true;
        }

        private void TWO_button4_Click(object sender, RoutedEventArgs e)   //双臂机器人从成品托盘取出成品
        {
            TwoRobotActionFlag3 = true;
        }

        private void TWO_button5_Click(object sender, RoutedEventArgs e)   //双臂机器人拆解成品
        {
            TwoRobotActionFlag4 = true;
        }

        private void TWO_button6_Click(object sender, RoutedEventArgs e)    //双臂机器人将零件放到零件托盘
        {
            TwoRobotActionFlag5 = true;
        }

        private void FAGV_button1_Click(object sender, RoutedEventArgs e)  //叉车AGV前往立库料台处取料
        {
            byte cmd = 0x01;
            FAGV.sendRequestcmd(cmd);
        }

        private void FAGV_button2_Click(object sender, RoutedEventArgs e)   //叉车AGV前往立库料台处送料
        {
            byte cmd = 0x02;
            FAGV.sendRequestcmd(cmd);
        }

        private void FAGV_button3_Click(object sender, RoutedEventArgs e)   //叉车AGV前往中转区取料
        {
            byte cmd = 0x03;
            FAGV.sendRequestcmd(cmd);
        }

        private void FAGV_button4_Click(object sender, RoutedEventArgs e)   //叉车AGV前往中转区送料
        {
            byte cmd = 0x04;
            FAGV.sendRequestcmd(cmd);
        }
        private void FAGV_button5_Click(object sender, RoutedEventArgs e)   //叉车AGV前往初始位置
        {
            byte cmd = 0x05;
            FAGV.sendRequestcmd(cmd);
        }
        private void FAGV_button6_Click(object sender, RoutedEventArgs e)   //叉车AGV前往安全区
        {
            byte cmd = 0x06;
            FAGV.sendRequestcmd(cmd);
        }       

        private void Connect_FAGV_Click(object sender, RoutedEventArgs e)
        {
            FAGVStart();
        }       
                
        private void FAGV_Charing_Click(object sender, RoutedEventArgs e)   //叉车AGV前往充电
        {
            byte cmd = 0x07;
            FAGV.sendRequestcmd(cmd);     //叉车AGV前往充电
        }       
        private void FAGV_unCharing_Click(object sender, RoutedEventArgs e)  //叉车AGV充电完成
        {
            byte cmd = 0x08;
            FAGV.sendRequestcmd(cmd);     //叉车AGV充电完成
        }
        # endregion

        # region 自动界面按钮事件
        private void Auto_ZP_Start_Click(object sender, RoutedEventArgs e)   //自动装配启动
        {
            Auto_ZP_Mode.IsEnabled = false;
            Auto_ZP_Start.IsEnabled = false;
            Auto_CJ_Mode.IsEnabled = false;
            Auto_CJ_Start.IsEnabled = false;
            Auto_CJ_closed.IsEnabled = false;
            AutoMode1 = true;     //自动模式1 装配模式
            test1.FrontPart = true;
            watchZPthread = new Thread(new ThreadStart(test1.watchexecuteState));
            watchZPthread.Start();
            watchZPthread.IsBackground = true;
            delayZPthread = new Thread(new ThreadStart(test1.dealysend));
            delayZPthread.Start();
            delayZPthread.IsBackground = true;
            test1.RefleshFlags();
            test1.TwoActionCount0 = 0;    //双臂动作0的次数
            CLkOutlibSignal1 = true;    //装配启动信号
            test1.Count = 0;
            test1.ControlOnlyOneFlag6 = false;         
        }

        private void Auto_ZP_closed_Click(object sender, RoutedEventArgs e)  //自动装配关闭
        {
            AutoMode1 = false;
            Auto_ZP_Mode.IsEnabled = true;
            Auto_ZP_Start.IsEnabled = true;
            Auto_CJ_Mode.IsEnabled = true;
            Auto_CJ_Start.IsEnabled = true;
            Auto_CJ_closed.IsEnabled = true;
        }

        private void Auto_CJ_closed_Click(object sender, RoutedEventArgs e)  //自动拆解关闭
        {
            AutoMode2 = false;
            Auto_CJ_Mode.IsEnabled = true;
            Auto_CJ_Start.IsEnabled = true;
            Auto_ZP_closed.IsEnabled = true;
            Auto_ZP_Mode.IsEnabled = true;
            Auto_ZP_Start.IsEnabled = true;
        }
        private void Auto_CJ_Start_Click(object sender, RoutedEventArgs e)   //自动拆解启动
        {
            Auto_CJ_Mode.IsEnabled = false;
            Auto_CJ_Start.IsEnabled = false;
            Auto_ZP_closed.IsEnabled = false;
            Auto_ZP_Mode.IsEnabled = false;
            Auto_ZP_Start.IsEnabled = false;
            AutoMode2 = true;    //自动模式2 拆解模式
            CJStartSignal = true;   //拆解启动信号
            CJClass.FrontPart = false;
            watchCJthread = new Thread(new ThreadStart(CJClass.watchexecuteState));
            watchCJthread.Start();
            watchCJthread.IsBackground = true;
            delayCJthread = new Thread(new ThreadStart(CJClass.dealysend));
            delayCJthread.Start();
            delayCJthread.IsBackground = true;
            CJClass.RefleshFlags();
            CJClass.ControlOnlyOneFlag10 = false;
        }

        private void btn_InsertZPkuwei_Click(object sender, RoutedEventArgs e)
        {
            if ((tb_zpPart1.Text == "") || (tb_zpPart2.Text == "") || (tb_zpPart3.Text == "") || tb_zpPart4.Text == "" || tb_zpAssembly.Text == "")
            {
                MessageBox.Show("输入库位不能为空");
            }
            ////检查看一下是否输入的库位是合法库位
            else if (!((CheckKuwei(tb_zpPart1.Text)) && (CheckKuwei(tb_zpPart2.Text)) && (CheckKuwei(tb_zpPart3.Text)) && (CheckKuwei(tb_zpPart4.Text)) && (CheckKuwei(tb_zpAssembly.Text))))
            {
                MessageBox.Show("输入的库位格式不正确或者库位不合法");
            }
            else if (!(CheckStyle(tb_zpPart1.Text, tb_zpPart2.Text, tb_zpPart3.Text, tb_zpPart4.Text, tb_zpAssembly.Text)))
            {
                MessageBox.Show("请按正确顺序输入不同样式的仓位");
            }
            else
            {
                DB.InsertKuwei(tb_zpPart1.Text, tb_zpPart2.Text, tb_zpPart3.Text, tb_zpPart4.Text, tb_zpAssembly.Text);
            }
        }

        private void btn_InsertCJkuwei_Click(object sender, RoutedEventArgs e)
        {
            /////检查库位是否为空
            if ((tb_cjPart1.Text == "") || (tb_cjPart2.Text == "") || (tb_cjPart3.Text == "") || tb_cjPart4.Text == "" || tb_cjAssembly.Text == "")
            {
                MessageBox.Show("输入库位不能为空");
            }           
            else if (!((CheckKuwei(tb_cjPart1.Text)) && (CheckKuwei(tb_cjPart2.Text)) && (CheckKuwei(tb_cjPart3.Text)) && (CheckKuwei(tb_cjPart4.Text)) && (CheckKuwei(tb_cjAssembly.Text))))
            {
                MessageBox.Show("输入的库位格式不正确或者库位不合法");
            }
            else if(!(CheckStyle(tb_cjPart1.Text,tb_cjPart2.Text,tb_cjPart3.Text,tb_cjPart4.Text,tb_cjAssembly.Text)))
            {
                MessageBox.Show("请按正确顺序输入不同样式的仓位");
            }           
            else
            {
                DB.InsertKuwei1(tb_cjPart1.Text, tb_cjPart2.Text, tb_cjPart3.Text, tb_cjPart4.Text, tb_cjAssembly.Text);
            }
        }

        private void Auto_CJ_Mode_Click(object sender, RoutedEventArgs e)
        {
            ZPMode = false;
            byte[] cmd = new byte[] { 0x02, 0x0A, 0x00, 0x07 };
            TWH.sendRequestcmd(cmd);                 //拆解模式请求
        }

        private void Auto_ZP_Mode_Click(object sender, RoutedEventArgs e)
        {
            ZPMode = true;
            byte[] cmd = new byte[] { 0x02, 0x0A, 0x00, 0x08 };
            TWH.sendRequestcmd(cmd);                //装配模式请求
        }

        private bool CheckStyle(string postion1,string postion2,string postion3,string postion4,string postion5)
        {
            byte[] array1 = Encoding.Default.GetBytes(postion1.Trim());
            if (array1[0] == 49)
            {
                byte[] array2 = Encoding.Default.GetBytes(postion2.Trim());
                if (array2[0] == 50)
                {
                    byte[] array3 = Encoding.Default.GetBytes(postion3.Trim());
                    if (array3[0] == 51)
                    {
                        byte[] array4 = Encoding.Default.GetBytes(postion4.Trim());
                        if (array4[0] == 52)
                        {
                            byte[] array5 = Encoding.Default.GetBytes(postion5.Trim());
                            if (array5[0] == 53 || array5[0] == 54)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
             
        }
        private bool CheckKuwei(string postion)
        {
            byte[] array = Encoding.Default.GetBytes(postion.Trim());
            if ((array[0] > 48) && (array[0] < 55))   //array[0]对应的AScii需要是1-6
            {
                if (array[1] == 45)    //array[1]对应的ascii需要是-
                {
                    if ((array[2] > 48) && (array[2] < 53))  //array[2]对应的ascii需要是1-4
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        # endregion

        private void btn_DeleteCmd_Click(object sender, RoutedEventArgs e)
        {
            DB.ResetDB_cmd();     //清空数据库中的指令表
            DB.ResetDB_Stock();     //更新数据库中的库存表
            DB.ResetDB_kuwei();     //更新数据表的库位表
            byte[] cmd = new byte[] { 0x02, 0x0A, 0x00, 0x09 };  //要求立库软件更新库位显示
            TWH.sendRequestcmd(cmd);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            test1.OutlibCompleted = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            test1.AssemblyOutlibCompleted = true;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            CJClass.OutlibAssemblyCompleted = true;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            CJClass.InlibAssemblyCompleted = true;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            CJClass.OutlibCompleted = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }






      






















    }
}

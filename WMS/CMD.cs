using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows.Forms;

namespace WMS
{
    class CMD
    {
        public static byte comidentify;  // 通讯标志位
        public static byte[] cmdidentify = new byte[4]; //命令码
        public static byte[] datalength = new byte[2];  //数据长度 
        public static byte[] packageindex = new byte[2] { 0x00, 0x01 };  //包序   //用于AGV1 AGV2
        public static byte[] packageindex1 = new byte[2] { 0x01, 0x00 };  //包序  //用于Mix的
        public static byte lastpackage = 0x01; //最后一包
        public static byte messageverify = 0; //消息体中的数据异或校验，不包括消息头
        public static byte[] sendbyte = new byte[100];  //定义发送数据数组
        //叉车AGV通讯协议
        public static byte[] MsgHeader=new byte[2];   //消息头
        public static byte[] Isreply=new byte[2];   //是否回复
        public static byte[] Datalength = new byte[2];  //数据长度
        public static byte[] TaskCode = new byte[2];   //任务码
        public static byte VerifyCode;   //校验和
        private static readonly object obj = new object();
        private static readonly object obj1 = new object();
        private static readonly object obj2 = new object();
        private static readonly object obj3 = new object();
        private static readonly object obj4 = new object();
        public delegate void ThreadCallBackDelegate(byte[] msg);  //定义一个AGV2的线程回调委托
        

        public static void sendDataE0(byte[] cmd, Socket s)    //（AGV1，AGV2，MixRobot）主控发送消息体为0的方法
        {
            lock (obj)
            {
                comidentify = 0x40;  //通讯位：0100 0000  请求帧：0 需应答：1
                cmdidentify[0] = cmd[0];
                cmdidentify[1] = cmd[1];
                cmdidentify[2] = cmd[2];
                cmdidentify[3] = cmd[3];
                datalength[0] = 0x00;
                datalength[1] = 0x00;  //无请求数据
                messageverify = 0x00; //无请求数据，消息体校验位位0             
                InitcmdAndSend(20,s);               
            }
        }
        public static void sendDataT0(byte[] cmd, Socket s)    //（MixRobot）主控发送消息体为0的方法
        {
            lock (obj1)
            {
                comidentify = 0x40;  //通讯位：0100 0000  请求帧：0 需应答：1
                cmdidentify[0] = cmd[0];
                cmdidentify[1] = cmd[1];
                cmdidentify[2] = cmd[2];
                cmdidentify[3] = cmd[3];
                datalength[0] = 0x00;
                datalength[1] = 0x00;  //无请求数据
                messageverify = 0x00; //无请求数据，消息体校验位位0             
                InitcmdAndSendT(20, s);
            }
        }
        //public static void sendDataE4(byte[] cmdcode, byte[] cmdMsg, Socket s)   //(AGV1 AGV2)发送消息体为1的方法
        //{
        //    lock (obj)
        //    {
        //        comidentify = 0x40;  //通讯位：0100 0000  请求帧：0 需应答：1
        //        cmdidentify[0] = cmdcode[0];
        //        cmdidentify[1] = cmdcode[1];
        //        cmdidentify[2] = cmdcode[2];
        //        cmdidentify[3] = cmdcode[3];
        //        datalength[0] = 0x01;
        //        datalength[1] = 0x00;  //无请求数据
        //        messageverify = CalculateMessageverify1(cmdMsg); //消息体校验位           
        //        InitcmdAndSend(21,cmdMsg,s);                
        //    }
        //}
        public static void sendDataE1(byte[] cmd, Socket s)     //双臂机器人的发送函数
        {
            lock (obj2)
            {                             
                byte[] sendbyte = new byte[26];
                sendbyte[0] = 0x05;
                sendbyte[1] = 0x0A;
                sendbyte[2] = 0x05;
                sendbyte[3] = 0x0A;    //消息头标识
                sendbyte[4] = 0x02;   //通讯标志位
                sendbyte[5] = 0x00;
                sendbyte[6] = 0x10;
                sendbyte[7] = 0x00;
                sendbyte[8] = 0x00;  //命令字
                sendbyte[9] = 0x06;
                sendbyte[10] =0x00;  //数据长度
                sendbyte[11] = packageindex1[0];
                sendbyte[12] = packageindex1[1];  //包序
                sendbyte[13] = lastpackage;     //最后一包
                sendbyte[14] = CalculateMessageverify(cmd);   //消息体校验
                sendbyte[15] = 0x00;
                sendbyte[16] = 0x00;
                sendbyte[17] = 0x00;
                sendbyte[18] = 0x00;    //保留字节
                byte x = 0;
                for (int i = 0; i < 19; i++)
                {
                    x ^= sendbyte[i];
                }
                sendbyte[19] = x;   //消息头校验字
                sendbyte[20] = cmd[0];
                sendbyte[21] = cmd[1];
                sendbyte[22] = cmd[2];
                sendbyte[23] = cmd[3];
                sendbyte[24] = cmd[4];
                sendbyte[25] = cmd[5];
                try
                {
                    int length = s.Send(sendbyte);
                }
                catch
                {
                    MessageBox.Show("向双臂机器人发送消息失败，请检查通讯是否断开");
                }
              //  ModifyTxt.PrintTxt(sendbyte);                                             
            }
        }
        public static void sendDataE2(byte cmd, Socket s)    //发送FAGV操作命令的方法
        {
            lock (obj3)
            {
                byte[] sendbyte = new byte[9];
                
                sendbyte[0] = 0x1A;    //消息头
                sendbyte[1] = 0x27;
                sendbyte[2] = 0x00;
                sendbyte[3] = 0x00;
                sendbyte[4] = 0x01;
                sendbyte[5] = 0x00;
                sendbyte[6] = cmd;
                sendbyte[7] = 0x00;
                byte b = 0;
                for (int i = 0; i < 8; i++)
                {
                    b += sendbyte[i];
                    if (b > 255)
                        b -= 255;
                }
                sendbyte[8] = b;
                int length = s.Send(sendbyte);
             //   ModifyTxt.PrintTxt(sendbyte);
                Array.Clear(sendbyte, 0, 9);                
            }
        }
        public static void sendDataE3(int cmd, Socket s)   //发送FAGV心跳消息的方法
        {
            lock (obj4)
            {
                MsgHeader = IntTo2Byte(cmd);
                sendbyte[0] = MsgHeader[0];
                sendbyte[1] = MsgHeader[1];
                for (int i = 0; i < 2; i++)
                {
                    VerifyCode ^= sendbyte[i];
                }
                sendbyte[2] = VerifyCode;
                int length = s.Send(sendbyte);
                //   ModifyTxt.PrintTxt(sendbyte);
                Array.Clear(sendbyte, 0, 100);
            }
        }       
        private static byte[]  IntTo2Byte(int number)
        {
            byte[] cmd=new byte[2];
            cmd[0] = (byte)(number >> 8);    //采用的是小端字节序
            cmd[1] = (byte)(number);
            return cmd;
        }
        private static  byte CalculateMessageverify(byte[] cmd)    //双臂机器人的消息体校验
        {
            byte temp=0;
            for (int i = 0; i < 6; i++)
            {
                sendbyte[20 + i] = cmd[i];
            }
            for (int j = 0; j < 6; j++)
            {
                temp ^= sendbyte[20 + j];
            }
            return temp;
        }
        //private static byte CalculateMessageverify1(byte[] cmd)    //(AGV1 AGV2)的消息体校验
        //{
        //    byte temp = 0;
        //    for (int i = 0; i < 1; i++)
        //    {
        //        sendbyte[20 + i] = cmd[i];
        //    }
        //    for (int j = 0; j < 1; j++)
        //    {
        //        temp ^= sendbyte[20 + j];
        //    }
        //    return temp;
        //}
        public static void InitcmdAndSend(int count,Socket s)    //初始化发送数据的消息头
        {
            byte[] sendbyte = new byte[count];
            sendbyte[0] = 0x05;
            sendbyte[1] = 0x0A;
            sendbyte[2] = 0x05;
            sendbyte[3] = 0x0A;    //消息头标识
            sendbyte[4] = comidentify;   //通讯标志位
            sendbyte[5] = cmdidentify[0];
            sendbyte[6] = cmdidentify[1];
            sendbyte[7] = cmdidentify[2];
            sendbyte[8] = cmdidentify[3];  //命令字
            sendbyte[9] = datalength[0];
            sendbyte[10] = datalength[1];  //数据长度
            sendbyte[11] = packageindex1[0];
            sendbyte[12] = packageindex1[1];  //包序
            sendbyte[13] = lastpackage;     //最后一包
            sendbyte[14] = messageverify;   //消息体校验
            sendbyte[15] = 0x00;
            sendbyte[16] = 0x00;
            sendbyte[17] = 0x00;
            sendbyte[18] = 0x00;    //保留字节
            byte x = 0;
            for (int i = 0; i < 19; i++)
            {
                x ^= sendbyte[i];
            }
            sendbyte[19] = x;   //消息头校验字
            int length = s.Send(sendbyte);   //发送数据
         //   ModifyTxt.PrintTxt(sendbyte);
            Array.Clear(sendbyte,0,count);
        }
        public static void InitcmdAndSendT(int count, Socket s)    //初始化发送数据的消息头
        {
            byte[] sendbyte = new byte[count];
            sendbyte[0] = 0x05;
            sendbyte[1] = 0x0A;
            sendbyte[2] = 0x05;
            sendbyte[3] = 0x0A;    //消息头标识
            sendbyte[4] = comidentify;   //通讯标志位
            sendbyte[5] = cmdidentify[0];
            sendbyte[6] = cmdidentify[1];
            sendbyte[7] = cmdidentify[2];
            sendbyte[8] = cmdidentify[3];  //命令字
            sendbyte[9] = datalength[0];
            sendbyte[10] = datalength[1];  //数据长度
            sendbyte[11] = packageindex1[0];
            sendbyte[12] = packageindex1[1];  //包序
            sendbyte[13] = lastpackage;     //最后一包
            sendbyte[14] = messageverify;   //消息体校验
            sendbyte[15] = 0x00;
            sendbyte[16] = 0x00;
            sendbyte[17] = 0x00;
            sendbyte[18] = 0x00;    //保留字节
            byte x = 0;
            for (int i = 0; i < 19; i++)
            {
                x ^= sendbyte[i];
            }
            sendbyte[19] = x;   //消息头校验字
            int length = s.Send(sendbyte);   //发送数据
         //   ModifyTxt.PrintTxt(sendbyte);
            Array.Clear(sendbyte, 0, count);
        }
        //public static void InitcmdAndSend(int count, byte[] data, Socket s)
        //{
        //    byte[] sendbyte = new byte[count];
        //    sendbyte[0] = 0x05;
        //    sendbyte[1] = 0x0A;
        //    sendbyte[2] = 0x05;
        //    sendbyte[3] = 0x0A;    //消息头标识
        //    sendbyte[4] = comidentify;   //通讯标志位
        //    sendbyte[5] = cmdidentify[0];
        //    sendbyte[6] = cmdidentify[1];
        //    sendbyte[7] = cmdidentify[2];
        //    sendbyte[8] = cmdidentify[3];  //命令字
        //    sendbyte[9] = datalength[0];
        //    sendbyte[10] = datalength[1];  //数据长度
        //    sendbyte[11] = packageindex[0];
        //    sendbyte[12] = packageindex[1];  //包序
        //    sendbyte[13] = lastpackage;     //最后一包
        //    sendbyte[14] = messageverify;   //消息体校验
        //    sendbyte[15] = 0x00;
        //    sendbyte[16] = 0x00;
        //    sendbyte[17] = 0x00;
        //    sendbyte[18] = 0x00;    //保留字节
        //    byte x = 0;
        //    for (int i = 0; i < 19; i++)
        //    {
        //        x ^= sendbyte[i];
        //    }
        //    sendbyte[19] = x;   //消息头校验字
        //    sendbyte[20] = data[0];
        //    int length = s.Send(sendbyte);   //发送数据
        // //   ModifyTxt.PrintTxt(sendbyte);
        //}

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS
{
    class Assembly   //装配类（自动化程序中的状态信息处理类）
    {
        public static bool OutlibCompleted = false;    //出库完成
        public static bool InlibCompleted = false;      //入库完成
        public static bool LastStep = false;           //最后一步，平台AGV2前往初始位置且无料盘
        public static int  Count = 0;         //计算进入到CLkOutlibSignal1 代码块中得次数

      //  public static bool ControlFlag1 = false;      //控制执行顺序标识1
        public static bool ControlFlag2 = false;      //控制执行顺序标识2
        public static bool ControlFlag3 = false;      //控制执行顺序标识2       
        public static bool Controllock1 = false;    //控制锁1
        public static bool Controllock2 = false;    //控制锁2
        //以下6个需要刷新
        public static bool ControlOnlyOneFalg1 = false;   //控制进入一次标识1
        public static bool ControlOnlyOneFlag2 = false;   //控制进入一次标识2
        public static bool ControlOnlyOneFlag3 = false;    //控制进入一次标识2
        public static bool ControlOnlyOneFlag4 = false;    //控制进入一次标识2
        public static bool ControlOnlyOneFlag5 = false;    //控制进入一次标识2
        public static bool ControlOnlyOneFlag6 = false;    //控制进入一次标识2

        public static bool TwoRobotActionCompleted0 = false;         //双臂动作0执行完成

        private static  void RefleshFlags()
        {
             ControlOnlyOneFalg1 = false;   //控制进入一次标识1
             ControlOnlyOneFlag2 = false;   //控制进入一次标识2
             ControlOnlyOneFlag3 = false;    //控制进入一次标识2
             ControlOnlyOneFlag4 = false;    //控制进入一次标识2
             ControlOnlyOneFlag5 = false;    //控制进入一次标识2
             ControlOnlyOneFlag6 = false;    //控制进入一次标识2
        }

        public static void watchexecuteState()
        {
            if (MainWindow.CLkOutlibSignal1 == true)   //界面UI要求零件开始出库
            {
                RefleshFlags();
                Count++;
                if (Count <= 2)
                {
                    byte[] cmd = new byte[] { 0x02, 0x0A, 0x00, 0x02 };  //发送立库出库命令
                    TWH.sendRequestcmd(cmd);
                    byte[] cmd1 = new byte[] { 0x01, 0x2A, 0x00, 0x00 };  //发送AGV1去往立库料台命令
                    AGV1.sendRequestcmd(cmd1);
                    MainWindow.CLkOutlibSignal1 = false;
                }
            }
            if (AGV1.PlatfromStateArray[2]==0x02&&AGV1.PlatfromStateArray[3]==0x00&&OutlibCompleted == true)   //AGV1达到立库料台处且无料盘且出库执行成功
            {                              
                    byte[] cmd = new byte[] { 0x05, 0x2A, 0x00, 0x00 };   //AGV1在立库料台取货
                    AGV1.sendRequestcmd(cmd);
                    OutlibCompleted = false;                             
            }
            if (AGV2.PlatfromStateArray[2] == 0x02 && AGV2.PlatfromStateArray[3] == 0x00 && OutlibCompleted == true)   //AGV2达到立库料台处且无料盘且出库执行成功
            {
                byte[] cmd = new byte[] { 0x05, 0x2A, 0x00, 0x00 };   //AGV2在立库料台取货
                AGV2.sendRequestcmd(cmd);
                OutlibCompleted = false;
            }
            if (AGV1.PlatfromStateArray[4] == 0x01)    //AGV1已从料台取出料盘
            {
                byte[] cmd = new byte[] { 0x02, 0x2A, 0x00, 0x00 };   //AGV1前往零件接转区
                AGV1.sendRequestcmd(cmd);
            }
            if (AGV2.PlatfromStateArray[4] == 0x01)  //AGV2已从立库料台处取货
            {
                if (AGV1.PlatfromStateArray[2] == 0x04)  //AGV1正在前往原点
                {
                    byte[] cmd = new byte[] { 0x02, 0x2A, 0x00, 0x00 };
                    AGV2.sendRequestcmd(cmd);                       //AGV2前往零件转接区
                }
            }
            if (AGV1.PlatfromStateArray[2] == 0x06)   //AGV1正在前往零件结转区
            {
                if (!ControlOnlyOneFalg1)   //控制代码块只进入一次，这里保证只发送一次出库命令
                {
                    byte[] cmd = new byte[] { 0x02, 0x0A, 0x00, 0x02 };  //发送立库出库命令
                    TWH.sendRequestcmd(cmd);
                    byte[] cmd1 = new byte[] { 0x01, 0x2A, 0x00, 0x00 };  //AGV2前往立库料台处
                    AGV2.sendRequestcmd(cmd1);
                    ControlOnlyOneFalg1 = true;
                }              
            }
            if (AGV1.PlatfromStateArray[2] == 0x03)   //AGV1到达零件转接区
            {
                if (!ControlOnlyOneFlag2)
                {
                    byte[] cmd = new byte[] { 0x01, 0x2A, 0x00, 0x00 };
                    MixRobot.sendRequestcmd(cmd);       //复合前往零件接转区
                    ControlOnlyOneFlag2 = true;
                }
            }
            if (AGV2.PlatfromStateArray[2] == 0x03)   //AGV2到达零件转接区
            {
                if (MixRobot.PlatfromStateArray[2] == 0x01)    //复合在初始位置
                {
                    if (!ControlOnlyOneFlag4)
                    {
                        byte[] cmd = new byte[] { 0x01, 0x2A, 0x00, 0x00 };
                        MixRobot.sendRequestcmd(cmd);       //复合前往零件接转区
                        ControlOnlyOneFlag4 = true;
                        ControlFlag3 = false;
                        ControlFlag2 = false;
                    }
                }
            }
            if (MixRobot.PlatfromStateArray[2] == 0x02 && MixRobot.PlatfromStateArray[3] == 0x00)//复合到达零件接转区且等待
            {
                byte[] cmd = new byte[] { 0x05, 0x2A, 0x00, 0x00 };
                MixRobot.sendRequestcmd(cmd);       //复合在接转区取/放零件料盘
            }
            if (AGV1.PlatfromStateArray[2]==0x03&&MixRobot.PlatfromStateArray[2] == 0x02 && MixRobot.PlatfromStateArray[3] == 0x02&&Controllock1==false)  //复合达到零件接转区且手臂动作已完成
            {
                if (!ControlFlag3)
                {
                    byte[] cmd = new byte[] { 0x02, 0x2A, 0x00, 0x00 };
                    MixRobot.sendRequestcmd(cmd);      //复合前往双臂装配区
                    ControlFlag3 = true;
                    Controllock1 = true;//加锁
                }
                else
                {
                    byte[] cmd = new byte[] { 0x04, 0x2A, 0x00, 0x00 };
                    MixRobot.sendRequestcmd(cmd);       //复合前往初始位置
                    byte[] cmd1 = new byte[] { 0x03, 0x2A, 0x00, 0x00 };
                    AGV1.sendRequestcmd(cmd1);         //AGV1前往初始位置
                }
            }
            if (AGV2.PlatfromStateArray[2] == 0x03 && MixRobot.PlatfromStateArray[2] == 0x02 && MixRobot.PlatfromStateArray[3] == 0x02 && Controllock1 == false)  //复合达到零件接转区且手臂动作已完成
            {
                if (!ControlFlag3)
                {
                    byte[] cmd = new byte[] { 0x02, 0x2A, 0x00, 0x00 };
                    MixRobot.sendRequestcmd(cmd);      //复合前往双臂装配区
                    ControlFlag3 = true;
                    Controllock1 = true;//加锁
                }
                else
                {
                    byte[] cmd = new byte[] { 0x04, 0x2A, 0x00, 0x00 };
                    MixRobot.sendRequestcmd(cmd);       //复合前往初始位置
                    byte[] cmd1 = new byte[] { 0x03, 0x2A, 0x00, 0x00 };
                    AGV2.sendRequestcmd(cmd1);         //AGV1前往初始位置
                }
            }
            if (MixRobot.PlatfromStateArray[2] == 0x03 && MixRobot.PlatfromStateArray[3] == 0x00)  //复合到达双臂装配处且等待
            {
                byte[] cmd = new byte[] { 0x06, 0x2A, 0x00, 0x00 };
                MixRobot.sendRequestcmd(cmd);     //复合在装配区取/放零件料盘
            }
            if (MixRobot.PlatfromStateArray[2] == 0x03 && MixRobot.PlatfromStateArray[3] == 0x02&&Controllock2==false)  //复合到达双臂装配区且手臂动作已完成
            {
                if (!ControlFlag2)
                {
                    MainWindow.TwoRobotActionFlag0 = true;    //双臂动作0：双臂机器人从零件托盘取出零件
                    ControlFlag2 = true;
                    Controllock2 = true;  //加锁
                }
                else
                {
                    byte[] cmd = new byte[] { 0x01, 0x2A, 0x00, 0x00 };   //双臂前往零件接转区
                    MixRobot.sendRequestcmd(cmd);
                }
            }
            if (AGV1.PlatfromStateArray[2] == 0x01&&AGV1.PlatfromStateArray[3]==0x01&& AGV2.PlatfromStateArray[2] == 0x01&&AGV2.PlatfromStateArray[3]==0x01)    //AGV1和AGV2都在初始位置且都有料盘
            {
                byte[] cmd = new byte[] { 0x01, 0x2A, 0x00, 0x00 };   //AGV1前往立库料台处
                AGV1.sendRequestcmd(cmd);
            }
            if (AGV1.PlatfromStateArray[2] == 0x02 && AGV1.PlatfromStateArray[3] == 0x01)    //AGV1到达立库料台处且有料盘
            {
                if (!ControlOnlyOneFlag5)
                {
                    byte[] cmd = new byte[] { 0x04, 0x2A, 0x00, 0x00 };
                    AGV1.sendRequestcmd(cmd);                                    //AGV1前往立库料台送货
                    byte[] cmd1 = new byte[] { 0x02, 0x0A, 0x00, 0x01 };
                    TWH.sendRequestcmd(cmd1);                             //入库
                    ControlOnlyOneFlag5 = true;
                }
            }
            if (AGV1.PlatfromStateArray[2] == 0x03 && AGV1.PlatfromStateArray[4] == 0x02)   //平台AGV1已达到立库料台且将料盘放回立库料台
            {
                byte[] cmd = new byte[] { 0x03, 0x2A, 0x00, 0x00 };
                AGV1.sendRequestcmd(cmd);                        //平台AGV1前往初始位置
            }
            if (AGV2.PlatfromStateArray[2] == 0x01 && AGV2.PlatfromStateArray[3] == 0x01 && InlibCompleted == true)  //平台AGV2在初始位置且有料盘且AGV1送得料盘入库成功
            {
                if (!ControlOnlyOneFlag6)
                {
                    byte[] cmd = new byte[] { 0x04, 0x2A, 0x00, 0x00 };     //平台AGV2前往立库料台送货
                    AGV2.sendRequestcmd(cmd);
                    byte[] cmd1 = new byte[] { 0x02, 0x0A, 0x00, 0x01 };
                    TWH.sendRequestcmd(cmd);                                   //入库
                    ControlOnlyOneFlag6 = true;
                    InlibCompleted = false;                   
                }
            }
            if (AGV2.PlatfromStateArray[2] == 0x03 && AGV2.PlatfromStateArray[4] == 0x02)    //平台AGV2已到达立库料台处且将料盘放回立库料台
            {
                byte[] cmd = new byte[] { 0x03, 0x2A, 0x00, 0x00 };
                AGV2.sendRequestcmd(cmd);                                      //平台AGV2前往初始位置
            }
            if (AGV2.PlatfromStateArray[2] == 0x04 && AGV2.PlatfromStateArray[3] == 0x00)       //平台AGV2正在前往初始位置且无料盘
            {
                LastStep = true;
            }
            if (AGV2.PlatfromStateArray[2] == 0x01 && AGV2.PlatfromStateArray[3] == 0x00 && LastStep == true)
            {
                MainWindow.CLkOutlibSignal1 = true;              //在次循环一次
                LastStep = false;
            }
            if (TwoRobotActionCompleted0==true)   //双臂动作0执行完成
            {
                byte[] cmd = new byte[] { 0x06, 0x2A, 0x00, 0x00 };
                MixRobot.sendRequestcmd(cmd);     //复合在装配区取/放零件料盘
                TwoRobotActionCompleted0 = false;                    
                Controllock1 = false;   //解锁
                Controllock2 = false;   //解锁
            }



            
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace WMS
{
    class test1
    {
        public static bool OutlibCompleted = false;    //出库完成      
        public static bool LastStep = false;           //最后一步，平台AGV2前往初始位置且无料盘
        public static int Count = 0;         //计算进入到CLkOutlibSignal1 代码块中得次数   //这个在主程序中刷新  //控制AGV1,AGV2能够走2遍
        public static int TwoActionCount0 = 0;     //动作0的次数      //这个在主程序中刷新  //控制双臂执行动作0四次后进行装配
        public static bool MixtoAGVOneStep = false;         //复合去AGV1接货
        public static bool MixtoAGVTwoStep = false;          //复合去AGV1送货
        public static bool TwoRobotActionComplete0 = false;        //双臂动作0完成标识
        public static bool TwoRobotActionComplete2 = false;        //双臂动作2完成标识
        public static bool MixInTwoCanLastStep = false;     //复合在双臂可以执行最后一步
        public static bool MixInTwoTakeAssembly = false;    //复合在双臂已经拿到成品
        public static bool FAGVInSafeArea = false;         //FAGV在安全区
        public static bool AGV1Falg = false;  //AGV1在零件接转区标识
        public static bool AGV2Flag = false;   //AGV2在零件接转区标识
        public static bool AGV1delaytime = false;     //AGV1延时
        public static bool AGV2delaytime = false;     //AGV2延时
        public static bool AGV2delaytime1 = false;     //AGV2延时在立库料台送料完成后延时
        public static bool delayOutlib = false;        //延时出库
        public static bool FrontPart = true;      //前半部分先执行
       


        public static bool AssemblyCompleted = false;   //双臂动作1完成标识  装配完成标识
        public static bool AssemblyOutlibCompleted = false;   //成品料盘出库完成标识
   

        //以下6个需要刷新
        public static bool ControlOnlyOneFalg1 = false;   //控制进入一次标识1
        public static bool ControlOnlyOneFlag2 = false;   //控制进入一次标识2
        public static bool ControlOnlyOneFlag3 = false;    //控制进入一次标识2
        public static bool ControlOnlyOneFlag4 = false;    //控制进入一次标识2
        public static bool ControlOnlyOneFlag5 = false;    //控制进入一次标识2
        public static bool ControlOnlyOneFlag6 = false;    //控制进入一次标识2    //这个在主程序中刷新  //控制AGV1,AGV2能够走2遍
        public static bool ControlOnlyOneFlag7 = false;
        public static bool ControlOnlyOneFlag8 = false;
        public static bool ControlOnlyOneFlag9 = false;
        public static bool ControlOnlyOneFlag10 = false;
        public static bool ControlOnlyOneFlag11 = false;
        public static bool ControlOnlyOneFlag12 = false;
        public static bool ControlOnlyOneFlag13 = false;
        public static bool ControlOnlyOneFlag14 = false;
        public static bool ControlOnlyOneFlag15 = false;
        public static bool ControlOnlyOneFlag16 = false;
        public static bool ControlOnlyOneFlag17 = false;
        public static bool ControlOnlyOneFlag18= false;
        public static bool ControlOnlyOneFlag19 = false;
        public static bool ControlOnlyOneFlag20 = false;
        public static bool ControlOnlyOneFlag21 = false;
        public static bool ControlOnlyOneFlag22 = false;
        public static bool ControlOnlyOneFlag23 = false;
        public static bool ControlOnlyOneFlag24 = false;
        public static bool ControlOnlyOneFlag25 = false;
        public static bool ControlOnlyOneFlag26 = false;
        public static bool ControlOnlyOneFlag27 = false;
        public static bool ControlOnlyOneFlag28 = false;
        public static bool ControlOnlyOneFlag29 = false;
       
        public static bool ControlLock1 = false;        //控制锁   /控制只通知双臂机器人一次
        public static bool ControlLock2 = false;        //控制锁   /控制只通知双臂机器人一次       

        public static void RefleshFlags()
        {
            OutlibCompleted = false;    //出库完成   
            LastStep = false;           //最后一步，平台AGV2前往初始位置且无料盘
            MixtoAGVOneStep = false;         //复合去AGV1接货
            MixtoAGVTwoStep = false;          //复合去AGV1送货
            TwoRobotActionComplete0 = false;        //双臂动作0完成标识
            TwoRobotActionComplete2 = false;        //双臂动作2完成标识
            MixInTwoCanLastStep = false;     //复合在双臂可以执行最后一步
            MixInTwoTakeAssembly = false;    //复合在双臂已经拿到成品
            FAGVInSafeArea = false;         //FAGV在安全区
            AGV1Falg = false;  //AGV1在零件接转区标识
            AGV2Flag = false;   //AGV2在零件接转区标识
            AGV1delaytime = false;     //AGV1延时
            AGV2delaytime = false;     //AGV2延时
            AGV2delaytime1 = false;     //AGV2延时在立库料台送料完成后延时
            delayOutlib = false;        //延时出库
            AssemblyCompleted = false;   //双臂动作1完成标识  装配完成标识
            AssemblyOutlibCompleted = false;   //成品料盘出库完成标识

            ControlOnlyOneFalg1 = false;   //控制进入一次标识1
            ControlOnlyOneFlag2 = false;   //控制进入一次标识2
            ControlOnlyOneFlag3 = false;    //控制进入一次标识2
            ControlOnlyOneFlag4 = false;    //控制进入一次标识2
            ControlOnlyOneFlag5 = false;    //控制进入一次标识2
      //      ControlOnlyOneFlag6 = false;    //控制进入一次标识2
            ControlOnlyOneFlag7 = false;
            ControlOnlyOneFlag8 = false;
            ControlOnlyOneFlag9 = false;
            ControlOnlyOneFlag10 = false;
            ControlOnlyOneFlag11 = false;
            ControlOnlyOneFlag12 = false;
            ControlOnlyOneFlag13 = false;
            ControlOnlyOneFlag14 = false;
            ControlOnlyOneFlag15 = false;
            ControlOnlyOneFlag16 = false;
            ControlOnlyOneFlag17 = false;
            ControlOnlyOneFlag18 = false;
            ControlOnlyOneFlag19 = false;
            ControlOnlyOneFlag20 = false;
            ControlOnlyOneFlag21 = false;
            ControlOnlyOneFlag22 = false;
            ControlOnlyOneFlag23 = false;
            ControlOnlyOneFlag24 = false;
            ControlOnlyOneFlag25 = false;
            ControlOnlyOneFlag26 = false;
            ControlOnlyOneFlag27 = false;
            ControlOnlyOneFlag28 = false;
            ControlOnlyOneFlag29 = false;

            ControlLock1 = false;
            ControlLock2 = false;
        }
        public static void watchexecuteState()
        {
            while (MainWindow.AutoMode1)
            {
                if (FrontPart == true)
                {
                    if (MainWindow.CLkOutlibSignal1 == true)   //界面UI要求零件开始工作
                    {
                        Count++;
                        if (Count <= 2)
                        {
                            RefleshFlags();
                            //////这里有一个出库指令
                            byte[] cmd = new byte[] { 0x02, 0x0A, 0x00, 0x02 };
                            TWH.sendRequestcmd(cmd);   //出库指令
                            ModifyTxt.PrintlogTxt("//出库指令");
                            byte[] cmd1 = new byte[] { 0x01, 0x2A, 0x00, 0x00 };  //发送AGV1去往立库料台命令                            
                            if (AGV1.PlatfromStateArray[2] != 0x05)   //如果AGV1不是正在去往料台
                            {
                                AGV1.sendRequestcmd(cmd1);
                                ModifyTxt.PrintlogTxt("发送AGV1去往立库料台命令");
                            }
                            MainWindow.CLkOutlibSignal1 = false;
                        }
                    }
                    if (AGV1.PlatfromStateArray[2] == 0x02 && AGV1.PlatfromStateArray[3] == 0x00 && OutlibCompleted == true)   //AGV1达到立库料台处且无料盘且出库执行成功
                    {
                        byte[] cmd = new byte[] { 0x05, 0x2A, 0x00, 0x00 };   //AGV1在立库料台取货
                        AGV1.sendRequestcmd(cmd);
                        ModifyTxt.PrintlogTxt("//AGV1在立库料台取货");
                        OutlibCompleted = false;
                    }
                    if (AGV1.PlatfromStateArray[4] == 0x01)    //AGV1已从料台取出料盘
                    {
                        if (!ControlOnlyOneFlag26)
                        {
                            byte[] cmd = new byte[] { 0x02, 0x2A, 0x00, 0x00 };   //AGV1前往零件接转区
                            AGV1.sendRequestcmd(cmd);
                            ModifyTxt.PrintlogTxt("//AGV1前往零件接转区");
                            ControlOnlyOneFlag26 = true;
                        }
                        
                    }
                    if (AGV1.PlatfromStateArray[2] == 0x06)   //AGV1正在前往零件结转区
                    {
                        if (!ControlOnlyOneFalg1)   //控制代码块只进入一次，这里保证只发送一次出库命令
                        {
                            //////这里有一个出库命令
                            byte[] cmd = new byte[] { 0x02, 0x0A, 0x00, 0x02 };
                            TWH.sendRequestcmd(cmd);        //出库命令
                            ModifyTxt.PrintlogTxt("//出库命令");
                            byte[] cmd1 = new byte[] { 0x01, 0x2A, 0x00, 0x00 };  //AGV2前往立库料台处
                            AGV2.sendRequestcmd(cmd1);
                            ModifyTxt.PrintlogTxt("//AGV2前往立库料台处");
                            ControlOnlyOneFalg1 = true;
                        }
                    }
                    if (AGV1.PlatfromStateArray[2] == 0x03)   //AGV1到达零件转接区
                    {
                        if (!ControlOnlyOneFlag2)
                        {
                            AGV1Falg = true;
                            MixtoAGVOneStep = true;    //通知复合来AGV1接货
                            ControlLock1 = false;   //解锁
                            ControlOnlyOneFlag2 = true;
                        }
                    }
                    if (AGV1.PlatfromStateArray[2] == 0x01 && AGV1.PlatfromStateArray[3] == 0x01)   //AGV1在初始位置且有料盘
                    {
                        if (!ControlOnlyOneFlag27)
                        {
                            byte[] cmd = new byte[] { 0x01, 0x2A, 0x00, 0x00 };   //AGV1前往立库料台处
                            AGV1.sendRequestcmd(cmd);
                            ModifyTxt.PrintlogTxt("//AGV1前往立库料台处");
                            ControlOnlyOneFlag27 = true;
                        }
                    }
                    if (AGV1.PlatfromStateArray[2] == 0x02 && AGV1.PlatfromStateArray[3] == 0x01 && AGV2.PlatfromStateArray[2] == 0x03)  //AGV1在立库料台处且有料盘且AGV2在零件接转区
                    {
                        if (!ControlOnlyOneFlag11)
                        {
                            byte[] cmd = new byte[] { 0x04, 0x2A, 0x00, 0x00 };   //AGV1将料盘送回立库
                            AGV1.sendRequestcmd(cmd);
                            ModifyTxt.PrintlogTxt("//AGV1将料盘送回立库");

                            byte[] cmd1 = new byte[] { 0x02, 0x0A, 0x00, 0x01 };
                            TWH.sendRequestcmd(cmd1);    //入库操作
                            ModifyTxt.PrintlogTxt("//入库操作");
                            ControlOnlyOneFlag11 = true;
                        }
                        ///这里有一个入库动作
                    }
                    if (AGV1.PlatfromStateArray[2] == 0x02 && AGV1.PlatfromStateArray[4] == 0x02)      //AGV1在立库料台处已将料盘送回立库
                    {
                        if (!ControlOnlyOneFlag28)
                        {
                            byte[] cmd = new byte[] { 0x03, 0x2A, 0x00, 0x00 };    //AGV1前往初始位置
                            AGV1.sendRequestcmd(cmd);
                            ModifyTxt.PrintlogTxt("//AGV1前往初始位置");
                            ControlOnlyOneFlag28 = true;
                        }
                    }
                    if (AGV2.PlatfromStateArray[2] == 0x02 && AGV2.PlatfromStateArray[3] == 0x00 && OutlibCompleted == true)   //AGV2在立库料台处且无料盘且出库完成
                    {
                        byte[] cmd = new byte[] { 0x05, 0x2A, 0x00, 0x00 };   //AGV2在立库料台取货
                        AGV2.sendRequestcmd(cmd);
                        ModifyTxt.PrintlogTxt(" //AGV2在立库料台取货");
                        OutlibCompleted = false;
                    }
                    if (AGV2.PlatfromStateArray[4] == 0x01)  //AGV2已从立库料台处取货
                    {
                        if (AGV1.PlatfromStateArray[2] == 0x04)  //AGV1正在前往原点
                        {
                            if (!ControlOnlyOneFlag29)
                            {
                                byte[] cmd = new byte[] { 0x02, 0x2A, 0x00, 0x00 };
                                AGV2.sendRequestcmd(cmd);                       //AGV2前往零件转接区
                                ModifyTxt.PrintlogTxt("//AGV2前往零件转接区");
                                ControlOnlyOneFlag29 = true;
                            }
                        }
                    }
                    if (AGV2.PlatfromStateArray[2] == 0x03)   //AGV2到达零件转接区
                    {
                        if (!ControlOnlyOneFlag3)
                        {
                            AGV2Flag = true;
                            MixtoAGVOneStep = true;    //通知复合来AGV接货
                            ControlLock1 = false;   //解锁
                            ControlOnlyOneFlag14 = false;
                            ControlOnlyOneFlag15 = false;
                            ControlOnlyOneFlag16 = false;
                            ControlOnlyOneFlag17 = false;
                            ControlOnlyOneFlag3 = true;
                        }
                    }
                    if (AGV2.PlatfromStateArray[2] == 0x02 && AGV2.PlatfromStateArray[3] == 0x01 && AGV1.PlatfromStateArray[2] == 0x01)  //AGV2在立库料台处且有料盘且AGV1在初始位置
                    {
                        if (!ControlOnlyOneFlag12)
                        {
                            byte[] cmd = new byte[] { 0x04, 0x2A, 0x00, 0x00 };   //AGV2将料盘送回立库
                            AGV2.sendRequestcmd(cmd);
                            ModifyTxt.PrintlogTxt("//AGV2将料盘送回立库");

                            byte[] cmd1 = new byte[] { 0x02, 0x0A, 0x00, 0x01 };
                            TWH.sendRequestcmd(cmd1);
                            ModifyTxt.PrintlogTxt(" ///入库动作");
                            ControlOnlyOneFlag12 = true;
                        }
                        ///还有个入库动作
                    }
                    if (AGV2.PlatfromStateArray[2] == 0x02 && AGV2.PlatfromStateArray[4] == 0x02)      //AGV2在立库料台处已将料盘送回立库
                    {
                        if (!ControlOnlyOneFlag13)
                        {
                            AGV2delaytime1 = true;     //AGV2延时在立库料台送料完成后延时       
                            ControlOnlyOneFlag13 = true;
                        }
                    }
                    if (AGV2.PlatfromStateArray[2] == 0x04 && AGV2.PlatfromStateArray[3] == 0x00)   //正在前往原点  且无料盘
                    {
                        LastStep = true;
                    }
                    if (AGV2.PlatfromStateArray[2] == 0x01 && AGV2.PlatfromStateArray[3] == 0x00 && LastStep == true)     //AGV2在初始位置且无料盘且最后一步
                    {
                        if (!ControlOnlyOneFlag6)
                        {
                            MainWindow.CLkOutlibSignal1 = true;
                            ControlOnlyOneFlag6 = true;
                            LastStep = false;
                        }
                        if (TwoActionCount0 >= 4)   //如果动作0执行了4次
                        {
                            FrontPart = false;    //开始执行下一部分
                        }
                    }
                    if (MixtoAGVOneStep == true && MixRobot.PlatfromStateArray[2] == 0x01)   //如果AGV要求复合到零件接转区且复合在初始位置
                    {
                        if (!ControlOnlyOneFlag14)
                        {
                            byte[] cmd = new byte[] { 0x01, 0x2A, 0x00, 0x00 };
                            MixRobot.sendRequestcmd(cmd);           //复合前往零件接转区
                            ModifyTxt.PrintlogTxt("复合前往零件接转区");
                            ControlOnlyOneFlag14 = true;
                        }
                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x02 && MixRobot.PlatfromStateArray[3] == 0x00)  //复合在零件接转区且等待
                    {
                        if (!ControlOnlyOneFlag15)
                        {
                            byte[] cmd = new byte[] { 0x05, 0x2A, 0x00, 0x00 };
                            MixRobot.sendRequestcmd(cmd);            //复合在接转区取/放零件料盘
                            ModifyTxt.PrintlogTxt("//复合在接转区取/放零件料盘");
                            ControlOnlyOneFlag15 = true;
                        }
                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x02 && MixRobot.PlatfromStateArray[3] == 0x02 && MixtoAGVOneStep == true)  //f复合在零件接转区且手臂动作已完成
                    {
                        byte[] cmd = new byte[] { 0x02, 0x2A, 0x00, 0x00 };
                        MixRobot.sendRequestcmd(cmd);              //复合去往双臂装配区
                        ModifyTxt.PrintlogTxt(" //复合去往双臂装配区");
                        MixtoAGVOneStep = false;
                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x02 && MixRobot.PlatfromStateArray[3] == 0x02 && MixtoAGVTwoStep == true)   //复合第二次在零件接转区且手臂动作已完成
                    {
                        byte[] cmd = new byte[] { 0x04, 0x2A, 0x00, 0x00 };
                        MixRobot.sendRequestcmd(cmd);          //复合前往初始位置
                        ModifyTxt.PrintlogTxt("//复合前往初始位置");
                        if (AGV1Falg == true)     //这是AGV1在接转区
                        {
                            AGV1delaytime = true;   //AGV1延时3秒再走，以防碰撞                    
                            AGV1Falg = false;
                        }
                        if (AGV2Flag == true)
                        {
                            AGV2delaytime = true;   //AGV2延时3秒再走，以防碰撞                            
                            AGV2Flag = false;
                        }
                        MixtoAGVTwoStep = false;
                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x03 && MixRobot.PlatfromStateArray[3] == 0x00)    //复合在双臂处且等待
                    {
                        if (!ControlOnlyOneFlag16)
                        {
                            byte[] cmd = new byte[] { 0x06, 0x2A, 0x00, 0x00 };
                            MixRobot.sendRequestcmd(cmd);                       //复合在装配区取/放零件托盘
                            ModifyTxt.PrintlogTxt("//复合在装配区取/放零件托盘处");
                            ControlOnlyOneFlag16 = true;
                        }
                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x03 && MixRobot.PlatfromStateArray[3] == 0x02 && ControlLock1 == false)       //复合在双臂处且手臂动作已完成
                    {
                        MainWindow.TwoRobotActionFlag0 = true;   //通知双臂机器人执行动作0
                        ControlLock1 = true;     //该条件不能再进入
                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x03 && MixRobot.PlatfromStateArray[3] == 0x02 && TwoRobotActionComplete0 == true)  //复合在双臂处且手臂动作已完成且双臂动作0完成
                    {
                        if (!ControlOnlyOneFlag17)
                        {
                            byte[] cmd = new byte[] { 0x06, 0x2A, 0x00, 0x00 };
                            MixRobot.sendRequestcmd(cmd);                 //复合在装配区取/放零件料盘
                            ModifyTxt.PrintlogTxt("//复合在装配区取/放零件料盘");
                            ControlOnlyOneFlag17 = true;
                        }
                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x03 && MixRobot.PlatfromStateArray[3] == 0x01 && TwoRobotActionComplete0 == true)   //复合在双臂处且手臂动作未完成且双臂动作0完成
                    {
                        TwoRobotActionComplete0 = false;
                        MixInTwoCanLastStep = true;

                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x03 && MixRobot.PlatfromStateArray[3] == 0x02 && MixInTwoCanLastStep == true)  //复合在双臂处且手臂已经将双臂料盘拿回来
                    {
                        byte[] cmd = new byte[] { 0x01, 0x2A, 0x00, 0x00 };
                        MixRobot.sendRequestcmd(cmd);                     //复合前往零件接转区
                        ModifyTxt.PrintlogTxt("//复合前往零件接转区");
                        TwoActionCount0++;     //动作0加1
                        if (TwoActionCount0 >= 4)   //如果动作0执行了4次
                        {
                            MainWindow.TwoRobotActionFlag1 = true;           //开始执行动作1：装配                          
                        }
                        MixtoAGVTwoStep = true;     //复合第二次前往接转区
                        ControlOnlyOneFlag15 = false;
                        MixInTwoCanLastStep = false;
                    }
                }


                /////////////////////////////////以下为装配完成后的动作//////////////////////////////////////
                if (FrontPart == false)
                {
                    if (AssemblyCompleted == true && MixRobot.PlatfromStateArray[2] == 0x01 && AGV2.PlatfromStateArray[2] == 0x01)   //如果装配完成且复合在初始位置且AGV2在初始位置
                    {
                        if (!ControlOnlyOneFlag25)
                        {
                            delayOutlib = true;     //延时出库
                            ControlOnlyOneFlag25 = true;
                        }
                        /////////这里有一个出库动作////////////////////
                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x01 && FAGVInSafeArea == true)  //如果复合在初始位置且叉车AGV在安全区
                    {
                        if (!ControlOnlyOneFlag18)
                        {
                            byte[] cmd = new byte[] { 0x03, 0x2A, 0x00, 0x00 };  //复合前往成品接转区
                            MixRobot.sendRequestcmd(cmd);
                            ModifyTxt.PrintlogTxt("//复合前往成品接转区");
                            ControlOnlyOneFlag18 = true;
                        }
                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x04 && MixRobot.PlatfromStateArray[3] == 0x00 && FAGVInSafeArea == true)  //复合在成品接转区且等待并且叉车AGV在安全区
                    {
                        if (!ControlOnlyOneFlag19)
                        {
                            byte[] cmd = new byte[] { 0x07, 0x2A, 0x00, 0x00 };   //复合在成品区取/放零件料盘
                            MixRobot.sendRequestcmd(cmd);
                            ModifyTxt.PrintlogTxt("//复合在成品区取/放零件料盘");
                            ControlOnlyOneFlag19 = true;
                        }
                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x04 && MixRobot.PlatfromStateArray[3] == 0x02 && FAGVInSafeArea == true)  //复合在成品区且手臂动作已完成，叉车AGV在安全区
                    {
                        if (!ControlOnlyOneFlag20)
                        {
                            byte[] cmd = new byte[] { 0x02, 0x2A, 0x00, 0x00 };
                            MixRobot.sendRequestcmd(cmd);          //复合前往零件装配区
                            ModifyTxt.PrintlogTxt("//复合前往零件装配区");
                            ControlOnlyOneFlag20 = true;
                        }
                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x03 && MixRobot.PlatfromStateArray[3] == 0x00 && FAGVInSafeArea == true)  //复合在装配区等待且等待 叉车AGV在安全区
                    {
                        if (!ControlOnlyOneFlag21)
                        {
                            byte[] cmd = new byte[] { 0x06, 0x2A, 0x00, 0x00 };   //复合在装配区取/放零件料盘
                            MixRobot.sendRequestcmd(cmd);
                            ModifyTxt.PrintlogTxt("//复合在装配区取/放零件料盘");
                            ControlOnlyOneFlag21 = true;
                        }
                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x03 && MixRobot.PlatfromStateArray[3] == 0x02 && FAGVInSafeArea == true && ControlLock2 == false) //复合在装配区且手臂动作已完成，叉车AGV在安全区
                    {
                        MainWindow.TwoRobotActionFlag2 = true;   //双臂将成品放到料盘中
                        ModifyTxt.PrintlogTxt("//双臂将成品放到料盘中");
                        FAGVInSafeArea = false;
                        ControlLock2 = true;
                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x03 && MixRobot.PlatfromStateArray[3] == 0x02 && TwoRobotActionComplete2 == true)  ////复合在装配区且手臂动作已完成，且双臂动作2完成
                    {
                        if (!ControlOnlyOneFlag22)
                        {
                            byte[] cmd = new byte[] { 0x06, 0x2A, 0x00, 0x00 };   //复合在装配区取/放零件料盘
                            MixRobot.sendRequestcmd(cmd);
                            ModifyTxt.PrintlogTxt("//复合在装配区取/放零件料盘");
                            ControlOnlyOneFlag22 = true;
                        }
                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x03 && MixRobot.PlatfromStateArray[3] == 0x01 && TwoRobotActionComplete2 == true)  ////复合在装配区且手臂动作未完成，且双臂动作2完成
                    {
                        TwoRobotActionComplete2 = false;
                        MixInTwoTakeAssembly = true;
                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x03 && MixRobot.PlatfromStateArray[3] == 0x02 && MixInTwoTakeAssembly == true)  ////复合在装配区且手臂动作已完成，已拿下成品
                    {
                        if (!ControlOnlyOneFlag23)
                        {
                            byte[] cmd = new byte[] { 0x03, 0x2A, 0x00, 0x00 };
                            MixRobot.sendRequestcmd(cmd);             //复合前往成品接转区
                            ModifyTxt.PrintlogTxt("//复合前往成品接转区");
                            ControlOnlyOneFlag23 = true;
                        }
                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x04 && MixRobot.PlatfromStateArray[3] == 0x00 && MixInTwoTakeAssembly == true)  ////复合在成品区且等待，已拿下成品
                    {
                        if (!ControlOnlyOneFlag24)
                        {
                            byte[] cmd = new byte[] { 0x07, 0x2A, 0x00, 0x00 };
                            MixRobot.sendRequestcmd(cmd);            //复合在成品区取/放零件料盘
                            ModifyTxt.PrintlogTxt("//复合在成品区取/放零件料盘");
                            ControlOnlyOneFlag24 = true;
                        }
                    }
                    if (MixRobot.PlatfromStateArray[2] == 0x04 && MixRobot.PlatfromStateArray[3] == 0x02 && MixInTwoTakeAssembly == true)  ////复合在成品区手臂动作已完成，已拿下成品
                    {
                        byte[] cmd = new byte[] { 0x04, 0x2A, 0x00, 0x00 };
                        MixRobot.sendRequestcmd(cmd);           //复合前往初始位置
                        ModifyTxt.PrintlogTxt("//复合前往初始位置");
                        byte cmd1 = 0x03;
                        FAGV.sendRequestcmd(cmd1);     //FAGV前往中转区取料
                        ModifyTxt.PrintlogTxt(" //FAGV前往中转区取料");
                        MixInTwoTakeAssembly = false;

                    }
                    if (FAGV.ExecuteState[3] == 0x03)      //叉车AGV前往中转区取料完成
                    {
                        if (!ControlOnlyOneFlag7)
                        {
                            byte cmd = 0x02;
                            FAGV.sendRequestcmd(cmd);        //叉车AGV前往立库料台送料
                            ModifyTxt.PrintlogTxt(" //叉车AGV前往立库料台送料");
                            ControlOnlyOneFlag7 = true;
                        }
                    }
                    if (FAGV.ExecuteState[3] == 0x02)    //叉车AGV前往立库处送料完成
                    {
                        if (!ControlOnlyOneFlag10)
                        {
                            //这里有一个入库命令
                            byte[] cmd1 = new byte[] { 0x02, 0x0A, 0x00, 0x04 };
                            TWH.sendRequestcmd(cmd1);           //成品入库操作
                            ModifyTxt.PrintlogTxt(" //成品入库操作");
                            byte cmd = 0x05;
                            FAGV.sendRequestcmd(cmd);    //叉车AGV前往初始位置
                            ControlOnlyOneFlag10 = true;
                            ModifyTxt.PrintlogTxt("叉车AGV前往初始位置");
                        }
                    }
                    if (AssemblyOutlibCompleted == true)  //成品料盘出库完成
                    {
                        byte cmd = 0x01;
                        FAGV.sendRequestcmd(cmd);     //FAGV前往立库料台处取料
                        ModifyTxt.PrintlogTxt("//FAGV前往立库料台处取料");
                        AssemblyOutlibCompleted = false;
                    }
                    if (FAGV.ExecuteState[3] == 0x01)    //FAGV前往立库料台取料完成
                    {
                        if (!ControlOnlyOneFlag8)
                        {
                            byte cmd = 0x04;
                            FAGV.sendRequestcmd(cmd);     //FAGV前往中转区送料
                            ModifyTxt.PrintlogTxt("//FAGV前往中转区送料");
                            ControlOnlyOneFlag8 = true;
                        }
                    }
                    if (FAGV.ExecuteState[3] == 0x04)   //FAGV前往中转区送料完成
                    {
                        if (!ControlOnlyOneFlag9)
                        {
                            byte cmd = 0x06;
                            FAGV.sendRequestcmd(cmd);     //FAGV前往安全区
                            ModifyTxt.PrintlogTxt("//FAGV前往安全区");
                            ControlOnlyOneFlag9 = true;
                        }
                    }
                }                                                              
               
            }
            FrontPart = true;
          
        }
        public static void dealysend()                 //延时程序
        {
            while (MainWindow.AutoMode1)
            {               
                if (AGV1delaytime == true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                    byte[] cmd1 = new byte[] { 0x03, 0x2A, 0x00, 0x00 };
                    AGV1.sendRequestcmd(cmd1);     //AGV1前往初始位置
                    ModifyTxt.PrintlogTxt("//AGV1前往初始位置");
                    AGV1delaytime = false;
                }
                if (AGV2delaytime == true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                    byte[] cmd2 = new byte[] { 0x01, 0x2A, 0x00, 0x00 };
                    AGV2.sendRequestcmd(cmd2);            //AGV2前往立库料台处
                    ModifyTxt.PrintlogTxt("//AGV2前往立库料台处");
                    AGV2delaytime = false;
                }
                if (AGV2delaytime1 == true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(80));    //延时80S
                    byte[] cmd = new byte[] { 0x03, 0x2A, 0x00, 0x00 };    //AGV2前往初始位置
                    AGV2.sendRequestcmd(cmd);
                    ModifyTxt.PrintlogTxt(" //AGV2前往初始位置");
                    AGV2delaytime1 = false;
                }
                if (delayOutlib == true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(30));    //延时30s
                    byte[] cmd1 = new byte[] { 0x02, 0x0A, 0x00, 0x05 };
                    TWH.sendRequestcmd(cmd1);       //出库操作
                    ModifyTxt.PrintlogTxt("//成品出库操作");
                    delayOutlib = false;                     
                }
            }
        }
    }
}

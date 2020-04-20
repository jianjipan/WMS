using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS
{
    class Decode
    {
        public static string[] comStateStr = new string[] { "正常应答", "指令码错误", "底盘未使能", "运行模式错误", "系统错误", "机器人未定位", "作业执行失败", "指令中字符串为空" };
        public static string[] TWHState = new string[] { "空闲", "零件入库完成", "零件出库完成", "成品入库完成", "成品出库完成"};
        public static string[] platformLocationStr = new string[] { "空闲", "到达初始位置", "到达立库料台处", "到达零件结转区", "正在去往原点", "正在去往立库料台处", "正在去往零件接转区","错误状态" };
        public static string[] isTrayStr = new string[] { "无料盘", "有料盘" };
        public static string[] WorkStateStr = new string[] { "其他", "平台AGV从料台取出料盘", "平台AGV上料盘放回立库料台" };
        public static string[] DataCheckStr = new string[] { "头校验出错", "异或校验出错" };
        public static string[] MixplatformLocationStr = new string[] { "空闲", "在初始位置", "到达零件接转区处", "到达双臂机器人处", "到达成品结转区处", "正在前往初始位置", "正在前往零件接转区", "正在前往双臂机器人处", "正在前往成品结转区处","错误状态"};
        public static string[] MixLocationStateStr1 = new string[] { "等待", "手臂动作未完成", "手臂动作已完成" };
        public static string[] MixLocationStateStr2 = new string[] { "等待", "手臂动作未完成", "手臂动作已完成"};
        public static string[] MixLocationStateStr3 = new string[] { "等待", "手臂动作未完成", "手臂动作已完成" };
        public static string[] TWOStartSignal = new string[] { "双臂机器人从零件托盘取出零件", "双臂机器人装配", "双臂机器人将成品放到成品托盘", "双臂机器人从成品托盘取出成品", "双臂机器人拆解成品", "双臂机器人将零件放到零件托盘" };
        public static string[] TWOIscomplete = new string[] { "完成", "未完成" };
        public static string[] TWOExecuteState = new string[] { "正常", "异常" };
        public static string[] FAGVcomStateStr = new string[] { "通讯正常", "通讯错误" };
        public static string[] FAGVExecuteState = new string[] { "其他", "叉车AGV前往立库料台处取料完成", "叉车AGV前往立库料台处送料完成", "叉车AGV前往中转区取料完成", "叉车AGV前往中转区送料完成","叉车AGV前往初始位置完成","叉车AGV前往安全区完成","  ","   " };
    }
}

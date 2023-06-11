using System.Collections.Generic;
using WpfProductionLineApp.DobotCommunicationsFrameWork.CommunicationClassification;

namespace WpfProductionLineApp.DobotCommunicationsFrameWork
{
    /// <summary>
    /// 用于存储数据信息，包括点动、PTP等通信指令
    /// </summary>
    static class CommunicationProtocol
    {
        /*----------------------------- ID字典存储 -----------------------------------*/
        /*----------------------------根据需要自行补充--------------------------------*/
        public static Dictionary<int, IType> idPairss = new Dictionary<int, IType>()
        {
            {0,new Chars()},//设置和获取设备序列号
            {1,new Chars()},//设置和获取设备名称
            {2,new Uint8s()},//获取设备版本号
            {3,new Uint8s()},//设置和获取设备滑轨状态
            {4,new Uint32s() },//获取设备系统滴答时钟
            {5,new Uint32s() },//获取设备 UID
            {10,new Floats() },// 获取实时位姿
            {13,new Floats() },//获取滑轨实时位姿
            {20,new Uint8s() },//获取报警状态、 清除系统的所有报警
            {30,new Uint64AndFloat() },//设置和获取零位参数
            {31,new Uint64AndUint8() },//执行回零命令
            {40,new Uint32s()},//设置和获取手持示教触发模式
            {41,new Uint8s() },//设置和获取触发模式输出使能/禁止
            {42,new Uint8s() },//获取触发输出状态
            {60,new Uint64AndFloat() },//设置和获取末端执行器参数
            {61,new Uint64AndUint8() },//设置和获取激光输出
            {62, new Uint64AndUint8()},//设置和获取吸盘输出
            {63, new Uint64AndUint8()},//设置和获取爪子输出
            /*-------JOG----------*/
            {70,new Uint64AndFloat()},//设置和获取关节点动参数
            {71,new Uint64AndFloat() },//设置和获取坐标轴点动参数
            {72,new Uint64AndFloat() },//设置和获取点动公共参数
            {73,new Uint64AndUint8() },//执行点动功能--Jog
            {74,new Uint64AndFloat() },//设置和获取滑轨L点动参数

            /*---PTP----*/
            {80, new Uint64AndFloat()},//设置和获取关节点位参数
            {81,new Uint64AndFloat()},//设置和获取坐标轴点位参数
            {82,new Uint64AndFloat()},//设置和获取门型模式点位参数
            {83,new Uint64AndFloat()},//设置和获取点位公共参数
            {84,new Uint64AndUint8()},//执行点位功能 ---PTP
            {85,new Uint64AndFloat()},//设置和获取滑轨关节点位参数
            {86,new Uint64AndUint8()},//执行带滑轨点位功能
            {87,new Uint64AndFloat()},//设置和获取门型模式点位参数2
            {246,new Uint64s()}//获取指令队列索引
        };

        /*----------------      报警状态字典              --------------*/
        public static Dictionary<int, string> alarmStateDic = new Dictionary<int, string>()
        {
            {0,"复位报警" }, {1,"未定义指令报警" }, {2,"文件系统错误报警" }, {3,"MCU 与 FPGA 通信失败报警" },
            {4,"角度传感器读取错误报警" }, {16,"规划报警-逆解算奇异报警" }, {17,"规划报警-逆解算无解报警" }, {18,"规划报警-逆解算限位报警" },
            {19,"规划报警-数据重复" },{21,"JUMP 参数错误" },{32,"运动报警-逆解算奇异报警" },{33,"运动报警-逆解算无解报警" },{34,"运动报警-逆解算限位报警" },
            {48,"关节 1 超速" },{49,"关节 2 超速" },{50,"关节 3 超速" },{51,"关节 4 超速" },
            {64,"关节 1 正向限位报警" },{65,"关节 1 负向限位报警" },{66,"关节 2 正向限位报警" },{67,"关节 2 负向限位报警" },
            {68,"关节 3 正向限位报警" },{69,"关节 3 负向限位报警" },{70,"关节 4 正向限位报警" },{71,"关节 4 负向限位报警" },
            {72,"平行四边形正向限位报警" },{73,"平行四边形负向限位报警" },{80,"关节 1 丢步" },{81,"关节 2 丢步" },
            {82,"关节 3 丢步" },{83,"关节 4 丢步"}
        };


        /*-------------------  设置和获取设备信息 -----------------------*/
        public static byte[] getDeviceName_Base = { 0xAA, 0xAA, 0x02 };

        /*-------------------  设置和获取点动相关参数 -----------------------*/
        public static byte[] jogCmd_Base = { 0xAA, 0xAA, 0x04 };
        public static byte[] getJogJointParams_Base = { 0xAA, 0xAA, 0x02 };
        public static byte[] getJOGCoordinateParams_Base = { 0xAA, 0xAA, 0x02 };
        public static byte[] getJOGCommonParams_Base = { 0xAA, 0xAA, 0x02 };

        /*---------------------   PTP(再现)指令--- 门型坐标轴运动、关节运动、直线运动   ---------------------*/
        public static byte[] PTP_Base = { 0xAA, 0xAA, 0x13 };
        public static byte[] getPTPJumpParams_Base = { 0xAA, 0xAA, 0x02 };
        public static byte[] getPTPJump2Params_Base = { 0xAA, 0xAA, 0x02 };
        public static byte[] getPTPJointParams_Base = { 0xAA, 0xAA, 0x02 };
        public static byte[] getPTPCoordinateParams_Base = { 0xAA, 0xAA, 0x02 };
        public static byte[] getPTPCommonParams_Base = { 0xAA, 0xAA, 0x02 };


        /*----------------------   设置和获取吸盘输出参数 ---------------------------*/

        public static byte[] SetEndEffectorSuctionCup_Base = { 0xAA, 0xAA, 0x04 };
        public static byte[] GetEndEffectorSuctionCup_Base = { 0xAA, 0xAA, 0x02 };


        /* ----------------------   设置/获取 执行回零功能 ---------------------*/
        public static byte[] setHomeCmd_Base = { 0xAA, 0xAA, 0x06 };
        public static byte[] setHomeParams_Base = { 0xAA, 0xAA, 0x12 };
        public static byte[] getHomeCmd_Base = { 0xAA, 0xAA, 0x02 };


        /*---------------------------  队列指令相关 ---------------------------*/

        public static byte[] setQueuedCmdClear_Base = { 0xAA, 0xAA, 0x02 };
        public static byte[] getQueuedCmdCurrentIndex_Base = { 0xAA, 0xAA, 0x02 };
        public static byte[] setQueuedCmdStartExec_Base = { 0xAA, 0xAA, 0x02 };
        public static byte[] setQueuedCmdStopExec_Base = { 0xAA, 0xAA, 0x02 };

        /*------------------------------  实时位姿 ------------------------------*/

        public static byte[] getPose_Base = { 0xAA, 0xAA, 0x02 };


        /*-------------------------------- 报警功能-------------------------------*/
        public static byte[] clearAllAlarmsState_Base = { 0xAA, 0xAA, 0x02 };
        public static byte[] GetAlarmsState_Base = { 0xAA, 0xAA, 0x02 };
    }
}

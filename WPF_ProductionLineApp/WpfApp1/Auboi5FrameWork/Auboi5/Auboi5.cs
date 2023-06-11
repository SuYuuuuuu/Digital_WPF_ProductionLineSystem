using System;
using System.Runtime.InteropServices;

namespace WpfProductionLineApp.Auboi5FrameWork
{
    public class Auboi5
    {
        public const int RSERR_SUCC = 0;
        //关节个数
        public const int ARM_DOF = 6;
        //机械臂IP地址
        public const string robotIP = "192.168.2.2";
        //机械臂端口号
        public const int serverPort = 8899;
        //M_PI
        public const double M_PI = 3.14159265358979323846;

        //接口板用户DI地址
        const int ROBOT_IO_F1 = 30;
        const int ROBOT_IO_F2 = 31;
        const int ROBOT_IO_F3 = 32;
        const int ROBOT_IO_F4 = 33;
        const int ROBOT_IO_F5 = 34;
        const int ROBOT_IO_F6 = 35;
        const int ROBOT_IO_U_DI_00 = 36;
        const int ROBOT_IO_U_DI_01 = 37;
        const int ROBOT_IO_U_DI_02 = 38;
        const int ROBOT_IO_U_DI_03 = 39;
        const int ROBOT_IO_U_DI_04 = 40;
        const int ROBOT_IO_U_DI_05 = 41;
        const int ROBOT_IO_U_DI_06 = 42;
        const int ROBOT_IO_U_DI_07 = 43;
        const int ROBOT_IO_U_DI_10 = 44;
        const int ROBOT_IO_U_DI_11 = 45;
        const int ROBOT_IO_U_DI_12 = 46;
        const int ROBOT_IO_U_DI_13 = 47;
        const int ROBOT_IO_U_DI_14 = 48;
        const int ROBOT_IO_U_DI_15 = 49;
        const int ROBOT_IO_U_DI_16 = 50;
        const int ROBOT_IO_U_DI_17 = 51;

        //接口板用户DO地址
        const int ROBOT_IO_U_DO_00 = 32;
        const int ROBOT_IO_U_DO_01 = 33;
        const int ROBOT_IO_U_DO_02 = 34;
        const int ROBOT_IO_U_DO_03 = 35;
        const int ROBOT_IO_U_DO_04 = 36;
        const int ROBOT_IO_U_DO_05 = 37;
        const int ROBOT_IO_U_DO_06 = 38;
        const int ROBOT_IO_U_DO_07 = 39;
        const int ROBOT_IO_U_DO_10 = 40;
        const int ROBOT_IO_U_DO_11 = 41;
        const int ROBOT_IO_U_DO_12 = 42;
        const int ROBOT_IO_U_DO_13 = 43;
        const int ROBOT_IO_U_DO_14 = 44;
        const int ROBOT_IO_U_DO_15 = 45;
        const int ROBOT_IO_U_DO_16 = 46;
        const int ROBOT_IO_U_DO_17 = 47;

        //接口板用户AI地址
        const int ROBOT_IO_VI0 = 0;
        const int ROBOT_IO_VI1 = 1;
        const int ROBOT_IO_VI2 = 2;
        const int ROBOT_IO_VI3 = 3;

        //接口板用户AO地址
        const int ROBOT_IO_VO0 = 0;
        const int ROBOT_IO_VO1 = 1;
        const int ROBOT_IO_CO0 = 2;
        const int ROBOT_IO_CO1 = 3;

        //接口板IO类型
        //
        //接口板用户DI(数字量输入)　可读可写
        const int Robot_User_DI = 4;
        //接口板用户DO(数字量输出)  可读可写
        const int Robot_User_DO = 5;
        //接口板用户AI(模拟量输入)  可读可写
        const int Robot_User_AI = 6;
        //接口板用户AO(模拟量输出)  可读可写
        const int Robot_User_AO = 7;

        //工具端IO类型
        //
        //工具端DI
        const int Robot_Tool_DI = 8;
        //工具端DO
        const int Robot_Tool_DO = 9;
        //工具端AI
        const int Robot_Tool_AI = 10;
        //工具端AO
        const int Robot_Tool_AO = 11;
        //工具端DI
        const int Robot_ToolIoType_DI = Robot_Tool_DI;
        //工具端DO
        const int Robot_ToolIoType_DO = Robot_Tool_DO;

        //工具端IO名称
        const string TOOL_IO_0 = ("T_DI/O_00");
        const string TOOL_IO_1 = ("T_DI/O_01");
        const string TOOL_IO_2 = ("T_DI/O_02");
        const string TOOL_IO_3 = ("T_DI/O_03");

        //工具端数字IO类型
        //
        //输入
        const int TOOL_IO_IN = 0;
        //输出
        const int TOOL_IO_OUT = 0;

        //工具端电源类型
        //
        const int OUT_0V = 0;
        const int OUT_12V = 1;
        const int OUT_24V = 2;

        //IO状态-无效
        const double IO_STATUS_INVALID = 0.0;
        //IO状态-有效
        const double IO_STATUS_VALID = 1.0;

        //坐标系枚举
        const int BaseCoordinate = 0;
        const int EndCoordinate = 1;
        const int WorldCoordinate = 2;

        //坐标系标定方法
        const int Origin_AnyPointOnPositiveXAxis_AnyPointOnPositiveYAxis = 0; // 原点、x轴正半轴、y轴正半轴
        const int Origin_AnyPointOnPositiveYAxis_AnyPointOnPositiveZAxis = 1; // 原点、y轴正半轴、z轴正半轴
        const int Origin_AnyPointOnPositiveZAxis_AnyPointOnPositiveXAxis = 2; // 原点、z轴正半轴、x轴正半轴
        const int Origin_AnyPointOnPositiveXAxis_AnyPointOnFirstQuadrantOfXOYPlane = 3; // 原点、x轴正半轴、x、y轴平面的第一象限上任意一点
        const int Origin_AnyPointOnPositiveXAxis_AnyPointOnFirstQuadrantOfXOZPlane = 4; // 原点、x轴正半轴、x、z轴平面的第一象限上任意一点
        const int Origin_AnyPointOnPositiveYAxis_AnyPointOnFirstQuadrantOfYOZPlane = 5; // 原点、y轴正半轴、y、z轴平面的第一象限上任意一点
        const int Origin_AnyPointOnPositiveYAxis_AnyPointOnFirstQuadrantOfYOXPlane = 6; // 原点、y轴正半轴、y、x轴平面的第一象限上任意一点
        const int Origin_AnyPointOnPositiveZAxis_AnyPointOnFirstQuadrantOfZOXPlane = 7; // 原点、z轴正半轴、z、x轴平面的第一象限上任意一点
        const int Origin_AnyPointOnPositiveZAxis_AnyPointOnFirstQuadrantOfZOYPlane = 8; // 原点、z轴正半轴、z、y轴平面的第一象限上任意一点

        //运动轨迹类型
        //
        //圆
        const int ARC_CIR = 2;
        //圆弧
        const int CARTESIAN_MOVEP = 3;

        //机械臂状态
        const int RobotStopped = 0;
        const int RobotRunning = 1;
        const int RobotPaused = 2;
        const int RobotResumed = 3;

        //机械臂工作模式
        const int RobotModeSimulator = 0; //机械臂仿真模式
        const int RobotModeReal = 1; //机械臂真实模式

        //路点位置信息的表示方法
        [StructLayout(LayoutKind.Sequential)]
        public struct Pos
        {
            public double x;
            public double y;
            public double z;
        }

        //路点位置信息的表示方法
        [StructLayout(LayoutKind.Sequential)]
        public struct cartesianPos_U
        {
            // 指定数组尺寸
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public double[] positionVector;
        };

        //姿态的四元素表示方法
        [StructLayout(LayoutKind.Sequential)]
        public struct Ori
        {
            public double w;
            public double x;
            public double y;
            public double z;
        };

        //姿态的欧拉角表示方法
        [StructLayout(LayoutKind.Sequential)]
        public struct Rpy
        {
            public double rx;
            public double ry;
            public double rz;
        };

        //描述机械臂的路点信息
        [StructLayout(LayoutKind.Sequential)]
        public struct wayPoint_S
        {
            //机械臂的位置信息　X,Y,Z
            public Pos cartPos;
            //机械臂姿态信息
            public Ori orientation;
            //机械臂关节角信息
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = ARM_DOF)]
            public double[] jointpos;
        };

        //机械臂关节速度加速度信息
        [StructLayout(LayoutKind.Sequential)]
        public struct JointVelcAccParam
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = ARM_DOF)]
            public double[] jointPara;
        };

        //机械臂关节角度
        [StructLayout(LayoutKind.Sequential)]
        public struct JointRadian
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = ARM_DOF)]
            public double[] jointRadian;
        };

        //机械臂工具端参数
        [StructLayout(LayoutKind.Sequential)]
        public struct ToolInEndDesc
        {
            //工具相对于末端坐标系的位置
            public Pos cartPos;
            //工具相对于末端坐标系的姿态
            public Ori orientation;
        };

        //坐标系结构体
        [StructLayout(LayoutKind.Sequential)]
        public struct CoordCalibrate
        {
            //坐标系类型：当coordType==BaseCoordinate或者coordType==EndCoordinate是，下面3个参数不做处理
            public int coordType;
            //坐标系标定方法
            public int methods;
            //用于标定坐标系的３个点（关节角），对应于机械臂法兰盘中心点基于基座标系
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public JointRadian[] jointPara;
            //标定的时候使用的工具描述
            public ToolInEndDesc toolDesc;
        };

        //转轴定义
        [StructLayout(LayoutKind.Sequential)]
        public struct MoveRotateAxis
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public double[] rotateAxis;
        };

        //描述运动属性中的偏移属性
        [StructLayout(LayoutKind.Sequential)]
        public struct MoveRelative
        {
            //是否使能偏移
            public byte enable;
            //偏移量 x,y,z
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] pos;
            //public Pos pos;
            //相对姿态偏移量
            public Ori orientation;
        };

        //该结构体描述工具惯量
        [StructLayout(LayoutKind.Sequential)]
        public struct ToolInertia
        {
            public double xx;
            public double xy;
            public double xz;
            public double yy;
            public double yz;
            public double zz;
        };

        //动力学参数
        [StructLayout(LayoutKind.Sequential)]
        public struct ToolDynamicsParam
        {
            public double positionX; //工具重心的X坐标
            public double positionY; //工具重心的Y坐标
            public double positionZ; //工具重心的Z坐标
            public double payload; //工具重量
            public ToolInertia toolInertia; //工具惯量
        };

        //机械臂事件
        [StructLayout(LayoutKind.Sequential)]
        public struct RobotEventInfo
        {
            public int eventType; //事件类型号
            public int eventCode; //事件代码
            public IntPtr eventContent; //事件内容(std::string)
        };

        //初始化机械臂控制库
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_initialize", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_initialize();

        //反初始化机械臂控制库
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_uninitialize", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_uninitialize();

        //创建机械臂控制上下文句柄
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_create_context", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_create_context(ref UInt16 rshd);

        //注销机械臂控制上下文句柄
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_destory_context", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_destory_context(UInt16 rshd);

        //链接机械臂服务器
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_login", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_login(UInt16 rshd, [MarshalAs(UnmanagedType.LPStr)] string addr, int port);

        //断开机械臂服务器链接
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_logout", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_logout(UInt16 rshd);

        //初始化全局的运动属性
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_init_global_move_profile", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_init_global_move_profile(UInt16 rshd);

        //设置六个关节轴动的最大速度（最大为180度/秒），注意如果没有特殊需求，6个关节尽量配置成一样！
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_global_joint_maxvelc", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_global_joint_maxvelc(UInt16 rshd, double[] max_velc);

        //获取六个关节轴动的最大速度
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_get_global_joint_maxvelc", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_get_global_joint_maxvelc(UInt16 rshd, ref JointVelcAccParam max_velc);

        //设置六个关节轴动的最大加速度 （十倍的最大速度），注意如果没有特殊需求，6个关节尽量配置成一样！
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_global_joint_maxacc", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_global_joint_maxacc(UInt16 rshd, double[] max_acc);

        //获取六个关节轴动的最大加速度
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_get_global_joint_maxacc", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_get_global_joint_maxacc(UInt16 rshd, ref JointVelcAccParam max_acc);

        //设置机械臂末端最大线加速度
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_global_end_max_line_acc", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_global_end_max_line_acc(UInt16 rshd, double max_acc);

        //设置机械臂末端最大线速度
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_global_end_max_line_velc", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_global_end_max_line_velc(UInt16 rshd, double max_velc);

        //获取机械臂末端最大线加速度
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_get_global_end_max_line_acc", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_get_global_end_max_line_acc(UInt16 rshd, ref double max_acc);

        //获取机械臂末端最大线速度
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_get_global_end_max_line_velc", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_get_global_end_max_line_velc(UInt16 rshd, ref double max_velc);

        //设置机械臂末端最大角加速度
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_global_end_max_angle_acc", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_global_end_max_angle_acc(UInt16 rshd, double max_acc);

        //设置机械臂末端最大角速度
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_global_end_max_angle_velc", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_global_end_max_angle_velc(UInt16 rshd, double max_velc);

        //获取机械臂末端最大角加速度
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_get_global_end_max_angle_acc", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_get_global_end_max_angle_acc(UInt16 rshd, ref double max_acc);

        //获取机械臂末端最大角加速度
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_get_global_end_max_angle_velc", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_get_global_end_max_angle_velc(UInt16 rshd, ref double max_velc);

        //设置用户坐标系
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_user_coord", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_user_coord(UInt16 rshd, ref CoordCalibrate user_coord);

        //设置基座坐标系
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_base_coord", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_base_coord(UInt16 rshd);

        //机械臂轴动
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_move_joint", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_move_joint(UInt16 rshd, double[] joint_radia, bool isblock);

        //机械臂直线运动
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_move_line", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_move_line(UInt16 rshd, double[] joint_radia, bool isblock);

        //保持当前位置变换姿态做旋转运动
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_move_rotate", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_move_rotate(UInt16 rshd, ref CoordCalibrate user_coord, ref MoveRotateAxis rotate_axis, double rotate_angle, bool isblock);

        //清除所有已经设置的全局路点
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_remove_all_waypoint", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_remove_all_waypoint(UInt16 rshd);

        //添加全局路点用于轨迹运动
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_add_waypoint", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_add_waypoint(UInt16 rshd, double[] joint_radia);

        //设置交融半径
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_blend_radius", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_blend_radius(UInt16 rshd, double radius);

        //设置圆运动圈数
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_circular_loop_times", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_circular_loop_times(UInt16 rshd, int times);

        //检查用户坐标系参数设置是否合理
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_check_user_coord", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_check_user_coord(UInt16 rshd, ref CoordCalibrate user_coord);

        //设置基于基座标系运动偏移量
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_relative_offset_on_base", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_relative_offset_on_base(UInt16 rshd, ref MoveRelative relative);

        //设置基于用户标系运动偏移量
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_relative_offset_on_user", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_relative_offset_on_user(UInt16 rshd, ref MoveRelative relative, ref CoordCalibrate user_coord);

        //取消提前到位设置
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_no_arrival_ahead", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_no_arrival_ahead(UInt16 rshd);

        //设置距离模式下的提前到位距离
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_arrival_ahead_distance", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_arrival_ahead_distance(UInt16 rshd, double distance);

        //设置时间模式下的提前到位时间
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_arrival_ahead_time", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_arrival_ahead_time(UInt16 rshd, double sec);

        //轨迹运动
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_move_track", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_move_track(UInt16 rshd, int sub_move_mode, bool isblock);

        //保持当前位姿通过直线运动的方式运动到目标位置
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_move_line_to", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_move_line_to(UInt16 rshd, ref Pos target, ref ToolInEndDesc tool, bool isblock);

        //保持当前位姿通过关节运动的方式运动到目标位置
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_move_joint_to", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_move_joint_to(UInt16 rshd, ref Pos target, ref ToolInEndDesc tool, bool isblock);

        //获取机械臂当前位置信息
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_get_current_waypoint", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_get_current_waypoint(UInt16 rshd, ref wayPoint_S waypoint);

        //正解
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_forward_kin", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_forward_kin(UInt16 rshd, double[] joint_radia, ref wayPoint_S waypoint);

        //逆解
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_inverse_kin", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_inverse_kin(UInt16 rshd, double[] joint_radia, ref Pos pos, ref Ori ori, ref wayPoint_S waypoint);

        //四元素转欧拉角
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_rpy_to_quaternion", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_rpy_to_quaternion(UInt16 rshd, ref Rpy rpy, ref Ori ori);

        //欧拉角转四元素
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_quaternion_to_rpy", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_quaternion_to_rpy(UInt16 rshd, ref Ori ori, ref Rpy rpy);

        //基座坐标系转用户坐标系
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_base_to_user", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_base_to_user(UInt16 rshd, ref Pos pos_onbase, ref Ori ori_onbase, ref CoordCalibrate user_coord, ref ToolInEndDesc tool_pos, ref Pos pos_onuser, ref Ori ori_onuser);

        //用户坐标系转基座坐标系
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_user_to_base", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_user_to_base(UInt16 rshd, ref Pos pos_onuser, ref Ori ori_onuser, ref CoordCalibrate user_coord, ref ToolInEndDesc tool_pos, ref Pos pos_onbase, ref Ori ori_onbase);

        //基坐标系转基座标得到工具末端点的位置和姿态
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_base_to_base_additional_tool", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_base_to_base_additional_tool(UInt16 rshd, ref Pos flange_center_pos_onbase, ref Ori flange_center_ori_onbase, ref ToolInEndDesc tool_pos, ref Pos tool_end_pos_onbase, ref Ori tool_end_ori_onbase);

        //设置工具的运动学参数
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_tool_end_param", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_tool_end_param(UInt16 rshd, ref ToolInEndDesc tool);

        //设置无工具的动力学参数
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_none_tool_dynamics_param", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_none_tool_dynamics_param(UInt16 rshd);

        //根据接口板IO类型和地址设置IO状态
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_board_io_status_by_addr", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_board_io_status_by_addr(UInt16 rshd, int io_type, int addr, double val);

        //根据接口板IO类型和地址获取IO状态
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_get_board_io_status_by_addr", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_get_board_io_status_by_addr(UInt16 rshd, int io_type, int addr, ref double val);

        //设置工具端IO状态
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_tool_do_status", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_tool_do_status(UInt16 rshd, string name, int val);

        //获取工具端IO状态
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_get_tool_io_status", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_get_tool_io_status(UInt16 rshd, string name, ref double val);

        //设置工具端电源电压类型
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_tool_power_type", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_tool_power_type(UInt16 rshd, int type);

        //获取工具端电源电压类型
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_get_tool_power_type", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_get_tool_power_type(UInt16 rshd, ref int type);

        //设置工具端数字量IO的类型
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_tool_io_type", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_tool_io_type(UInt16 rshd, int addr, int type);

        //设置工具的动力学参数
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_tool_dynamics_param", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_tool_dynamics_param(UInt16 rshd, ref ToolDynamicsParam tool);

        //获取工具的动力学参数
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_get_tool_dynamics_param", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_get_tool_dynamics_param(UInt16 rshd, ref ToolDynamicsParam tool);

        //设置无工具运动学参数
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_none_tool_kinematics_param", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_none_tool_kinematics_param(UInt16 rshd);

        //设置工具的运动学参数
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_tool_kinematics_param", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_tool_kinematics_param(UInt16 rshd, ref ToolInEndDesc tool);

        //获取工具的运动学参数
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_get_tool_kinematics_param", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_get_tool_kinematics_param(UInt16 rshd, ref ToolInEndDesc tool);

        //启动机械臂
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_robot_startup", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_robot_startup(UInt16 rshd, ref ToolDynamicsParam tool, byte colli_class, bool read_pos, bool static_colli_detect, int board_maxacc, ref int state);

        //关闭机械臂
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_robot_shutdown", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_robot_shutdown(UInt16 rshd);

        //停止机械臂运动
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_move_stop", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_move_stop(UInt16 rshd);

        //停止机械臂运动
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_move_fast_stop", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_move_fast_stop(UInt16 rshd);

        //暂停机械臂运动
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_move_pause", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_move_pause(UInt16 rshd);

        //暂停后回复机械臂运动
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_move_continue", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_move_continue(UInt16 rshd);

        //机械臂碰撞后恢复
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_collision_recover", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_collision_recover(UInt16 rshd);

        //获取机械臂当前状态
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_get_robot_state", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_get_robot_state(UInt16 rshd, ref int state);

        //设置机械臂服务器工作模式
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_work_mode", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_work_mode(UInt16 rshd, int state);

        //获取机械臂服务器当前工作模式
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_get_work_mode", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_get_work_mode(UInt16 rshd, ref int state);

        //设置机械臂碰撞等级
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_set_collision_class", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_set_collision_class(UInt16 rshd, int grade);

        //获取socket链接状态
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_get_socket_status", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_get_socket_status(UInt16 rshd, ref byte connected);

        //设置是否允许实时路点信息推送
        [DllImport("libserviceinterface.dll", EntryPoint = "rs_enable_push_realtime_roadpoint", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rs_enable_push_realtime_roadpoint(UInt16 rshd, bool enable);

        //实时路点回调函数
        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public delegate void REALTIME_ROADPOINT_CALLBACK(ref wayPoint_S waypoint, IntPtr arg);

        [DllImport("libserviceinterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void rs_setcallback_realtime_roadpoint(UInt16 rshd, [MarshalAs(UnmanagedType.FunctionPtr)] REALTIME_ROADPOINT_CALLBACK CurrentPositionCallback, IntPtr arg);

        //机械臂事件
        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public delegate void ROBOT_EVENT_CALLBACK(ref RobotEventInfo rs_event, IntPtr arg);

        [DllImport("libserviceinterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void rs_setcallback_robot_event(UInt16 rshd, [MarshalAs(UnmanagedType.FunctionPtr)] ROBOT_EVENT_CALLBACK RobotEventCallback, IntPtr arg);

        //回调函数
        public static void CurrentPositionCallback(ref wayPoint_S waypoint, IntPtr arg)
        {
            PrintWaypoint(waypoint);
        }

        //打印路点信息
        public static void PrintWaypoint(wayPoint_S point)
        {
            Console.Out.WriteLine("---------------------------------------------------------------------------------------");
            Console.Out.WriteLine("pos.x={0} y={1} z={2}", point.cartPos.x, point.cartPos.y, point.cartPos.z);
            Console.Out.WriteLine("ori.w={0} x={1} y={2} z={3}", point.orientation.w, point.orientation.x, point.orientation.y, point.orientation.z);
            Console.Out.WriteLine("joint1={0} joint2={1} joint3={2}", point.jointpos[0] * 180 / M_PI, point.jointpos[1] * 180 / M_PI, point.jointpos[2] * 180 / M_PI);
            Console.Out.WriteLine("joint4={0} joint5={1} joint6={2}", point.jointpos[3] * 180 / M_PI, point.jointpos[4] * 180 / M_PI, point.jointpos[5] * 180 / M_PI);
            Console.Out.WriteLine("---------------------------------------------------------------------------------------");
        }


        public static void RobotEventCallback(ref RobotEventInfo rs_event, IntPtr arg)
        {
            Console.Out.WriteLine("---------------------------------------------------------------------------------------");
            Console.Out.WriteLine("robot event.type={0}", rs_event.eventType);
            Console.Out.WriteLine("robot event.eventCode={0}", rs_event.eventCode);
            Console.Out.WriteLine("robot event.eventContent={0}", Marshal.PtrToStringAnsi(rs_event.eventContent));
            Console.Out.WriteLine("---------------------------------------------------------------------------------------");
        }
    }
}

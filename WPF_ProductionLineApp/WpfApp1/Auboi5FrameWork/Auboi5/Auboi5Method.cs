using System;
using static WpfProductionLineApp.Auboi5FrameWork.Auboi5;


namespace WpfProductionLineApp.Auboi5FrameWork
{
    public class Auboi5Method
    {
        static REALTIME_ROADPOINT_CALLBACK RobotPosCallBack;
        static UInt16 rshd = 0xffff;
        private static float[]? jointAngle;
        public static float[]? JointAngle { get => jointAngle; set => jointAngle = value; }

        public static bool Connect()
        {
            int result = 0xffff;
            double[] max_velc = { 1, 1, 1, 1, 1, 1 };
            double[] max_acc = { 3, 3, 3, 3, 3, 3 };
            int state = 0;
            ToolDynamicsParam toolDynamicsParam = new ToolDynamicsParam();

            result = rs_initialize();
            //Console.Out.WriteLine("rs_initialize.ret={0}", result);
            if (RSERR_SUCC == result)
            {
                //创建机械臂控制上下文句柄
                if (rs_create_context(ref rshd) == RSERR_SUCC)
                {
                    //Console.Out.WriteLine("rshd={0}", rshd);
                    //链接机械臂服务器
                    if (rs_login(rshd, robotIP, serverPort) == RSERR_SUCC)
                    {
                        //Console.Out.WriteLine("login succ.");
                        result = rs_robot_startup(rshd, ref toolDynamicsParam, 6, true, true, 5, ref state);
                        if (result == RSERR_SUCC)
                        {
                            //Console.Out.WriteLine("startup rs Succ!");                          
                        }

                        else
                        {
                            //Console.Out.WriteLine("startup rs Failed!");
                            return false;
                        }


                        //设置是否允许实时路点信息推送
                        rs_enable_push_realtime_roadpoint(rshd, true);

                        //函数指针实例化
                        RobotPosCallBack = new REALTIME_ROADPOINT_CALLBACK(CurrentPositionCallback);
                        rs_setcallback_realtime_roadpoint(rshd, RobotPosCallBack, IntPtr.Zero);


                        //初始化全局的运动属性
                        rs_init_global_move_profile(rshd);

                        //设置六个关节轴动的最大加速度
                        rs_set_global_joint_maxacc(rshd, max_acc);

                        //设置六个关节轴动的最大速度
                        rs_set_global_joint_maxvelc(rshd, max_velc);



                        return true;

                    }
                    else
                    {
                        //Console.Error.WriteLine("login failed!");
                        return false;
                    }

                }
                else
                {
                    //Console.Error.WriteLine("rs_create_context failed!");
                    return false;
                }

            }
            else
            {
                //Console.Error.WriteLine("rs_initialize failed!");
                return false;
            }


        }

        public static void DisConnect()
        {
            //断开机械臂服务器链接
            rs_logout(rshd);
            //注销机械臂控制上下文句柄
            rs_destory_context(rshd);
            //反初始化机械臂控制库
            rs_uninitialize();
        }


        public static void CurrentPositionCallback(ref wayPoint_S waypoint, IntPtr arg)
        {
            jointAngle = Array.ConvertAll(waypoint.jointpos, d => (float)Math.Round(d * 180 / M_PI, 2));
        }
    }
}



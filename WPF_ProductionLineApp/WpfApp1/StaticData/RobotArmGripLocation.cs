using System.Collections.Generic;

namespace WpfProductionLineApp.StaticData
{
    struct NumToLoc
    {
        public int _1count;
        public int _2count;
        public int _3count;
        public int _4count;
    } //抓取位置索引结构体
    static class RobotArmGripLocation
    {

        #region 机械臂抓取位置点位
        public static float[] homePara = new float[]
       {
           200f,
           0f,
           30f,
           0f
        };
        public static float[] _1ptpCmd_moveJ1 = new float[]
       {
           164.6393f,
           -47.07957f,
           -43.01137f,
           100f
        };
        public static float[] _1ptpCmd_moveJ2 = new float[]
       {
           164.6393f,
           50.07957f,
           -43.01137f,
           100f
        };
        public static float[] _1ptpCmd_moveJ3 = new float[]
       {
            230.6393f,
            -47.07957f,
            -43.01137f,
            95f
        };
        public static float[] _1ptpCmd_moveJ4 = new float[]
       {
            230.6393f,
            50.07957f,
            -43.01137f,
            95f
        };
        public static float[] _1ptpCmd_moveJ5 = new float[]
       {
           296.6393f,
           -47.07957f,
           -43.01137f,
           95f
        };
        public static float[] _1ptpCmd_moveJ6 = new float[]
       {
           296.6393f,
           50.07957f,
           -43.01137f,
           95f
        };
        public static float[] _1ptpCmd_Jump = new float[]
       {
           77.76627f,
           229.557f,
           -62.31813f,
           0f
        };

        public static float[] _2ptpCmd_moveJ1 = new float[]
       {
            165.2645f,
            -57.16714f,
            -48.03595f,
            -50f
        };
        public static float[] _2ptpCmd_moveJ2 = new float[]
       {
            165.2645f,
            37.16714f,
            -48.03595f,
            -50f
        };
        public static float[] _2ptpCmd_moveJ3 = new float[]
      {
            238.2645f,
            -57.16714f,
            -48.03595f,
            -50f
       };

        public static float[] _2ptpCmd_moveJ4 = new float[]
      {
            238.2645f,
            38.16714f,
            -48.03595f,
            -50f
       };
        public static float[] _2ptpCmd_moveJ5 = new float[]
      {
           312.2645f,
           -55.16714f,
           -48.03595f,
           -50f
       };
        public static float[] _2ptpCmd_moveJ6 = new float[]
     {
           311.2645f,
           40.86714f,
           -48.03595f,
           -48f
      };
        public static float[] _2ptpCmd_Jump = new float[]
     {
           206.1438f,
           -229.301f,
           -60.75597f,
           -48.04415f
      };

        public static float[] _3ptpCmd_moveJ1 = new float[]
    {
           198.0092f,
           -143.0254f,
           -69.50287f,
           -5f
     };
        public static float[] _3ptpCmd_moveJ2 = new float[]
    {
           196.0092f,
           -67.0254f,
           -69.50287f,
           -5f
     };
        public static float[] _3ptpCmd_moveJ3 = new float[]
    {
           196.0092f,
           11.0254f,
           -70.50287f,
           -5f
     };
        public static float[] _3ptpCmd_moveJ4 = new float[]
    {
           196.0092f,
           89.0254f,
           -70.50287f,
           -5f
     };
        public static float[] _3ptpCmd_moveJ5 = new float[]
    {
            294.0151f,
            -84.93764f,
            -71.50287f,
            -93f
     };
        public static float[] _3ptpCmd_moveJ6 = new float[]
    {
            292.0151f,
            67f,
            -71.50287f,
            -93f
     };
        public static float[] _3ptpCmd_Jump = new float[]
    {
            98.79887f,
            232.1253f,
            -55.6809f,
            0f
     };
    //-68.6809f,

        public static float[] _4ptpCmd_moveJ1 = new float[]
    {
            196.4578f,
            -121.5232f,
            -61.5817f,
            5f
     };
        public static float[] _4ptpCmd_moveJ2 = new float[]
    {
            196.4578f,
            -46f,
            -61.5817f,
            5f
     };
        public static float[] _4ptpCmd_moveJ3 = new float[]
    {
            196.4578f,
            27f,
            -61.5817f,
            5f
     };
        public static float[] _4ptpCmd_moveJ4 = new float[]
    {
            196.4578f,
            102f,
            -61.5817f,
            5f
     };
        public static float[] _4ptpCmd_moveJ5 = new float[]
    {
            305.8351f,
            -85f,
            -61.5817f,
            90f
     };
        public static float[] _4ptpCmd_moveJ6 = new float[]
    {
            306.8351f,
            62f,
            -61.5817f,
            92f
     };
        public static float[] _4ptpCmd_Jump = new float[]
    {
            140.1508f,
            -277.2743f,
            -47.7805f,
            0f
     };


        #endregion

        #region 位置数组
        public static List<float[]> locGripArr = new List<float[]>() //抓取点位数组
        {
            _1ptpCmd_moveJ1, _1ptpCmd_moveJ2,_1ptpCmd_moveJ3,_1ptpCmd_moveJ4,
            _1ptpCmd_moveJ5,_1ptpCmd_moveJ6,_2ptpCmd_moveJ1,_2ptpCmd_moveJ2,
            _2ptpCmd_moveJ3,_2ptpCmd_moveJ4,_2ptpCmd_moveJ5,_2ptpCmd_moveJ6,
            _3ptpCmd_moveJ1, _3ptpCmd_moveJ2,_3ptpCmd_moveJ3,_3ptpCmd_moveJ4,
            _3ptpCmd_moveJ5,_3ptpCmd_moveJ6,_4ptpCmd_moveJ1,_4ptpCmd_moveJ2,
            _4ptpCmd_moveJ3,_4ptpCmd_moveJ4,_4ptpCmd_moveJ5,_4ptpCmd_moveJ6
        };

        public static List<float[]> locPlaceArr = new List<float[]>()//放置位置数组
        {
            _1ptpCmd_Jump,_2ptpCmd_Jump,_3ptpCmd_Jump,_4ptpCmd_Jump
        };
        #endregion



    }
}

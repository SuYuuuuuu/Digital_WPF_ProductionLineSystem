using System;
using System.IO.Ports;
using System.Windows;
using static DobotRealTimeData.Types;

namespace WpfProductionLineApp.DobotCommunicationsFrameWork.Common
{


    public static class DataHelper
    {
        private static DobotNonRealTimeData? dobotNoneRealTimeData;
        private static DobotRealTimeData? dobotRealTimeData;
        static float[]? pose;
        static float[]? homeParams;
        static float[]? jogJointParams;
        static float[]? jogCooParams;
        static float[]? jogComParams;
        static float[]? ptpJointParams;
        static float[]? ptpCooParams;
        static float[]? ptpComParams;
        //static float[] ptpJumpParams;
        static string? name;
        static bool[]? suctionCup;

        public static DobotNonRealTimeData GetDobotNonRealTimeData(int id, SerialPort port)
        {
            name = new string(DobotHelper.GetDeviceName(port));
            homeParams = DobotHelper.GetHomeParams(port);
            jogJointParams = DobotHelper.GetJOGJointParams(port);
            jogCooParams = DobotHelper.GetJOGCoordinateParams(port);
            jogComParams = DobotHelper.GetJOGCommonParams(port);
            ptpJointParams = DobotHelper.GetPTPJointParams(port);
            ptpCooParams = DobotHelper.GetPTPCoordinateParams(port);
            ptpComParams = DobotHelper.GetPTPCommonParams(port);

            dobotNoneRealTimeData = new DobotNonRealTimeData()
            {
                Id = id,
                Name = name,
                HomeParams = { homeParams },
                JogJointVelocityWithAcceleration = { jogJointParams },//前四个为速度，后四个为加速度 单位mm/s
                JogCoordinateVelocityWithAcceleration = { jogCooParams },
                JogCommonVelocityRatioWithAcceleration = { jogComParams },
                PtpJointVelocityWithAcceleration = { jogJointParams },
                PtpCoordinateVelocityWithAcceleration = { ptpCooParams },
                PtpCommonVelocityRatioWithAcceleration = { ptpComParams },
                DateTime = DateTime.Now.Ticks.ToString()
            };
            //MessageBox.Show(dobotData.DateTime);
            return dobotNoneRealTimeData;
        }


        public static DobotRealTimeData GetDobotRealTimeData(int id,SerialPort port)
        {
            pose = DobotHelper.GetPose(port);
            suctionCup = DobotHelper.GetEndEffectorSuctionCup(port);
            dobotRealTimeData = new DobotRealTimeData()
            {
                Id = id,
                LiveState = DobotConnectState.Connected,//
                Pose = {pose},
                AlarmState = string.Empty,//
                EndEffectorSuctionCup = {suctionCup},
                DateTime = DateTime.Now.Ticks.ToString()
            };

            return dobotRealTimeData;

        }
    }
}

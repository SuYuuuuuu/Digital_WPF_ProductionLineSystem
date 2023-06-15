using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Windows;
using HslCommunication.Profinet.Siemens;
using static DobotRealTimeData.Types;

namespace WpfProductionLineApp.DobotCommunicationsFrameWork.Common
{


    public static class DataHelper
    {
        private static DobotNonRealTimeData? dobotNoneRealTimeData;
        private static DobotRealTimeData? dobotRealTimeData;
        private static Conveyor_Data? conveyorData;
        private static List<Cylinder_Data> cylinderDatas = new List<Cylinder_Data>();
        private static List<PositionSensor_Data> positionSensorDatas = new List<PositionSensor_Data>();
        static string alarmStates;
        static DobotConnectState liveState;
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
        private static bool[]? inputSignal;
        private static bool[]? outputSignal;
        private static int cylinder_Num = 5; //气缸个数
        private static int positionSensorData_Num = 8;//红外传感器个数

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


        public static DobotRealTimeData GetDobotRealTimeData(int id, SerialPort port)
        {
            pose = DobotHelper.GetPose(port);
            suctionCup = DobotHelper.GetEndEffectorSuctionCup(port);
            alarmStates = DobotHelper.GetAlarmsState(port);
            liveState = DobotHelper.Port_data_Dic.ContainsKey(port)?DobotConnectState.Connected:DobotConnectState.Disconnected;

            dobotRealTimeData = new DobotRealTimeData()
            {
                Id = id,
                LiveState = liveState,//通过查询串口列表检测机械臂连接状态
                Pose = { pose },
                AlarmState = alarmStates,//设置报警状态
                EndEffectorSuctionCup = { suctionCup },
                DateTime = DateTime.Now.Ticks.ToString()
            };

            return dobotRealTimeData;

        }


        static byte[] data;
        static byte[] res_in;
        static byte[] res;
        //获取PLC数据，包含了传送带状态、红外传感器状态、气缸状态数据
        //这里可以设置获取规则，譬如分开获取，一起获取等,这里为一起读取
        public static void SendPLCData(SiemensS7Net plc_Server, Socket client)
        {
            if (plc_Server == null) return;
            inputSignal = plc_Server.ReadBool("I0.0", 16).Content;
            outputSignal = plc_Server.ReadBool("Q0.0", 16).Content;
            //开始分配---------------------,根据具体PLC程序内写的信号地址赋值
            conveyorData = new Conveyor_Data()
            {
                Id = 20000,
                IsOpened = outputSignal[8]
            };
            data = CommonHelper.Serialize(conveyorData);
            res = new byte[] { 2, (byte)data.Length }.Concat(data).ToArray();
            for (int i = 0; i < cylinder_Num; i++)
            {
                Cylinder_Data cylinderData = new Cylinder_Data()
                {
                    Id = 30000 + i,
                    IsOpened = outputSignal[1 + i],
                    DataTime = DateTime.Now.Ticks.ToString()
                };
                data = CommonHelper.Serialize(cylinderData);
                res_in = new byte[] { 3, (byte)data.Length }.Concat(data).ToArray();
                res = res.Concat(res_in).ToArray();
                //cylinderDatas.Add(cylinderData);
            }

            for (int i = 0; i < positionSensorData_Num; i++)
            {
                PositionSensor_Data positionSensorData = new PositionSensor_Data()
                {
                    Id = 40000 + i,
                    IsActived = inputSignal[3 + i],
                    DataTime = DateTime.Now.Ticks.ToString()
                };
                data = CommonHelper.Serialize(positionSensorData);
                res_in = new byte[] { 4, (byte)data.Length }.Concat(data).ToArray();
                res = res.Concat(res_in).ToArray();
                //positionSensorDatas.Add(positionSensorData);
            }

            // byte[] data = CommonHelper.Serialize(inputSignal);
            // byte[] lenArr = new byte[] { (byte)2, (byte)data.Length }; //前缀包括两个字节(数据类型和长度)
            // byte[] res_in = lenArr.Concat(data).ToArray();
            // data = CommonHelper.Serialize(outputSignal);
            // lenArr = new byte[] { (byte)3, (byte)data.Length }; //前缀包括两个字节(数据类型和长度)
            // byte[] res_out = lenArr.Concat(data).ToArray();
            // byte[] end = res_in.Concat(res_out).ToArray();
            client.BeginSend(res, 0, res.Length, SocketFlags.None, EndSendData, client);
        }

        private static void EndSendData(IAsyncResult ar)
        {
            Socket client = ar.AsyncState as Socket;
            client.EndSend(ar);
        }


        public static void SendDisconnectData(int id,Socket client)
        {
            byte[] res = new byte[6];
            res[0] = 5;
            res[1] = 4;
            Buffer.BlockCopy(BitConverter.GetBytes(id), 0, res, 2, 4);
            client.BeginSend(res,0,res.Length,SocketFlags.None, EndSendData, client);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace WpfProductionLineApp.DobotCommunicationsFrameWork
{

    /// <summary>
    /// 字节转换方法（获取校验位、获取点动字节、获取ptp模式字节等）（待补充）
    /// </summary>
    internal static class DobotHelper
    {
        private static Dictionary<SerialPort, DataMethod> port_data_Dic = new Dictionary<SerialPort, DataMethod>();//每一个port对应一个实例数据接收和处理方法，用字典存储起来
        /// <summary>
        /// 发送端获取校验位字节
        /// </summary>
        /// <param name="bytes">可变字节数组，包含负载所有的字节</param>
        /// <returns></returns>
        public static byte GetByteOfPayloadCheckSum_Send(params byte[] bytes)
        {
            byte res = 0;
            if (bytes.Length > 0)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    res += bytes[i];
                }
            }
            return (byte)(256 - res % 256);

        }

        /// <summary>
        /// 接收端获取校验位字节
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte GetByteOfPayloadCheckSum_Receive(params byte[] bytes)
        {
            byte res = 0;
            if (bytes.Length > 0)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    res += bytes[i];
                }
            }
            return Convert.ToByte(res % 256);
        }


        /// <summary>
        /// 浮点数转换为十六进制（用四个字节表示）
        /// </summary>
        /// <param name="num">xyzr坐标或关节坐标</param>
        /// <returns></returns>
        private static byte[] FloatToHex(float num)
        {
            byte[] bytes = BitConverter.GetBytes(num);//浮点数转为字节
            string hex = Convert.ToHexString(bytes);//字节转为十六进制字符串
            byte[] result = new byte[hex.Length / 2];
            for (int i = 0; i < result.Length; i++)//十六进制字符串转为十六进制字节
            {
                result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);//以两个字符为间隔分割，并转化为16进制字节
            }
            return result;
        }

        #region 连接
        /// <summary>
        /// 连接串口函数
        /// </summary>
        /// <param name="com">串口号</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="stopBits">停止位</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="parity">校验位</param>
        /// <returns></returns>
        public static SerialPort Connect(string com, int baudRate = 115200, StopBits stopBits = StopBits.One, int dataBits = 8, Parity parity = Parity.None)
        {
            try
            {
                SerialPort port = new SerialPort(com, baudRate, parity, dataBits, stopBits);
                DataMethod method = new DataMethod();
                port.DataReceived += new SerialDataReceivedEventHandler(method.DataReceived);
                port.Open();
                if (port.IsOpen)
                {
                    Console.WriteLine("串口成功连接！");
                    if (!port_data_Dic.ContainsKey(port))
                        port_data_Dic.Add(port, method);
                    SetQueuedCmdClear(port);//连接后清空指令队列
                }
                return port;
            }
            catch (Exception ex)
            {
                MessageBox.Show("串口连接失败！" + "  原因：" + ex.ToString());
                return null;
            }

        }

        public static void Disconnect(SerialPort port)
        {
            if (port.IsOpen)
            {
                port.Close();
                Console.WriteLine("串口已关闭！");
                port.DataReceived -= new SerialDataReceivedEventHandler(port_data_Dic[port].DataReceived);
                port_data_Dic?.Remove(port);
            }

        }

        #endregion

        #region 点动相关函数
        private static byte[] GetBytesOfPayloadJog(jogCmd cmd, jogMode jogMode, out byte checkSum, bool isQueued)
        {
            byte[] res = new byte[4];
            res[0] = 0x49;
            res[1] = isQueued ? (byte)0x03 : (byte)0x01;
            res[2] = cmd.isJoint;
            res[3] = Convert.ToByte(jogMode);
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }

        /// <summary>
        /// 机械臂点动
        /// </summary>
        /// <param name="port"></param>
        /// <param name="cmd"></param>
        /// <param name="jogMode"></param>
        /// <param name="isQueued"></param>
        public static void SetJogCmd(this SerialPort port, jogCmd cmd, jogMode jogMode, bool isQueued)
        {
            byte[] res = new byte[8];
            int len = CommunicationProtocol.jogCmd_Base.Length;
            CommunicationProtocol.jogCmd_Base.CopyTo(res, 0);
            GetBytesOfPayloadJog(cmd, jogMode, out byte checkSum, isQueued).CopyTo(res, len);
            res[res.Length - 1] = checkSum;
            port?.Write(res, 0, res.Length);
            Thread.Sleep(10);
        }

        /// <summary>
        /// 获取关节点动参数
        /// </summary>
        /// <param name="checkSum"></param>
        /// <returns></returns>
        private static byte[] GetBytesOfPayloadJOGJointParams(out byte checkSum)
        {
            byte[] res = new byte[2] { 0x46, 0x00 };
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }
        private static byte[] GetJOGJointParams()
        {
            int len = CommunicationProtocol.getJogJointParams_Base.Length;
            byte[] res = new byte[len + 3];
            CommunicationProtocol.getJogJointParams_Base.CopyTo(res, 0);
            GetBytesOfPayloadJOGJointParams(out byte checkSum).CopyTo(res, len);
            res[res.Length - 1] = checkSum;
            return res;
        }

        public static float[] GetJOGJointParams(this SerialPort port)
        {
            byte[] res = GetJOGJointParams();
            port?.Write(res, 0, res.Length);
            Thread.Sleep(1);
            return port_data_Dic[port].JogJointParams;
        }


        /// <summary>
        /// 获取坐标轴点动参数
        /// </summary>
        /// <param name="checkSum"></param>
        /// <returns></returns>
        private static byte[] GetBytesOfPayloadJOGCoordinateParams(out byte checkSum)
        {
            byte[] res = new byte[2] { 0x47, 0x00 };//
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }
        private static byte[] GetJOGCoordinateParams()
        {
            int len = CommunicationProtocol.getJOGCoordinateParams_Base.Length;
            byte[] res = new byte[len + 3];//
            CommunicationProtocol.getJOGCoordinateParams_Base.CopyTo(res, 0);
            GetBytesOfPayloadJOGCoordinateParams(out byte checkSum).CopyTo(res, len);
            res[res.Length - 1] = checkSum;
            return res;
        }

        public static float[] GetJOGCoordinateParams(this SerialPort port)
        {
            byte[] res = GetJOGCoordinateParams();
            port?.Write(res, 0, res.Length);
            Thread.Sleep(1);
            return port_data_Dic[port].JogCoordinateParams;
        }


        /// <summary>
        /// 获取点动公共参数
        /// </summary>
        /// <param name="checkSum"></param>
        /// <returns></returns>
        private static byte[] GetBytesOfPayloadJOGCommonParams(out byte checkSum)
        {
            byte[] res = new byte[2] { 0x48, 0x00 };//
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }
        private static byte[] GetJOGCommonParams()
        {
            int len = CommunicationProtocol.getJOGCommonParams_Base.Length;
            byte[] res = new byte[len + 3];//
            CommunicationProtocol.getJOGCommonParams_Base.CopyTo(res, 0);
            GetBytesOfPayloadJOGCommonParams(out byte checkSum).CopyTo(res, len);
            res[res.Length - 1] = checkSum;
            return res;
        }

        public static float[] GetJOGCommonParams(this SerialPort port)
        {
            byte[] res = GetJOGCommonParams();
            port?.Write(res, 0, res.Length);
            Thread.Sleep(1);
            return port_data_Dic[port].JogCommonParams;
        }
        #endregion

        #region PTP模式相关函数


        /// <summary>
        /// 获取负载的十六进制字节数组
        /// </summary>
        /// <returns></returns>
        private static byte[] GetBytesOfPtpPayload(ptpMode ptpMode, float[] cor, out byte checkSum, bool isQueued)
        {
            byte[] res = new byte[19];//初始化校验位字节
            res[0] = 0x54;//ID
            if (isQueued)
                res[1] = 0x03;//Ctrl
            else
                res[1] = 0x01;
            res[2] = Convert.ToByte((int)ptpMode);//ptpMode


            for (int i = 0; i < cor.Length; i++)//将四个坐标的字节以八位形式全部相加起来
            {
                byte[] temp = FloatToHex(cor[i]);//首先获取输入坐标转化的四个字节（一个字节数组存储）
                for (int j = 0; j < 4; j++)
                {
                    res[4 * i + j + 3] = temp[j];//将每一个字节添加到数组中
                }
            }

            checkSum = GetByteOfPayloadCheckSum_Send(res);//最后一位
            return res;
        }

        /// <summary>
        /// 根据直角坐标或关节坐标得到要写入串口的字节数组（十六进制）
        /// </summary>
        /// <param name="ptpMode"></param>
        /// <param name="cord"></param>
        /// <returns></returns>
        private static byte[] SetPTPCmd(ptpMode ptpMode, float[] cord, bool isQueued)
        {
            int len = CommunicationProtocol.PTP_Base.Length;
            byte[] res = new byte[len + 20];
            CommunicationProtocol.PTP_Base.CopyTo(res, 0);
            byte[] bytes = GetBytesOfPtpPayload(ptpMode, cord, out byte checkSum, isQueued);
            bytes.CopyTo(res, len);
            res[res.Length - 1] = checkSum;
            /*            foreach (var item in res)
                        {
                            Console.WriteLine(item);
                        }*/
            return res;
        }

        public static void SetPTPCmd(this SerialPort port, float[] cords, ptpMode ptpMode, ref ulong cmdIndex, bool isQueued)//执行PTP点位功能
        {
            byte[] res = SetPTPCmd(ptpMode, cords, isQueued);
            port.Write(res, 0, res.Length);
            /*while (port_data_Dic[port].Ulongs == null)
                continue;*/
            Thread.Sleep(1);
            if (port_data_Dic[port].Ulongs == null) return;
            cmdIndex = port_data_Dic[port].Ulongs[0];
        }


        /// <summary>
        /// 获取ptp模式关节点位参数
        /// </summary>
        /// <param name="checkSum"></param>
        /// <returns></returns>
        private static byte[] GetBytesOfPayloadPTPJointParams(out byte checkSum)
        {
            byte[] res = new byte[2] { 0x50, 0x00 };//
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }
        private static byte[] GetPTPJointParams()
        {
            int len = CommunicationProtocol.getPTPJointParams_Base.Length;
            byte[] res = new byte[len + 3];//
            CommunicationProtocol.getPTPJointParams_Base.CopyTo(res, 0);
            GetBytesOfPayloadPTPJointParams(out byte checkSum).CopyTo(res, len);
            res[res.Length - 1] = checkSum;
            return res;
        }

        public static float[] GetPTPJointParams(this SerialPort port)
        {
            byte[] res = GetPTPJointParams();
            port?.Write(res, 0, res.Length);
            Thread.Sleep(1);
            return port_data_Dic[port].PTPJointParams;
        }


        /// <summary>
        /// 获取ptp模式坐标轴点位参数
        /// </summary>
        /// <param name="checkSum"></param>
        /// <returns></returns>
        private static byte[] GetBytesOfPayloadPTPCoordinateParams(out byte checkSum)
        {
            byte[] res = new byte[2] { 0x51, 0x00 };//
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }
        private static byte[] GetPTPCoordinateParams()
        {
            int len = CommunicationProtocol.getPTPCoordinateParams_Base.Length;
            byte[] res = new byte[len + 3];//
            CommunicationProtocol.getPTPCoordinateParams_Base.CopyTo(res, 0);
            GetBytesOfPayloadPTPCoordinateParams(out byte checkSum).CopyTo(res, len);
            res[res.Length - 1] = checkSum;
            return res;
        }

        public static float[] GetPTPCoordinateParams(this SerialPort port)
        {
            byte[] res = GetPTPCoordinateParams();
            port?.Write(res, 0, res.Length);
            Thread.Sleep(1);
            return port_data_Dic[port].PTPCoordinateParams;
        }


        /// <summary>
        /// 获取门型模式点位参数
        /// </summary>
        /// <param name="checkSum"></param>
        /// <returns></returns>
        private static byte[] GetBytesOfPayloadPTPJumpParams(out byte checkSum)
        {
            byte[] res = new byte[2] { 0x52, 0x00 };//
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }
        private static byte[] GetPTPJumpParams()
        {
            int len = CommunicationProtocol.getPTPJumpParams_Base.Length;
            byte[] res = new byte[len + 3];//
            CommunicationProtocol.getPTPJumpParams_Base.CopyTo(res, 0);
            GetBytesOfPayloadPTPJumpParams(out byte checkSum).CopyTo(res, len);
            res[res.Length - 1] = checkSum;
            return res;
        }

        public static float[] GetPTPJumpParams(this SerialPort port)
        {
            byte[] res = GetPTPJumpParams();
            port?.Write(res, 0, res.Length);
            Thread.Sleep(1);
            return port_data_Dic[port].PTPJumpParams;
        }


        /// <summary>
        /// 获取ptp模式公共参数
        /// </summary>
        /// <param name="checkSum"></param>
        /// <returns></returns>
        private static byte[] GetBytesOfPayloadPTPCommonParams(out byte checkSum)
        {
            byte[] res = new byte[2] { 0x53, 0x00 };//
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }
        private static byte[] GetPTPCommonParams()
        {
            int len = CommunicationProtocol.getPTPCommonParams_Base.Length;
            byte[] res = new byte[len + 3];//
            CommunicationProtocol.getPTPCommonParams_Base.CopyTo(res, 0);
            GetBytesOfPayloadPTPCommonParams(out byte checkSum).CopyTo(res, len);
            res[res.Length - 1] = checkSum;
            return res;
        }

        public static float[] GetPTPCommonParams(this SerialPort port)
        {
            byte[] res = GetPTPCommonParams();
            port?.Write(res, 0, res.Length);
            Thread.Sleep(1);
            return port_data_Dic[port].PTPCommonParams;
        }


        /// <summary>
        /// 获取门型模式拓展参数
        /// </summary>
        /// <param name="checkSum"></param>
        /// <returns></returns>
        private static byte[] GetBytesOfPayloadPTPJump2Params(out byte checkSum)
        {
            byte[] res = new byte[2] { 0x57, 0x00 };//
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }
        private static byte[] GetPTPJump2Params()
        {
            int len = CommunicationProtocol.getPTPJump2Params_Base.Length;
            byte[] res = new byte[len + 3];//
            CommunicationProtocol.getPTPJump2Params_Base.CopyTo(res, 0);
            GetBytesOfPayloadPTPJump2Params(out byte checkSum).CopyTo(res, len);
            res[res.Length - 1] = checkSum;
            return res;
        }

        public static float[] GetPTPJump2Params(this SerialPort port)
        {
            byte[] res = GetPTPJump2Params();
            port?.Write(res, 0, res.Length);
            Thread.Sleep(1);
            return port_data_Dic[port].PTPJump2Params;
        }
        #endregion

        #region 吸盘相关函数


        /// <summary>
        /// 设置吸盘使能状态
        /// </summary>
        /// <param name="isEnable"></param>
        /// <param name="isSucked"></param>
        /// <returns></returns>
        private static byte[] SetEndEffectorSuctionCup(bool isEnable, bool isSucked, bool isQueued)
        {
            byte[] res = new byte[8];
            int len = CommunicationProtocol.SetEndEffectorSuctionCup_Base.Length;
            CommunicationProtocol.SetEndEffectorSuctionCup_Base.CopyTo(res, 0);
            GetBytesOfPayloadEndEffectorSuctionCup(isEnable, isSucked, isQueued, out byte checkSum).CopyTo(res, len);
            res[res.Length - 1] = checkSum;
            return res;
        }

        public static void SetEndEffectorSuctionCup(this SerialPort port, bool isEnable, bool isSucked, bool isQueued)
        {
            byte[] res = SetEndEffectorSuctionCup(isEnable, isSucked, isQueued);
            port.Write(res, 0, res.Length);
            Thread.Sleep(1);
        }

        private static byte[] GetBytesOfPayloadEndEffectorSuctionCup(bool isEnable, bool isSucked, bool isQueued, out byte checkSum)
        {
            if (!isQueued)
            {
                byte enable = Convert.ToByte(isEnable);
                byte suck = Convert.ToByte(isSucked);
                byte[] res = new byte[4] { 0x3E, 0x01, enable, suck };
                checkSum = GetByteOfPayloadCheckSum_Send(res);
                return res;
            }
            else
            {
                byte enable = Convert.ToByte(isEnable);
                byte suck = Convert.ToByte(isSucked);
                byte[] res = new byte[4] { 0x3E, 0x03, enable, suck };
                checkSum = GetByteOfPayloadCheckSum_Send(res);
                return res;
            }

        }



        /// <summary>
        /// 获取吸盘使能状态
        /// </summary>
        /// <returns></returns>
        private static byte[] GetEndEffectorSuctionCup()
        {
            byte[] res = new byte[6];
            int len = CommunicationProtocol.GetEndEffectorSuctionCup_Base.Length;
            CommunicationProtocol.GetEndEffectorSuctionCup_Base.CopyTo(res, 0);
            GetBytesOfPayloadEndEffectorSuctionCup(out byte checkSum).CopyTo(res, len);
            res[res.Length - 1] = checkSum;
            return res;
        }

        public static bool[] GetEndEffectorSuctionCup(this SerialPort port)
        {
            byte[] res = GetEndEffectorSuctionCup();
            port.Write(res, 0, res.Length);
            Thread.Sleep(1);
            if (port_data_Dic.ContainsKey(port))
            {
                bool[] state = new bool[2];//保存两个状态值
                for (int i = 0; i < port_data_Dic[port].Bytes?.Length; i++)
                {
                    state[i] = Convert.ToBoolean(port_data_Dic[port].Bytes[i]);
                }
                return state;
            }
            return null;
        }

        private static byte[] GetBytesOfPayloadEndEffectorSuctionCup(out byte checkSum)
        {
            byte[] res = new byte[2] { 0x3E, 0x00 };
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }

        #endregion

        #region 回零功能函数

        /// <summary>
        /// 执行回零功能
        /// </summary>
        private static byte[] SetHomeCmd(bool isQueued)
        {
            byte[] end = new byte[10];
            int len = CommunicationProtocol.setHomeCmd_Base.Length;
            CommunicationProtocol.setHomeCmd_Base.CopyTo(end, 0);
            GetBytesOfPayloadSetHomeCmd(out byte checkSum, isQueued).CopyTo(end, len);
            end[end.Length - 1] = checkSum;
            return end;
        }

        public static void SetHomeCmd(this SerialPort port, bool isQueued)
        {
            byte[] res = SetHomeCmd(isQueued);
            port.Write(res, 0, res.Length);
            Thread.Sleep(10);
        }

        /// <summary>
        /// 获取负载字节数组和校验位字节
        /// </summary>
        /// <param name="checkSum"></param>
        /// <returns></returns>
        private static byte[] GetBytesOfPayloadSetHomeCmd(out byte checkSum, bool isQueued)
        {
            byte[] res;
            if (isQueued)
                res = new byte[6] { 0x1F, 0x03, 0, 0, 0, 0 };
            else
                res = new byte[6] { 0x1F, 0x01, 0, 0, 0, 0 };

            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }


        /// <summary>
        /// 获取回零参数
        /// </summary>
        /// <returns></returns>
        private static byte[] GetHomeParams()
        {
            byte[] end = new byte[6];
            int len = CommunicationProtocol.getHomeCmd_Base.Length;
            CommunicationProtocol.getHomeCmd_Base.CopyTo(end, 0);
            GetBytesOfPayloadGetHomeParams(out byte checkSum).CopyTo(end, len);
            end[end.Length - 1] = checkSum;
            return end;
        }

        public static float[] GetHomeParams(SerialPort port)
        {
            if (!port_data_Dic.ContainsKey(port)) return null;

            byte[] res = GetHomeParams();
            port.Write(res, 0, res.Length);
            Thread.Sleep(1);
            return port_data_Dic[port].HomeParams;
        }

        private static byte[] GetBytesOfPayloadGetHomeParams(out byte checkSum)
        {
            byte[] res = new byte[2] { 0x1E, 0x00 };
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }

        /// <summary>
        /// 设置回零参数
        /// </summary>
        /// <param name="cord">机械臂坐标系xyzr</param>
        /// <returns></returns>
        private static byte[] SetHomeParams(float[] cord)
        {
            byte[] end = new byte[22];
            int len = CommunicationProtocol.setHomeParams_Base.Length;
            CommunicationProtocol.setHomeParams_Base.CopyTo(end, 0);
            GetBytesOfPayloadSetHomeParams(cord, out byte checkSum).CopyTo(end, len);
            end[end.Length - 1] = checkSum;
            return end;
        }

        public static void SetHomeParms(SerialPort port, float[] cord)
        {
            byte[] res = SetHomeParams(cord);
            port.Write(res, 0, res.Length);
            Thread.Sleep(10);
        }

        private static byte[] GetBytesOfPayloadSetHomeParams(float[] cord, out byte checkSum)
        {
            byte[] res = new byte[18];
            res[0] = 0x1E;
            res[1] = 0x01;
            for (int i = 0; i < 4; i++)
            {
                byte[] temp = FloatToHex(cord[i]);
                for (int j = 0; j < temp.Length; j++)
                {
                    res[4 * i + j + 2] = temp[j];
                }
            }
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }

        #endregion

        #region 队列控制函数

        /// <summary>
        /// 清空指令队列
        /// </summary>
        /// <returns></returns>
        private static byte[] SetQueuedCmdClear()
        {
            byte[] end = new byte[6];
            int len = CommunicationProtocol.setQueuedCmdClear_Base.Length;
            CommunicationProtocol.setQueuedCmdClear_Base.CopyTo(end, 0);
            GetBytesOfPayloadSetQueuedCmdClear(out byte checkSum).CopyTo(end, len);
            end[end.Length - 1] = checkSum;
            return end;
        }

        public static void SetQueuedCmdClear(this SerialPort port)
        {
            byte[] res = SetQueuedCmdClear();
            port.Write(res, 0, res.Length);
            Thread.Sleep(10);
        }

        private static byte[] GetBytesOfPayloadSetQueuedCmdClear(out byte checkSum)
        {
            byte[] res = new byte[2] { 0xF5, 0x01 };
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }

        /// <summary>
        /// 获取指令队列索引
        /// </summary>
        /// <returns></returns>
        private static byte[] GetQueuedCmdCurrentIndex()
        {
            byte[] end = new byte[6];
            int len = CommunicationProtocol.getQueuedCmdCurrentIndex_Base.Length;
            CommunicationProtocol.getQueuedCmdCurrentIndex_Base.CopyTo(end, 0);
            GetBytesOfPayloadGetQueuedCmdCurrentIndex(out byte checkSum).CopyTo(end, len);
            end[end.Length - 1] = checkSum;
            return end;

        }

        public static ulong GetQueuedCmdCurrentIndex(this SerialPort port)
        {
            if (!port_data_Dic.ContainsKey(port)) return 0;
            byte[] res = GetQueuedCmdCurrentIndex();
            port.Write(res, 0, res.Length);
            Thread.Sleep(1);//需要时间处理数据
            return port_data_Dic[port].Ulongs[0];
        }

        private static byte[] GetBytesOfPayloadGetQueuedCmdCurrentIndex(out byte checkSum)
        {
            byte[] res = new byte[2] { 0xF6, 0x00 };
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }

        /// <summary>
        /// 启动指令队列运行
        /// </summary>
        /// <param name="port"></param>
        public static void SetQueuedCmdStartExec(this SerialPort port)
        {
            byte[] bytes = SetQueuedCmdStartExec();
            port.Write(bytes, 0, bytes.Length);
            Thread.Sleep(10);
        }

        private static byte[] SetQueuedCmdStartExec()
        {
            byte[] end = new byte[6];
            int len = CommunicationProtocol.setQueuedCmdStartExec_Base.Length;
            CommunicationProtocol.setQueuedCmdStartExec_Base.CopyTo(end, 0);
            GetBytesOfPayloadSetQueuedCmdStartExec(out byte checkSum).CopyTo(end, len);
            end[end.Length - 1] = checkSum;
            return end;
        }

        private static byte[] GetBytesOfPayloadSetQueuedCmdStartExec(out byte checksum)
        {
            byte[] res = new byte[2] { 0xF0, 0x01 };
            checksum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }


        /// <summary>
        /// 停止指令队列运行
        /// </summary>
        /// <param name="port"></param>
        public static void SetQueuedCmdStopExec(this SerialPort port)
        {
            byte[] bytes = SetQueuedCmdStopExec();
            port.Write(bytes, 0, bytes.Length);
            Thread.Sleep(10);
        }

        private static byte[] SetQueuedCmdStopExec()
        {
            byte[] end = new byte[6];
            int len = CommunicationProtocol.setQueuedCmdStopExec_Base.Length;
            CommunicationProtocol.setQueuedCmdStopExec_Base.CopyTo(end, 0);
            GetBytesOfPayloadSetQueuedCmdStopExec(out byte checkSum).CopyTo(end, len);
            end[end.Length - 1] = checkSum;
            return end;
        }

        private static byte[] GetBytesOfPayloadSetQueuedCmdStopExec(out byte checksum)
        {
            byte[] res = new byte[2] { 0xF1, 0x01 };
            checksum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }

        #endregion

        #region 实时位姿


        public static float[] GetPose(this SerialPort port)
        {
            try
            {
                byte[] bytes = GetPose();
                port.Write(bytes, 0, bytes.Length);
                Thread.Sleep(1);
                if (port_data_Dic.ContainsKey(port))
                {
                    return port_data_Dic[port].Pose;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("获取位姿失败！ " + e.Message);
                return null;
            }

        }


        private static byte[] GetPose()
        {
            byte[] end = new byte[6];
            int len = CommunicationProtocol.getPose_Base.Length;
            CommunicationProtocol.getPose_Base.CopyTo(end, 0);
            GetBytesOfPayloadGetPose(out byte checkSum).CopyTo(end, len);
            end[end.Length - 1] = checkSum;
            return end;
        }

        private static byte[] GetBytesOfPayloadGetPose(out byte checkSum)
        {
            byte[] res = new byte[2] { 0x0A, 0x00 };
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }
        #endregion

        #region 报警功能

        /// <summary>
        /// 清除所有异常报警状态
        /// </summary>
        /// <param name="port"></param>
        public static void ClearAllAlarmsState(this SerialPort port)
        {
            byte[] res = ClearAllAlarmsState();
            port.Write(res, 0, res.Length);
            Thread.Sleep(10);
        }

        private static byte[] ClearAllAlarmsState()
        {
            byte[] end = new byte[6];
            int len = CommunicationProtocol.clearAllAlarmsState_Base.Length;
            CommunicationProtocol.clearAllAlarmsState_Base.CopyTo(end, 0);
            GetBytesOfPayloadClearAllAlarmsState(out byte checkSum).CopyTo(end, len);
            end[end.Length - 1] = checkSum;
            return end;
        }

        private static byte[] GetBytesOfPayloadClearAllAlarmsState(out byte checkSum)
        {
            byte[] res = new byte[2] { 0x14, 0x01 };
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }


        /// <summary>
        /// 获取报警状态
        /// </summary>
        /// <param name="port"></param>
        public static void GetAlarmsState(this SerialPort port)
        {
            byte[] res = GetAlarmsState();
            port.Write(res, 0, res.Length);
            Thread.Sleep(10);
        }

        private static byte[] GetAlarmsState()
        {
            byte[] end = new byte[6];
            int len = CommunicationProtocol.GetAlarmsState_Base.Length;
            CommunicationProtocol.GetAlarmsState_Base.CopyTo(end, 0);
            GetBytesOfPayloadGetAlarmsState(out byte checkSum).CopyTo(end, len);
            end[end.Length - 1] = checkSum;
            return end;
        }

        private static byte[] GetBytesOfPayloadGetAlarmsState(out byte checkSum)
        {
            byte[] res = new byte[2] { 0x14, 0x00 };
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }
        #endregion

        #region 设备信息
        /// <summary>
        /// 获取设备名称
        /// </summary>
        /// <param name="checkSum"></param>
        /// <returns></returns>
        private static byte[] GetBytesOfPayloadDeviceName(out byte checkSum)
        {
            byte[] res = new byte[2] { 0x01, 0x00 };
            checkSum = GetByteOfPayloadCheckSum_Send(res);
            return res;
        }
        private static byte[] GetDeviceName()
        {
            int len = CommunicationProtocol.getDeviceName_Base.Length;
            byte[] res = new byte[len + 3];
            CommunicationProtocol.getDeviceName_Base.CopyTo(res, 0);
            byte[] bytes = GetBytesOfPayloadDeviceName(out byte checkSum);
            bytes.CopyTo(res, len);
            res[res.Length - 1] = checkSum;
            return res;
        }

        public static char[] GetDeviceName(this SerialPort port)
        {
            byte[] res = GetDeviceName();
            port.Write(res, 0, res.Length);
            Thread.Sleep(1);
            return port_data_Dic[port].Chars;
        }
        #endregion
    }



}

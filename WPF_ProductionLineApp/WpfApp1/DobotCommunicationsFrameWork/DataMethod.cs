using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Windows;


namespace WpfProductionLineApp.DobotCommunicationsFrameWork

{
    /// <summary>
    /// 数据方法（数据接收、数据处理等）
    /// </summary>
    public class DataMethod
    {
        public DataMethod()
        {
            bufferList = new List<byte>(2048);
            //Id2ParamsDic = new Dictionary<int, float[]>()
            //{
            //    {10,Pose},{30,HomeParams},{70,JogJointParams},{71,JogCoordinateParams},{72,JogCommonParams},
            //    {80,PTPJointParams},{81,PTPCoordinateParams},{82,PTPJumpParams},{83,PTPCommonParams},{87,PTPJump2Params}
            //};
        }


        private List<byte> bufferList;
        //Dictionary<int,float[]> Id2ParamsDic;

        private char[] chars;
        public char[] Chars { get { return chars; } private set { } }
        private float[] floats;
        public float[] Floats { get { return floats; } private set { } }

        public float[] Pose { get; private set; }
        public float[] HomeParams { get; private set; }
        public float[] JogJointParams { get; private set; }
        public float[] JogCoordinateParams { get; private set; }
        public float[] JogCommonParams { get; private set; }
        public float[] PTPJointParams { get; private set; }
        public float[] PTPCoordinateParams { get; private set; }
        public float[] PTPJumpParams { get; private set; }
        public float[] PTPJump2Params { get; private set; }
        public float[] PTPCommonParams { get; private set; }

        private byte[] bytes;
        public byte[] Bytes { get { return bytes; } private set { } }
        private UInt32[] uints32;
        public UInt32[] Uints32 { get { return uints32; } private set { } }
        private ulong[] ulongs;
        public ulong[] Ulongs { get { return ulongs; } private set { } }
        private bool[] bools;
        public bool[] Bools { get { return bools; } private set { } }

        /// <summary>
        /// 数据接收后执行的委托方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort serialPort = (SerialPort)sender;
                byte[] buffer = new byte[serialPort.BytesToRead];
                byte[] receivedBytes;
                serialPort.Read(buffer, 0, buffer.Length);
                bufferList.AddRange(buffer);//将待读入数据添加到链表
                while (bufferList.Count >= 4)
                {
                    //至少包含数据头（2字节）、长度（1字节）、校验位（1字节）
                    //查找数据标记头
                    if (bufferList[0] == 0xAA && bufferList[1] == 0xAA)
                    {
                        int len = bufferList[2];//负载长度
                        if (bufferList.Count < len + 4)//数据区尚未接收完整
                        {
                            break;
                        }
                        receivedBytes = new byte[len + 4];
                        //得到完整的数据，复制到ReceiveBytes中进行校验
                        bufferList.CopyTo(0, receivedBytes, 0, len + 4);
                        byte checkSum = DobotHelper.GetByteOfPayloadCheckSum_Receive(receivedBytes[3..(len + 3)]);//左闭右开
                        if ((checkSum + receivedBytes[len + 3]) % 256 == 0)//将结果（低8位）与校验字节相加，如果等于0，则说明校验正确，进行数据处理
                        {
                            DataProcess(receivedBytes);//数据处理
                            bufferList.RemoveRange(0, len + 4);
                        }
                        else //说明校验不正确
                        {
                            //从缓冲区清除
                            bufferList.RemoveRange(0, len + 4);
                            Console.WriteLine("数据包不正确！");
                            continue;
                        }
                    }
                    else//帧头不正确
                    {
                        bufferList.Remove(0);
                    }
                }
            }
            catch (Exception ex) //开始标记或s
            {
                MessageBox.Show(ex.ToString());
            }
        }



        /// <summary>
        /// 数据处理函数
        /// </summary>
        /// <param name="receivedBytes">一帧指令包的字节数组</param>
        private void DataProcess(byte[] receivedBytes)
        {
            int len = receivedBytes[2];//负载长度
            if (len == 2) return;//说明负载中没有参数

            int id = receivedBytes[3];//根据ID判断所在功能区间
            byte ctrl = receivedBytes[4];//获取ctrl用于判断指令
            byte[] pars = receivedBytes[5..(receivedBytes.Length - 1)];//返回参数字节数组
            Type type;
            /*if (!CommunicationProtocol.idPairs.ContainsKey(id)) return;
            var res = CommunicationProtocol.idPairs[id];//返回参数类型数组
            Type type = res.GetType();//获取id所对应类型*/


            if (!CommunicationProtocol.idPairss.ContainsKey(id)) return;
            IType res = CommunicationProtocol.idPairss[id];//返回参数类型数组
            if (ctrl == 0) //获取第二个函数的返回类型
                type = res.GetOneType();
            else
                type = res.GetZeroType();


            if (type == typeof(char[]))
            {
                chars = new char[len - 2];
                for (int i = 0; i < pars.Length; i++)
                {
                    chars[i] = Convert.ToChar(pars[i]);
                }
            }
            else if (type == typeof(float[]))
            {
                switch (id)
                {
                    case 10:
                        Pose = SelectArrAndProcess(len, pars);
                        break;
                    case 30:
                        HomeParams = SelectArrAndProcess(len, pars);
                        break;
                    case 70:
                        JogJointParams = SelectArrAndProcess(len, pars);
                        break;
                    case 71:
                        JogCoordinateParams = SelectArrAndProcess(len, pars);
                        break;
                    case 72:
                        JogCommonParams = SelectArrAndProcess(len, pars);
                        break;
                    case 80:
                        PTPJointParams = SelectArrAndProcess(len, pars);
                        break;
                    case 81:
                        PTPCoordinateParams = SelectArrAndProcess(len, pars);
                        break;
                    case 82:
                        PTPJumpParams = SelectArrAndProcess(len, pars);
                        break;
                    case 83:
                        PTPCommonParams = SelectArrAndProcess(len, pars);
                        break;
                    case 87:
                        PTPJump2Params = SelectArrAndProcess(len, pars);
                        break;
                    default:
                        break;
                } //根据id选择存储的数组

            }
            else if (type == typeof(byte[]))
            {
                bytes = new byte[len - 2];
                bytes = pars;
                string alarmTxt = ByteToString(bytes);
                List<int> alarmList = new List<int>();
                for (int i = alarmTxt.Length - 1; i >= 0; i--) //转换为对应的key值
                {
                    if (alarmTxt[i] == '1')
                        alarmList.Add(127 - i);
                }
                for (int i = 0; i < alarmList.Count; i++)
                {
                    if (CommunicationProtocol.alarmStateDic.ContainsKey(alarmList[i]))
                        MessageBox.Show(CommunicationProtocol.alarmStateDic[alarmList[i]]);
                }
            }
            else if (type == typeof(UInt32[]))
            {
                uints32 = new UInt32[(len - 2) / 4];
                for (int i = 0, count = 0; i < pars.Length; i += 4, count++)
                {
                    uints32[count] = BitConverter.ToUInt32(new byte[]
                    {
                        pars[i],pars[i+1],pars[i+2],pars[i+3]
                    }, 0);
                }
            }
            else if (type == typeof(ulong[]))
            {
                ulongs = new ulong[(len - 2) / 8];
                for (int i = 0, count = 0; i < pars.Length; i += 8, count++)
                {
                    ulongs[count] = BitConverter.ToUInt64(new byte[]
                    {
                        pars[i],pars[i+1],pars[i+2],pars[i+3],
                        pars[i+4],pars[i+5],pars[i+6],pars[i+7]
                    }, 0);
                    Console.WriteLine(ulongs[count]);
                }
            }
            else if (type == typeof(bool[]))
            {
                bools = new bool[len - 2];
                for (int i = 0, count = 0; i < bools.Length; i++, count++)
                {
                    bools[count] = BitConverter.ToBoolean(new byte[] { pars[i] }, 0);
                }
            }
            else return;

        }

        private string ByteToString(byte[] data)
        {
            if (data == null) return null;
            StringBuilder stringBuilder = new StringBuilder();

            for (int j = data.Length - 1; j >= 0; j--)
            {
                for (int i = 0; i < 8; i++)
                {
                    //右移 与1相与 从右向左一位一位取    目的数.ToString
                    var t = ((data[j] >> (7 - i)) & 1).ToString();

                    //字符串拼接
                    stringBuilder.Append(t);
                }
            }


            return stringBuilder.ToString();
        }

        private float[] SelectArrAndProcess(int len, byte[] pars)
        {

            float[] res = new float[(len - 2) / 4];
            for (int i = 0, count = 0; i < pars.Length; i += 4, count++)
            {
                res[count] = (float)Math.Round(BitConverter.ToSingle(new byte[] {
                        pars[i],pars[i+1],
                        pars[i+2], pars[i+3]
                    }, 0), 4);
                //Console.WriteLine(floats[count]);
            }
            return res;
        }
    }
}

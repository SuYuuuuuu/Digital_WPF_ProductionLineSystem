using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Fleck;
using HslCommunication;
using HslCommunication.Profinet.Siemens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfProductionLineApp.Auboi5FrameWork;
using WpfProductionLineApp.DobotCommunicationsFrameWork;
using WpfProductionLineApp.DobotCommunicationsFrameWork.Common;
using WpfProductionLineApp.Models;
using WpfProductionLineApp.StaticData;
using WpfProductionLineApp.WebSocketServerModule;

namespace WpfProductionLineApp.ViewModels
{
    partial class ServerViewModel : ObservableObject
    {
        public ServerViewModel()
        {
            ConnectPLCCmd = new RelayCommand<object>(ConnectPLCServer);
            DisConnectPLCCmd = new RelayCommand(DisConnectPLCServer);
            OpenTcpServerCmd = new RelayCommand<object>(OpenTcpServer);
            CloseTcpServerCmd = new RelayCommand(CloseTcpServer);
            ServerModel = new ServerModel()
            {
                PLCServerConnectText = "已断开连接！",
                PLCServerState = ConnectState.Disconnected,
                SocketLists = new ObservableCollection<Socket>(),
                ClientBufferDic = new Dictionary<Socket, byte[]>(),
                TcpServerState = ConnectState.Disconnected,
                WebSocketState = ConnectState.Disconnected
            };
            PLCTypes = new string[2] { "S7200smart", "S7-1200" };
            WeakReferenceMessenger.Default.Send<ServerModel>(ServerModel);
            WeakReferenceMessenger.Default.Send<ServerViewModel>(serverViewModel);
            WeakReferenceMessenger.Default.Register<RobotArmModel>(this, OnReceived);
            dobotType = 4;
        }
        public ServerViewModel serverViewModel
        { get => this; private set { } }


        private List<byte> bufferList = new List<byte>(4096);
        private RobotArmModel armModel;
        private WebSocket_Server webSocketServer;
        private bool isClosed;//标识Tcp服务端关闭
        private bool isWebSocketServerClosed;
        private bool[] SignalInputs_Send;
        private bool[] SignalOutputs_Send;
        private bool[] SignalInputs_Receive;
        private bool[] SignalOutputs_Receive;

        public byte dobotType { get; set; }

        [ObservableProperty]
        private int _pLCType;
        [ObservableProperty]
        private string _plcIpAddress = "192.168.2.1";
        [ObservableProperty]
        private string _tcpIpAddress = "192.168.8.168";
        [ObservableProperty]
        private string _tcpPort = "12888";
        [ObservableProperty]
        private string _maxCount = "5";
        [ObservableProperty]
        private string _uri = "192.168.8.168:9000";

        private float[] msgFloat_1;
        public float[] MsgFloat_1 { get => msgFloat_1; private set { } }
        private float[] msgFloat_2;
        public float[] MsgFloat_2 { get => msgFloat_2; private set { } }
        private float[] msgFloat_3;
        public float[] MsgFloat_3 { get => msgFloat_3; private set { } }
        private float[] msgFloat_4;
        public float[] MsgFloat_4 { get => msgFloat_4; private set { } }



        private string[] _pLCTypes;
        public string[] PLCTypes
        {
            get { return _pLCTypes; }
            set { _pLCTypes = value; }
        }

        private CancellationTokenSource source_Plc;
        private CancellationToken token_Plc;

        private CancellationTokenSource source_Tcp;
        private CancellationToken token_Tcp;

        private ServerModel _serverModel;
        public ServerModel ServerModel
        {
            get { return _serverModel; }
            set { _serverModel = value; }
        }

        private RelayCommand<object> _connectPLCCmd;
        public RelayCommand<object> ConnectPLCCmd
        {
            get { return _connectPLCCmd; }
            set { _connectPLCCmd = value; }
        }

        private RelayCommand _disconnectPLCCmd;
        public RelayCommand DisConnectPLCCmd
        {
            get { return _disconnectPLCCmd; }
            set { _disconnectPLCCmd = value; }
        }

        private RelayCommand<object> _openTcpServerCmd;
        public RelayCommand<object> OpenTcpServerCmd
        {
            get { return _openTcpServerCmd; }
            set { _openTcpServerCmd = value; }
        }

        private RelayCommand _closeTcpServerCmd;
        public RelayCommand CloseTcpServerCmd
        {
            get { return _closeTcpServerCmd; }
            set { _closeTcpServerCmd = value; }
        }


        [RelayCommand]
        private void OpenWebSocketServer(object res)
        {
            string uri = res.ToString();
            webSocketServer = new WebSocket_Server();
            webSocketServer.Open(uri);
            ServerModel.WebSocketState = ConnectState.Connected;

        }

        [RelayCommand]
        private void CloseWebSocketServer()
        {
            webSocketServer.Close();
            ServerModel.WebSocketState = ConnectState.Disconnected;
        }

        private void OnReceived(object recipient, RobotArmModel message)
        {
            armModel = message;
        }

        private void OpenTcpServer(object res)//开启SocketTcp服务端
        {
            ObservableCollection<Socket> temp;
            source_Tcp = new CancellationTokenSource();
            token_Tcp = source_Tcp.Token;
            isClosed = false;
            object[] objects = res as object[];
            string ip = objects[0] as string;
            int port = int.Parse(objects[1].ToString());
            int maxCount = int.Parse(objects[2].ToString());
            try
            {
                ServerModel.ServerSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                ServerModel.ServerSocket.Bind(endPoint);//绑定端口
                ServerModel.ServerSocket.Listen(maxCount);//开始监听，同时设置最大连接数
                ServerModel.ServerSocket.BeginAccept(OnAccepted, ServerModel.ServerSocket);//开启异步监听
                ServerModel.TcpServerState = ConnectState.Connected;
                Task task = Task.Run(async () =>
                {
                    while (true)
                    {
                        if (source_Tcp.IsCancellationRequested) return;
                        await Task.Delay(10000);//每十秒一次轮询
                        temp = ServerModel.SocketLists;
                        for (int i = 0; i < temp.Count; i++)
                        {
                            if (temp[i].Poll(10, SelectMode.SelectRead))
                            {
                                ServerModel.SocketLists.Remove(temp[i]);
                                //ServerModel.Server2ClientDic.Remove(temp[i]);
                            }
                        }
                    }
                }, token_Tcp);//开启轮询线程
            }
            catch (Exception ex)
            {
                MessageBox.Show("开启失败：   " + ex.ToString());
            }


        }
        private void CloseTcpServer() //关闭Tcp服务端连接
        {
            /*ServerModel.ServerSocket?.Shutdown(SocketShutdown.Both);
            ServerModel.ServerSocket?.Close();*/
            if (ServerModel.SocketLists.Count > 0)
            {
                foreach (var item in ServerModel.SocketLists)
                {
                    item.Shutdown(SocketShutdown.Both);
                    item.Close();
                }
            }
            isClosed = true;
            source_Tcp.Cancel();
            ServerModel.ServerSocket.Close();//这个暂定
            ServerModel.TcpServerState = ConnectState.Disconnected;
        }

        private void OnAccepted(IAsyncResult ar)
        {
            if (ServerModel.ServerSocket != null)
            {
                Socket serverSocket = (Socket)ar.AsyncState;
                if (isClosed) return;
                Socket client_ServerSocket = serverSocket.EndAccept(ar);//连接后开启新的Socket用于数据传输
                ServerModel.SocketLists.Add(client_ServerSocket);
                Task.Run(async () =>
                {
                    DobotNonRealTimeData dobotData;
                    while (true)
                    {
                        if (source_Tcp.IsCancellationRequested) return;
                        //方案三：一个一个发送 + 数据打包发送
                        for (int i = 0; i < armModel.portLists.Count; i++)
                        {
                            //机械臂的Id从10000开始
                            dobotData = DataHelper.GetDobotNonRealTimeData(armModel.port2IndexDic[armModel.portLists[i]] - 1 + 10000, armModel.portLists[i]);
                            SendDobotData(dobotData, client_ServerSocket);
                        }
                        await Task.Delay(200);//相当于发送时间(已经不等于发送时间了）
                    }

                });//开启发送非实时数据线程
                Task.Run(async () =>
                {
                    DobotRealTimeData dobotData;
                    while (true)
                    {
                        if (source_Tcp.IsCancellationRequested) break;
                        for (int i = 0; i < armModel.portLists.Count; i++)
                        {
                            dobotData = DataHelper.GetDobotRealTimeData(armModel.port2IndexDic[armModel.portLists[i]] - 1 + 10000, armModel.portLists[i]);
                            SendDobotData(dobotData, client_ServerSocket);
                        }
                        if (armModel.StateAubo == ConnectState.Connected)
                            SendData(Auboi5Method.JointAngle, client_ServerSocket, 5); //到时候整合到上面去
                        await Task.Delay(20);
                    }

                });//开启发送实时数据线程
                ReceiveData(client_ServerSocket);//开始接收数据
                serverSocket.BeginAccept(OnAccepted, ServerModel.ServerSocket);//继续监听
            }
        }//连接回调函数
        private void ReceiveData(Socket client_ServerSocket)
        {
            if (!ServerModel.ClientBufferDic.TryGetValue(client_ServerSocket, out byte[] buffer))
            {
                buffer = new byte[1024];
                ServerModel.ClientBufferDic.Add(client_ServerSocket, buffer);
            }
            client_ServerSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, EndReceiveData, new ReceiveState
            {
                cilentSocket = client_ServerSocket,
                buffer = buffer
            });
        }//异步接收数据
        private void EndReceiveData(IAsyncResult ar)
        {
            ReceiveState receiveState = (ReceiveState)ar.AsyncState;//接收从beginReceive中传递来的参数
            Socket client = receiveState.cilentSocket;
            byte[] buffer = receiveState.buffer;
            bufferList.AddRange(buffer);
            try
            {
                if (client.Poll(10, SelectMode.SelectRead)) //用于检测客户端是否存活
                    return;
                /*-----数据处理函数------*/
                ProcessData(buffer, client, ar);
                /*------继续异步接收数据----------*/
                ReceiveData(client);
            }
            catch (Exception ex)
            {
                client.Close();
                ServerModel.ClientBufferDic.Remove(client);
                ServerModel.SocketLists.Remove(client);
            }
        }//接收回调函数
        private void ProcessData(byte[] buffer, Socket client, IAsyncResult result)
        {
            if (client == null || !client.Connected)
                return;
            int length = client.EndReceive(result);//包含了数据头的长度，原有数据长度为length-1（这里结束接收数据）
            byte[] receivedBytes;
            while (bufferList.Count >= 4)
            {
                if (bufferList[0] == 0xAA)//包头字节
                {
                    int len = bufferList[2];
                    if (bufferList.Count < len + 4) //数据区尚未接收完整
                    {
                        break;
                    }
                    receivedBytes = new byte[len + 4];
                    //得到完整的数据，复制到ReceiveBytes中进行校验
                    bufferList.CopyTo(0, receivedBytes, 0, len + 4);
                    byte checkSum = DobotHelper.GetByteOfPayloadCheckSum_Receive(receivedBytes[3..(receivedBytes.Length - 1)]);
                    if ((checkSum + receivedBytes[receivedBytes.Length - 1]) % 256 == 0)
                    {
                        DataProcess(receivedBytes);
                        bufferList.RemoveRange(0, len + 4);
                    }
                    else
                    {
                        bufferList.RemoveRange(0, len + 4);
                        //Debug.Log("数据校验不正确");
                        continue;
                    }
                }
                else//帧头不正确
                {
                    bufferList.RemoveAt(0);
                    //Debug.Log("帧头不正确");
                }
            }
        }//接收的数据处理函数
        private void DataProcess(byte[] receivedBytes)
        {
            byte dataType = receivedBytes[1];//数据头，代表了哪种数据类型
            int len = receivedBytes[2];
            if (len == 0) return;//负载中没有数据
            switch (dataType)
            {
                case 0://byte[]-----string
                    string msgString = Encoding.GetEncoding("GBK").GetString(receivedBytes, 3, len);
                    break;

                case 1://byte[]-----uint[]
                    uint[] MsgUint = new uint[len / 4];
                    for (int i = 3, count = 0; i < len + 3; i += 4, count++)
                    {
                        MsgUint[count] = BitConverter.ToUInt32(receivedBytes[i..(i + 4)]);
                    }
                    break;

                case 2://byte[]-----bool[]
                    byte[] res = new byte[len];
                    int signalType = receivedBytes[3];//信号类型
                    Buffer.BlockCopy(receivedBytes, 4, res, 0, len);
                    switch (signalType)
                    {
                        case 1:
                            SignalInputs_Receive = Array.ConvertAll(res, value => value == 1 ? true : false);
                            break;
                        case 2:
                            SignalOutputs_Receive = Array.ConvertAll(res, value => value == 1 ? true : false);
                            break;
                        default:
                            break;
                    }
                    break;

                case 3://byte[]-----float[]
                    int dobotType = receivedBytes[3];
                    switch (dobotType)
                    {
                        case 1:
                            ChangeByteToFloat(out msgFloat_1, receivedBytes, len);
                            SetRobotJoint(msgFloat_1, dobotType);
                            break;
                        case 2:
                            ChangeByteToFloat(out msgFloat_2, receivedBytes, len);
                            SetRobotJoint(msgFloat_2, dobotType);
                            break;
                        case 3:
                            ChangeByteToFloat(out msgFloat_3, receivedBytes, len);
                            SetRobotJoint(msgFloat_3, dobotType);
                            break;
                        case 4:
                            ChangeByteToFloat(out msgFloat_4, receivedBytes, len);
                            SetRobotJoint(msgFloat_4, dobotType);
                            break;
                        default:
                            break;
                    }
                    break;
            }
        }

        private void ChangeByteToFloat(out float[] data, byte[] bytes, int len)
        {
            float[] floats = new float[(len - 1) / 4];
            for (int i = 4, count = 0; i < 3 + len; i += 4, count++)
            {
                floats[count] = BitConverter.ToSingle(bytes[i..(i + 4)]);
            }
            data = floats;
        }
        private void SetRobotJoint(float[] target, int dobotType)
        {
            if (target == null || !armModel.Index2portDic.ContainsKey(dobotType)) return;
            ulong cmdIndex = 0;
            DobotHelper.SetPTPCmd(armModel.Index2portDic[dobotType], target, ptpMode.MOVJ_ANGLE, ref cmdIndex, false);
        }




        /*------------此处需要自己拟定数据通信协议-------------*/
        /*------------包头+数据类型+负载帧长+负载帧+检验位 -------------*/
        /*-------------其中，包头为一个字节0xAA，数据类型一个字节（0-string'1-Uint32,2-bool,3-float），负载帧长一个字节，负载帧N个字节，校验位一个字节（校验位计算为负载字节相加后取反）*/
        /*-------------验证校验位方法-----将负载字节和校验位字节全部相加，若结果为0则正确----------*/
        public void SendData(string msg, Socket client)
        {
            if (msg == null || client == null || !ServerModel.SocketLists.Contains(client)) return;
            try
            {
                byte[] bytes = Encoding.GetEncoding("GBK").GetBytes(msg);
                byte[] res = new byte[bytes.Length + 4];//为新建字节数组添加数据头
                res[0] = 0xAA;//包头
                res[1] = 0;//数据类型
                res[2] = (byte)bytes.Length;//负载帧长
                Buffer.BlockCopy(bytes, 0, res, 3, bytes.Length);
                res[res.Length - 1] = DobotHelper.GetByteOfPayloadCheckSum_Send(bytes);
                client.BeginSend(res, 0, res.Length, SocketFlags.None, EndSendData, client);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("发送字符串失败失败：" + ex.Message);
            }

        }//发送数据（字符串类型）
        public void SendData(UInt32[] msg, Socket client)
        {
            if (msg == null || client == null || !ServerModel.SocketLists.Contains(client)) return;
            try
            {
                byte[] bytes = new byte[msg.Length * 4 + 4]; //包含包头、数据类型、负载帧长、负载、校验位
                bytes[0] = 0xAA;
                bytes[1] = 1;
                bytes[2] = Convert.ToByte((msg.Length * 4 + 1));
                int count = 3;
                foreach (var item in msg)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(item), 0, bytes, count, 4);//将得到的每个uint转换的byte[]添加到原有的字节数组上
                    count += 4;
                }
                bytes[bytes.Length - 1] = DobotHelper.GetByteOfPayloadCheckSum_Send(bytes[3..(bytes.Length - 1)]);
                client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, EndSendData, client);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("发送Uint32数据错误，原因为：" + ex.Message);
            }

        }//发送数据（uint类型）
        public void SendData(bool[] msg, Socket client, int signalType)//signalType为发送信号类型，1为输入，2为输出，0为其他，也可以定义其他，这里定义10->机械臂1 11-->机械臂2 12--机械臂3 13机械臂4 14机械臂5
        {
            if (msg == null || client == null || !ServerModel.SocketLists.Contains(client)) return;
            try
            {
                byte[] bytes = Array.ConvertAll(msg, value => value ? (byte)1 : (byte)0);//负载
                byte[] res = new byte[bytes.Length + 5];//为新建字节数组添加数据头
                res[0] = 0xAA;//包头
                res[1] = 2;//数据类型
                res[2] = (byte)(bytes.Length + 1);//负载帧长
                res[3] = (byte)signalType;
                Buffer.BlockCopy(bytes, 0, res, 4, bytes.Length);
                res[res.Length - 1] = DobotHelper.GetByteOfPayloadCheckSum_Send(res[3..(res.Length - 1)]);//校验位

                client.BeginSend(res, 0, res.Length, SocketFlags.None, EndSendData, client);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("发送布尔值失败:" + ex.Message);
                return;
            }

        }//发送数据（布尔类型）
        public void SendData(float[] msg, Socket client, int robotType)//robotType为机械臂型号标识，共有1、2、3、4、5；若发送其他浮点数据该位字节为0即可
        {

            if (msg == null || client == null || !ServerModel.SocketLists.Contains(client)) return;
            try
            {
                byte[] bytes = new byte[msg.Length * 4 + 5];//负载帧第一位为机械臂型号，其余为浮点数;总长为包头(1)+负载帧长(1)+负载帧(1+msg.Length*4)+校验位(1)
                bytes[0] = 0xAA;
                bytes[1] = 3;//数据类型
                bytes[2] = Convert.ToByte((msg.Length * 4) + 1);
                bytes[3] = (byte)robotType;
                int count = 4;
                foreach (var item in msg)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(item), 0, bytes, count, 4);//将得到的每个float转换的byte[]添加到原有的字节数组上
                    count += 4;
                }
                bytes[bytes.Length - 1] = DobotHelper.GetByteOfPayloadCheckSum_Send(bytes[3..(bytes.Length - 1)]);//最后一位为校验位
                client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, EndSendData, client);//(这里开始发送数据)
            }
            catch (Exception ex)
            {
                //MessageBox.Show("发送浮点数失败：" + ex.Message);
            }


        }//发送数据（浮点数类型）

        public void SendDobotData(DobotNonRealTimeData dobotData, Socket client)
        {
            byte[] data = CommonHelper.Serialize(dobotData);
            byte[] lenArr = new byte[] { (byte)1, (byte)data.Length }; //前缀包括两个字节(数据类型和长度)
            byte[] res = lenArr.Concat(data).ToArray();
            client.BeginSend(res, 0, res.Length, SocketFlags.None, EndSendData, client);
        }

        public void SendDobotData(DobotRealTimeData dobotData, Socket client)
        {
            byte[] data = CommonHelper.Serialize(dobotData);
            byte[] lenArr = new byte[] { (byte)0, (byte)data.Length };
            byte[] res = lenArr.Concat(data).ToArray();
            client.BeginSend(res, 0, res.Length, SocketFlags.None, EndSendData, client);
        }



        //此处不直接发送机械臂的返回的字节数组是因为不只有机械臂数据需要发送，还有PLC数据（到了unity端一样要按照协议处理数据）
        private void EndSendData(IAsyncResult result)
        {
            Socket client = (Socket)result.AsyncState;
            client.EndSend(result);
        }





        /*-----------------PLC服务端的开启连接------------------------------------*/

        private void DisConnectPLCServer()
        {
            if (ServerModel.PLCServerState == ConnectState.Disconnected) return;
            ServerModel.PlcServer.ConnectClose();
            ServerModel.PLCServerConnectText = "已断开连接！";
            ServerModel.PLCServerState = ConnectState.Disconnected;
            source_Plc.Cancel();
        }//断开PLC连接

        private void ConnectPLCServer(object res)
        {
            object[] objects = res as object[];
            ServerModel.IpAddress = (string)objects[0];
            int index = (int)objects[1];
            switch (index)
            {
                case 0:
                    CreatePLCServer(SiemensPLCS.S200Smart, ServerModel.IpAddress);
                    break;
                case 1:
                    CreatePLCServer(SiemensPLCS.S1200, ServerModel.IpAddress);
                    break;
                default:
                    break;
            }

        }//根据输入的ip和型号连接PLC

        private void CreatePLCServer(SiemensPLCS type, string ip)
        {
            ServerModel.PlcServer = new SiemensS7Net(type, ip);
            OperateResult connect = ServerModel.PlcServer.ConnectServer();
            if (connect.IsSuccess)
            {
                ServerModel.PLCServerConnectText = "已连接！";
                ServerModel.PLCServerState = ConnectState.Connected;
                source_Plc = new CancellationTokenSource();
                token_Plc = source_Plc.Token;
                Task.Run(async () =>
                {
                    while (true)
                    {
                        if (token_Plc.IsCancellationRequested) return;
                        await Task.Delay(500);//每0.5秒发送数据到客户端
                        //SignalInputs_Send = ServerModel.PlcServer.ReadBool("I0.0", 16).Content;
                        //SignalOutputs_Send = ServerModel.PlcServer.ReadBool("Q0.0", 16).Content;

                        for (int i = 0; i < ServerModel.SocketLists.Count; i++)
                        {
                            // input = GetInputSignal();
                            // output = GetOutputSignal();
                            // // SendData(SignalInputs_Send, ServerModel.SocketLists[i], 1);
                            // // SendData(SignalOutputs_Send, ServerModel.SocketLists[i], 2);
                            // SendPLCSignalData(input, output, ServerModel.SocketLists[i]);
                            DataHelper.SendPLCData(ServerModel.PlcServer,ServerModel.SocketLists[i]);
                        }
                    }
                }, token_Plc);//开启发送PLC数据的线程
            }
            else
            {
                ServerModel.PLCServerConnectText = "连接失败！";
                ServerModel.PLCServerState = ConnectState.Disconnected;
            }
        }//创建新的PLC连接




        // private void SendPLCData(Socket client)
        // {
        //     byte[] data = CommonHelper.Serialize(inputSignal);
        //     byte[] lenArr = new byte[] { (byte)2, (byte)data.Length }; //前缀包括两个字节(数据类型和长度) 2---输入信号
        //     byte[] res_in = lenArr.Concat(data).ToArray();
        //     data = CommonHelper.Serialize(outputSignal);
        //     lenArr = new byte[] { (byte)3, (byte)data.Length }; //前缀包括两个字节(数据类型和长度)  3---输出信号
        //     byte[] res_out = lenArr.Concat(data).ToArray();
        //     byte[] end = res_in.Concat(res_out).ToArray();
        //     client.BeginSend(end, 0, end.Length, SocketFlags.None, EndSendData, client);
        // }
    }
}

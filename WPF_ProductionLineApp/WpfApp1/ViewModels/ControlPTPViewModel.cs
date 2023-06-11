using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfProductionLineApp.DobotCommunicationsFrameWork;
using WpfProductionLineApp.Models;
using WpfProductionLineApp.StaticData;

namespace WpfProductionLineApp.ViewModels
{
    public partial class ControlPTPViewModel : ObservableObject
    {

        public ControlPTPViewModel()
        {
            ChangeTypeCmd = new RelayCommand<string>(ChangeCoordinateType);
            SetPtpCmd = new RelayCommand<object>(SetPTPCmd);
            AutoSystemOpenCmd = new RelayCommand(AutoSystemOpen);
            AutoSystemCloseCmd = new RelayCommand(AutoSystemClose);
            GetAlarmStateCmd = new RelayCommand(GetAlarmState);
            ClearAllAlarmStateCmd = new RelayCommand(ClearAllAlarmState);
            TextModel = new TextModel()
            {
                CordText_1 = "X",
                CordText_2 = "Y",
                CordText_3 = "Z",
                CordText_4 = "R"
            };
            PtpMode = new ObservableCollection<string> { "JUMP", "MOVEJ", "MOVEL" };
            numToLoc = new NumToLoc();
            bufferList = new List<byte>(1024);
            socketLists = new List<Socket>();

            WeakReferenceMessenger.Default.Register<ControlJogViewModel>(this, OnReceived);//在ptp模式界面中进行移动时，需先选择相应的机械臂
            WeakReferenceMessenger.Default.Register<RobotArmModel>(this, OnReceivedOfRobotModel);
            WeakReferenceMessenger.Default.Register<ServerModel>(this, OnReceivedOfServerModel);
            WeakReferenceMessenger.Default.Register<ConnectionViewModel>(this, OnReceivedOfConnectionViewModel);
        }

        private void OnReceivedOfConnectionViewModel(object recipient, ConnectionViewModel message)
        {
            curConnectionViewModel = message;
        }

        private void OnReceivedOfServerModel(object recipient, ServerModel message)
        {
            curServerModel = message;
        }

        private void OnReceivedOfRobotModel(object recipient, RobotArmModel message)
        {
            curRobotArmModel = message;
        }

        private void OnReceived(object recipient, ControlJogViewModel message)
        {
            curControlJogViewModel = message;
        }

        private NumToLoc numToLoc;
        private CancellationTokenSource cancellationTokenSource;//管理Task的
        private CancellationToken token;//Task标识
        private RobotArmModel curRobotArmModel;
        private ServerModel curServerModel;
        private ConnectionViewModel curConnectionViewModel;
        private ControlJogViewModel curControlJogViewModel;
        private string tempData; //暂存数据中转存储处
        private bool isClosed;
        private Socket serverSocket;
        private List<byte> bufferList;
        private List<Socket> socketLists;

        [ObservableProperty]
        private bool _cartesianCor_isChecked;
        [ObservableProperty]
        private bool _jointCor_isChecked;
        [ObservableProperty]
        private string _x_J1;
        [ObservableProperty]
        private string _y_J2;
        [ObservableProperty]
        private string _z_J3;
        [ObservableProperty]
        private string _r_J4;
        [ObservableProperty]
        private int _ptpMode_SelectedIndex;

        private ObservableCollection<string> _ptpMode;
        public ObservableCollection<string> PtpMode
        {
            get { return _ptpMode; }
            set { SetProperty(ref _ptpMode, value); }
        }


        private TextModel _textModel;
        public TextModel TextModel
        {
            get { return _textModel; }
            set { _textModel = value; }
        }


        public bool[] SignalInputs { get; set; }
        public bool[] SignalOutputs { get; set; }

        private RelayCommand<string> _changeTypeCmd;
        public RelayCommand<string> ChangeTypeCmd
        {
            get { return _changeTypeCmd; }
            set { _changeTypeCmd = value; }
        }

        private RelayCommand<object> _setPtpCmd;
        public RelayCommand<object> SetPtpCmd
        {
            get { return _setPtpCmd; }
            set { _setPtpCmd = value; }
        }

        private RelayCommand _autoSystemOpenCmd;
        public RelayCommand AutoSystemOpenCmd
        {
            get { return _autoSystemOpenCmd; }
            set { _autoSystemOpenCmd = value; }
        }

        private RelayCommand _autoSystemCloseCmd;
        public RelayCommand AutoSystemCloseCmd
        {
            get { return _autoSystemCloseCmd; }
            set { _autoSystemCloseCmd = value; }
        }

        private RelayCommand _getAlarmStateCmd;
        public RelayCommand GetAlarmStateCmd { get => _getAlarmStateCmd; set => _getAlarmStateCmd = value; }

        private RelayCommand _clearAllAlarmStateCmd;
        public RelayCommand ClearAllAlarmStateCmd
        {
            get { return _clearAllAlarmStateCmd; }
            set { _clearAllAlarmStateCmd = value; }
        }

        [RelayCommand]
        private void TestRobotLocation()
        {
            Gripping("COM4", 3, 0);
        }


        private void AutoSystemClose()
        {
            cancellationTokenSource.Cancel();
            CloseTcpServer();
        }//自动化系统关闭

        private void AutoSystemOpen()
        {
            if (curServerModel.PLCServerState == ConnectState.Disconnected) return;//PLC服务端未连接，直接返回
            cancellationTokenSource = new CancellationTokenSource();
            token = cancellationTokenSource.Token;
            for (int i = 0; i < curRobotArmModel.portLists.Count; i++)
            {
                curRobotArmModel.portLists[i].SetHomeCmd(false);
            }
            //ReaderWriterLockSlim rwlock= new ReaderWriterLockSlim(); //开多线程是为了实现机械臂同时抓取，异步编程也能实现？
            Task.Run(async () =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested) return;//若检测到线程取消，则退出循环
                    //自定义时间
                    //rwlock.EnterWriteLock();
                    SignalInputs = curServerModel.PlcServer.ReadBool("I0.0", 16).Content;//预测读取时间平均在5ms左右
                    SignalOutputs = curServerModel.PlcServer.ReadBool("Q0.0", 16).Content;//预测读取时间平均在5ms左右
                    //rwlock.ExitWriteLock();
                    await Task.Delay(500);
                }
            }, token); //主线程，读取PLC数据（这里没有对数据加读写锁，因为加锁后反而不能并行执行，很奇怪）
            Task.Run(async () =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested) return;//若检测到线程取消，则退出循环
                    await Task.Delay(500);
                    //rwlock.EnterReadLock();
                    if (SignalInputs[6] && SignalOutputs[2])//第一台机械臂------>>这里输出信号的开关已经隐含了传送带的开关（PLC程序），所以不需要再加传送带开关判断
                    {
                        StartGripTask("COM6");//执行对应的抓取方法
                        curServerModel.PlcServer.Write("Q0.2", false);//关闭气缸
                    }
                    //rwlock.ExitReadLock();
                }
            });//检测第一台机械臂
            Task.Run(async () =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested) return;//若检测到线程取消，则退出循环
                    await Task.Delay(500);
                    //rwlock.EnterReadLock();
                    if (SignalInputs[7] && SignalOutputs[3])//第二台机械臂
                    {
                        StartGripTask("COM5");
                        curServerModel.PlcServer.Write("Q0.3", false);//关闭气缸
                    }
                    //rwlock.ExitReadLock();
                }
            });//检测第二台机械臂
            Task.Run(async () =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested) return;//若检测到线程取消，则退出循环
                    await Task.Delay(500);
                    //rwlock.EnterReadLock();
                    if (SignalInputs[8] && SignalOutputs[4])//第三台机械臂
                    {
                        StartGripTask("COM7");
                        curServerModel.PlcServer.Write("Q0.4", false);//关闭气缸
                    }
                    //rwlock.ExitReadLock();
                }
            });//检测第三台机械臂
            Task.Run(async () =>
            {
                Process process = CameraScratch.StartWorking();//开启进程
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        foreach (Socket item in socketLists) //发送停止信号
                        {
                            SendData("stop", item);
                        }
                        process.CloseMainWindow();
                        process.WaitForExit();
                        break;
                    }//若检测到线程取消，则退出循环}
                    await Task.Delay(500);
                    //rwlock.EnterReadLock();
                    if (SignalInputs[9] && SignalOutputs[5])//第四台机械臂
                    {
                        //StartGripTask("COM4");
                        //这里需要将机械臂关闭，否则无法正常开启进程----这里现在改为由c#上位机控制机械臂抓取而不是python那边控制，可以正常开启机械臂
                        foreach (Socket item in socketLists) //发送启动信号
                        {
                            SendData("start", item);
                        }
                        StartScratch("COM4"); //等待接收坐标进行抓取
                        curServerModel.PlcServer.Write("Q0.5", false);//关闭气缸
                    }
                    //rwlock.ExitReadLock();
                }
            });//检测第四台机械臂

            /*Task task = Task.Run(async () =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested) return;//若检测到线程取消，则退出循环
                    await Task.Delay(500);//自定义时间
                    SignalInputs = curServerModel.PlcServer.ReadBool("I0.0", 16).Content;//预测读取时间平均在5ms左右
                    SignalOutputs = curServerModel.PlcServer.ReadBool("Q0.0", 16).Content;//预测读取时间平均在5ms左右
                    if (SignalInputs[6] && SignalOutputs[2])//第一台机械臂------>>这里输出信号的开关已经隐含了传送带的开关（PLC程序），所以不需要再加传送带开关判断
                    {
                        StartGripTask("COM6");//执行对应的抓取方法
                        curServerModel.PlcServer.Write("Q0.2", false);//关闭气缸
                    }
                    if (SignalInputs[7] && SignalOutputs[3])//第二台机械臂
                    {
                        StartGripTask("COM5");
                        curServerModel.PlcServer.Write("Q0.3", false);//关闭气缸

                    }
                    if (SignalInputs[8] && SignalOutputs[4])//第三台机械臂
                    {
                        StartGripTask("COM7");
                        curServerModel.PlcServer.Write("Q0.4", false);//关闭气缸
                    }
                    if (SignalInputs[9] && SignalOutputs[5])//第四台机械臂
                    {
                        StartGripTask("COM4");
                        curServerModel.PlcServer.Write("Q0.5", false);//关闭气缸
                    }
                }
            },token);*///单台检测，没有并行执行
            OpenTcpServer("192.168.8.168", 10002, 3);


        }//自动化系统开启(根据传感器信号自动抓取物体)

        private void StartGripTask(string com)
        {
            switch (com)
            {
                case "COM6"://dobot1#
                    Gripping(com, 0, numToLoc._1count);
                    AddCount(ref numToLoc._1count);
                    break;
                case "COM5"://dobot2#
                    Gripping(com, 1, numToLoc._2count);
                    AddCount(ref numToLoc._2count);
                    break;
                case "COM7"://dobot3#
                    Gripping(com, 2, numToLoc._3count);
                    AddCount(ref numToLoc._3count);
                    break;
                case "COM4"://dobot4#
                    Gripping(com, 3, numToLoc._4count);
                    AddCount(ref numToLoc._4count);
                    break;
            }
        }

        /// <summary>
        /// 设置连接状态和抓取点位
        /// </summary>
        /// <param name="com">串口号</param>
        /// <param name="robotIndex">机械臂索引号，从零开始</param>
        /// <param name="gripLocationIndex">抓取的位置索引</param>
        private void Gripping(string com, int robotIndex, int gripLocationIndex)
        {
            if (!curRobotArmModel.serialPorts.ContainsKey(com)) //说明机械臂未连接，进行连接(这里其实有延迟)
            {
                curConnectionViewModel.ConnectDobot(com);
            }
            SerialPort curPort = curRobotArmModel.serialPorts[com];
            //设置点位
            ulong cmdIndex = 0;//最后指令索引
            ulong curIndex = 0;//当前指令索引
            DobotHelper.SetQueuedCmdClear(curPort);
            Thread.Sleep(10);//
            DobotHelper.SetQueuedCmdStartExec(curPort);
            DobotHelper.SetPTPCmd(curPort, RobotArmGripLocation.locGripArr[robotIndex * 6 + gripLocationIndex], ptpMode.MOVL_XYZ, ref cmdIndex, true);//移动到抓取位置处
            DobotHelper.SetEndEffectorSuctionCup(curPort, true, true, true);//开启吸盘
            DobotHelper.SetPTPCmd(curPort, RobotArmGripLocation.locPlaceArr[robotIndex], ptpMode.JUMP_XYZ, ref cmdIndex, true);//移动到放置位置处
            DobotHelper.SetEndEffectorSuctionCup(curPort, false, false, true);//关闭吸盘
            DobotHelper.SetPTPCmd(curPort, RobotArmGripLocation.homePara, ptpMode.JUMP_XYZ, ref cmdIndex, true);//回到初始点位
            while (cmdIndex > curIndex)
            {
                curIndex = DobotHelper.GetQueuedCmdCurrentIndex(curPort);
                Thread.Sleep(200);
            }
            DobotHelper.SetQueuedCmdStopExec(curPort);

        }

        private void AddCount(ref int count)
        {
            count += 1;
            if (count > 5)
                count = 0;
        }//计数函数，用于每次执行后变更抓取位置

        private void ChangeCoordinateType(string? btnName)
        {
            switch (btnName)
            {
                case "XYZR":
                    TextModel.CordText_1 = "X";
                    TextModel.CordText_2 = "Y";
                    TextModel.CordText_3 = "Z";
                    TextModel.CordText_4 = "R";
                    CartesianCor_isChecked = true;
                    break;
                case "Joint":
                    TextModel.CordText_1 = "J1";
                    TextModel.CordText_2 = "J2";
                    TextModel.CordText_3 = "J3";
                    TextModel.CordText_4 = "J4";
                    JointCor_isChecked = true;
                    break;
                default: break;
            }
        }//更改坐标系文字

        private void SetPTPCmd(object res)//PTP模式移动
        {
            if (curControlJogViewModel.RobotPort != null)
            {
                object[] objects = (object[])res;
                float[] floats = new float[4]
                {
                    float.Parse((string)objects[0]),
                    float.Parse((string)objects[1]),
                    float.Parse((string)objects[2]),
                    float.Parse((string)objects[3]),
                };
                int index = (int)objects[4];//ptp模式
                bool ptpMode_1 = (bool)objects[5];//选坐标系
                bool ptpMode_2 = (bool)objects[6];//同上
                ulong cmdIndex = 0;
                if (!ptpMode_1 && !ptpMode_2) return;
                if (ptpMode_1)//模式为XYZR
                {
                    switch (index)
                    {
                        case 0://
                            DobotHelper.SetPTPCmd(curControlJogViewModel.RobotPort, floats, ptpMode.JUMP_XYZ, ref cmdIndex, false);
                            break;
                        case 1:
                            DobotHelper.SetPTPCmd(curControlJogViewModel.RobotPort, floats, ptpMode.MOVJ_XYZ, ref cmdIndex, false);
                            break;
                        case 2:
                            DobotHelper.SetPTPCmd(curControlJogViewModel.RobotPort, floats, ptpMode.MOVL_XYZ, ref cmdIndex, false);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (index)
                    {
                        case 0://
                            DobotHelper.SetPTPCmd(curControlJogViewModel.RobotPort, floats, ptpMode.JUMP_ANGLE, ref cmdIndex, false);
                            break;
                        case 1:
                            DobotHelper.SetPTPCmd(curControlJogViewModel.RobotPort, floats, ptpMode.MOVJ_ANGLE, ref cmdIndex, false);
                            break;
                        case 2:
                            DobotHelper.SetPTPCmd(curControlJogViewModel.RobotPort, floats, ptpMode.MOVL_ANGLE, ref cmdIndex, false);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void GetAlarmState()//获取报警状态
        {
            curControlJogViewModel.RobotPort?.GetAlarmsState();
        }

        private void ClearAllAlarmState()//清除所有报警状态
        {
            curControlJogViewModel.RobotPort?.ClearAllAlarmsState();
        }

        private void OpenTcpServer(string ip, int port, int maxCount) //此服务端用于接收视觉检测后的结果和控制视觉检测的开启
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            isClosed = false;
            serverSocket = socket;
            socket.Bind(endPoint);//绑定端口
            socket.Listen(maxCount);//开始监听，同时设置最大连接数
            socket.BeginAccept(OnAccepted, socket);//开启异步监听
        }

        private void CloseTcpServer()
        {
            foreach (var item in socketLists)
            {
                item.Shutdown(SocketShutdown.Both);
                item.Close();
            }
            socketLists.Clear();
            isClosed = true;
            serverSocket.Close();
        }

        private void OnAccepted(IAsyncResult ar)
        {
            if (isClosed) return;
            byte[] buffer = new byte[1024];
            Socket serverSocket = (Socket)ar.AsyncState;//这是用于监听客户端连接的socket
            Socket client_ServerSocket = serverSocket.EndAccept(ar);//这是用于和客户端进行数据传输的socket
            socketLists.Add(client_ServerSocket);
            client_ServerSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, EndReceiveData, new ReceiveState
            {
                cilentSocket = client_ServerSocket,
                buffer = buffer
            });
            serverSocket.BeginAccept(OnAccepted, serverSocket);//继续监听
        }

        private void EndReceiveData(IAsyncResult ar)
        {
            ReceiveState receiveState = (ReceiveState)ar.AsyncState;//接收从beginReceive中传递来的参数
            Socket client = receiveState.cilentSocket;
            byte[] buffer = receiveState.buffer;
            bufferList.AddRange(buffer);
            try
            {
                int len = client.EndReceive(ar);
                char[] chs = new char[len];
                tempData = Encoding.UTF8.GetString(buffer, 0, len);
                bufferList.RemoveRange(0, len);
                //ProcessData(client, buffer);
            }
            catch (Exception ex)
            {
                client.Close();
            }
        }

        /// 定义一个传输协议：包头+数据类型+数据长度+内容
        // 0-->Uint32  1-->string 2-->float32
        private void ProcessData(Socket client, byte[] buffer)
        {
            if (client == null || !client.Connected)
                return;
            while (bufferList.Count > 4)
            {
                if (bufferList[0] == 0xAA)
                {
                    int type = bufferList[1];//数据类型
                    int len = bufferList[2];//负载长度
                    switch (type)
                    {
                        case 0:
                            uint[] MsgUint = new uint[len / 4];
                            for (int i = 3, count = 0; i < len + 3; i += 4, count++)
                            {
                                MsgUint[count] = BitConverter.ToUInt32(buffer[i..(i + 4)]);
                            }
                            break;
                        case 1:
                            string msgString = Encoding.GetEncoding("GBK").GetString(buffer, 3, len);
                            break;
                        case 2:
                            float[] MsgFloat = new float[len / 4];
                            for (int i = 3, count = 0; i < len + 3; i += 4, count++)
                            {
                                MsgFloat[count] = BitConverter.ToSingle(buffer[i..(i + 4)]);
                            }
                            break;
                        default:
                            break;
                    }
                    bufferList.RemoveRange(0, len + 3);
                }
                else//帧头不正确
                {
                    bufferList.RemoveAt(0);
                }
            }
        }

        private void SendData(string msg, Socket client)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(msg);
                client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, EndSendData, client);
            }
            catch (Exception ex)
            {
                MessageBox.Show("发送字符串失败失败：" + ex.Message);
            }
        }

        private void EndSendData(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            client.EndSend(ar);
        }

        private void StartScratch(string com)
        {
            string res = tempData;
            if (!curRobotArmModel.serialPorts.ContainsKey(com)) return;
            string[] result;
            ulong cmdIndex = 0;
            ulong curIndex = 0;
            SerialPort port = curRobotArmModel.serialPorts[com];
            while (true)
            {
                if (tempData == null) continue;
                result = tempData.Split(' ');
                if (result.Length < 4) break;//说明数据接收不完整
                float w_x = float.Parse(result[0]);
                float w_y = float.Parse(result[1]);
                float width = float.Parse(result[2]);
                float height = float.Parse(result[3]);
                float angle = -90f;//默认是横屏状态的旋转角度
                if (width < height) { w_x -= 2f; w_y -= 1.5f; angle = -4.5f; }//竖屏状态的参数
                DobotHelper.SetQueuedCmdStartExec(port);
                DobotHelper.SetPTPCmd(port, new float[] { w_x, w_y, -30f, 0f }, ptpMode.MOVL_XYZ, ref cmdIndex, true);
                DobotHelper.SetPTPCmd(port, new float[] { w_x, w_y, -63.4053f, 0f }, ptpMode.MOVL_XYZ, ref cmdIndex, true);
                DobotHelper.SetEndEffectorSuctionCup(port, true, true, true);
                DobotHelper.SetPTPCmd(port, new float[] { 140.1508f, -277.2743f, -47.7805f, angle }, ptpMode.JUMP_XYZ, ref cmdIndex, true);
                DobotHelper.SetEndEffectorSuctionCup(port, false, false, true);
                DobotHelper.SetPTPCmd(port, new float[] { 200f, -180f, 30f, 0f }, ptpMode.JUMP_XYZ, ref cmdIndex, true);
                while (cmdIndex > curIndex)
                {
                    curIndex = DobotHelper.GetQueuedCmdCurrentIndex(port);
                    Thread.Sleep(100);
                }
                DobotHelper.SetQueuedCmdStopExec(port);
                break;//执行到这说明机械臂已经完成动作，跳出循环
            }
        }
    }
}

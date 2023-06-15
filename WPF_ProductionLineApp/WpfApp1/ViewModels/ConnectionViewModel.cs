using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Threading;
using WpfProductionLineApp.Auboi5FrameWork;
using WpfProductionLineApp.DobotCommunicationsFrameWork;
using WpfProductionLineApp.DobotCommunicationsFrameWork.Common;
using WpfProductionLineApp.Models;

namespace WpfProductionLineApp.ViewModels
{
    public partial class ConnectionViewModel : ObservableObject
    {
        public ConnectionViewModel()
        {
            robotModel = new RobotArmModel()
            {
                SerialPort_1 = "COM6",
                StateDobot_1 = ConnectState.Disconnected,
                Dobot1_ConnectState = "断开连接",
                SerialPort_2 = "COM5",
                StateDobot_2 = ConnectState.Disconnected,
                Dobot2_ConnectState = "断开连接",
                SerialPort_3 = "COM7",
                StateDobot_3 = ConnectState.Disconnected,
                Dobot3_ConnectState = "断开连接",
                SerialPort_4 = "COM4",
                StateDobot_4 = ConnectState.Disconnected,
                Dobot4_ConnectState = "断开连接",
                StateAubo = ConnectState.Disconnected,
                Aubo_ConnectState = "断开连接"
            };
            WeakReferenceMessenger.Default.Send<RobotArmModel>(robotModel);//在连接时发送实例引用给其他viewModel
            WeakReferenceMessenger.Default.Send<ConnectionViewModel>(connectionViewModel);       //将自身引用发送给注册号的ViewModel
            //WeakReferenceMessenger.Default.Register<ServerViewModel>(this, OnReceived);
        }



        public ConnectionViewModel connectionViewModel
        {
            get { return this; }
            private set { }
        }


        private Dictionary<string, DispatcherTimer> timersDic = new Dictionary<string, DispatcherTimer>();//根据串口号存储计时器

        public RobotArmModel robotModel { get; set; }

        private RelayCommand<string> _connectCmd;
        public RelayCommand<string> ConnectCmd
        {
            get
            {
                if (_connectCmd == null)
                {
                    _connectCmd = new RelayCommand<string>(ConnectDobot);
                }
                return _connectCmd;
            }
        }

        private RelayCommand<string> _disconnectCmd;
        public RelayCommand<string> DisconnectCmd
        {
            get
            {
                if (_disconnectCmd == null)
                {
                    _disconnectCmd = new RelayCommand<string>(DisconnectDobot);
                }
                return _disconnectCmd;
            }
        }

        [RelayCommand]
        private async Task ConnectAuboi5()
        {
            await Task.Run(() =>
            {
                bool res = Auboi5Method.Connect();
                if (res)
                {
                    robotModel.StateAubo = ConnectState.Connected;
                    robotModel.Aubo_ConnectState = "连接成功";
                }
                else
                {
                    robotModel.StateAubo = ConnectState.Disconnected;
                    robotModel.Aubo_ConnectState = "连接失败";
                }
            });

        }
        [RelayCommand]
        private async Task DisconnectAuboi5()
        {
            await Task.Delay(100);
            Auboi5Method.DisConnect();
            robotModel.StateAubo = ConnectState.Disconnected;
            robotModel.Aubo_ConnectState = "断开连接";
        }

        public void ConnectDobot(string com)
        {
            if (com == null || robotModel.serialPorts.ContainsKey(com)) return;

            SerialPort port = DobotHelper.Connect(com);
            if (port == null) return;
            robotModel.serialPorts.Add(com, port);
            robotModel.portLists.Add(port);
            bool[] state = port.GetEndEffectorSuctionCup();
            if (!robotModel.robot2EndEffector.ContainsKey(com))
                robotModel.robot2EndEffector.Add(com, state[1] ? RobotSuckState.open : RobotSuckState.close);//添加末端状态，
            else
                robotModel.robot2EndEffector[com] = state[1] ? RobotSuckState.open : RobotSuckState.close;
            //WeakReferenceMessenger.Default.Send<RobotArmModel>(robotModel);//在连接时发送实例引用给其他viewModel
            switch (com)
            {
                case "COM6":
                    robotModel.StateDobot_1 = ConnectState.Connected;
                    robotModel.Dobot1_ConnectState = "连接成功";
                    robotModel.port2IndexDic.Add(port, 10000);
                    robotModel.Index2portDic.Add(10000, port);
                    //CreateTaskWithCancelToken(tokenSource_1, token_1,com);
                    StartTimer(com);
                    break;
                case "COM5":
                    robotModel.StateDobot_2 = ConnectState.Connected;
                    robotModel.Dobot2_ConnectState = "连接成功";
                    robotModel.port2IndexDic.Add(port, 10001);
                    robotModel.Index2portDic.Add(10001, port);
                    StartTimer(com);
                    //CreateTaskWithCancelToken(tokenSource_2, token_2,com);
                    break;
                case "COM7":
                    robotModel.StateDobot_3 = ConnectState.Connected;
                    robotModel.Dobot3_ConnectState = "连接成功";
                    robotModel.port2IndexDic.Add(port, 10002);
                    robotModel.Index2portDic.Add(10002, port);
                    StartTimer(com);
                    //CreateTaskWithCancelToken(tokenSource_3, token_3,com);
                    break;
                case "COM4":
                    robotModel.StateDobot_4 = ConnectState.Connected;
                    robotModel.Dobot4_ConnectState = "连接成功";
                    robotModel.port2IndexDic.Add(port, 10003);
                    robotModel.Index2portDic.Add(10003, port);
                    StartTimer(com);
                    //CreateTaskWithCancelToken(tokenSource_4, token_4,com);
                    break;
                default:
                    return;
            }


        }//连接机械臂

        public void DisconnectDobot(string com)
        {
            if (com == null) return;
            if (robotModel.serialPorts.ContainsKey(com))
            {
                ObservableCollection<Socket> sockets = Locator.ServiceProvide.GetService<ServerViewModel>().ServerModel.SocketLists;
                foreach (Socket socket in sockets) //发送机械臂断开消息
                {
                    DataHelper.SendDisconnectData(robotModel.port2IndexDic[robotModel.serialPorts[com]], socket);
                }                
                DobotHelper.Disconnect(robotModel.serialPorts[com]);
                robotModel.Index2portDic.Remove(robotModel.port2IndexDic[robotModel.serialPorts[com]]);
                robotModel.port2IndexDic.Remove(robotModel.serialPorts[com]);
                robotModel.portLists.Remove(robotModel.serialPorts[com]);
                robotModel.serialPorts.Remove(com);
                robotModel.robot2EndEffector[com] = RobotSuckState.Disconnect;
                //这里需要对JogViewModel的SuckState进行赋值---
                //robotModel.robot2EndEffector.Remove(com);//移除末端状态，
                switch (com)
                {
                    case "COM6":
                        robotModel.StateDobot_1 = ConnectState.Disconnected;
                        robotModel.Dobot1_ConnectState = "断开连接";
                        //tokenSource_1?.Cancel();
                        CloseTimer(com);
                        break;
                    case "COM5":
                        robotModel.StateDobot_2 = ConnectState.Disconnected;
                        robotModel.Dobot2_ConnectState = "断开连接";
                        //tokenSource_2?.Cancel();
                        CloseTimer(com);
                        break;
                    case "COM7":
                        robotModel.StateDobot_3 = ConnectState.Disconnected;
                        robotModel.Dobot3_ConnectState = "断开连接";
                        //tokenSource_3?.Cancel();
                        CloseTimer(com);
                        break;
                    case "COM4":
                        robotModel.StateDobot_4 = ConnectState.Disconnected;
                        robotModel.Dobot4_ConnectState = "断开连接";
                        //tokenSource_4?.Cancel();
                        CloseTimer(com);
                        break;
                    default:
                        return;
                }
            }

        }//断开机械臂并终止数据传输线程

        private void StartTimer(string com)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(400);//每0.4s执行事件，获取角度数据
            switch (com)
            {
                case "COM6":
                    timer.Tick += new EventHandler(Timer_Elapsed_Dobot_1);
                    break;
                case "COM5":
                    timer.Tick += new EventHandler(Timer_Elapsed_Dobot_2);
                    break;
                case "COM7":
                    timer.Tick += new EventHandler(Timer_Elapsed_Dobot_3);
                    break;
                case "COM4":
                    timer.Tick += new EventHandler(Timer_Elapsed_Dobot_4);
                    break;
                default:
                    break;
            }
            timer.Start();
            timersDic.Add(com, timer);
        }//开启定时器获取机械臂角度数据
        private void Timer_Elapsed_Dobot_4(object? sender, EventArgs e)
        {
            if (robotModel.StateDobot_4 == ConnectState.Connected)
            {
                float[] res = DobotHelper.GetPose(robotModel.serialPorts["COM4"]);
                if (res == null || res.Length < 8) return;
                robotModel.Dobot4_X = (float)Math.Round(res[0], 4);
                robotModel.Dobot4_Y = (float)Math.Round(res[1], 4);
                robotModel.Dobot4_Z = (float)Math.Round(res[2], 4);
                robotModel.Dobot4_R = (float)Math.Round(res[3], 4);
                robotModel.Dobot4_J1 = (float)Math.Round(res[4], 4);
                robotModel.Dobot4_J2 = (float)Math.Round(res[5], 4);
                robotModel.Dobot4_J3 = (float)Math.Round(res[6], 4);
                robotModel.Dobot4_J4 = (float)Math.Round(res[7], 4);
            }
        }

        private void Timer_Elapsed_Dobot_3(object? sender, EventArgs e)
        {
            if (robotModel.StateDobot_3 == ConnectState.Connected)
            {
                float[] res = DobotHelper.GetPose(robotModel.serialPorts["COM7"]);
                if (res == null || res.Length < 8) return;
                robotModel.Dobot3_X = (float)Math.Round(res[0], 4);
                robotModel.Dobot3_Y = (float)Math.Round(res[1], 4);
                robotModel.Dobot3_Z = (float)Math.Round(res[2], 4);
                robotModel.Dobot3_R = (float)Math.Round(res[3], 4);
                robotModel.Dobot3_J1 = (float)Math.Round(res[4], 4);
                robotModel.Dobot3_J2 = (float)Math.Round(res[5], 4);
                robotModel.Dobot3_J3 = (float)Math.Round(res[6], 4);
                robotModel.Dobot3_J4 = (float)Math.Round(res[7], 4);
            }
        }

        private void Timer_Elapsed_Dobot_2(object? sender, EventArgs e)
        {
            if (robotModel.StateDobot_2 == ConnectState.Connected)
            {
                float[] res = DobotHelper.GetPose(robotModel.serialPorts["COM5"]);
                if (res == null || res.Length < 8) return;
                robotModel.Dobot2_X = (float)Math.Round(res[0], 4);
                robotModel.Dobot2_Y = (float)Math.Round(res[1], 4);
                robotModel.Dobot2_Z = (float)Math.Round(res[2], 4);
                robotModel.Dobot2_R = (float)Math.Round(res[3], 4);
                robotModel.Dobot2_J1 = (float)Math.Round(res[4], 4);
                robotModel.Dobot2_J2 = (float)Math.Round(res[5], 4);
                robotModel.Dobot2_J3 = (float)Math.Round(res[6], 4);
                robotModel.Dobot2_J4 = (float)Math.Round(res[7], 4);
            }
        }

        private void Timer_Elapsed_Dobot_1(object? sender, EventArgs e)
        {
            if (robotModel.StateDobot_1 == ConnectState.Connected)
            {
                float[] res = DobotHelper.GetPose(robotModel.serialPorts["COM6"]);
                if (res == null || res.Length < 8) return;
                robotModel.Dobot1_X = (float)Math.Round(res[0], 4);
                robotModel.Dobot1_Y = (float)Math.Round(res[1], 4);
                robotModel.Dobot1_Z = (float)Math.Round(res[2], 4);
                robotModel.Dobot1_R = (float)Math.Round(res[3], 4);
                robotModel.Dobot1_J1 = (float)Math.Round(res[4], 4);
                robotModel.Dobot1_J2 = (float)Math.Round(res[5], 4);
                robotModel.Dobot1_J3 = (float)Math.Round(res[6], 4);
                robotModel.Dobot1_J4 = (float)Math.Round(res[7], 4);
            }
        }

        private void CloseTimer(string com)
        {
            if (timersDic.ContainsKey(com))
            {
                timersDic[com].Stop();
                timersDic.Remove(com);
            }
        }












    }


}

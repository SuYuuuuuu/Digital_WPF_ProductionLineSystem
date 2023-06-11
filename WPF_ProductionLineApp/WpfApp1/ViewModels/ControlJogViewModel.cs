using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.IO.Ports;
using WpfProductionLineApp.DobotCommunicationsFrameWork;
using WpfProductionLineApp.Models;

namespace WpfProductionLineApp.ViewModels
{
    public enum RobotSuckState
    {
        Disconnect,
        open,
        close
    }
    public partial class ControlJogViewModel : ObservableObject
    {
        public ControlJogViewModel()
        {
            WeakReferenceMessenger.Default.Register<RobotArmModel>(this, OnReceived);
            SelectBtnCmd = new RelayCommand<string>(SelectBtn);//绑定选择机械臂型号的方法
            ZeroCmd = new RelayCommand(Zero);
            SuckOpenCmd = new RelayCommand(SetSuckOpen);
            SuckCloseCmd = new RelayCommand(SetSuckClose);
            //JogCmd = new RelayCommand<string>(SetJogCmd);
            WeakReferenceMessenger.Default.Send<ControlJogViewModel>(this);
        }

        private void OnReceived(object recipient, RobotArmModel message)
        {
            robotModel = message;
        }


        [ObservableProperty]
        private bool _dobot1_isChecked;
        [ObservableProperty]
        private bool _dobot2_isChecked;
        [ObservableProperty]
        private bool _dobot3_isChecked;
        [ObservableProperty]
        private bool _dobot4_isChecked;

        private RobotArmModel robotModel;
        public RobotArmModel RobotModel { get => robotModel; private set { } }


        private RelayCommand<string>? _selectBtnCmd;
        public RelayCommand<string>? SelectBtnCmd
        {
            get { return _selectBtnCmd; }
            set { _selectBtnCmd = value; }
        }

        private SerialPort? _robotPort;
        public SerialPort? RobotPort //当前的机械臂串口
        {
            get { return _robotPort; }
            set { _robotPort = value; }
        }

        private RobotSuckState _robotEndEffectorState;
        public RobotSuckState RobotEndEffectorState //当前的机械臂末端开启状态
        {
            get { return _robotEndEffectorState; }
            set { SetProperty(ref _robotEndEffectorState, value); }
        }

        private RelayCommand _zeroCmd;
        public RelayCommand ZeroCmd
        {
            get { return _zeroCmd; }
            set { _zeroCmd = value; }
        }

        private RelayCommand _suckOpenCmd;
        public RelayCommand SuckOpenCmd
        {
            get { return _suckOpenCmd; }
            set { _suckOpenCmd = value; }
        }

        private RelayCommand _suckCloseCmd;
        public RelayCommand SuckCloseCmd
        {
            get { return _suckCloseCmd; }
            set { _suckCloseCmd = value; }
        }

        private RelayCommand<string> _jogCmd;
        public RelayCommand<string> JogCmd
        {
            get { return _jogCmd; }
            set { _jogCmd = value; }
        }



        private void SelectBtn(string? name)
        {
            switch (name)
            {
                case "Dobot1#":
                    if (robotModel.StateDobot_1 == ConnectState.Connected)
                    {
                        RobotPort = robotModel.serialPorts[robotModel.SerialPort_1];
                        if (robotModel.robot2EndEffector.ContainsKey(robotModel.SerialPort_1))
                            RobotEndEffectorState = robotModel.robot2EndEffector[robotModel.SerialPort_1];
                        Dobot1_isChecked = true;
                    }
                    else
                    {
                        RobotPort = null;
                        RobotEndEffectorState = RobotSuckState.Disconnect;
                    }

                    break;
                case "Dobot2#":
                    if (robotModel.StateDobot_2 == ConnectState.Connected)
                    {
                        RobotPort = robotModel.serialPorts[robotModel.SerialPort_2];
                        if (robotModel.robot2EndEffector.ContainsKey(robotModel.SerialPort_2))
                            RobotEndEffectorState = robotModel.robot2EndEffector[robotModel.SerialPort_2];
                        Dobot2_isChecked = true;
                    }
                    else
                    {
                        RobotPort = null;
                        RobotEndEffectorState = RobotSuckState.Disconnect;
                    }
                    break;
                case "Dobot3#":
                    if (robotModel.StateDobot_3 == ConnectState.Connected)
                    {
                        RobotPort = robotModel.serialPorts[robotModel.SerialPort_3];
                        if (robotModel.robot2EndEffector.ContainsKey(robotModel.SerialPort_3))
                            RobotEndEffectorState = robotModel.robot2EndEffector[robotModel.SerialPort_3];
                        Dobot3_isChecked = true;
                    }
                    else
                    {
                        RobotPort = null;
                        RobotEndEffectorState = RobotSuckState.Disconnect;
                    }
                    break;
                case "Dobot4#":
                    if (robotModel.StateDobot_4 == ConnectState.Connected)
                    {
                        RobotPort = robotModel.serialPorts[robotModel.SerialPort_4];
                        if (robotModel.robot2EndEffector.ContainsKey(robotModel.SerialPort_4))
                            RobotEndEffectorState = robotModel.robot2EndEffector[robotModel.SerialPort_4];
                        Dobot4_isChecked = true;
                    }
                    else
                    {
                        RobotPort = null;
                        RobotEndEffectorState = RobotSuckState.Disconnect;
                    }
                    break;
                default:
                    RobotPort = null;
                    break;
            }
        }//根据按钮名字选择机械臂并获取对应的串口实例

        private void Zero()
        {
            if (RobotPort != null)
            {
                //回零
                DobotHelper.SetHomeCmd(RobotPort, false);
            }
        }

        private void SetSuckOpen()
        {
            if (RobotPort != null)
            {
                //吸盘开
                DobotHelper.SetEndEffectorSuctionCup(RobotPort, true, true, false);
                if (robotModel.port2IndexDic.ContainsKey(RobotPort))
                {
                    RobotEndEffectorState = RobotSuckState.open;
                    switch (robotModel.port2IndexDic[RobotPort])
                    {
                        case 1:
                            robotModel.robot2EndEffector["COM6"] = RobotSuckState.open;
                            break;
                        case 2:
                            robotModel.robot2EndEffector["COM5"] = RobotSuckState.open;
                            break;
                        case 3:
                            robotModel.robot2EndEffector["COM7"] = RobotSuckState.open;
                            break;
                        case 4:
                            robotModel.robot2EndEffector["COM4"] = RobotSuckState.open;
                            break;
                    }
                }
            }
        }

        private void SetSuckClose()
        {
            if (RobotPort != null)
            {
                //吸盘关
                DobotHelper.SetEndEffectorSuctionCup(RobotPort, false, false, false);
                if (robotModel.port2IndexDic.ContainsKey(RobotPort))
                {
                    RobotEndEffectorState = RobotSuckState.close;
                    switch (robotModel.port2IndexDic[RobotPort])
                    {
                        case 1:
                            robotModel.robot2EndEffector["COM6"] = RobotSuckState.close;
                            break;
                        case 2:
                            robotModel.robot2EndEffector["COM5"] = RobotSuckState.close;
                            break;
                        case 3:
                            robotModel.robot2EndEffector["COM7"] = RobotSuckState.close;
                            break;
                        case 4:
                            robotModel.robot2EndEffector["COM4"] = RobotSuckState.close;
                            break;
                    }
                }
            }
        }



    }
}

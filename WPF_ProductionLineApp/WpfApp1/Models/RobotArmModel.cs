using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.IO.Ports;
using WpfProductionLineApp.ViewModels;

namespace WpfProductionLineApp.Models
{


    public enum ConnectState
    {
        Connected,
        Disconnected
    }
    public partial class RobotArmModel : ObservableObject
    {
        public Dictionary<string, SerialPort> serialPorts = new Dictionary<string, SerialPort>();//已经连接的串口号和对应的机械臂
        public List<SerialPort> portLists = new List<SerialPort>();//已经连接的机械臂
        public Dictionary<SerialPort, int> port2IndexDic = new Dictionary<SerialPort, int>();//根据串口可以查找到其索引（第几台）
        public Dictionary<int, SerialPort> Index2portDic = new Dictionary<int, SerialPort>();//根据索引能够查找到串口
        public Dictionary<string, RobotSuckState> robot2EndEffector = new Dictionary<string, RobotSuckState>(); //根据

        private string serialPort_1;
        public string SerialPort_1
        {
            get { return serialPort_1; }
            set { SetProperty(ref serialPort_1, value); }
        }
        private string serialPort_2;
        public string SerialPort_2
        {
            get { return serialPort_2; }
            set { SetProperty(ref serialPort_2, value); }
        }
        private string serialPort_3;
        public string SerialPort_3
        {
            get { return serialPort_3; }
            set { SetProperty(ref serialPort_3, value); }
        }
        private string serialPort_4;
        public string SerialPort_4
        {
            get { return serialPort_4; }
            set { SetProperty(ref serialPort_4, value); }
        }


        private ConnectState _stateDobot_1;
        public ConnectState StateDobot_1
        {
            get => _stateDobot_1;
            set => SetProperty(ref _stateDobot_1, value);
        }

        private ConnectState _stateDobot_2;
        public ConnectState StateDobot_2
        {
            get => _stateDobot_2;
            set => SetProperty(ref _stateDobot_2, value);
        }

        private ConnectState _stateDobot_3;
        public ConnectState StateDobot_3
        {
            get => _stateDobot_3;
            set => SetProperty(ref _stateDobot_3, value);
        }

        private ConnectState _stateDobot_4;
        public ConnectState StateDobot_4
        {
            get => _stateDobot_4;
            set => SetProperty(ref _stateDobot_4, value);
        }

        [ObservableProperty]
        private ConnectState _stateAubo;

        [ObservableProperty]
        private string _ipAddress_Aubo;

        [ObservableProperty]
        private string _aubo_ConnectState;





        private string dobot1_ConnectState;

        public string Dobot1_ConnectState
        {
            get { return dobot1_ConnectState; }
            set { SetProperty(ref dobot1_ConnectState, value); }
        }

        private string dobot2_ConnectState;

        public string Dobot2_ConnectState
        {
            get { return dobot2_ConnectState; }
            set { SetProperty(ref dobot2_ConnectState, value); }
        }

        private string dobot3_ConnectState;

        public string Dobot3_ConnectState
        {
            get { return dobot3_ConnectState; }
            set { SetProperty(ref dobot3_ConnectState, value); }
        }

        private string dobot4_ConnectState;

        public string Dobot4_ConnectState
        {
            get { return dobot4_ConnectState; }
            set { SetProperty(ref dobot4_ConnectState, value); }
        }



        /*--------------------机械臂角度数据属性---------------------*/
        #region 机械臂角度数据属性
        private float _dobot1_X;
        public float Dobot1_X
        {
            get { return _dobot1_X; }
            set { SetProperty(ref _dobot1_X, value); }
        }

        private float _dobot2_X;
        public float Dobot2_X
        {
            get { return _dobot2_X; }
            set { SetProperty(ref _dobot2_X, value); }
        }

        private float _dobot3_X;
        public float Dobot3_X
        {
            get { return _dobot3_X; }
            set { SetProperty(ref _dobot3_X, value); }
        }

        private float _dobot4_X;
        public float Dobot4_X
        {
            get { return _dobot4_X; }
            set { SetProperty(ref _dobot4_X, value); }
        }

        private float _dobot1_Y;
        public float Dobot1_Y
        {
            get { return _dobot1_Y; }
            set { SetProperty(ref _dobot1_Y, value); }
        }

        private float _dobot2_Y;
        public float Dobot2_Y
        {
            get { return _dobot2_Y; }
            set { SetProperty(ref _dobot2_Y, value); }
        }

        private float _dobot3_Y;
        public float Dobot3_Y
        {
            get { return _dobot3_Y; }
            set { SetProperty(ref _dobot3_Y, value); }
        }

        private float _dobot4_Y;
        public float Dobot4_Y
        {
            get { return _dobot4_Y; }
            set { SetProperty(ref _dobot4_Y, value); }
        }

        private float _dobot1_Z;
        public float Dobot1_Z
        {
            get { return _dobot1_Z; }
            set { SetProperty(ref _dobot1_Z, value); }
        }

        private float _dobot2_Z;
        public float Dobot2_Z
        {
            get { return _dobot2_Z; }
            set { SetProperty(ref _dobot2_Z, value); }
        }

        private float _dobot3_Z;
        public float Dobot3_Z
        {
            get { return _dobot3_Z; }
            set { SetProperty(ref _dobot3_Z, value); }
        }

        private float _dobot4_Z;
        public float Dobot4_Z
        {
            get { return _dobot4_Z; }
            set { SetProperty(ref _dobot4_Z, value); }
        }

        private float _dobot1_R;
        public float Dobot1_R
        {
            get { return _dobot1_R; }
            set { SetProperty(ref _dobot1_R, value); }
        }

        private float _dobot2_R;
        public float Dobot2_R
        {
            get { return _dobot2_R; }
            set { SetProperty(ref _dobot2_R, value); }
        }

        private float _dobot3_R;
        public float Dobot3_R
        {
            get { return _dobot3_R; }
            set { SetProperty(ref _dobot3_R, value); }
        }

        private float _dobot4_R;
        public float Dobot4_R
        {
            get { return _dobot4_R; }
            set { SetProperty(ref _dobot4_R, value); }
        }

        private float _dobot1_J1;
        public float Dobot1_J1
        {
            get { return _dobot1_J1; }
            set { SetProperty(ref _dobot1_J1, value); }
        }

        private float _dobot1_J2;
        public float Dobot1_J2
        {
            get { return _dobot1_J2; }
            set { SetProperty(ref _dobot1_J2, value); }
        }

        private float _dobot1_J3;
        public float Dobot1_J3
        {
            get { return _dobot1_J3; }
            set { SetProperty(ref _dobot1_J3, value); }
        }

        private float _dobot1_J4;
        public float Dobot1_J4
        {
            get { return _dobot1_J4; }
            set { SetProperty(ref _dobot1_J4, value); }
        }

        private float _dobot2_J1;
        public float Dobot2_J1
        {
            get { return _dobot2_J1; }
            set { SetProperty(ref _dobot2_J1, value); }
        }

        private float _dobot2_J2;
        public float Dobot2_J2
        {
            get { return _dobot2_J2; }
            set { SetProperty(ref _dobot2_J2, value); }
        }

        private float _dobot2_J3;
        public float Dobot2_J3
        {
            get { return _dobot2_J3; }
            set { SetProperty(ref _dobot2_J3, value); }
        }

        private float _dobot2_J4;
        public float Dobot2_J4
        {
            get { return _dobot2_J4; }
            set { SetProperty(ref _dobot2_J4, value); }
        }

        private float _dobot3_J1;
        public float Dobot3_J1
        {
            get { return _dobot3_J1; }
            set { SetProperty(ref _dobot3_J1, value); }
        }

        private float _dobot3_J2;
        public float Dobot3_J2
        {
            get { return _dobot3_J2; }
            set { SetProperty(ref _dobot3_J2, value); }
        }

        private float _dobot3_J3;
        public float Dobot3_J3
        {
            get { return _dobot3_J3; }
            set { SetProperty(ref _dobot3_J3, value); }
        }

        private float _dobot3_J4;
        public float Dobot3_J4
        {
            get { return _dobot3_J4; }
            set { SetProperty(ref _dobot3_J4, value); }
        }

        private float _dobot4_J1;
        public float Dobot4_J1
        {
            get { return _dobot4_J1; }
            set { SetProperty(ref _dobot4_J1, value); }
        }

        private float _dobot4_J2;
        public float Dobot4_J2
        {
            get { return _dobot4_J2; }
            set { SetProperty(ref _dobot4_J2, value); }
        }

        private float _dobot4_J3;
        public float Dobot4_J3
        {
            get { return _dobot4_J3; }
            set { SetProperty(ref _dobot4_J3, value); }
        }

        private float _dobot4_J4;
        public float Dobot4_J4
        {
            get { return _dobot4_J4; }
            set { SetProperty(ref _dobot4_J4, value); }
        }


    }
    #endregion


}






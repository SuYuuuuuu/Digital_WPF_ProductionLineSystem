using Microsoft.Extensions.DependencyInjection;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using WpfProductionLineApp.DobotCommunicationsFrameWork;
using WpfProductionLineApp.ViewModels;

namespace WpfProductionLineApp.Views
{
    /// <summary>
    /// RobotControl_Jog.xaml 的交互逻辑
    /// </summary>
    public partial class RobotControl_Jog : Page
    {
        public RobotControl_Jog()
        {
            InitializeComponent();
            this.DataContext = Locator.ServiceProvide?.GetService<ControlJogViewModel>();
            this.gridRoot.AddHandler(Button.PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(BtnDown));
            this.gridRoot.AddHandler(Button.PreviewMouseLeftButtonUpEvent, new RoutedEventHandler(BtnUp));
        }

        jogCmd jogCmd;
        private void BtnUp(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            if (btn == null) return;
            SerialPort RobotPort = Locator.ServiceProvide?.GetService<ControlJogViewModel>().RobotPort;
            RobotPort?.SetJogCmd(jogCmd, jogMode.IDEL, false);
        }
        private void BtnDown(object sender, RoutedEventArgs e)
        {
            jogCmd = new jogCmd();
            Button btn = e.Source as Button;
            if (btn == null) return;
            SerialPort RobotPort = Locator.ServiceProvide?.GetService<ControlJogViewModel>().RobotPort;
            switch (btn?.Content)
            {
                case "J1+":
                    jogCmd.isJoint = 1;
                    RobotPort?.SetJogCmd(jogCmd, jogMode.AP_DOWN, false);
                    break;
                case "X+":
                    jogCmd.isJoint = 0;
                    RobotPort?.SetJogCmd(jogCmd, jogMode.AP_DOWN, false);
                    break;
                case "J2+":
                    jogCmd.isJoint = 1;
                    RobotPort?.SetJogCmd(jogCmd, jogMode.BP_DOWN, false);
                    break;
                case "Y+":
                    jogCmd.isJoint = 0;
                    RobotPort?.SetJogCmd(jogCmd, jogMode.BP_DOWN, false);
                    break;
                case "J3+":
                    jogCmd.isJoint = 1;
                    RobotPort?.SetJogCmd(jogCmd, jogMode.CP_DOWN, false);
                    break;
                case "Z+":
                    jogCmd.isJoint = 0;
                    RobotPort?.SetJogCmd(jogCmd, jogMode.CP_DOWN, false);
                    break;
                case "J4+":
                    jogCmd.isJoint = 1;
                    RobotPort?.SetJogCmd(jogCmd, jogMode.DP_DOWN, false);
                    break;
                case "R+":
                    jogCmd.isJoint = 0;
                    RobotPort?.SetJogCmd(jogCmd, jogMode.DP_DOWN, false);
                    break;
                case "J1-":
                    jogCmd.isJoint = 1;
                    RobotPort?.SetJogCmd(jogCmd, jogMode.AN_DOWN, false);
                    break;
                case "X-":
                    jogCmd.isJoint = 0;
                    RobotPort?.SetJogCmd(jogCmd, jogMode.AN_DOWN, false);
                    break;
                case "J2-":
                    jogCmd.isJoint = 1;
                    RobotPort?.SetJogCmd(jogCmd, jogMode.BN_DOWN, false);
                    break;
                case "Y-":
                    jogCmd.isJoint = 0;
                    RobotPort?.SetJogCmd(jogCmd, jogMode.BN_DOWN, false);
                    break;
                case "J3-":
                    jogCmd.isJoint = 1;
                    RobotPort?.SetJogCmd(jogCmd, jogMode.CN_DOWN, false);
                    break;
                case "Z-":
                    jogCmd.isJoint = 0;
                    RobotPort?.SetJogCmd(jogCmd, jogMode.CN_DOWN, false);
                    break;
                case "J4-":
                    jogCmd.isJoint = 1;
                    RobotPort?.SetJogCmd(jogCmd, jogMode.DN_DOWN, false);
                    break;
                case "R-":
                    jogCmd.isJoint = 0;
                    RobotPort?.SetJogCmd(jogCmd, jogMode.DN_DOWN, false);
                    break;
                default: break;
            }

        }
    }
}

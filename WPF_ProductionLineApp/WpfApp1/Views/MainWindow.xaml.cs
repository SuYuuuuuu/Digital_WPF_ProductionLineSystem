using System.Windows;
using WpfProductionLineApp.ViewModels;
using WpfProductionLineApp.Views;

namespace WpfProductionLineApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new Locator();

            RobotControl_PTP robotControl_PTP = new RobotControl_PTP();//必须最早初始化以注册接收事件
            RobotControl_Jog robotControl_Jog = new RobotControl_Jog();
            Server server = new Server();
            RobotConnect robotConnect = new RobotConnect();//先new一个初始界面                                                           //
            //this.mainFrame.Content= new Frame() { Content= robotConnect };
            this.startButton.IsChecked = true;
        }

    }
}

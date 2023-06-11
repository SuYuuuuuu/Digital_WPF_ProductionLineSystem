using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using WpfProductionLineApp.ViewModels;

namespace WpfProductionLineApp.Views
{
    /// <summary>
    /// RobotControl.xaml 的交互逻辑
    /// </summary>
    public partial class RobotControl : Page
    {
        public RobotControl()
        {
            InitializeComponent();
            this.DataContext = Locator.ServiceProvide?.GetService<ControlViewModel>();
            //RobotControl_Jog robotControl_Jog = new RobotControl_Jog();
            //this.controlFrame.Content = new Frame() { Content = robotControl_Jog };
            //this.JogBtn.IsChecked = true;
        }
    }
}

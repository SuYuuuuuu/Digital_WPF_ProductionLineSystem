using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using WpfProductionLineApp.ViewModels;

namespace WpfProductionLineApp.Views
{
    /// <summary>
    /// RobotControl_PTP.xaml 的交互逻辑
    /// </summary>
    public partial class RobotControl_PTP : Page
    {
        public RobotControl_PTP()
        {
            InitializeComponent();
            this.DataContext = Locator.ServiceProvide.GetService<ControlPTPViewModel>();
            this.startBtn.IsChecked = true;
        }
    }
}

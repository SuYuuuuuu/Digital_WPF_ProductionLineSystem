using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using WpfProductionLineApp.ViewModels;

namespace WpfProductionLineApp.Views
{
    /// <summary>
    /// RobotConnect.xaml 的交互逻辑
    /// </summary>
    public partial class RobotConnect : Page
    {
        public RobotConnect()
        {
            InitializeComponent();
            this.DataContext = Locator.ServiceProvide.GetService<ConnectionViewModel>();
        }
    }
}

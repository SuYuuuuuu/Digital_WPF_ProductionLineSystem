using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using WpfProductionLineApp.ViewModels;

namespace WpfProductionLineApp.Views
{
    /// <summary>
    /// Server.xaml 的交互逻辑
    /// </summary>
    public partial class Server : Page
    {
        public Server()
        {
            InitializeComponent();
            this.DataContext = Locator.ServiceProvide.GetService<ServerViewModel>();
        }
    }
}

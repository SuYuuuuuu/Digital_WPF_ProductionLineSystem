using Microsoft.Extensions.DependencyInjection;
using System;

namespace WpfProductionLineApp.ViewModels
{
    /// <summary>
    /// IOC容器（依赖注入）
    /// </summary>
    public class Locator
    {
        public static IServiceProvider? ServiceProvide { get; private set; }
        public Locator()
        {
            ServiceProvide = GetService();
        }


        private IServiceProvider GetService()
        {
            var service = new ServiceCollection();
            service.AddSingleton<MainViewModel>();
            service.AddSingleton<ConnectionViewModel>();
            service.AddSingleton<ControlJogViewModel>();
            service.AddSingleton<ControlViewModel>();
            service.AddSingleton<ControlPTPViewModel>();
            service.AddSingleton<ServerViewModel>();         
            return service.BuildServiceProvider();
        }
        public MainViewModel MainViewModel
        {
            get
            {
                return ServiceProvide.GetService<MainViewModel>();
            }
        }
    }
}

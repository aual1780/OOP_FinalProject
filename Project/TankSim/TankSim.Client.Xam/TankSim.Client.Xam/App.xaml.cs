using Prism;
using Prism.Ioc;
using TankSim.Client.Xam.ViewModels;
using TankSim.Client.Xam.Views;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials.Implementation;
using Xamarin.Forms;
using Prism.Unity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ArdNet.DependencyInjection;
using Unity.Microsoft.DependencyInjection;

namespace TankSim.Client.Xam
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();

            var unityContainer = containerRegistry.GetContainer();

            IServiceCollection services = new ServiceCollection();
            _ = services
                .AddMessageHubSingleton()
                .AddIpResolver()
                .AddArdNet()
                .AddClientScoped()
                .AddTankSimConfig();

            _ = services.BuildServiceProvider(unityContainer);
        }
    }
}

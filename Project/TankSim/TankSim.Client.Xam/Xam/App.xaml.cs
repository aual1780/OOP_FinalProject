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
using System;
using Microsoft.Extensions.Options;

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

            _ = await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            _ = containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();

            var unityContainer = containerRegistry.GetContainer();

            IServiceCollection services = new ServiceCollection();
            //add game services
            _ = services
                .AddGameIDService();
            //add ardnet
            _ = services
                .AddSingleton<IOptions<ArdNetBasicConfig>>((sp) =>
                {
                    var opt = new ArdNetBasicConfig()
                    {
                        AppID = "ArdNet.TankSim.MultiController",
                        ServerPort = 52518,
                        ClientPort = 0
                    };

                    return Options.Create(opt);
                })
                .AddMessageHubSingleton()
                .AddIpResolver()
                .AddArdNet()
                .AddClientScoped()
                .AddTankSimConfig()
                .AddConfigModifier((x, y) =>
                {
                    y.TCP.HeartbeatConfig.ForceStrictHeartbeat = true;
                    y.TCP.HeartbeatConfig.RespondToHeartbeats = true;
                    y.TCP.HeartbeatConfig.HeartbeatToleranceMultiplier = 3;
                    var pingRate = 300;
                    y.TCP.HeartbeatConfig.HeartbeatInterval = TimeSpan.FromMilliseconds(pingRate);
                })
                .AutoRestart();

            _ = services.BuildServiceProvider(unityContainer);
        }
    }
}

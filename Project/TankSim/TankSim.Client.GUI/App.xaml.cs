using J2i.Net.XInputWrapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using TankSim.Client.GUI.Frames.ClientName;
using TankSim.Client.GUI.Frames.GameScope;
using TankSim.Client.GUI.Frames.Operations;
using TankSim.Client.OperatorModules;
using TankSim.Client.Services;
using TankSim.Config;

namespace TankSim.Client.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Startup
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            var path = AppContext.BaseDirectory;

            var configBuilder =
                new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("TankSim.Client.config.json", optional: false, reloadOnChange: true);
            var config = configBuilder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, config);

            using (var sp = serviceCollection.BuildServiceProvider())
            using (var mainWindow = sp.GetRequiredService<MainWindow>())
            {
                _ = mainWindow.ShowDialog();
            }
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            //setup keybinding configs
            _ = services
                .AddKeyBindings(config.GetSection(nameof(KeyBindingConfig)));
            //setup game services
            _ = services
                .AddGameIDService()
                .AddOperatorModules()
                .AddScoped<IOperatorModuleFactory<IOperatorInputModule>, OperatorModuleFactory<IOperatorInputModule>>()
                .AddScoped<IOperatorModuleFactory<IOperatorUIModule>, OperatorModuleFactory<IOperatorUIModule>>()
                .AddScoped<IGamepadService, GamepadService>()
                .AddScoped<IRoleResolverService, RoleResolverService>()
                .AddScoped<IOperatorInputProcessorService, OperatorInputProcessorService>()
                .AddSingleton((sp) =>
                {
                    var xm = XboxControllerManager.GetInstance();
                    xm.UpdateFrequency = 50;
                    return xm;
                })
            ;

            //setup ArdNet
            _ = services
                .AddArdNetClient(config)
                .AddDebugLogger()
                .AddReleaseCrasher();
            //setup main window
            _ = services
                .AddTransient<MainWindow>()
                .AddTransient<MainWindowVM>()
                .AddTransient<GameScopeControl>()
                .AddTransient<GameScopeControlVM>()
                .AddTransient<ClientNameControl>()
                .AddTransient<ClientNameControlVM>()
                .AddTransient<OperatorModuleControl>()
                .AddTransient<OperatorModuleControlVM>()
            ;

        }
    }
}

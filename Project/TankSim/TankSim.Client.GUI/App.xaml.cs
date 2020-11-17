using J2i.Net.XInputWrapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
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

        private IServiceProvider ServiceProvider { get; set; }
        private IConfiguration Configuration { get; set; }
        private IConfigurationSection ArdNetConfiguration { get; set; }

        /// <summary>
        /// Startup
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var configBuilder =
                new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("config.json", optional: false, reloadOnChange: true);
            var config = configBuilder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, config);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            using (var scope = ServiceProvider.CreateScope())
            {
                var mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
                _ = mainWindow.ShowDialog();
            }
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            XboxController.UpdateFrequency = 50;
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

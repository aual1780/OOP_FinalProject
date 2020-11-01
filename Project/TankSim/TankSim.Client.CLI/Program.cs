using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TankSim.Client.CLI.Services;
using TankSim.Client.DependencyInjection;
using TankSim.Client.OperatorModules;
using TankSim.Config;

namespace TankSim.Client.CLI
{
    class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        static async Task<int> Main()
        {
            var configBuilder =
                new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("config.json", optional: false, reloadOnChange: true);
            var config = configBuilder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, config);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            //application scope
            using (var appScope = ServiceProvider.CreateScope())
            {
                //get valid ArdNet game connection
                var scopeService = appScope.ServiceProvider.GetRequiredService<IGameScopeService>();
                using (var gameScope = await scopeService.GetValidGameScope())
                {
                    //run main controller code
                    var controllerService = gameScope.ServiceProvider.GetRequiredService<ControllerExecService>();
                    var roles = await controllerService.LoadOperatorRoles();
                    Console.WriteLine($"Your roles: {roles}");
                    //blocking call to handle user controls
                    controllerService.HandleUserInput();
                }
            }
            return 0;
        }


        private static void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            //setup keybinding configs
            _ = services
                .AddKeyBindings(config.GetSection(nameof(KeyBindingConfig)));
            //setup game services
            _ = services
                .AddGameIDService()
                .AddGameScopeService()
                .AddScoped<IOperatorModuleFactory, OperatorModuleFactory<IOperatorInputModule>>()
                .AddScoped<IOperatorModuleFactory<IOperatorInputModule>, OperatorModuleFactory<IOperatorInputModule>>()
                .AddControllerExecService();
            //setup ArdNet
            _ = services.AddArdNetClient(config);
        }
    }
}

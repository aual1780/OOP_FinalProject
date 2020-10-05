using ArdNet.Client;
using ArdNet.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using TankSim.Client.CLI.OperatorModules;
using TankSim.Client.CLI.Services;
using TankSim.Client.Config;
using TankSim.Client.Services;

namespace TankSim.Client.CLI
{
    class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static IConfiguration Configuration { get; private set; }


        static async Task<int> Main()
        {
            var configBuilder = 
                new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("config.json", optional: false, reloadOnChange: true);
            Configuration = configBuilder.Build();

            var serviceCollection = new ServiceCollection();
            _ = serviceCollection
                .Configure<KeyBindingConfig>(Configuration.GetSection(nameof(KeyBindingConfig)));
            _ = serviceCollection
                .AddGameIDService()
                .AddGameScopeService();
            _ = serviceCollection
                .AddMessageHubSingleton()
                .AddIpResolver()
                .AddArdNet(Configuration.GetSection("ArdNet"))
                .AddClientScoped()
                .AddTankSimConfig()
                .AutoRestart();
            _ = serviceCollection
                .AddSingleton<OperatorModuleFactory>()
                .AddControllerExecService();
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
                    await controllerService.LoadOperatorRoles();
                    //blocking call to handle user controls
                    controllerService.HandleUserInput();
                }
            }
            return 0;
        }
    }
}

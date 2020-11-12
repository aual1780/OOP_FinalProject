using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TankSim.Client.CLI.Services;
using TankSim.Client.DependencyInjection;
using TankSim.Client.OperatorModules;
using TankSim.Client.Services;
using TankSim.Config;

namespace TankSim.Client.CLI
{
    class Program
    {
        static async Task<int> Main()
        {
            Console.WriteLine(Constants.GameName);
            Console.Title = Constants.GameName;
            var sp = BuildServiceProvider();

            //application scope
            using (var appScope = sp.CreateScope())
            {
                //get valid ArdNet game connection
                var scopeService = appScope.ServiceProvider.GetRequiredService<IGameScopeService>();
                using (var gameScope = await scopeService.GetValidGameScope())
                {
                    //run main controller code
                    //get roles from server
                    var controllerService = gameScope.ServiceProvider.GetRequiredService<ControllerExecService>();
                    var roleTask = controllerService.LoadOperatorRoles();

                    //send username to gamehost
                    Console.Write("Enter username: ");
                    var username = Console.ReadLine();
                    await controllerService.SendUsername(username);

                    //display roles to user
                    var roles = await roleTask;
                    Console.WriteLine($"Your roles: {roles}");

                    //blocking call to handle user controls
                    controllerService.HandleUserInput();
                }
            }
            return 0;
        }

        private static ServiceProvider BuildServiceProvider()
        {
            var configBuilder =
                new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("config.json", optional: false, reloadOnChange: true);
            var config = configBuilder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, config);
            return serviceCollection.BuildServiceProvider();
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
                .AddScoped<IRoleResolverService, RoleResolverService>()
                .AddScoped<IOperatorInputProcessorService, OperatorInputProcessorService>()
                .AddControllerExecService();
            //setup ArdNet
            _ = services.AddArdNetClient(config);
        }
    }
}

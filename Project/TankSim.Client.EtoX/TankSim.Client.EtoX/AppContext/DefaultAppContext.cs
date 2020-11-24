using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using TankSim.Client.EtoX.Frames.GameScope;
using TankSim.Client.OperatorModules;
using TankSim.Client.Services;
using TankSim.Config;

namespace TankSim.Client.EtoX
{
    public partial class AppContextBuilder
    {
        /// <summary>
        /// Generate new default context builder nased on the standard client services
        /// </summary>
        /// <returns></returns>
        public static AppContextBuilder Default()
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var configBuilder =
                new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("TankSim.Client.config.json", optional: false, reloadOnChange: true);
            var config = configBuilder.Build();

            var builder = new AppContextBuilder();
            var services = builder.Services;
            //Add ArdNet
            _ = services
                .AddArdNetClient(config);

            //setup keybinding configs
            _ = services
                .AddKeyBindings(config.GetSection(nameof(KeyBindingConfig)));

            //Add game services
            _ = services
                .AddGameIDService()
                //.AddOperatorModules()
                .AddScoped<IOperatorModuleFactory<IOperatorInputModule>, OperatorModuleFactory<IOperatorInputModule>>()
                .AddScoped<IOperatorModuleFactory<IOperatorUIModule>, OperatorModuleFactory<IOperatorUIModule>>()
                .AddScoped<IGamepadService, GamepadService>()
                .AddScoped<IRoleResolverService, RoleResolverService>()
                .AddScoped<IOperatorInputProcessorService, OperatorInputProcessorService>()
            //.AddSingleton((sp) =>
            //{
            //    var xm = XboxControllerManager.GetInstance();
            //    xm.UpdateFrequency = 50;
            //    return xm;
            //})
            ;

            _ = services
                //setup main window
                .AddScoped<MainForm>()
                .AddScoped<MainFormVM>()
                .AddScoped<GameScopeCtrl>()
                .AddScoped<GameScopeVM>()
        //.AddTransient<ClientNameControl>()
        //.AddTransient<ClientNameControlVM>()
        //.AddTransient<OperatorModuleControl>()
        //.AddTransient<OperatorModuleControlVM>()
        ;

            return builder;
        }

    }
}

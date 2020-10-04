using System;
using System.Collections.Generic;
using System.Text;
using TankSim.Client.CLI.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GameScopeServiceExtensions
    {
        public static IServiceCollection AddGameScopeService(this IServiceCollection services)
        {
            _ = services.AddSingleton<IGameScopeService, GameScopeService>();
            return services;
        }
    }
}

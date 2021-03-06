﻿using TankSim.Client.CLI.Services;
using TankSim.Client.DependencyInjection;

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

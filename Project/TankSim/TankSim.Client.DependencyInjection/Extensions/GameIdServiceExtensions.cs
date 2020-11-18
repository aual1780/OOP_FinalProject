using TankSim.Client.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class GameIdServiceExtensions
    {
        /// <summary>
        /// Add GameIDService to set GameID for ardNet config
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddGameIDService(this IServiceCollection services)
        {
            _ = services.AddScoped<GameIdService>();
            return services;
        }
    }
}

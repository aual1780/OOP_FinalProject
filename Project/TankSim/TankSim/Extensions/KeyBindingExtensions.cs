using Microsoft.Extensions.Configuration;
using TankSim.Config;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class KeyBindingExtensions
    {
        /// <summary>
        /// Add an immutable keybind config to the service collection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Config"></param>
        /// <returns></returns>
        public static IServiceCollection AddKeyBindings(this IServiceCollection services, IConfiguration Config)
        {
            return services.Configure<KeyBindingConfig>(Config, (opt) =>
            {
                opt.BindNonPublicProperties = true;
            });
        }
    }
}

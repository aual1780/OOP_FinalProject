using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace TankSim.Client.DependencyInjection
{
    /// <summary>
    /// Service for getting valid ArdNet game connection
    /// </summary>
    public interface IGameScopeService
    {
        /// <summary>
        /// Get ServiceScope with valid ArdNet game connection
        /// </summary>
        /// <returns></returns>
        Task<IServiceScope> GetValidGameScope();
    }
}

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TankSim.Client.Uno.Frames.GameScope
{
    public interface IGameScopeControl
    {
        Task<IServiceScope> GetGameScopeAsync();
    }
}

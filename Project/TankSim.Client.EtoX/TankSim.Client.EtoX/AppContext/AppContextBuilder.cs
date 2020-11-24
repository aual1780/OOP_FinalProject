using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TankSim.Client.EtoX
{
    public partial class AppContextBuilder
    {
        private readonly IServiceCollection _services = new ServiceCollection();

        public IServiceCollection Services => _services;

        public AppContext Build() => new(_services.BuildServiceProvider());

    }
}

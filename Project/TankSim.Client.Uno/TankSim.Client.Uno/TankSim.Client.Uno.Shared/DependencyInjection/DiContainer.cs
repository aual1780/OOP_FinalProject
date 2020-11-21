using System;
using System.Collections.Generic;
using System.Text;

namespace TankSim.Client.Uno
{
    public static class DiContainer
    {
        public static IServiceProvider Instance() => DiContainerBuilder.Instance().GetServiceProvider();
    }
}

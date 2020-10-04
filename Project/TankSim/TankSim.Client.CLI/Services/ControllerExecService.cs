using ArdNet.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TankSim.Client.CLI.OperatorModules;

namespace TankSim.Client.CLI.Services
{
    public class ControllerExecService : IDisposable
    {
        readonly OpModuleCollection _opCollection;
        public ControllerExecService(IArdNetClient ArdClient, OpModuleFactory OperatorFactory)
        {
            //TODO
        }

        public async Task DoWork()
        {
            //TODO
        }

        public void Dispose()
        {
            _opCollection?.Dispose();
        }
    }
}

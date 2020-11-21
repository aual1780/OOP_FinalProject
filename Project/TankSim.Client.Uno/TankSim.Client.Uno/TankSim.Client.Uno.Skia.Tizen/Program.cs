using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace TankSim.Client.Uno.Skia.Tizen
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new TizenHost(() => new TankSim.Client.Uno.App(), args);
            host.Run();
        }
    }
}

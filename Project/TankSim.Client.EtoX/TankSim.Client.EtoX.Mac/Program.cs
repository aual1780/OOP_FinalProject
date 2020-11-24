using System;
using Eto.Forms;

namespace TankSim.Client.EtoX.Mac
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            using (var ac = AppContextBuilder.Default().Build())
            {
                using var app = new Application(Eto.Platforms.Mac64);
                var mainForm = ac.GetRequiredService<MainForm>();
                app.Run(mainForm);
            }
        }
    }
}

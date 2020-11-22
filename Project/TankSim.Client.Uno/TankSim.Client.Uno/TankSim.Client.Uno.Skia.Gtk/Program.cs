using GLib;
using Microsoft.Extensions.DependencyInjection;
using System;
using TankSim.Client.Uno.Frames.GameScope;
using TankSim.Client.Uno.Skia.Gtk.Frames.GameScope;
using Uno.UI.Runtime.Skia;

namespace TankSim.Client.Uno.Skia.Gtk
{
    class Program
    {
        static void Main(string[] args)
        {
            ExceptionManager.UnhandledException += delegate (UnhandledExceptionArgs expArgs)
            {
                Console.WriteLine("GLIB UNHANDLED EXCEPTION" + expArgs.ExceptionObject.ToString());
                expArgs.ExitApplication = true;
            };

            _ = DiContainerBuilder.Instance()
                .Services
                .AddTransient<IGameScopeControl, GameScopeControl>();

            var host = new GtkHost(() => new App(), args);

            host.Run();
        }
    }
}

using System;
using System.Windows.Input;

namespace TankSim.Client.GUI.Extensions
{
    public static class InputKeyExtensions
    {
        public static ConsoleKeyInfo ToConsoleKey(this Key key)
        {
            (ConsoleKey Console, char Char) newEvent;
            //letters
            if (key >= Key.A && key <= Key.Z)
            {
                var tmp = ConsoleKey.A + (key - Key.A);
                newEvent = (tmp, tmp.ToString()[0]);
            }
            //number bar
            else if (key >= Key.D0 && key <= Key.D9)
            {
                newEvent = (ConsoleKey.D0 + (key - Key.D0), (key - Key.D0).ToString()[0]);
            }
            //numpad
            else if (key >= Key.NumPad0 && key <= Key.NumPad9)
            {
                newEvent = (ConsoleKey.NumPad0 + (key - Key.NumPad0), (key - Key.NumPad0).ToString()[0]);
            }
            //F keys
            else if (key >= Key.F1 && key <= Key.F24)
            {
                var tmp = ConsoleKey.F1 + (key - Key.F1);
                newEvent = (tmp, '\0');
            }
            else if (key == Key.LeftShift || key == Key.RightShift)
            {
                return new ConsoleKeyInfo('\0', 0, true, false, false);
            }
            else if (key == Key.LeftCtrl || key == Key.RightCtrl)
            {
                return new ConsoleKeyInfo('\0', 0, false, false, true);
            }
            else if (key == Key.LeftAlt || key == Key.RightAlt)
            {
                return new ConsoleKeyInfo('\0', 0, false, true, false);
            }
            else
            {
                newEvent = key switch
                {
                    Key.Enter => (ConsoleKey.Enter, '\n'),
                    Key.Left => (ConsoleKey.LeftArrow, '\0'),
                    Key.Up => (ConsoleKey.UpArrow, '\0'),
                    Key.Right => (ConsoleKey.RightArrow, '\0'),
                    Key.Down => (ConsoleKey.DownArrow, '\0'),
                    Key.Tab => (ConsoleKey.Tab, '\t'),
                    Key.OemTilde => (ConsoleKey.Oem3, '~'),
                    _ => (0, '\0')
                };
            }

            return new ConsoleKeyInfo(newEvent.Char, newEvent.Console, false, false, false);

        }
    }
}

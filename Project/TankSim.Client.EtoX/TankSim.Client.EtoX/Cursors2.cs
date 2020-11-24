using Eto.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TankSim.Client.EtoX
{
    public static class Cursors2
    {
        private static readonly object _waitLock = new();
        private static Cursor _wait;

        public static Cursor Wait
        {
            get
            {
                if (_wait != null)
                {
                    return _wait;
                }
                lock (_waitLock)
                {
                    if (_wait == null)
                    {
                        var name = "./Windows_hourglass_cursor.cur";
                        var path = new FileInfo(name);
                        if (!path.Exists)
                        {
                            throw new FileNotFoundException("Wait cursor file could not be found", name);
                        }
                        _wait = new Cursor(path.FullName);
                    }
                    return _wait;

                }
            }
        }


    }
}

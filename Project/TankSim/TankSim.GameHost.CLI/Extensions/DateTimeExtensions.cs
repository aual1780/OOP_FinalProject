using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TIPC.Core.Tools;

namespace TankSim.GameHost.CLI.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Return datetime diff in milliseconds
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static double GetTimeDiff(this DateTime dt)
        {
            return (HighResolutionDateTime.UtcNow - dt).TotalMilliseconds;
        }
    }
}

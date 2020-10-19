using System;

namespace TankSim.GameHost.CLI.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class RangeExtensions
    {
        /// <summary>
        /// Check if a range contains the target value (inclusive bounds).
        /// Does not work on ranges that use "from end" syntax
        /// </summary>
        /// <param name="r"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool Contains(this Range r, int target)
        {
            if(r.Start.IsFromEnd || r.End.IsFromEnd)
            {
                throw new InvalidOperationException("Cannot evaluate indexes from end");
            }
            return r.Start.Value <= target && r.End.Value >= target;
        }
    }
}

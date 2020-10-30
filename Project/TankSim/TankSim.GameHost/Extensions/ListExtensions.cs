using System.Collections.Generic;

namespace TankSim.GameHost.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Remove the last element from the list and return it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static T Pop<T>(this List<T> list)
        {
            if (list.Count == 0)
            {
                return default;
            }
            var idx = list.Count - 1;
            var result = list[idx];
            list.RemoveAt(idx);
            return result;
        }
    }
}

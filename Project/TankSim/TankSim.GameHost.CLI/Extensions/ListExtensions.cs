using System;
using System.Collections.Generic;

namespace TankSim.GameHost.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Add index support to RemoveAt
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="Idx"></param>
        public static void RemoveAt<T>(this List<T> list, Index Idx)
        {
            if (!Idx.IsFromEnd)
                list.RemoveAt(Idx.Value);
            else
                list.RemoveAt(list.Count - Idx.Value);
        }
    }
}

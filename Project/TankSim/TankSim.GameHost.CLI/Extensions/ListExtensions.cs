using System;
using System.Collections.Generic;

namespace TankSim.GameHost.CLI.Extensions
{
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

        /// <summary>
        /// Remove the last element from the list and return it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static T Pop<T>(this List<T> list)
        {
            var result = list[^1];
            list.RemoveAt(^1);
            return result;
        }
    }
}

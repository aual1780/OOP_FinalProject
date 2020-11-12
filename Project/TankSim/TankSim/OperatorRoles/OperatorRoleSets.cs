using System;
using System.Collections.Generic;

namespace TankSim
{
    /// <summary>
    /// These are the distribution sets that roles will be divided into given n game players
    /// </summary>
    public static class OperatorRoleSets
    {
        /// <summary>
        /// Return the list of operator role distributions for the given number of players.
        /// Each array index can be randomly assigned to a game client
        /// </summary>
        /// <param name="PlayerCount"></param>
        /// <returns></returns>
        public static IEnumerable<OperatorRoles> GetDistributionSets(int PlayerCount)
        {
            return PlayerCount switch
            {
                1 => new OperatorRoles[]
                {
                    OperatorRoles.All
                },
                2 => new OperatorRoles[]
                {
                    OperatorRoles.Driver | OperatorRoles.Navigator | OperatorRoles.GunLoader,
                    OperatorRoles.GunRotation | OperatorRoles.RangeFinder| OperatorRoles.FireControl
                },
                3 => new OperatorRoles[]
                {
                    OperatorRoles.Driver | OperatorRoles.Navigator,
                    OperatorRoles.GunRotation | OperatorRoles.RangeFinder,
                    OperatorRoles.FireControl | OperatorRoles.GunLoader
                },
                4 => new OperatorRoles[]
                {
                    OperatorRoles.Driver | OperatorRoles.Navigator,
                    OperatorRoles.GunRotation | OperatorRoles.RangeFinder,
                    OperatorRoles.FireControl,
                    OperatorRoles.GunLoader
                },
                5 => new OperatorRoles[]
                {
                    OperatorRoles.Driver,
                    OperatorRoles.Navigator,
                    OperatorRoles.GunRotation | OperatorRoles.RangeFinder,
                    OperatorRoles.FireControl,
                    OperatorRoles.GunLoader
                },
                6 => new OperatorRoles[]
                {
                    OperatorRoles.Driver,
                    OperatorRoles.Navigator,
                    OperatorRoles.GunRotation,
                    OperatorRoles.RangeFinder,
                    OperatorRoles.FireControl,
                    OperatorRoles.GunLoader
                },
                _ => throw new ArgumentOutOfRangeException(nameof(PlayerCount), $"{nameof(PlayerCount)} must be between 1 and 6, inclusive"),
            };
        }
    }
}

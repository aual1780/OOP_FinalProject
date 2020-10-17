using System;
using System.Collections.Generic;
using System.Text;

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
            switch (PlayerCount)
            {
                case 1:
                    return new OperatorRoles[]
                    {
                        OperatorRoles.All
                    };
                case 2:
                    return new OperatorRoles[]
                    {
                        OperatorRoles.Driver | OperatorRoles.Navigator | OperatorRoles.GunLoader,
                        OperatorRoles.GunRotation | OperatorRoles.RangeFinder| OperatorRoles.FireControl
                    };
                case 3:
                    return new OperatorRoles[]
                    {
                        OperatorRoles.Driver | OperatorRoles.Navigator,
                        OperatorRoles.GunRotation | OperatorRoles.RangeFinder,
                        OperatorRoles.FireControl | OperatorRoles.GunLoader
                    };
                case 4:
                    return new OperatorRoles[]
                    {
                        OperatorRoles.Driver | OperatorRoles.Navigator,
                        OperatorRoles.GunRotation | OperatorRoles.RangeFinder,
                        OperatorRoles.FireControl,
                        OperatorRoles.GunLoader
                    };
                case 5:
                    return new OperatorRoles[]
                    {
                        OperatorRoles.Driver,
                        OperatorRoles.Navigator,
                        OperatorRoles.GunRotation | OperatorRoles.RangeFinder,
                        OperatorRoles.FireControl,
                        OperatorRoles.GunLoader
                    };
                case 6:
                    return new OperatorRoles[]
                    {
                        OperatorRoles.Driver,
                        OperatorRoles.Navigator,
                        OperatorRoles.GunRotation,
                        OperatorRoles.RangeFinder,
                        OperatorRoles.FireControl,
                        OperatorRoles.GunLoader
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(PlayerCount), $"{nameof(PlayerCount)} must be between 1 and 6, inclusive");
            }
        }
    }
}

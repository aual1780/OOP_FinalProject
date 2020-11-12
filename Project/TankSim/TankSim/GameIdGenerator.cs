using System;
using System.Net;
using System.Threading;
using TIPC.Core.Tools.Extensions;

namespace TankSim
{
    /// <summary>
    /// GameID generator/validator
    /// </summary>
    public static class GameIdGenerator
    {
        /// <summary>
        /// Validate GameID string
        /// </summary>
        /// <param name="GameID"></param>
        /// <returns></returns>
        public static bool Validate(string GameID)
        {
            if (GameID is null)
                return false;
            if (!int.TryParse(GameID, out var gameInt))
            {
                return false;
            }
            if (!GameID.RgxIsMatch($@"^[0-9]+$"))
            {
                return false;
            }
            if (gameInt < IPEndPoint.MinPort || gameInt > IPEndPoint.MaxPort)
            {
                return false;
            }
            return true;
        }

    }
}

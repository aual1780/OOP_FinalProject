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
        private const int _keyLength = 4;

        /// <summary>
        /// Generate a new GameID string
        /// </summary>
        /// <returns></returns>
        [Obsolete("", true)]
        public static string GetID()
        {
            var rand = Thread.CurrentThread.LocalRandom();
            string str = "";
            for (int i = 0; i < _keyLength; ++i)
            {
                str += rand.Next(0, 10);
            }

            if (!Validate(str))
            {
                throw new Exception("Failed to generate valid GameID");
            }
            return str;
        }

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

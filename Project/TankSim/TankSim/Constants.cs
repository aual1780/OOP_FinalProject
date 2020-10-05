using System;
using System.Collections.Generic;
using System.Text;

namespace TankSim
{
    /// <summary>
    /// Game constances
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// ArdNet channel names
        /// </summary>
        public static class ChannelNames
        {
            /// <summary>
            /// Tank operator channels
            /// </summary>
            public static class TankOperations
            {
                /// <summary>
                /// Tank driver.  Forward/Backward
                /// </summary>
                public static string Driver { get; } = $"Cmd.TankOperations.{nameof(Driver)}";
                /// <summary>
                /// Tank navigator.  Left/Right steering
                /// </summary>
                public static string Navigator { get; } = $"Cmd.TankOperations.{nameof(Navigator)}";
                /// <summary>
                /// Gun angle operator.  Turn the main turret left/right
                /// </summary>
                public static string GunAngle { get; } = $"Cmd.TankOperations.{nameof(GunAngle)}";
                /// <summary>
                /// Weapon range finder.  Control gun distance
                /// </summary>
                public static string RangeFinder { get; } = $"Cmd.TankOperations.{nameof(RangeFinder)}";
                /// <summary>
                /// Weapon fire control.  Shoot the guns
                /// </summary>
                public static string FireControl { get; } = $"Cmd.TankOperations.{nameof(FireControl)}";
                /// <summary>
                /// Gun loader.  Load the gun and change ammo type
                /// </summary>
                public static string Loader { get; } = $"Cmd.TankOperations.{nameof(Loader)}";

            }
        }

        /// <summary>
        /// System query strings
        /// </summary>
        public static class Queries
        {
            /// <summary>
            /// Get queries relevant to controller startup
            /// </summary>
            public static class ControllerInit
            {
                /// <summary>
                /// Get operator roles for a controller
                /// </summary>
                public static string GetOperatorRoles = "Request.OperatorRoles.GetCurrent";
            }
        }


    }
}

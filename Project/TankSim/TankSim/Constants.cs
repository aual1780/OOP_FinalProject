using System;

namespace TankSim
{
    /// <summary>
    /// Game constances
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Public game name
        /// </summary>
        public static string GameName { get; } = "BattleCOD 3: Eastern Front 2";

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
                /// Gun rotation operator.  Turn the main turret left/right
                /// </summary>
                public static string GunRotation { get; } = $"Cmd.TankOperations.{nameof(GunRotation)}";
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
                public static string GunLoader { get; } = $"Cmd.TankOperations.{nameof(GunLoader)}";

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
                public static string GetOperatorRoles { get; } = "Request.OperatorRoles.GetCurrent";
            }
        }

        /// <summary>
        /// System commmand strings
        /// </summary>
        public static class Commands
        {
            /// <summary>
            /// Get commands relevant to controller startup
            /// </summary>
            public static class ControllerInit
            {
                /// <summary>
                /// Set client name
                /// </summary>
                public static string SetClientName { get; } = "Command.ClientInfo.SetName";
            }
        }

        /// <summary>
        /// Gameplay configs
        /// </summary>
        public static class Gameplay
        {
            /// <summary>
            /// Main gun reload time
            /// </summary>
            public static TimeSpan ReloadDuration { get; } = TimeSpan.FromMilliseconds(1000);
        }

    }
}

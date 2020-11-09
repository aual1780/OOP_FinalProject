using ArdNet;
using System;
using System.Collections.Generic;
using System.Text;
using TankSim.OperatorCmds;
using TankSim.TankSystems;
using TIPC.Core.Tools;

namespace TankSim.OperatorDelegates
{
    public sealed class TankWeaponDelegate : IDisposable
    {
        private readonly object _loadLockObj = new object();
        private bool _isLoaded = false;
        private DateTime _lastLoadTime = DateTime.MinValue;

        private readonly FireControlDelegate _fireDelegate;
        private readonly GunLoaderDelegate _loaderDelegate;

        /// <summary>
        /// Event triggered when main gun is fired.
        /// Arg indicates the state that the weapon was in at the moment of firing
        /// </summary>
        public event Action<IConnectedSystemEndpoint, PrimaryWeaponFireState> PrimaryWeaponFired;

        /// <summary>
        /// Event triggered when secondary gun is fired
        /// </summary>
        public event Action<IConnectedSystemEndpoint> SecondaryWeaponFired;

        /// <summary>
        /// Event triggered when main gun is loaded
        /// </summary>
        public event Action<IConnectedSystemEndpoint> PrimaryGunLoaded;

        /// <summary>
        /// Event triggered when primary ammo type is cycled
        /// </summary>
        public event Action<IConnectedSystemEndpoint> PrimaryAmmoCycled;

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="FireDelegate"></param>
        /// <param name="LoaderDelegate"></param>
        public TankWeaponDelegate(
            FireControlDelegate FireDelegate,
            GunLoaderDelegate LoaderDelegate)
        {
            if (FireDelegate is null)
            {
                throw new ArgumentNullException(nameof(FireDelegate));
            }
            if (LoaderDelegate is null)
            {
                throw new ArgumentNullException(nameof(LoaderDelegate));
            }

            _fireDelegate = FireDelegate;
            _fireDelegate.CmdReceived += FireDelegate_CmdReceived;
            _loaderDelegate = LoaderDelegate;
            _loaderDelegate.CmdReceived += LoaderDelegate_CmdReceived;
        }

        private void FireDelegate_CmdReceived(IConnectedSystemEndpoint Endpoint, FireControlCmd Cmd)
        {
            if (Cmd.WeaponType == FireControlType.Primary)
            {
                lock (_loadLockObj)
                {
                    var now = HighResolutionDateTime.UtcNow;
                    var diff = now - _lastLoadTime;
                    var isValidFire = diff >= Constants.Gameplay.ReloadDuration;

                    int fireType = 0;
                    var isLoaded = Convert.ToInt32(_isLoaded);
                    var isMisfire = Convert.ToInt32(_isLoaded & !isValidFire);
                    fireType += isLoaded + isMisfire;
                    var fireState = (PrimaryWeaponFireState)fireType;

                    PrimaryWeaponFired?.Invoke(Endpoint, fireState);

                    _isLoaded = false;
                }
            }
            else if (Cmd.WeaponType == FireControlType.Secondary)
            {
                SecondaryWeaponFired?.Invoke(Endpoint);
            }
        }

        private void LoaderDelegate_CmdReceived(IConnectedSystemEndpoint Endpoint, GunLoaderCmd Cmd)
        {
            if (Cmd.LoaderType == GunLoaderType.Load)
            {
                lock (_loadLockObj)
                {
                    _isLoaded = true;
                    _lastLoadTime = HighResolutionDateTime.UtcNow;
                    PrimaryGunLoaded?.Invoke(Endpoint);
                }
            }
            else if (Cmd.LoaderType == GunLoaderType.CycleAmmoType)
            {
                PrimaryAmmoCycled?.Invoke(Endpoint);
            }
        }

        /// <summary>
        /// Unhook event handlers
        /// </summary>
        public void Dispose()
        {
            _fireDelegate.CmdReceived -= FireDelegate_CmdReceived;
            _loaderDelegate.CmdReceived -= LoaderDelegate_CmdReceived;
            PrimaryWeaponFired = null;
            SecondaryWeaponFired = null;
            PrimaryGunLoaded = null;
            PrimaryAmmoCycled = null;
        }
    }
}

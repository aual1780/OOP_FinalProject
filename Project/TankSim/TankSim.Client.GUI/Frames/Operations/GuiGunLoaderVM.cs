using ArdNet.Client;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TankSim.OperatorDelegates;
using TIPC.Core.ComponentModel;
using TIPC.Core.Tools;

namespace TankSim.Client.GUI.Frames.Operations
{
    public class GuiGunLoaderVM : ViewModelBase, IDisposable
    {
        private DateTime _reloadStart;
        private readonly FireControlDelegate _fireDelegate;
        private readonly GunLoaderDelegate _loaderDelegate;
        private readonly Timer _loadTimer;
        private readonly int _tickRateMillis;
        private int _fillPercent;
        public int FillPercent
        {
            get => _fillPercent;
            set => SetField(ref _fillPercent, value);
        }


        public GuiGunLoaderVM(IArdNetClient ArdClient)
        {
            _tickRateMillis = (int)Constants.Gameplay.ReloadDuration.TotalMilliseconds / 50;
            _loadTimer = new Timer(LoadTimer_Tick);
            _loaderDelegate = new GunLoaderDelegate(ArdClient);
            _loaderDelegate.CmdReceived += (x, y) =>
            {
                if (y.LoaderType == GunLoaderType.Load)
                {
                    AnimateProgBar();
                }
            };
            _fireDelegate = new FireControlDelegate(ArdClient);
            _fireDelegate.CmdReceived += (x, y) =>
            {
                if (y.WeaponType == FireControlType.Primary)
                {
                    if (FillPercent == 100)
                    {
                        FillPercent = 0;
                    }
                }
            };
        }

        public override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public void AnimateProgBar()
        {
            _reloadStart = HighResolutionDateTime.UtcNow;
            FillPercent = 0;
            _ = _loadTimer.Change(_tickRateMillis, _tickRateMillis);
        }

        private void LoadTimer_Tick(object state)
        {
            var tmpFill = Math.Min(100, FillPercent + 2);

            var now = HighResolutionDateTime.UtcNow;
            var elap = (now - _reloadStart).TotalMilliseconds;
            if (elap >= Constants.Gameplay.ReloadDuration.TotalMilliseconds)
            {
                tmpFill = 100;
            }

            if (tmpFill == 100)
            {
                _ = _loadTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            FillPercent = tmpFill;
        }

        public void Dispose()
        {
            _loadTimer.Dispose();
            _loaderDelegate.Dispose();
            _fireDelegate.Dispose();
        }
    }
}

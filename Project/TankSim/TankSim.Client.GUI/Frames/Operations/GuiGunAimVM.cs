using ArdNet;
using ArdNet.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TankSim.OperatorDelegates;
using TIPC.Core.ComponentModel;

namespace TankSim.Client.GUI.Frames.Operations
{
    public class GuiGunAimVM : ViewModelBase, IDisposable
    {
        private readonly IArdNetClient _ardClient;
        private readonly TankAimingDelegate _movDelegate;
        private MovementDirection _dir;


        public bool DirNW => _dir == (MovementDirection.North | MovementDirection.West);
        public bool DirN => _dir == (MovementDirection.North);
        public bool DirNE => _dir == (MovementDirection.North | MovementDirection.East);

        public bool DirW => _dir == (MovementDirection.West);
        public bool DirE => _dir == (MovementDirection.East);

        public bool DirSW => _dir == (MovementDirection.South | MovementDirection.West);
        public bool DirS => _dir == (MovementDirection.South);
        public bool DirSE => _dir == (MovementDirection.South | MovementDirection.East);


        public GuiGunAimVM(IArdNetClient ArdClient)
        {
            _ardClient = ArdClient;
            _movDelegate = new TankAimingDelegate(_ardClient);
            _movDelegate.AimChanged += MovDelegate_MovementChanged;
        }

        public override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        private void MovDelegate_MovementChanged(IConnectedSystemEndpoint Endpoint, MovementDirection Dir)
        {
            //dont show ui input if not connected
            if (_ardClient.IsServerConnected)
            {
                _dir = Dir;
            }
            else
            {
                _dir = MovementDirection.Stop;
            }

            InvokePropertyChanged(nameof(DirNW));
            InvokePropertyChanged(nameof(DirN));
            InvokePropertyChanged(nameof(DirNE));

            InvokePropertyChanged(nameof(DirW));
            InvokePropertyChanged(nameof(DirE));

            InvokePropertyChanged(nameof(DirSW));
            InvokePropertyChanged(nameof(DirS));
            InvokePropertyChanged(nameof(DirSE));
        }


        public void Dispose()
        {
            _movDelegate?.Dispose();
        }
    }
}

using ArdNet;
using ArdNet.Client;
using System;
using System.Threading.Tasks;
using TankSim.OperatorDelegates;
using TIPC.Core.ComponentModel;

namespace TankSim.Client.GUI.Frames.Operations
{
    public class GuiDriverVM : ViewModelBase, IDisposable
    {
        private readonly IArdNetClient _ardClient;
        private readonly TankMovementDelegate _movDelegate;
        private MovementDirection _dir;


        public bool DirNW => _dir == (MovementDirection.North | MovementDirection.West);
        public bool DirN => _dir == (MovementDirection.North);
        public bool DirNE => _dir == (MovementDirection.North | MovementDirection.East);

        public bool DirW => _dir == (MovementDirection.West);
        public bool DirE => _dir == (MovementDirection.East);

        public bool DirSW => _dir == (MovementDirection.South | MovementDirection.West);
        public bool DirS => _dir == (MovementDirection.South);
        public bool DirSE => _dir == (MovementDirection.South | MovementDirection.East);


        public GuiDriverVM(IArdNetClient ArdClient)
        {
            _ardClient = ArdClient;
            _movDelegate = new TankMovementDelegate(_ardClient);
            _movDelegate.MovementChanged += MovDelegate_MovementChanged;
        }

        public override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        private void MovDelegate_MovementChanged(IConnectedSystemEndpoint Endpoint, MovementDirection Dir)
        {
            _dir = Dir;

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

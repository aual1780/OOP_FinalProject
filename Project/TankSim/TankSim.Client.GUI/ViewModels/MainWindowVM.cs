using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TIPC.Core.ComponentModel;

namespace TankSim.Client.GUI.ViewModels
{
    public class MainWindowVM : ViewModelBase
    {
        public override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        private object _frameContent;
        public object FrameContent
        {
            get => _frameContent;
            set => SetField(ref _frameContent, value);
        }
    }
}

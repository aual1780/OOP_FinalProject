using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TIPC.Core.ComponentModel;

namespace TankSim.Client.EtoX
{
    public class MainFormVM : ViewModelBase
    {
        private object _frameContent;
        public object FrameContent
        {
            get => _frameContent;
            set => SetField(ref _frameContent, value);
        }


        public override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2i.Net.XInputWrapper
{
    /// <summary>
    /// Event arg for controller state change events
    /// </summary>
    public class XboxControllerStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// New input state
        /// </summary>
        public XInputState CurrentInputState { get; set; }
        /// <summary>
        /// old input state
        /// </summary>
        public XInputState PreviousInputState { get; set; }
    }
}

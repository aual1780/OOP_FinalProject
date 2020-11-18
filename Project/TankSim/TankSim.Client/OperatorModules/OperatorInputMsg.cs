using TIPC.Core.Channels;

namespace TankSim.Client.OperatorModules
{
    /// <summary>
    /// Message for sending operator inputs through TIPC channel hub
    /// </summary>
    public class OperatorInputMsg : IChannelMessage<OperatorInputEventArg>
    {
        /// <summary>
        /// Msg category
        /// </summary>
        public MessageCategoryTypes MessageCategory => MessageCategoryTypes.StandardMessages;
        IChannelEventArgs IChannelMessage.Arg => Arg;
        /// <summary>
        /// Message src
        /// </summary>
        public object Sender { get; }
        /// <summary>
        /// Event arg
        /// </summary>
        public OperatorInputEventArg Arg { get; }

        /// <summary>
        /// Create operator input message for channel hub
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Arg"></param>
        public OperatorInputMsg(object Sender, OperatorInputEventArg Arg)
        {
            this.Sender = Sender;
            this.Arg = Arg;
        }
    }
}

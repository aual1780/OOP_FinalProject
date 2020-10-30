using ArdNet.Topics;
using System;
using System.Collections.Generic;
using System.Text;
using TIPC.Core.Tools.Threading;

namespace TankSim.OperatorDelegates
{
    /// <summary>
    /// Operator delegate event validator.
    /// Allows consumers to intercept events and prevent invalid messages from bubbling
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DelegateEventValidator<T>
    {
        private Predicate<T> _predicate = (e) => true;

        /// <summary>
        /// Validate event using filter rules
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Validate(T msg)
        {
            //use local reference for thread safety
            var pred = _predicate;
            return pred(msg);
        }

        /// <summary>
        /// Add new filter to validation chain.
        /// Filters are ANDed together with previous entries
        /// </summary>
        /// <param name="Filter"></param>
        public void AddFilter(Predicate<T> Filter)
        {
            ThreadTools.LockFreeUpdate(ref _predicate, (pred) =>
            {
                bool y(T x)
                {
                    return Filter(x) && pred(x);
                }
                return y;
            });
        }
    }
}

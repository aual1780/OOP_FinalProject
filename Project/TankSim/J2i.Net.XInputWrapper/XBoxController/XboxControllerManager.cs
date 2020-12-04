using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TIPC.Core.Tools.Threading;

namespace J2i.Net.XInputWrapper
{
    /// <summary>
    /// Xbox global manager. Controls hooks and polling.  Simplifies teardown process
    /// </summary>
    public sealed class XboxControllerManager : IDisposable
    {
        #region const
        /// <summary>
        /// Max connected controller count
        /// </summary>
        public const int MAX_CONTROLLER_COUNT = 8;
        /// <summary>
        /// Index of first available controller
        /// </summary>
        public const int FIRST_CONTROLLER_INDEX = 0;
        /// <summary>
        /// Index of last possible available controller.
        /// Does not mean that there are this many controllers connected
        /// </summary>
        public const int LAST_CONTROLLER_INDEX = MAX_CONTROLLER_COUNT - 1;
        #endregion const

        #region static
        private static readonly object _initLock = new();
        private static XboxControllerManager _controllerManager;

        /// <summary>
        /// Get singleton instance of controller manager. The instance must be disposed at app teardown
        /// </summary>
        /// <returns></returns>
        public static XboxControllerManager GetInstance()
        {
            if (_controllerManager is not null)
                return _controllerManager;

            lock (_initLock)
            {
                _controllerManager ??= new XboxControllerManager();
                return _controllerManager;
            }
        }

        #endregion static


        int _updateFrequency;
        int _waitTime;
        bool _isRunning;
        readonly XboxController[] _controllers;
        readonly object _syncLock = new();
        CancelThread<object> _pollingThread;

        /// <summary>
        /// Global controller input polling frequency (hz)
        /// </summary>
        public int UpdateFrequency
        {
            get { return _updateFrequency; }
            set
            {
                if (UpdateFrequency < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(UpdateFrequency), "Value must be greater than zero");
                }
                _updateFrequency = value;
                _waitTime = Math.Max(1, (int)(1000.00 / _updateFrequency));
            }
        }


        private XboxControllerManager()
        {
            _controllers = new XboxController[MAX_CONTROLLER_COUNT];
            for (int i = FIRST_CONTROLLER_INDEX; i <= LAST_CONTROLLER_INDEX; ++i)
            {
                _controllers[i] = new XboxController(i);
            }
            UpdateFrequency = 25;
        }

        /// <summary>
        /// Get controller at specified index.
        /// Value will always be non-null, but that does not mean that the controller is connected.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public XboxController RetrieveController(int index)
        {
            return _controllers[index];
        }

        /// <summary>
        /// Start controller input polling.
        /// This is a noop if the system is already polling
        /// </summary>
        public void StartPolling()
        {
            if (!_isRunning)
            {
                lock (_syncLock)
                {
                    if (!_isRunning)
                    {
                        _pollingThread?.Dispose();
                        _pollingThread = new CancelThread<object>(PollerLoop)
                        {
                            IsBackground = true
                        };
                        _pollingThread.Start();
                    }
                }
            }
        }

        /// <summary>
        /// Stop controller input polling and wait for polling thread to terminate
        /// </summary>
        public void StopPolling()
        {
            _pollingThread?.Dispose();
            _pollingThread?.Join();
        }

        private void PollerLoop(object State, CancellationToken Token)
        {
            lock (_syncLock)
            {
                if (_isRunning)
                    return;
                _isRunning = true;
            }
            while (!Token.IsCancellationRequested)
            {
                for (int i = FIRST_CONTROLLER_INDEX; i <= LAST_CONTROLLER_INDEX; ++i)
                {
                    _controllers[i].UpdateState();
                }
                Thread.Sleep(_waitTime);
            }
            lock (_syncLock)
            {
                _isRunning = false;
            }
        }



        /// <summary>
        /// Shutdown polling loop and release resources
        /// </summary>
        public void Dispose()
        {
            StopPolling();
            _pollingThread?.Dispose();
        }


    }
}

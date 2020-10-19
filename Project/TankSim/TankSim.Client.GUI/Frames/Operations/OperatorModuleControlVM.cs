﻿using ArdNet;
using ArdNet.Client;
using ArdNet.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using TankSim.Client.OperatorModules;
using TIPC.Core.ComponentModel;

namespace TankSim.Client.GUI.Frames.Operations
{
    public class OperatorModuleControlVM : ViewModelBase, IDisposable
    {
        private readonly IArdNetClient _ardClient;
        private readonly IOperatorModuleFactory _moduleFactory;
        private IConnectedSystemEndpoint _gameHost;
        private OperatorRoles _roles = 0;
        private IOperatorModuleCollection _moduleCollection;
        private readonly CancellationTokenSource _initSyncTokenSrc = new CancellationTokenSource();

        public OperatorRoles Roles
        {
            get => _roles;
            set => SetField(ref _roles, value);
        }
        public IConnectedSystemEndpoint GameHost
        {
            get => _gameHost;
            set => SetField(ref _gameHost, value);
        }
        public IOperatorModuleCollection ModuleCollection
        {
            get => _moduleCollection;
            set
            {
                if (SetField(ref _moduleCollection, value))
                {
                    InvokePropertyChanged(nameof(ModuleCtrlCollection));
                }
            }
        }
        public IEnumerable<UserControl> ModuleCtrlCollection
        {
            get => _moduleCollection?.OfType<UserControl>();
        }

        public OperatorModuleControlVM(IArdNetClient ArdClient, IOperatorModuleFactory ModuleFactory)
        {
            _ardClient = ArdClient;
            _moduleFactory = ModuleFactory;
        }

        public override async Task InitializeAsync()
        {
            GameHost = await _ardClient.ConnectAsync();
            var qry = Constants.Queries.ControllerInit.GetOperatorRoles;
            var request = new AsyncRequestPushedArgs(qry, null, _initSyncTokenSrc.Token, Timeout.InfiniteTimeSpan);
            try
            {
                var task = _ardClient.SendTcpQueryAsync(request);
                var response = await task;
                var responseStr = response?.Single()?.Response ?? "0";
                Roles = Enum.Parse<OperatorRoles>(responseStr);
                ModuleCollection = _moduleFactory.GetModuleCollection(Roles);
            }
            catch (OperationCanceledException)
            {
                //noop
                //early shutdown
            }
        }

        public void Dispose()
        {
            try
            {
                _initSyncTokenSrc.Cancel();
                _initSyncTokenSrc.Dispose();
            }
            catch { }
            _moduleCollection?.Dispose();
        }
    }
}

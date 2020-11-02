using ArdNet.Client;
using ArdNet.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TIPC.Core.Tools.Extensions;

namespace TankSim.Client.Services
{
    /// <summary>
    /// Service to get operator roles from server
    /// </summary>
    public interface IRoleResolverService : IDisposable
    {
        /// <summary>
        /// Get roles from server
        /// </summary>
        /// <returns></returns>
        Task<OperatorRoles> GetRolesAsync();
    }

    /// <summary>
    /// Service to get operator roles from server
    /// </summary>
    public class RoleResolverService : IRoleResolverService
    {
        private readonly IArdNetClient _ardClient;
        private readonly object _requestLock = new object();
        private volatile bool _isDisposed = false;
        private volatile bool _isRequestSent = false;
        private readonly CancellationTokenSource _initSyncTokenSrc = new CancellationTokenSource();
        readonly TaskCompletionSource<OperatorRoles> _roleTask = new TaskCompletionSource<OperatorRoles>();
        private volatile int _connectCount = 0;
        private readonly string _clientUid = Guid.NewGuid().ToString();
        private OperatorRoles _roles = 0;

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="ArdClient"></param>
        public RoleResolverService(IArdNetClient ArdClient)
        {
            _ardClient = ArdClient;
            _ardClient.TcpEndpointConnected += ArdClient_TcpEndpointConnected;
        }

        private void ArdClient_TcpEndpointConnected(object Sender, ArdNet.IConnectedSystemEndpoint e)
        {
            try
            {
                if (_connectCount == 0)
                {
                    return;
                }

                var qry = Constants.Queries.ControllerInit.GetOperatorRoles;
                var request = new RequestPushedArgs(
                    Request: qry,
                    RequestArgs: new string[] { _clientUid },
                    Callback: RoleResponseReceivedHandler,
                    UserState: null,
                    CallbackCancellationToken: _initSyncTokenSrc.Token,
                    CallbackTimeout: Timeout.InfiniteTimeSpan);

                _ardClient.SendTcpQuery(request);
            }
            finally
            {
                _ = Interlocked.Increment(ref _connectCount);
            }
        }

        /// <summary>
        /// Request roles from server
        /// </summary>
        /// <returns></returns>
        public Task<OperatorRoles> GetRolesAsync()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(RoleResolverService));
            }
            if (_isRequestSent)
            {
                return _roleTask.Task;
            }
            var qry = Constants.Queries.ControllerInit.GetOperatorRoles;
            var request = new RequestPushedArgs(
                Request: qry,
                RequestArgs: new string[] { _clientUid },
                Callback: RoleResponseReceivedHandler,
                UserState: null,
                CallbackCancellationToken: _initSyncTokenSrc.Token,
                CallbackTimeout: Timeout.InfiniteTimeSpan);

            if (_ardClient.IsServerConnected)
            {
                lock (_requestLock)
                {
                    if (_isRequestSent)
                    {
                        return _roleTask.Task;
                    }
                    _ardClient.SendTcpQuery(request);
                    _isRequestSent = true;
                    return _roleTask.Task;
                }
            }
            else
            {
                lock (_requestLock)
                {
                    if (_isRequestSent)
                    {
                        return _roleTask.Task;
                    }
                    _isRequestSent = true;
                    return GetRolesAsyncHelper();
                }
            }

            async Task<OperatorRoles> GetRolesAsyncHelper()
            {
                _ = await _ardClient.ConnectAsync();
                _ardClient.SendTcpQuery(request);
                return await _roleTask.Task;
            }
        }

        private void RoleResponseReceivedHandler(object sender, RequestResponseReceivedArgs e)
        {
            var responseStr = e.Response ?? "0";
            _roles = (OperatorRoles)Enum.Parse(typeof(OperatorRoles), responseStr);
            _ = _roleTask.TrySetResult(_roles);
        }

        /// <summary>
        /// Cancel pending requests
        /// </summary>
        public void Dispose()
        {
            _isDisposed = true;
            _ardClient.TcpEndpointConnected -= ArdClient_TcpEndpointConnected;
            _ = _roleTask.TrySetException(new OperationCanceledException());

            try
            {
                _initSyncTokenSrc.Cancel();
                _initSyncTokenSrc.Dispose();
            }
            catch
            {
                //noop
            }
        }
    }
}

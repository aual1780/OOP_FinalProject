using ArdNet.Client;
using ArdNet.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

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
        private volatile bool _isRequestSent = false;
        private readonly CancellationTokenSource _initSyncTokenSrc = new CancellationTokenSource();
        readonly TaskCompletionSource<OperatorRoles> _roleTask = new TaskCompletionSource<OperatorRoles>();
        private OperatorRoles _roles = 0;

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="ArdClient"></param>
        public RoleResolverService(IArdNetClient ArdClient)
        {
            _ardClient = ArdClient;
        }

        /// <summary>
        /// Request roles from server
        /// </summary>
        /// <returns></returns>
        public Task<OperatorRoles> GetRolesAsync()
        {
            if (_isRequestSent)
            {
                return _roleTask.Task;
            }
            var qry = Constants.Queries.ControllerInit.GetOperatorRoles;
            var request = new RequestPushedArgs(
                Request: qry,
                RequestArgs: null,
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

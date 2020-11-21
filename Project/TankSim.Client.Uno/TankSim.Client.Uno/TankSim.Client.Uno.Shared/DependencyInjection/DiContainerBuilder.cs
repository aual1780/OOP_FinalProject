using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TankSim.Client.Uno
{

    public class DiContainerBuilder : IDisposable
    {
        private static readonly object _initLock = new object();    
        private static readonly object _diBuildLock = new object();
        private static readonly IServiceCollection _serviceCollection = DiContainerConfig.DefaultServices();
        private static DiContainerBuilder _containerBuilderInstance;
        private static ServiceProvider _serviceProvider;

        public static DiContainerBuilder Instance()
        {
            if (_containerBuilderInstance != null)
            {
                return _containerBuilderInstance;
            }
            lock (_initLock)
            {
                if (_containerBuilderInstance == null)
                {
                    _containerBuilderInstance = new DiContainerBuilder();
                }
                return _containerBuilderInstance;
            }
        }

        public IServiceCollection Services
        {
            get
            {
                var str = $"The {nameof(IServiceCollection)} is not available after the service provider has been constructed";
                if (_serviceProvider != null)
                {
                    throw new InvalidOperationException(str);
                }

                lock (_diBuildLock)
                {
                    if (_serviceProvider != null)
                    {
                        throw new InvalidOperationException(str);
                    }
                    return _serviceCollection;
                }
            }
        }



        public IServiceProvider GetServiceProvider()
        {
            if (_serviceProvider != null)
            {
                return _serviceProvider;
            }
            lock (_diBuildLock)
            {
                if (_serviceProvider == null)
                {
                    _serviceProvider = _serviceCollection.BuildServiceProvider();
                }
                return _serviceProvider;
            }
        }

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }

    }
}

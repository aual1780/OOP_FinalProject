using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TankSim.Client.EtoX
{
    public class AppContext : IDisposable
    {
        private readonly ServiceProvider _serviveProvider;

        public AppContext(ServiceProvider Provider)
        {
            _serviveProvider = Provider;
        }

        public IServiceProvider ServiceProvider => _serviveProvider;


        public T GetService<T>() => ServiceProvider.GetService<T>();
        public IEnumerable<T> GetServices<T>() => ServiceProvider.GetServices<T>();
        public T GetRequiredService<T>() => ServiceProvider.GetRequiredService<T>();

        public IServiceScope GetScope() => ServiceProvider.CreateScope();


        public void Dispose()
        {
            _serviveProvider?.Dispose();
        }

    }
}

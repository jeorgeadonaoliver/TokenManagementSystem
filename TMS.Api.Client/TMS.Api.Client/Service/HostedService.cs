
using Microsoft.EntityFrameworkCore;
using Quartz.Impl.AdoJobStore.Common;

namespace TMS.Api.Client.Service
{
    public class HostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public HostedService(IServiceProvider serviceProvider)
        {

            _serviceProvider = serviceProvider;

        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<DbContext>();
            await context.Database.EnsureCreatedAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

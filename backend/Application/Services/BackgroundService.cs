using Infrastracture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class NotificationCleanupService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;

        public NotificationCleanupService( IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWorkAsync, null, TimeSpan.Zero, TimeSpan.FromHours(12)); 
            return Task.CompletedTask;
        }

        private async void DoWorkAsync(object state)
        {
          

            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<Infrastracture.DataContext>();
                    var cutoffDate = DateTime.UtcNow.AddDays(-7);
                    var oldNotifications = dbContext.Notifications.Where(n => n.CreatedAt < cutoffDate).ToList();
                    dbContext.Notifications.RemoveRange(oldNotifications);
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                }
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

}

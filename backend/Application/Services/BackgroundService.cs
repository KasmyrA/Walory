using Infrastracture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class NotificationCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var threshold = DateTime.UtcNow.AddDays(-14);
                var oldNotifications = db.Notifications.Where(n => n.CreatedAt < threshold);

                db.Notifications.RemoveRange(oldNotifications);
                await db.SaveChangesAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); //24 interval
            }
        }
    }

}

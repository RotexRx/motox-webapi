using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services
{
    public class UserCleanupWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public UserCleanupWorker(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var threshold = DateTime.UtcNow.AddHours(-24);
                var usersToDelete = await db.Users
                    .Where(u => !u.EmailConfirmed && u.CreatedAt < threshold)
                    .ToListAsync();

                if (usersToDelete.Any())
                {
                    db.Users.RemoveRange(usersToDelete);
                    await db.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}

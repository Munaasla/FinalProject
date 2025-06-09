using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Services
{
    public class VaccinationReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly NotificationService _notificationService;
        private readonly ILogger<VaccinationReminderService> _logger;

        public VaccinationReminderService(
            IServiceProvider serviceProvider,
            NotificationService notificationService,
            ILogger<VaccinationReminderService> logger)
        {
            _serviceProvider = serviceProvider;
            _notificationService = notificationService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("VaccinationReminderService started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var today = DateTime.Today;
                        var tomorrow = today.AddDays(1);

                        var babiesWithVaccines = await dbContext.Babies
                            .Where(b => b.Vaccinations.Any(v => v.Date.Date == today || v.Date.Date == tomorrow))
                            .Include(b => b.Parent)
                            .ToListAsync(stoppingToken);

                        foreach (var baby in babiesWithVaccines)
                        {
                            var parent = baby.Parent;
                            if (parent == null || string.IsNullOrEmpty(parent.Email))
                                continue;

                            var body = $"תזכורת: מחר {baby.Name} צריך לקבל חיסון!";
                            await _notificationService.SendEmailAsync(parent.Email, body);
                            dbContext.Vaccinations.Add(new Vaccination
                            {
                                BabyId = baby.Id,
                                Date = tomorrow,
                            });
                        }
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in VaccinationReminderService");
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}

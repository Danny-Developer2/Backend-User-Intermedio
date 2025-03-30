using Microsoft.Extensions.Hosting;
using NCrontab;
using Microsoft.Extensions.DependencyInjection;

namespace prueba.Services
{
    public class AttendanceReportScheduler : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly CrontabSchedule _schedule;
    private DateTime _nextRun;

    public AttendanceReportScheduler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        // _schedule = CrontabSchedule.Parse("55 8 * * *");
        _schedule = CrontabSchedule.Parse("35 2 * * *");
        _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            if (now > _nextRun)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var asistenciaService = scope.ServiceProvider.GetRequiredService<IAsistenciaService>();
                    await asistenciaService.SendDailyReportWhatsApp();
                }
                _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            }
            
            await Task.Delay(1000, stoppingToken);
        }
    }
}
}
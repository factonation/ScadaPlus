
using ScadaPlus.Data.Models;
using ScadaPlus.Data.Repositories;
using ScadaPlus.Web.States;

namespace ScadaPlus.Web.BackgroundServices;

public class ApplicationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    public ApplicationBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var services = scope.ServiceProvider;
            var applicationState = services.GetRequiredService<ApplicationState>();
            await applicationState.InitializeAsync();
        }
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var updateStateLastRun = DateTime.UtcNow;
        var updateDatabaseLastRun = DateTime.UtcNow;

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;

            // 5 seconds task
            if (now - updateStateLastRun >= TimeSpan.FromSeconds(5))
            {
                updateStateLastRun = now; // Update last run time
                using (var scope = _serviceProvider.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var applicationState = services.GetRequiredService<ApplicationState>();
                    applicationState.UpdateState();
                }
            }

            // 1 minute task
            if (now - updateDatabaseLastRun >= TimeSpan.FromMinutes(1))
            {
                updateDatabaseLastRun = now; // Update last run time
                using (var scope = _serviceProvider.CreateScope())
                {
                    // Perform the 1-minute task
                    var services = scope.ServiceProvider;
                    var applicationState = services.GetRequiredService<ApplicationState>();
                    var jobRepository = services.GetRequiredService<JobRepository>();

                    foreach (var machineState in applicationState.MachineStates)
                    {
                        if (machineState.IsConnected) // will update only if connected
                        {
                            await jobRepository.UpdateJobAsync(new Job
                            {
                                JobId = machineState.ActiveJobId,
                                IdealCycleTime = machineState.IdealCycleTime,
                                OkCount = machineState.OkCount,
                                NgCount = machineState.NgCount,
                                Availability = machineState.Availability,
                                Performance = machineState.Performance,
                                Quality = machineState.Quality,
                                OEE = machineState.OEE
                            });
                        }
                    }
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}

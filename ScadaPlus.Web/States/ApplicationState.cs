using ScadaPlus.Data.Models;
using ScadaPlus.Data.Repositories;

namespace ScadaPlus.Web.States;

public class ApplicationState
{
    private readonly IServiceProvider _serviceProvider;
    public MachineState[] MachineStates { get; set; } = new MachineState[3];
    public List<Job> Jobs { get; set; } = new List<Job>();
    public ApplicationState(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

    public async Task InitializeAsync()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var machineRepository = scope.ServiceProvider.GetRequiredService<MachineRepository>();
            var machines = await machineRepository.ReadMachinesAsync();
            for (int i = 0; i < MachineStates.Length; i++) 
            {
                MachineStates[i] = new MachineState();
                MachineStates[i].MachineId = machines[i].MachineId;
                MachineStates[i].MachineCode = machines[i].Code;
            }

            var jobRepository = scope.ServiceProvider.GetRequiredService<JobRepository>();
            Jobs = await jobRepository.ReadJobsAsync();
        }
    }

    public async Task RefreshJobsAsync()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var jobRepository = scope.ServiceProvider.GetRequiredService<JobRepository>();
            Jobs = await jobRepository.ReadJobsAsync();
        }
    }

    public void UpdateState()
    {
        var now = DateTime.Now;

        foreach (var machineState in MachineStates)
        {
            Job? activeJob = Jobs.FirstOrDefault(j =>
                j.MachineId == machineState.MachineId &&
                j.StartTime <= now &&
                j.EndTime >= now);

            if (activeJob is not null)
            {
                machineState.IsJobActive = true;
                machineState.ActiveJobId = activeJob.JobId;
                machineState.StartTime = activeJob.StartTime;
                machineState.EndTime = activeJob.EndTime;
            }
            else
            {
                machineState.IsJobActive = false;
                machineState.ActiveJobId = -1;
                machineState.StartTime = DateTime.MinValue;
                machineState.EndTime = DateTime.MinValue;
            }
        }
    }

    public List<MachineState> GetConnectedMachineStates()
    {
        return MachineStates
            .Where(ms => ms.IsConnected)
            .ToList();
    }
}

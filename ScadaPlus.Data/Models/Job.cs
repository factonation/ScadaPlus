namespace ScadaPlus.Data.Models;

public class Job
{
    public int JobId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan FaultOrIdleTime { get; set; }
    public double IdealCycleTime { get; set; }
    public int OkCount { get; set; }
    public int NgCount { get; set; }
    public double Availability { get; set; }
    public double Performance { get; set; }
    public double Quality { get; set; }
    public double OEE { get; set; }
    public int MachineId { get; set; }
    public Machine? Machine { get; set; }
}

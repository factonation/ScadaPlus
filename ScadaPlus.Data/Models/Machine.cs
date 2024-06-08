namespace ScadaPlus.Data.Models;

public class Machine
{
    public int MachineId { get; set; }
    public string Code { get; set; } = string.Empty;
    public List<Job> Jobs { get; set; } = new List<Job>();
}


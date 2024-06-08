using ScadaPlus.Data.Devices;

namespace ScadaPlus.Web.States;

public class MachineState
{
    // Database data
    public int MachineId { get; set; }
    public string MachineCode { get; set; } = string.Empty;
    public bool IsJobActive { get; set; }
    public int ActiveJobId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    // Modbus device
    public bool IsConnected { get; set; }
    public bool[]? Inputs { get; set; }
    public bool[]? Outputs { get; set; }
    public short[]? HoldingRegisters { get; set; }
    public DateTime CurrentTime { get; set; }

    public bool IsRunning
    {
        get
        {
            if (Outputs is not null) return Outputs[0];
            else return false;
        }
    }

    public int FaultOrIdleTimeSeconds
    {
        get
        {
            if (HoldingRegisters is not null) return HoldingRegisters[2];
            else return 0;
        }
    }

    public int IdealCycleTime { get; set; } = 1;

    public int OkCount
    {
        get
        {
            if (HoldingRegisters is not null) return HoldingRegisters[0];
            else return 0;
        }
    }

    public int NgCount
    {
        get
        {
            if (HoldingRegisters is not null) return HoldingRegisters[1];
            else return 0;
        }
    }

    public double Availability
    {
        get
        {
            return ((EndTime - StartTime).TotalSeconds - FaultOrIdleTimeSeconds) * 100 / (EndTime - StartTime).TotalSeconds;
        }
    }

    public double Performance
    {
        get
        {
            return (IdealCycleTime * (OkCount + NgCount)) * 100 / ((EndTime - StartTime).TotalSeconds - FaultOrIdleTimeSeconds);
        }
    }
    public double Quality
    {
        get
        {
            if (OkCount + NgCount == 0)
            {
                return 0;
            }
            else
            {
                return (double)OkCount * 100 / (OkCount + NgCount);
            }
        }
    }
    public double OEE
    {
        get
        {
            return Availability * Performance * Quality / 10000;
        }
    }

    public void UpdateStateFromModbusDeviceData(ModbusDeviceData data)
    {
        IsConnected = data.IsConnected;
        Inputs = data.Inputs;
        Outputs = data.Outputs;
        HoldingRegisters = data.HoldingRegisters;
        CurrentTime = data.CurrentTime;
    }
}

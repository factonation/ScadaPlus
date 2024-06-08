using Newtonsoft.Json;

namespace ScadaPlus.Data.Devices;

public class ModbusDeviceData
{
    [JsonProperty(nameof(IsConnected))]
    public bool IsConnected { get; set; }

    [JsonProperty(nameof(Inputs))]
    public bool[]? Inputs { get; set; }

    [JsonProperty(nameof(Outputs))]
    public bool[]? Outputs { get; set; }

    [JsonProperty(nameof(HoldingRegisters))]
    public short[]? HoldingRegisters { get; set; }

    [JsonProperty(nameof(CurrentTime))]
    public DateTime CurrentTime { get; set; }
}

using Microsoft.AspNetCore.SignalR;
using ScadaPlus.Data.Devices;

namespace ScadaPlus.Web.Hubs;

public class ScadaPlusBaseHub : Hub
{
    public async Task SendModbusDeviceData(ModbusDeviceData modbusDeviceData)
    {
        await Clients.All.SendAsync("ReceiveModbusDeviceData", modbusDeviceData);
    }
}

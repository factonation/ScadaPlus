using FluentModbus;
using Microsoft.Extensions.Logging;

namespace ScadaPlus.Data.Devices;

public class Client
{
    private ModbusTcpClient _modbusTcpClient;

    private readonly ILogger<Client> _logger;

    public bool IsConnected
    {
        get
        {
            return _modbusTcpClient.IsConnected;
        }
    }

    public Client(ILogger<Client> logger)
    {
        _modbusTcpClient = new ModbusTcpClient();
        _logger = logger;
    }

    public bool Connect(string ipAddress)
    {
        try
        {
            _modbusTcpClient.Connect(ipAddress, ModbusEndianness.BigEndian);
            _logger.LogInformation($"Connected to {ipAddress}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return false;
        }
    }

    public void Disconnect()
    {
        _modbusTcpClient.Disconnect();
        _logger.LogInformation($"Disconnected");
    }

    // FC01
    public async Task<Memory<byte>?> ReadCoilsAsync(int unitIdentifier, int startingAddress, int quantity)
    {
        try
        {
            if (!IsConnected) throw new InvalidOperationException($"Client not connected");

            _logger.LogInformation($"{nameof(ReadCoilsAsync)} {unitIdentifier} {startingAddress} {quantity}");
            return await _modbusTcpClient.ReadCoilsAsync(unitIdentifier, startingAddress, quantity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    // FC02
    public async Task<Memory<byte>?> ReadDiscreteInputsAsync(int unitIdentifier, int startingAddress, int quantity)
    {
        try
        {
            if (!IsConnected) throw new InvalidOperationException($"Client not connected");

            _logger.LogInformation($"{nameof(ReadDiscreteInputsAsync)} {unitIdentifier} {startingAddress} {quantity}");
            return await _modbusTcpClient.ReadDiscreteInputsAsync(unitIdentifier, startingAddress, quantity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    // FC03
    public async Task<Memory<short>?> ReadHoldingRegistersAsync(int unitIdentifier, int startingAddress, int count)
    {
        try
        {
            if (!IsConnected) throw new InvalidOperationException($"Client not connected");

            _logger.LogInformation($"{nameof(ReadHoldingRegistersAsync)} {unitIdentifier} {startingAddress} {count}");
            return await _modbusTcpClient.ReadHoldingRegistersAsync<short>(unitIdentifier, startingAddress, count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    // FC04
    public async Task<Memory<short>?> ReadInputRegisterAsync(int unitIdentifier, int startingAddress, int count)
    {
        try
        {
            if (!IsConnected) throw new InvalidOperationException($"Client not connected");

            _logger.LogInformation($"{nameof(ReadInputRegisterAsync)} {unitIdentifier} {startingAddress} {count}");
            return await _modbusTcpClient.ReadInputRegistersAsync<short>(unitIdentifier, startingAddress, count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    // FC05
    public async Task WriteSingleCoilAsync(int unitIdentifier, int registerAddress, bool value)
    {
        try
        {
            if (!IsConnected) throw new InvalidOperationException($"Client not connected");

            _logger.LogInformation($"{nameof(WriteSingleCoilAsync)} {unitIdentifier} {registerAddress} {value}");
            await _modbusTcpClient.WriteSingleCoilAsync(unitIdentifier, registerAddress, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    // FC06
    public async Task WriteSingleRegisterAsync(int unitIdentifier, int registerAddress, short value)
    {
        try
        {
            if (!IsConnected) throw new InvalidOperationException($"Client not connected");

            _logger.LogInformation($"{nameof(WriteSingleRegisterAsync)} {unitIdentifier} {registerAddress} {value}");
            await _modbusTcpClient.WriteSingleRegisterAsync(unitIdentifier, registerAddress, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    // FC16
    public async Task WriteMultipleRegistersAsync(int unitIdentifier, int startingAddress, short[] dataset)
    {
        try
        {
            if (!IsConnected) throw new InvalidOperationException($"Client not connected");

            _logger.LogInformation($"{nameof(WriteMultipleRegistersAsync)} {unitIdentifier} {startingAddress} {dataset}");
            await _modbusTcpClient.WriteMultipleRegistersAsync(unitIdentifier, startingAddress, dataset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}

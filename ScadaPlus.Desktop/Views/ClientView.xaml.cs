using Microsoft.AspNetCore.SignalR.Client;
using ScadaPlus.Data.Constants;
using ScadaPlus.Data.Devices;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ScadaPlus.Desktop.Views
{
    /// <summary>
    /// Interaction logic for ClientView.xaml
    /// </summary>
    public partial class ClientView : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private Timer _timer;

        public Client? Client { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string HubPath { get; set; } = string.Empty;
        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; OnPropertyChanged(nameof(IsConnected)); }
        }
        public ObservableCollection<string> StatusMessages { get; set; } = new ObservableCollection<string>();

        public int InputStartAddress { get; set; }
        private bool[] _inputs = new bool[16];
        public bool[] Inputs
        {
            get { return _inputs; }
            set { _inputs = value; OnPropertyChanged(nameof(Inputs)); }
        }
        public int OutputStartAddress { get; set; }
        private bool[] _outputs = new bool[10];
        public bool[] Outputs
        {
            get { return _outputs; }
            set { _outputs = value; OnPropertyChanged(nameof(Outputs)); }
        }
        public int HoldingRegisterStartAddress { get; set; }
        private short[] _holdingRegisters = new short[3];
        public short[] HoldingRegisters
        {
            get { return _holdingRegisters; }
            set { _holdingRegisters = value; OnPropertyChanged(nameof(HoldingRegisters)); }
        }

        public DateTime CurrentTime { get; set; }

        private HubConnection? _hubConnection;

        public ClientView()
        {
            InitializeComponent();

            DataContext = this;

            _timer = new Timer(HandleModbusData, null, Timeout.Infinite, Timeout.Infinite);
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{Constant.ServerHost}/{HubPath}")
                .WithAutomaticReconnect()
                .Build();
            _hubConnection.Reconnecting += HubConnection_Reconnecting;
            _hubConnection.Reconnected += HubConnection_Reconnected;
            _hubConnection.Closed += HubConnection_Closed;

            try
            {
                if (_hubConnection is not null)
                {
                    await _hubConnection.StartAsync();
                    AddStatusMessage("เชื่อมต่อกับ Hub สำเร็จ");
                }
            }
            catch (Exception ex)
            {
                AddStatusMessage($"เชื่อมต่อกับ Hub ไม่สำเร็จ {ex.Message}");
            }
        }

        private async Task HubConnection_Closed(Exception? arg)
        {
            AddStatusMessage("การเชื่อมต่อกับ Hub ถูกปิด");
            await Task.Delay(1);
        }

        private async Task HubConnection_Reconnected(string? arg)
        {
            AddStatusMessage("เชื่อมต่อกับ Hub สำเร็จ");
            await Task.Delay(1);
        }

        private async Task HubConnection_Reconnecting(Exception? arg)
        {
            AddStatusMessage("กำลังเชื่อมต่อกับ Hub ใหม่");
            await Task.Delay(1);
        }
        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (Client is null || Client.IsConnected)
            {
                MessageBox.Show("เชื่อมต่ออยู่แล้ว");
                return;
            }

            bool connectionResult = Client.Connect(IpAddress);
            if (connectionResult)
            {
                IsConnected = true;
                AddStatusMessage("เชื่อมต่อกับ PLC สำเร็จ");

                _timer.Change(TimeSpan.Zero, Constant.DataUpdateFrequency);
            }
            else
            {
                MessageBox.Show("เชื่อมต่อกับ PLC ไม่สำเร็จ");
                AddStatusMessage("เชื่อมต่อกับ PLC ไม่สำเร็จ");
            }
        }

        private async void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (Client is null || !Client.IsConnected)
            {
                MessageBox.Show("ยังไม่ได้เชื่อมต่อ");
                return;
            }

            Client.Disconnect();

            AddStatusMessage("ยกเลิกการเชื่อมต่อกับ PLC");
            await SetInitialState();
        }

        private async Task SetInitialState()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            Client?.Disconnect();
            IsConnected = false;
            await SendDataToHub();
        }

        private async void HandleModbusData(object? state)
        {
            if (Client is null) return;

            if (!Client.IsConnected)
            {
                AddStatusMessage("ไม่ได้เชื่อมต่ออยู่กับ PLC");
                await SetInitialState();
                return;
            }

            try
            {
                Memory<byte>? readInputsData = await Client.ReadDiscreteInputsAsync(0, InputStartAddress, 16);
                if (readInputsData is not null)
                {
                    Inputs = ConvertMemoryToBoolArray(readInputsData.Value);
                }

                Memory<byte>? readCoilsData = await Client.ReadCoilsAsync(0, OutputStartAddress, 10);
                if (readCoilsData is not null)
                {
                    Outputs = ConvertMemoryToBoolArray(readCoilsData.Value);
                }

                Memory<short>? readHoldingRegistersData = await Client.ReadHoldingRegistersAsync(0, HoldingRegisterStartAddress, 3);
                if (readHoldingRegistersData is not null)
                {
                    HoldingRegisters = readHoldingRegistersData.Value.ToArray();
                }

                await SendDataToHub();
            }
            catch (Exception ex)
            {
                AddStatusMessage($"เกิดปัญหาจากการอ่านข้อมูล PLC {ex.Message}");
                await SetInitialState();
            }
        }

        private async Task SendDataToHub()
        {
            var modbusDeviceData = new ModbusDeviceData
            {
                IsConnected = IsConnected,
                Inputs = Inputs,
                Outputs = Outputs,
                HoldingRegisters = HoldingRegisters,
                CurrentTime = DateTime.Now
            };

            if (_hubConnection is not null)
                await _hubConnection.SendAsync("SendModbusDeviceData", modbusDeviceData);
        }


        private void AddStatusMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                StatusMessages.Add($"{DateTime.Now}: {message}");
            });
        }

        private bool[] ConvertMemoryToBoolArray(Memory<byte> memory)
        {
            byte[] byteArray = memory.ToArray();
            int numBits = byteArray.Length * 8;
            bool[] boolArray = new bool[numBits];

            for (int i = 0; i < numBits; i++)
            {
                int byteIndex = i / 8;
                int bitIndex = i % 8;
                boolArray[i] = (byteArray[byteIndex] & (1 << bitIndex)) != 0;
            }

            return boolArray;
        }
    }
}

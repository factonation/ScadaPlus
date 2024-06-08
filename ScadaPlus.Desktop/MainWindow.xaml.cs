using ScadaPlus.Data.Devices;
using System.Windows;

namespace ScadaPlus.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(Client client1,
            Client client2,
            Client client3)
        {
            InitializeComponent();

            ClientView1.Client = client1;
            ClientView1.InputStartAddress = 0;
            ClientView1.OutputStartAddress = 0;
            ClientView1.HoldingRegisterStartAddress = 0;

            ClientView2.Client = client2;
            ClientView2.InputStartAddress = 0;
            ClientView2.OutputStartAddress = 0;
            ClientView2.HoldingRegisterStartAddress = 0;

            ClientView3.Client = client3;
            ClientView3.InputStartAddress = 1024;
            ClientView3.OutputStartAddress = 1280;
            ClientView3.HoldingRegisterStartAddress = 4096;
        }
    }
}
using System.Windows;
using System.Windows.Controls;

namespace ScadaPlus.Desktop.Components
{
    /// <summary>
    /// Interaction logic for BitLamps.xaml
    /// </summary>
    public partial class BitLamps : UserControl
    {
        public bool[] Bits
        {
            get { return (bool[])GetValue(BitsProperty); }
            set { SetValue(BitsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Bits.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BitsProperty =
            DependencyProperty.Register("Bits", typeof(bool[]), typeof(BitLamps), new PropertyMetadata(null));


        public BitLamps()
        {
            InitializeComponent();

            DataContext = this;
        }
    }
}

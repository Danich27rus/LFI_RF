using System.Windows;
using System.IO.Ports;
using test_app2.SerialPortDevice;
using test_app2.ViewModels;


namespace test_app2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SerialPort kek;
        public MainWindow()
        {
            InitializeComponent();
            //test_app2.SerialPortDevice.CustomParity;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((MainViewModel)DataContext).SerialPort.CloseAll();
        }
    }

}

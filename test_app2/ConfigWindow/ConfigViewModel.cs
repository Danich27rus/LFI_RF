using TheRFramework.Utilities;
using test_app2.SerialPortDevice;

namespace test_app2.Config
{
    public class ConfigViewModel : BaseViewModel
    {
        public Command ConfigCommand { get; }

        public SerialPortViewModel SerialPort { get; set; }

        public ConfigViewModel(SerialPortViewModel serialPort)
        {
            SerialPort = serialPort;
            ConfigCommand = new Command(ShowConfigWindow);
        }

        private void ShowConfigWindow()
        {
            var configWindow = new ConfigWindow
            {
                DataContext = this
            };
            configWindow.Show();
        }
    }
}

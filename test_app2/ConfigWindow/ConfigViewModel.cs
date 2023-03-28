using TheRFramework.Utilities;
using test_app2.SerialPortDevice;
using test_app2.FaultIndicators;

namespace test_app2.Config
{
    public class ConfigViewModel : BaseViewModel
    {
        public Command ConfigCommand { get; }

        public SerialPortViewModel SerialPort { get; set; }

        public FaultIndicatorViewModel FaultIndicator { get; set; }

        public ConfigViewModel(SerialPortViewModel serialPort, FaultIndicatorViewModel faultIndicator)
        {
            SerialPort = serialPort;
            FaultIndicator = faultIndicator;
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

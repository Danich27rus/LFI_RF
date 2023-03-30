using TheRFramework.Utilities;
using test_app2.SerialPortDevice;
using test_app2.FaultIndicators;

namespace test_app2.Config
{
    public class ConfigViewModel : BaseViewModel
    {
        public Command ConfigCommand { get; }

        public SerialPortViewModel SerialPort { get; set; }

        public IndicatorDataViewModel IndicatorData { get; set; }

        public ConfigViewModel(SerialPortViewModel serialPort, IndicatorDataViewModel indicatorData)
        {
            SerialPort = serialPort;
            IndicatorData = indicatorData;
            //Selected
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

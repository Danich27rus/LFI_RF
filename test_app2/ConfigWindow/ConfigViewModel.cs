using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using TheRFramework.Utilities;
using System.Collections.ObjectModel;

namespace test_app2.Config
{
    public class ConfigViewModel : BaseViewModel
    {
        public Command ConfigCommand { get; }

        public ConfigWindow Config { get; set; }

        public SerialPort Port { get; set; }

        public ConfigViewModel()
        {
            Config = new ConfigWindow();
            ConfigCommand = new Command(ShowConfigWindow);
        }
        private void ShowConfigWindow()
        {
            Config.Show();
        }
    }
}

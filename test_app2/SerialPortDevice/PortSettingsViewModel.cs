using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRFramework.Utilities;

namespace test_app2.SerialPortDevice
{
    public class PortSettingsViewModel : BaseViewModel
    {
        private string _selectedComPort;
        private int _selectedBaudRate;
        private Parity _selectedParityBits;
        private int _selectedDataBits;
        private StopBits _selectedStopBits;
        public string SelectedCOMPort 
        {
            get => _selectedComPort; 
            set => RaisePropertyChanged(ref _selectedComPort, value); 
        }
        public int SelectedBaudRate
        {
            get => _selectedBaudRate;
            set => RaisePropertyChanged(ref _selectedBaudRate, value);
        }
        public StopBits SelectedStopBits
        {
            get => _selectedStopBits;
            set => RaisePropertyChanged(ref _selectedStopBits, value);
        }
        public Parity SelectedParityBits
        {
            get => _selectedParityBits;
            set => RaisePropertyChanged(ref _selectedParityBits, value);
        }
        public int SelectedDataBits
        {
            get => _selectedDataBits;
            set => RaisePropertyChanged(ref _selectedDataBits, value);
        }
        public ObservableCollection<string> AvaliablePorts { get; set; }

        public ObservableCollection<int> BaudRates { get; set; }

        public ObservableCollection<int> DataBits { get; set; }

        public ObservableCollection<double> StopBits { get; set; }

        public ObservableCollection<String> ParityBits { get; set; }

        //public
        //public ObservableCollection<int> FlowControl { get; set; }

        public Command RefreshPortsCommand { get; }

        public PortSettingsViewModel() 
        {
            AvaliablePorts = new ObservableCollection<string>();

            DataBits = new ObservableCollection<int>
            {
                5, 6, 7, 8
            };

            StopBits = new ObservableCollection<double>
            {
                0, 1, 1.5, 2
            };

            /*ParityBits = new ObservableCollection<string>
            {
                "Odd", "Even", "Mark", "Space"
            };*/

            BaudRates = new ObservableCollection<int> 
            { 
                110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 56000, 57600, 115200, 128000, 230400, 256000, 460800, 500000, 512000
            };

            RefreshPortsCommand = new Command(RefreshPorts);
            RefreshPorts();
        }

        private void RefreshPorts()
        {
            AvaliablePorts.Clear();
            foreach (string port in SerialPort.GetPortNames())
            {
                AvaliablePorts.Add(port);
            }
        }
    }
}

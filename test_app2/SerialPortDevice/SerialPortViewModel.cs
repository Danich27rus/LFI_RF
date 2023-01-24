using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using test_app2.Utilities;
using test_app2.SerialPMessages;

namespace test_app2.SerialPortDevice
{
    public class SerialPortViewModel : BaseViewModel
    {
        // will be used to bind to the currently connected port
        private string _connectedPort;
        public string ConnectedPort
        {
            get => _connectedPort;
            set => RaisePropertyChanged(ref _connectedPort, value);
        }

        public SerialPort Port { get; set; }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set => RaisePropertyChanged(ref _isConnected, value);
        }

        // Because a button is used to connect/disconnect, well....
        public Command AutoConnectDisconnectCommand { get; }
        // NOt really that useful, but can be used to clear the serialport's receive/send buffers, but they probably wont fill up unless you send a giant message and noone responds... sort of
        public Command ClearBuffersCommand { get; }

        public PortSettingsViewModel Settings { get; set; }

        public SerialPortMessagesViewModel Messages { get; set; }

        public SerialPortMessagesReceive Receiver { get; set; }

        public SerialPortMessagesSend Sender { get; set; }

        public SerialPortViewModel()
        {

        }

        public void CloseAll()
        {
            Disconnect();
            Receiver.StopThreadLoop();
        }
    }
}

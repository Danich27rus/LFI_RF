using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test_app2.SerialPMessages;
using test_app2.SerialPortDevice;
using test_app2.Config;
using test_app2.UI;

namespace test_app2.ViewModels
{
    internal class MainViewModel
    { 
        //19 00 00 04 08 00 00 00 00 00 00 F4 8B
        //тест для последовательного порта через КДТН
        public SerialPortMessagesViewModel Messages { get; set; }

        public SerialPortMessagesReceive Receiver { get; set; }

        public SerialPortMessagesSend Sender { get; set; }

        public SerialPortViewModel SerialPort { get; set; }

        public ConfigViewModel Config { get; set; }

        public MainViewModel()
        {
            SerialPort = new SerialPortViewModel();
            Receiver = new SerialPortMessagesReceive();
            Sender = new SerialPortMessagesSend();
            Messages = new SerialPortMessagesViewModel();

            // hmm
            Receiver.Messages = Messages;
            Messages.Sender = Sender;
            Sender.Messages = Messages;

            SerialPort.Receiver = Receiver;
            SerialPort.Sender = Sender;
            SerialPort.Messages = Messages;

            Receiver.Port = SerialPort.Port;
            Sender.Port = SerialPort.Port;

            Config = new ConfigViewModel(SerialPort);
            //Config.SerialPort = SerialPort;
            //Config.Port = SerialPort.Port;
            //Config.Messages = SerialPort.Messages;
            //Config.Sender = Sender;
            //Config.Receiver = Receiver;

            //Config.Config.DataContext = this;
        }
    }
}

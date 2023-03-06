using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Controls;

namespace test_app2.SerialPMessages
{
    /// <summary>
    /// Used for receiving messages from serial port
    /// </summary>
    public class SerialPortMessagesReceive
    {
        public static CancellationTokenSource cancelSource = new CancellationTokenSource();
        private Thread ReceiverThread { get; set; }
        public bool CanReceive { get; set; }
        public bool ShouldShutDownPermanently { get; set; }

        public SerialPortMessagesViewModel SerialPortModel { get; set; }
        public SerialPort Port { get; set; }
        public SerialPortMessagesViewModel Messages { get; set; }
        public SerialPortMessagesReceive() 
        {
            CanReceive = true; 
            ShouldShutDownPermanently = false;

            new Thread(() =>
            {
                try
                {
                    ReceiveLoop(cancelSource.Token);
                }
                catch (OperationCanceledException)
                {
                    SerialPortModel.AddReceivedMessage("Галя, отмена по токену!");
                }
            }).Start();
            //ReceiverThread.Start();
        }

        private void ReceiveLoop(CancellationToken cancelToken)
        {
            string message = "";
            char read;

            while (true)
            {
                if (cancelToken.IsCancellationRequested) {
                    ShouldShutDownPermanently = true;
                }
                if (ShouldShutDownPermanently)
                {
                    return;
                }
                if (CanReceive)
                {
                    if (Port != null && Port.IsOpen)
                    {
                        while(Port.BytesToRead > 0)
                        {
                            read = (char)Port.ReadChar();
                            switch (read)
                            {
                                case '\r':
                                    break;
                                case '\n':
                                    SerialPortModel.AddReceivedMessage(message);
                                    message = "";
                                    break;
                                default:
                                    message += read;
                                    break;
                            }
                        }
                    }
                }
                Thread.Sleep(1);
            }
            //Dispatcher.Thread.Interrupt();
        }

        public void StopThreadLoop()//CancellationTokenSource cancelToken)
        {
            CanReceive = false;
            ShouldShutDownPermanently = true; //&= ReceiverThread.IsAlive;
            cancelSource.Cancel();
            //if (ReceiverThread != null)
            //{
                //ReceiverThread.Interrupt();
            //}
        }
    }
}

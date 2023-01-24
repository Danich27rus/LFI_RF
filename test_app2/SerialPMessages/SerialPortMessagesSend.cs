using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace test_app2.SerialPMessages
{
    /// <summary>
    /// Used for sending messages to the serial port (on the main thread...). Just an easier way to manage receiving/sending tbh
    /// </summary>
    public class SerialPortMessagesSend
    {
        public SerialPort Port;
        public bool CanSend { get; set; }
        public SerialPortMessagesSend()
        {
            CanSend = true;
        }

        public void SendMessage(string message, bool shouldSendNewLine = true)
        {
            // Adds a new line if needed
            string newMessage = message + (shouldSendNewLine ? "\n" : "");
            // Gets the bytes of the message using the serial port's encoding
            // Will be using a byte buffer because it's faster than sending strings using 
            // the serial port's build in methods... apparently
            byte[] buffer = Port.Encoding.GetBytes(newMessage);

            for (int i = 0; i < buffer.Length; i++)
            {
                Port.BaseStream.WriteByte(buffer[i]);
            }
        }
    }
}

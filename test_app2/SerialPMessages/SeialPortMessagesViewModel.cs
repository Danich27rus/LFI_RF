using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRFramework.Utilities;

namespace test_app2.SerialPMessages
{
    public class SerialPortMessagesViewModel : BaseViewModel
    {
        private string _messagesText;
        private string _toBeSentText;

        public string MessagesText
        {
            get => _messagesText;
            set => RaisePropertyChanged(ref _messagesText, value);
        }

        public string ToBeSentText
        {
            get => _toBeSentText;
            set => RaisePropertyChanged(ref _toBeSentText, value);
        }

        public Command ClearMessagesCommand { get; }
        public Command SendMessageCommand { get; }

        public SerialPortMessagesSend Sender { get; set; }

        public SerialPortMessagesViewModel()
        {
            //MessagesCount = 0;
            MessagesText = "";
            ToBeSentText = "";

            ClearMessagesCommand = new Command(ClearMessages);
            SendMessageCommand = new Command(SendMessage);
        }

        private void SendMessage()
        {
            if (!string.IsNullOrEmpty(ToBeSentText))
            {
                try
                {
                    Sender.SendMessage(ToBeSentText);
                    AddSentMessage(ToBeSentText);
                    ToBeSentText = "";
                }
                catch(TimeoutException timeout) 
                {
                    AddMessage("Время ожидания отправки истекло. Не удалось отаправить сообщение");
                }
                catch(Exception e)
                {
                    AddMessage("Ошибка: " + e.ToString());
                }

            }
        }
        private void ClearMessages()
        {
            MessagesText = "";
            //MessagesCount = 0;
        }

        public void AddSentMessage(string message)
        {
            // (Date) | TX> hello there
            AddMessage($"{DateTime.Now} | TX> {message}");
        }

        public void AddReceivedMessage(string message)
        {
            // (Date) | RX> hello there
            AddMessage($"{DateTime.Now} | RX> {message}");
        }

        public void AddMessage(string message)
        {
            WriteLine(message);
            //MessagesCount++;
        }

        public void WriteLine(string text)
        {
            MessagesText += text + '\n';
        }
    }
}

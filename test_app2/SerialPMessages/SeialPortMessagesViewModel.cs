using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using test_app2.FaultIndicators;
using TheRFramework.Utilities;

namespace test_app2.SerialPMessages
{
    public class SerialPortMessagesViewModel : BaseViewModel
    {
        private string _messagesText;
        private string _toBeSentText;
        private bool _isHEX;
        private bool _isRepeat; //тест репетативной отправки
        private bool _addNewLine;

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

        public bool IsHEX
        {
            get => _isHEX;
            set => RaisePropertyChanged(ref _isHEX, value);
        }

        public bool AddNewLine
        {
            get => _addNewLine;
            set => RaisePropertyChanged(ref _addNewLine, value);
        }

        public bool Repeat
        {
            get => _isRepeat;
            set => RaisePropertyChanged(ref _isRepeat, value);
        }

        public Command ClearMessagesCommand { get; }
        public Command SendMessageCommand { get; }
        public Command SendIndicatorMessageCommand { get; }

        public SerialPortMessagesSend Sender { get; set; }

        public ObservableCollection<FaultIndicatorViewModel> Indicators { get; set; }


        public SerialPortMessagesViewModel()
        {
            //MessagesCount = 0;
            Indicators = new ObservableCollection<FaultIndicatorViewModel>();
            MessagesText = "";
            ToBeSentText = "";
            IsHEX = false;
            AddNewLine = false;

            ClearMessagesCommand = new Command(ClearMessages);
            SendMessageCommand = new Command(SendMessage);
            //SendIndicatorMessageCommand = new Command(SendIndicatorMessage);
        }

        private void SendMessage()
        {
            if (!Sender.Port.IsOpen)
            {
                AddMessage("Порт не открыт, не удалось отправить сообщение");
                return;
            }
            if (!string.IsNullOrEmpty(ToBeSentText))
            {
                try
                {
                    if (IsHEX)
                    {
                        string HEXpattern = @"^[0-9A-Fa-f]{2}( [0-9A-Fa-f]{2})*$";
                        if (!Regex.IsMatch(ToBeSentText, HEXpattern))
                        {
                            AddMessage("Сообщение не соотвествует формату\r\nФормат: 6A 80 BC 73 70 E2");
                        }
                        else
                        {
                            Sender.SendHEXMessage(ToBeSentText, AddNewLine);
                            AddSentMessage(ToBeSentText);
                        }
                    }
                    else
                    {
                        Sender.SendMessage(ToBeSentText, AddNewLine);
                        AddSentMessage(ToBeSentText);
                    }
                    ToBeSentText = "";
                }
                catch (TimeoutException timeout)
                {
                    AddMessage("Время ожидания отправки истекло. Не удалось отправить сообщение");
                }
                catch (Exception e)
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
            // byte[] bytes = Encoding.UTF8.GetBytes(message);
            // AddMessage($"{DateTime.Now} | TX> {bytes}");
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
        }

        public void WriteLine(string text)
        {
            MessagesText += text + '\n';
        }
    }
}

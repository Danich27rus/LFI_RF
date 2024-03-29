﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        private bool _isHEX;
        private bool _isRepeat; //тест репетативной отправки

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

        public bool Repeat
        {
            get => _isRepeat;
            set => RaisePropertyChanged(ref _isRepeat, value);
        }

        public Command ClearMessagesCommand { get; }
        public Command SendMessageCommand { get; }

        public SerialPortMessagesSend Sender { get; set; }

        public SerialPortMessagesViewModel()
        {
            //MessagesCount = 0;
            MessagesText = "";
            ToBeSentText = "";
            IsHEX = false;

            ClearMessagesCommand = new Command(ClearMessages);
            SendMessageCommand = new Command(SendMessage);
        }

        private void SendMessage()
        {
            if (Sender.Port.IsOpen)
            {
                if (!string.IsNullOrEmpty(ToBeSentText))
                {
                    try
                    {
                        Sender.SendMessage(ToBeSentText);
                        AddSentMessage(ToBeSentText);
                        ToBeSentText = "";
                    }
                    catch (TimeoutException timeout)
                    {
                        AddMessage("Время ожидания отправки истекло. Не удалось отаправить сообщение");
                    }
                    catch (Exception e)
                    {
                        AddMessage("Ошибка: " + e.ToString());
                    }

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
            if (_isHEX)
            {

            }
            else
            {
                AddMessage($"{DateTime.Now} | TX> {message}");
            } 
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

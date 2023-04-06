using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using test_app2.SerialPMessages;
using test_app2.Utilities;
using TheRFramework.Utilities;

namespace test_app2.FaultIndicators
{
    public class IndicatorDataViewModel : BaseViewModel
    {

        //TODO: Сделать enumы
        private string[] _namingModel = {
            "РИМ ИКЗ-10",
            "JYL-FF-FI",
            "JYL-FF-CN-HP",
            "JYZ(W)-FF-FI V1.0",
            "JYZ(W)-FF-FI V2.0 (JYZ-HW-FI V1.0)",
            "DYZ-FF-FI",
            "DYZ-FF-FI",
            "JYZ-HW-FI V2.0",
            "JYZ-LB",
            "F-LTS100",
            "JYZ(W)-FF-FI V2.0",
            "JYZ-HW-LoRa",
            "JYZ-LH-LoRa"
        };

        private string[] _namingProtocol =
        {
            "XY-Old",
            "XY-New",
            "LB-FF"
        };

        private string[] _namingFamily =
        {
            "JYZ-FF",
            "JYZ-HW",
            "JYZ-HW V2.0"
        };

        private string[] _patterns =
        {
            "E5 E5",        //Подтверждение от RF
            "A5 5A 11 84",  //Кол-во индикаторов
            "A5 5A 18 44"   //MAC адреса индикаторов
        };

        private int _deviceModelNum;
        private int _deviceFamilyNum;
        private int _deviceCommunicationProtocolNum;
        private string _deviceModel;
        private string _deviceFamily;
        private string _deviceCommunicationProtocol;
        private string[] _request;
        private byte[] _requestTest;
        private int _callAdress;
        private int _callFrequency;
        private int _callTime;
        private int _waitTime;
        private int _trailer;
        private static int _indicatorsAmount;
        private string _mac;
        //TODO: Разобраться что снизу
        private int _comFrequency;
        private int _callLevel;

        public int DeviceModelNum
        {
            get => _deviceModelNum;
            set => RaisePropertyChanged(ref _deviceModelNum, value);
        }
        public int DeviceFamilyNum
        {
            get => _deviceFamilyNum;
            set => RaisePropertyChanged(ref _deviceFamilyNum, value);
        }
        public int DeviceCommunicationProtocolNum
        {
            get => _deviceCommunicationProtocolNum;
            set => RaisePropertyChanged(ref _deviceCommunicationProtocolNum, value);
        }
        public string DeviceModel
        {
            get => _deviceModel;
            set => RaisePropertyChanged(ref _deviceModel, value);
        }
        public string DeviceFamily
        {
            get => _deviceFamily;
            set => RaisePropertyChanged(ref _deviceFamily, value);
        }
        public string DeviceCommunicationProtocol
        {
            get => _deviceCommunicationProtocol;
            set => RaisePropertyChanged(ref _deviceCommunicationProtocol, value);
        }
        public int CallAdress
        {
            get => _callAdress;
            set => RaisePropertyChanged(ref _callAdress, value);
        }
        public int CallFrequency
        {
            get => _callFrequency;
            set => RaisePropertyChanged(ref _callFrequency, value);
        }
        public int CallTime
        {
            get => _callTime;
            set => RaisePropertyChanged(ref _callTime, value);
        }
        public int WaitTime
        {
            get => _waitTime;
            set => RaisePropertyChanged(ref _waitTime, value);
        }
        public int IndicatorsAmount
        {
            get => _indicatorsAmount;
            set => RaisePropertyChanged(ref _indicatorsAmount, value);
        }
        public int Trailer
        {
            get => _trailer;
            set => RaisePropertyChanged(ref _trailer, value);
        }
        public string MAC
        {
            get => _mac;
            set => RaisePropertyChanged(ref _mac, value);
        }
        public string[] Request
        {
            get => _request;
            set => RaisePropertyChanged(ref _request, value);
        }
        public byte[] RequestTest
        {
            get => _requestTest;
            set => RaisePropertyChanged(ref _requestTest, value);
        }

        public static CancellationTokenSource cancelSource = new CancellationTokenSource();

        public Command UpdateModelContentCommand { get; }

        public Command ReadIndicatorParametersCommand { get; }

        public Command UpdateFamilyContentCommand { get; }

        public Command UpdateCommunicationProtocolCommand { get; }

        public Command ClearAllContentCommand { get; }

        public SerialPortMessagesViewModel Messages { get; set; }

        public SerialPortMessagesReceive Receiver { get; set; }

        public SerialPortMessagesSend Sender { get; set; }

        public ObservableCollection<FaultIndicatorViewModel> Indicators { get; set; }

        public string IndicatorConfirm { get; set; }
        
        public Checksum checksum { get; set; }

        public IndicatorDataViewModel()
        {
            Trailer = 22;
            IndicatorConfirm = "";

            Messages = new SerialPortMessagesViewModel();
            Receiver = new SerialPortMessagesReceive();
            Sender = new SerialPortMessagesSend();
            checksum = new Checksum();

            Indicators = new ObservableCollection<FaultIndicatorViewModel>
            {
                //new FaultIndicatorViewModel() { MACAdress="68-04-00-DA" },
                //new FaultIndicatorViewModel() { MACAdress="68-04-00-DB" },
                //new FaultIndicatorViewModel() { MACAdress="68-04-00-DC" }
            };

            UpdateModelContentCommand = new Command(UpdateModelContent);
            UpdateFamilyContentCommand = new Command(UpdateFamilyContent);
            ClearAllContentCommand = new Command(ClearAllContent);
            UpdateCommunicationProtocolCommand = new Command(UpdateCommunicationContent);
            ReadIndicatorParametersCommand = new Command(ReadIndicatorsParameters);

            /*new Thread(() =>
            {
                try
                {
                    ParseLoop(cancelSource.Token);
                }
                catch (OperationCanceledException)
                {
                    Messages.AddReceivedMessage("Галя, отмена по токену x2!");
                }
            }).Start();*/

        }

        public void ParseCommand()
        {
            //string message = "";
            //string byteMessage = "";
            //char readChar;
            //int readByte;

            if (string.IsNullOrEmpty(IndicatorConfirm))
            {
                return;
            }
            if (IndicatorConfirm.Contains(_patterns[0]))
            {
                Messages.AddMessage("Пришел ответ от RF конвертора...");
                return;
            }
            if (IndicatorConfirm.Contains(_patterns[1]))
            {
                IndicatorsAmount++;
                return;
            }
            if (IndicatorConfirm.Contains(_patterns[2]))
            {
                if (IndicatorsAmount == 0)
                {
                    Messages.AddMessage("НЕ МОЖЕТ БЫТЬ!");
                }
                string[] msg;
                IndicatorsAmount--;
                msg = IndicatorConfirm.Split(' ');

                MAC = $"{msg[7]}-{msg[6]}-{msg[5]}-{msg[4]}";
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    if (Indicators.Any(x => x.MACAdress == MAC))
                    {
                        return;
                    }
                    Indicators.Add(new FaultIndicatorViewModel { MACAdress = MAC });
                });
            }
            IndicatorConfirm = "";
            //Thread.Sleep(5);
        }
        //Dispatcher.Thread.Interrupt();

        public void ReadIndicatorsParameters()
        {
            ushort crc;

            if (!Sender.Port.IsOpen)
            {
                Messages.AddMessage("Порт не открыт, не удалось отправить сообщение");
                return;
            }

            Request = new string[] { "A5", "5A", "15", "24", "0A", "1E", "00", "0B", "00", "00", "00", "00", "00", "00", $"{CallAdress.ToString("X2")}", "00", "01", "00", "8E", "0E", $"{Trailer}" };

            crc = checksum.CheckSum_CRC(Request, 2, Request.Length);
            Request[^2] = string.Join("", (crc & 255).ToString("X2"));
            Request[^3] = string.Join("", (crc >> 8).ToString("X2"));

            Sender.SendHEXMessage(string.Join(" ", Request));
            Messages.AddSentMessage(string.Join(" ", Request));
        }

        public void StopThreadLoop()//CancellationTokenSource cancelToken)
        {
            //CanReceive = false;
            //ShouldShutDownPermanently = true; //&= ReceiverThread.IsAlive;
            //cancelSource.Cancel();
            //if (ReceiverThread != null)
            //{
            //ReceiverThread.Interrupt();
            //}
        }

        public void UpdateModelContent()
        {
            //TODO: Исправить баг с выбором при отсчёте с 0
            DeviceModel = _namingModel[DeviceModelNum - 1];
        }

        public void UpdateFamilyContent()
        {
            //TODO: Исправить баг с выбором при отсчёте с 0
            DeviceFamily = _namingFamily[DeviceFamilyNum - 1];
        }

        public void UpdateCommunicationContent()
        {
            //TODO: Исправить баг с выбором при отсчёте с 0
            DeviceCommunicationProtocol = _namingProtocol[DeviceCommunicationProtocolNum - 1];
        }

        public void ClearAllContent()
        {
            DeviceFamily = "";
            DeviceModel = "";
            DeviceCommunicationProtocol = "";
            DeviceModelNum = 0;
            DeviceFamilyNum = 0;
            DeviceCommunicationProtocolNum = 0;
        }
    }
}

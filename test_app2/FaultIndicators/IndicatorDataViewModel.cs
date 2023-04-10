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

        private Dictionary<string, int> _namingProtocol = new()
        {
            { "XY-Old", 85 },
            { "XY-New", 165 },
            { "LB-FF", 165 }
        };

        private Dictionary<string, int> _namingFamily = new()
        {
            { "JYZ-FF", 90 },
            { "JYZ-HW", 90 },
            { "JYZ-HW V2.0", 90 }
        };

        private string[] _patterns =
        {
            "E5 E5",        //Подтверждение от RF
            "11 84",        //Кол-во индикаторов
            "18 44",        //MAC адреса индикаторов
            "2F 45"
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
        //TODO: Разобраться что снизу
        private int _comFrequency;
        private int _callLevel;
        /////////////////////////////
        private int _trailer;
        private static int _indicatorsAmount;
        private int _functionCallNumStart;
        private int _functionCallNumEnd;
        //Всё что ниже - относится к индикатору
        private string _mac;
        private string _macShortened;

        private bool _buttonCheckAnyIndicators = false;

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
        public string MACShoertened
        {
            get => _macShortened;
            set => RaisePropertyChanged(ref _macShortened, value);
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
        public bool ButtonCheckAnyIndicators
        {
            get => _buttonCheckAnyIndicators;
            set => RaisePropertyChanged(ref _buttonCheckAnyIndicators, value);
        }
        public int FunctionCallNumStart
        {
            get => _functionCallNumStart;
            set => RaisePropertyChanged(ref _functionCallNumStart, value);
        }
        public int FunctionCallNumEnd
        {
            get => _functionCallNumEnd;
            set => RaisePropertyChanged(ref _functionCallNumEnd, value);
        }


        public static CancellationTokenSource cancelSource = new CancellationTokenSource();

        public Command UpdateModelContentCommand { get; }

        public Command CheckAvailibleIndicatorsCommand { get; }

        public Command UpdateFamilyContentCommand { get; }

        public Command UpdateCommunicationProtocolCommand { get; }

        public Command ClearAllContentCommand { get; }

        public Command ReadIndicatorsBaseParametersCommand { get; }

        public SerialPortMessagesViewModel Messages { get; set; }

        public SerialPortMessagesReceive Receiver { get; set; }

        public SerialPortMessagesSend Sender { get; set; }

        public ObservableCollection<FaultIndicatorViewModel> Indicators { get; set; }

        public string IndicatorConfirm { get; set; }
        
        public Checksum checksum { get; set; }

        public IndicatorDataViewModel()
        {
            Trailer = 22;
            CallTime = 10;
            WaitTime = 30;
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
            CheckAvailibleIndicatorsCommand = new Command(CheckAvailibleIndicators);
            ReadIndicatorsBaseParametersCommand = new Command(ReadIndicatorsBaseParameters);

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
                    return;
                }
                ButtonCheckAnyIndicators = true;
                string[] msg;
                IndicatorsAmount--;
                msg = IndicatorConfirm.Split(' ');

                MACShoertened = $"{msg[7]}-{msg[6]}-{msg[5]}-{msg[4]}";
                MAC = $"{msg[9]}-{msg[8]}-{msg[7]}-{msg[6]}-{msg[5]}-{msg[4]}";
                
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    if (Indicators.Any(x => x.MACAdress == MAC))
                    {
                        Messages.AddMessage("Индикатор уже есть в списке проверяемых");
                        return;
                    }
                    Indicators.Add(new FaultIndicatorViewModel { MACAdress = MAC, MACAdressShow = MACShoertened });
                });
            }
            if (IndicatorConfirm.Contains(_patterns[3]))
            {
                if (Indicators.Count == 0)
                {
                    Messages.AddMessage("НЕ МОЖЕТ БЫТЬ!");
                    return;
                }
                string[] msg;
                msg = IndicatorConfirm.Split(' ');
            }
            IndicatorConfirm = "";
            //Thread.Sleep(5);
        }
        //Dispatcher.Thread.Interrupt();

        public void CheckAvailibleIndicators()
        {
            ushort crc;
            int oldFunc;

            oldFunc = FunctionCallNumStart;
            FunctionCallNumStart = 1;
            FunctionCallNumEnd = 0;

            if (!Sender.Port.IsOpen)
            {
                Messages.AddMessage("Порт не открыт, не удалось отправить сообщение");
                return;
            }
            if (DeviceCommunicationProtocolNum == 0 && DeviceFamilyNum == 0)
            {
                Messages.AddMessage("Для ИКЗ не выбран протокол общения или семейство устройств");
                return;
            }

            Request = 
                new string[] { $"{_namingProtocol.ElementAt(DeviceCommunicationProtocolNum - 1).Value:X2}", 
                    $"{_namingFamily.ElementAt(DeviceFamilyNum - 1).Value:X2}", 
                    "15", 
                    "24", 
                    $"{CallTime:X2}", 
                    $"{WaitTime:X2}", 
                    "00", 
                    $"{CallFrequency:X2}", 
                    "00", 
                    "00", 
                    "00", 
                    "00", 
                    "00", 
                    "00", 
                    $"{CallAdress.ToString("X2")}", 
                    "00", 
                    $"{FunctionCallNumStart:X2}", 
                    $"{FunctionCallNumEnd:X2}", 
                    "00", 
                    "00", 
                    $"{Trailer:X2}" };

            crc = checksum.CheckSum_CRC(Request, 2, Request.Length);
            Request[^2] = string.Join("", (crc & 255).ToString("X2"));
            Request[^3] = string.Join("", (crc >> 8).ToString("X2"));

            Sender.SendHEXMessage(string.Join(" ", Request));
            Messages.AddSentMessage(string.Join(" ", Request));

            FunctionCallNumStart = oldFunc;
        }

        public void ReadIndicatorsBaseParameters()
        {
            ushort crc, orderNumber;

            //_functionCallNumStart = 2;
            //_functionCallNumEnd = 0;
            orderNumber = 5;

            if (!Sender.Port.IsOpen)
            {
                Messages.AddMessage("Порт не открыт, не удалось отправить сообщение");
                return;
            }
            if (DeviceCommunicationProtocolNum == 0 && DeviceFamilyNum == 0)
            {
                Messages.AddMessage("Для ИКЗ не выбран протокол общения или семейство устройств");
                return;
            }
            //TODO: вынести в отдельный поток
            foreach(var indicator in Indicators)
            {
                //MAC = indicator.MACAdress;

                var splittedMac = indicator.MACAdress.Split('-');
                //MAC = $"{msg[7]}-{msg[6]}-{msg[5]}-{msg[4]}";
                Request =
                new string[] { $"{_namingProtocol.ElementAt(DeviceCommunicationProtocolNum - 1).Value:X2}", 
                    $"{_namingFamily.ElementAt(DeviceFamilyNum - 1).Value:X2}", 
                    "15", 
                    "25", 
                    $"{CallTime:X2}", 
                    $"{WaitTime:X2}", 
                    "00", 
                    $"{CallFrequency:X2}", 
                    "00",
                    $"{splittedMac[5]:X2}",
                    $"{splittedMac[4]:X2}",
                    $"{splittedMac[3]:X2}",
                    $"{splittedMac[2]:X2}", 
                    $"{splittedMac[1]:X2}", 
                    $"{splittedMac[0]:X2}", 
                    "00", 
                    $"{_functionCallNumStart:X2}", 
                    $"{_functionCallNumEnd:X2}", 
                    "00", 
                    "00", 
                    $"{Trailer:X2}" };

                crc = checksum.CheckSum_CRC(Request, 2, Request.Length);
                Request[^2] = string.Join("", (crc & 255).ToString("X2"));
                Request[^3] = string.Join("", (crc >> 8).ToString("X2"));
                orderNumber++;

                Sender.SendHEXMessage(string.Join(" ", Request));
                Messages.AddSentMessage(string.Join(" ", Request));

                Thread.Sleep(200);
            }
        }

        public void UpdateModelContent()
        {
            //TODO: Исправить баг с выбором при отсчёте с 0
            DeviceModel = _namingModel[DeviceModelNum - 1];
        }

        public void UpdateFamilyContent()
        {
            //TODO: Исправить баг с выбором при отсчёте с 0
            DeviceFamily = _namingFamily.ElementAt(DeviceFamilyNum - 1).Key;
        }

        public void UpdateCommunicationContent()
        {
            //TODO: Исправить баг с выбором при отсчёте с 0
            DeviceCommunicationProtocol = _namingProtocol.ElementAt(DeviceCommunicationProtocolNum - 1).Key;
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

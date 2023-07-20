using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using test_app2.SerialPMessages;
using test_app2.SerialPortDevice;
using test_app2.Utilities;
using test_app2.ViewModels;
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
            //{ "JYZ-FF", 90 },
            //{ "JYZ-HW", 90 },
            { "ИКЗ с механической индикацией", 90 },
            { "ИКЗ с LED индикацией", 90 },
            { "ИКЗ с LED индикацией (7 цветов)", 90 }
        };

        private string[] _patterns =
        {
            "E5 E5",        //Подтверждение от RF
            "11 84",        //Кол-во индикаторов
            "18 44",        //MAC адреса индикаторов
            "2F 45",        //Чтение данных
            "11 C7",        //action
            "11 C8",        //return
            "11 CA",        //short circuit, ground
            "11 CB",        //Рестарт
            "22 43"         //Телеметрия
        };

        private int[] _blackListIndesxes =
        {
            0, 2
        };

        private int _deviceModelNum;
        private int _deviceFamilyNum;
        private int _deviceCommunicationProtocolNum;
        private string _deviceModel;
        private string _deviceFamily;
        private string _deviceCommunicationProtocol;
        private string[] _request;
        //private byte[] _requestTest;
        private int _callAdress;
        private int _callFrequency;
        private int _callTime;
        private int _waitTime;
        //TODO: Разобраться в параметрах снизу
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
        private int _ledControl;
        private int _ledCommand;
        private bool _visibleBoxes;
        private string _visibility;

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
        public int LEDControl
        {
            get => _ledControl;
            set => RaisePropertyChanged(ref _ledControl, value);
        }
        public int LEDCommand
        {
            get => _ledCommand;
            set => RaisePropertyChanged(ref _ledCommand, value);
        }
        /*public byte[] RequestTest
        {
            get => _requestTest;
            set => RaisePropertyChanged(ref _requestTest, value);
        }*/
        public bool ButtonCheckAnyIndicators
        {
            get => _buttonCheckAnyIndicators;
            set => RaisePropertyChanged(ref _buttonCheckAnyIndicators, value);
        }
        public int FunctionCallNumStart
        {
            get => _functionCallNumStart;
            set
            {
                RaisePropertyChanged(ref _functionCallNumStart, value);
                VisibleBoxes = !Convert.ToBoolean(value);
                if (!VisibleBoxes)
                {
                    Visibility = "Hidden";
                }
                else
                {
                    Visibility = "Visible";
                }
            }
        }
        public int FunctionCallNumEnd
        {
            get => _functionCallNumEnd;
            set => RaisePropertyChanged(ref _functionCallNumEnd, value); 
        }

        public bool VisibleBoxes
        {
            get => _visibleBoxes;
            set => RaisePropertyChanged(ref _visibleBoxes, value);
        }

        public string Visibility
        {
            get => _visibility;
            set => RaisePropertyChanged(ref _visibility, value);
        }



        public static CancellationTokenSource cancelSource = new CancellationTokenSource();

        public Command UpdateModelContentCommand { get; }

        public Command CheckAvailibleIndicatorsCommand { get; }

        public Command UpdateFamilyContentCommand { get; }

        public Command UpdateCommunicationProtocolCommand { get; }

        public Command ClearAllContentCommand { get; }

        public Command ReadIndicatorsBaseParametersCommand { get; }

        public Command WriteIndicatorsBaseParametersCommand { get; }

        public Command SoftwareVersionParameterCommand { get; }

        public Command ControlActionParameterCommand { get; }

        public Command ControlReturnParameterCommand { get; }

        public Command ShortCircuitCheckCommand { get; }

        public Command GroundShortCircuitCheckCommand { get; }

        public Command RestartCommand { get; }

        public Command LEDConfCommand { get; }

        public Command ClearDataGridContentCommand { get; }

        public Command ReadTelemetryCommand { get; }

        public SerialPortMessagesViewModel Messages { get; set; }

        public SerialPortMessagesReceive Receiver { get; set; }

        public SerialPortMessagesSend Sender { get; set; }

        public SerialPortViewModel SerialPort { get; set; }

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
            ClearDataGridContentCommand = new Command(ClearDataGridContent);
            ClearAllContentCommand = new Command(ClearAllContent);
            UpdateCommunicationProtocolCommand = new Command(UpdateCommunicationContent);
            CheckAvailibleIndicatorsCommand = new Command(CheckAvailibleIndicators);
            ReadIndicatorsBaseParametersCommand = new Command(ReadIndicatorsBaseParameters);
            WriteIndicatorsBaseParametersCommand = new Command(WriteIndicatorParameters);
            SoftwareVersionParameterCommand = new Command(SoftwareVersionParameter);
            ControlActionParameterCommand = new Command(ControlActionParameter);
            ControlReturnParameterCommand = new Command(ControlReturnParameter);
            ShortCircuitCheckCommand = new Command(ShortCircuitCheck);
            GroundShortCircuitCheckCommand = new Command(GroundShortCircuitCheck);
            ReadTelemetryCommand = new Command(ReadTelemetry);
            RestartCommand = new Command(Restart);
            LEDConfCommand = new Command(LEDConf);

            DeviceCommunicationProtocolNum = 2;
            DeviceModel = _namingModel[DeviceModelNum];
            DeviceFamily = _namingFamily.ElementAt(DeviceFamilyNum).Key;
            DeviceCommunicationProtocol = _namingProtocol.ElementAt(DeviceCommunicationProtocolNum - 1).Key;

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
            if (_blackListIndesxes.Contains(DeviceFamilyNum))
            {
                Messages.AddMessage("Этот тип индикаторов пока нельзя использовать");
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
                /*if (IndicatorsAmount == 0)
                {
                    Messages.AddMessage("НЕ МОЖЕТ БЫТЬ!");
                    return;
                }*/
                ButtonCheckAnyIndicators = true;
                string[] msg;
                //IndicatorsAmount--;
                msg = IndicatorConfirm.Split(' ');

                MACShoertened = $"{msg[7]}-{msg[6]}-{msg[5]}-{msg[4]}";
                MAC = $"{msg[10]}-{msg[9]}-{msg[8]}-{msg[7]}-{msg[6]}-{msg[5]}-{msg[4]}";

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
                /*if (Indicators.Count != 0)
                {
                    Messages.AddMessage("НЕ МОЖЕТ БЫТЬ!");
                    return;
                }*/
                string[] msg;
                msg = IndicatorConfirm.Split(' ');

                MAC = $"{msg[10]}-{msg[9]}-{msg[8]}-{msg[7]}-{msg[6]}-{msg[5]}-{msg[4]}";
                var CollectionIndex = Indicators.IndexOf(Indicators.FirstOrDefault(x => x.MACAdress == MAC));

                if (CollectionIndex == null)
                {
                    Messages.AddMessage("НЕ МОЖЕТ БЫТЬ!");
                    return;
                }
                else
                {
                    switch (Convert.ToInt16(msg[11]))
                    {
                        //run
                        case 2:
                            Indicators[CollectionIndex].FieldThreshold = Convert.ToInt16(msg[16], 16) + (Convert.ToInt16(msg[17], 16) << 8);
                            Indicators[CollectionIndex].CurrentThreshold = Convert.ToInt16(msg[18], 16) / 10;
                            Indicators[CollectionIndex].UploadT1 = Convert.ToInt16(msg[23], 16) + (Convert.ToInt16(msg[24], 16) << 8);
                            Indicators[CollectionIndex].UploadT2 = Convert.ToInt16(msg[25], 16) + (Convert.ToInt16(msg[26], 16) << 8) +
                                                                  (Convert.ToInt16(msg[27], 16) << 16) + (Convert.ToInt16(msg[28], 16) << 24);
                            Indicators[CollectionIndex].HeartBeat = Convert.ToInt16(msg[29], 16) + (Convert.ToInt16(msg[30], 16) << 8);
                            Indicators[CollectionIndex].CurrentIValue = Convert.ToInt16(msg[31], 16) / 10;
                            Indicators[CollectionIndex].CurrentRelativeValue = Convert.ToInt16(msg[32], 16);
                            Indicators[CollectionIndex].CurrentAbsoluteValue = (Convert.ToInt16(msg[33], 16) + (Convert.ToInt16(msg[34], 16) << 8)) / 10;
                            break;
                        //current
                        case 3:
                            Indicators[CollectionIndex].TimeDelay = Convert.ToInt16(msg[14], 16) + (Convert.ToInt16(msg[15], 16) << 8) +
                                                                    (Convert.ToInt16(msg[16], 16) << 16) + (Convert.ToInt16(msg[17], 16) << 24);
                            Indicators[CollectionIndex].RepowerDelay = Convert.ToInt16(msg[18], 16) + (Convert.ToInt16(msg[19], 16) << 8);
                            Indicators[CollectionIndex].ShortCurrent = (Convert.ToInt16(msg[20], 16) + (Convert.ToInt16(msg[21], 16) << 8)) / 10;
                            break;
                        //ground
                        case 4:
                            Indicators[CollectionIndex].TransientFieldDrop = Convert.ToInt16(msg[26], 16);
                            Indicators[CollectionIndex].FieldDropDelay = Convert.ToInt16(msg[27], 16) + (Convert.ToInt16(msg[28], 16) << 8);
                            break;
                        //software
                        case 5:
                            Indicators[CollectionIndex].Information = "";
                            for (int i = 13; i < 43; ++i)
                            {
                                if (msg[i] == "00")
                                {
                                    break;
                                }
                                Indicators[CollectionIndex].Information += (char)Int16.Parse(msg[i], NumberStyles.AllowHexSpecifier);
                            }
                            break;
                    }
                }
            }
            if (IndicatorConfirm.Contains(_patterns[4]) ||
               IndicatorConfirm.Contains(_patterns[5]) ||
               IndicatorConfirm.Contains(_patterns[6]) ||
               IndicatorConfirm.Contains(_patterns[7]))
            {
                Dictionary<string, string> CommandType = new()
                {
            //{ "JYZ-FF", 90 },
            //{ "JYZ-HW", 90 },
                    { "C7", "Откидная пластина в порядке - " },
                    { "C8", "Откидная задняя пластина в порядке - " },
                    { "CA", "Проверка на " },
                    { "CB", "Перезапуск индикаторов - " },
                };
                string[] msg;
                msg = IndicatorConfirm.Split(' ');

                MAC = $"{msg[10]}-{msg[9]}-{msg[8]}-{msg[7]}-{msg[6]}-{msg[5]}-{msg[4]}";
                var CollectionIndex = Indicators.IndexOf(Indicators.FirstOrDefault(x => x.MACAdress == MAC));

                if (CollectionIndex == null)
                {
                    Messages.AddMessage("НЕ МОЖЕТ БЫТЬ!");
                    return;
                }
                Indicators[CollectionIndex].Information = CommandType[msg[3]];
                if (msg[13] != "01" && (msg[3] == "C7" || msg[3] == "C8"))
                {
                    if (msg[3] == "C7")
                    {
                        Indicators[CollectionIndex].Information = "Включение";
                    }
                    if (msg[3] == "C8")
                    {
                        Indicators[CollectionIndex].Information = "Отключение";
                    }
                    if (msg[13] == "00") Indicators[CollectionIndex].Information += " красного индикатора - ";
                    if (msg[13] == "02") Indicators[CollectionIndex].Information += " зелёного индикатора - ";
                    if (msg[13] == "03") Indicators[CollectionIndex].Information += " желтого индикатора - ";
                }
                if (msg[12] == "01" && msg[3] == "CA")
                {
                    Indicators[CollectionIndex].Information += "межфазное замыкание - ";
                }
                else if (msg[3] == "CA")
                {
                    Indicators[CollectionIndex].Information += "замыкание на землю - ";
                }
                switch (Convert.ToInt16(msg[11], 16))
                {
                    case 229:
                        Indicators[CollectionIndex].Information += "успешно";
                        break;
                    default:
                        Indicators[CollectionIndex].Information += "неудачно";
                        break;
                }
            }
            if (IndicatorConfirm.Contains(_patterns[8]))
            {
                string[] msg;
                //IndicatorsAmount--;
                msg = IndicatorConfirm.Split(' ');

                MAC = $"{msg[10]}-{msg[9]}-{msg[8]}-{msg[7]}-{msg[6]}-{msg[5]}-{msg[4]}";
                var CollectionIndex = Indicators.IndexOf(Indicators.FirstOrDefault(x => x.MACAdress == MAC));

                if (CollectionIndex == null)
                {
                    Messages.AddMessage("НЕ МОЖЕТ БЫТЬ!");
                    return;
                }
                if (CollectionIndex == -1)
                {
                    Messages.AddMessage("Баганный индикатор которого нет в списке, по нему пришла телеметрия");
                    return;
                }

                Indicators[CollectionIndex].TRoute = (Convert.ToInt16(msg[8], 16) >> 2);
                Indicators[CollectionIndex].TPhase = (char)(Convert.ToInt16(msg[8], 16) & 3);
                Indicators[CollectionIndex].TGroupAddr = Convert.ToInt16(msg[10], 16) << 8 + Convert.ToInt16(msg[9], 16);
                Indicators[CollectionIndex].TElectricalFieldValue = Convert.ToInt16(msg[15], 16) << 8 + Convert.ToInt16(msg[14]);
                Indicators[CollectionIndex].TCurrentValue = (Convert.ToInt16(msg[17], 16) << 8 + Convert.ToInt16(msg[16], 16)) / 10;
                Indicators[CollectionIndex].TCurrentSettingOverflowValue = (Convert.ToInt16(msg[19], 16) << 8 + Convert.ToInt16(msg[18], 16)) / 10;
                Indicators[CollectionIndex].TBatteryVoltage = (Convert.ToInt16(msg[21], 16) << 8 + Convert.ToInt16(msg[20], 16)) / 1000;
                Indicators[CollectionIndex].TransientFieldDrop = Convert.ToInt16(msg[27], 16);
                Indicators[CollectionIndex].TTemprature = (int)((Convert.ToInt16(msg[23], 16) << 8 + Convert.ToInt16(msg[22], 16)));
                Indicators[CollectionIndex].TQuickShortCircuit = Convert.ToBoolean(((Convert.ToInt16(msg[26], 16) << 8 + Convert.ToInt16(msg[25], 16)) >> 7) & 1);
                Indicators[CollectionIndex].TPermanentShortCircuit = Convert.ToBoolean(((Convert.ToInt16(msg[26], 16) << 8 + Convert.ToInt16(msg[25], 16)) >> 8) & 1);
                Indicators[CollectionIndex].TOverCurrent = Convert.ToBoolean(((Convert.ToInt16(msg[26], 16) << 8 + Convert.ToInt16(msg[25], 16)) >> 1) & 1);
                Indicators[CollectionIndex].TGroundShortCircuit = Convert.ToBoolean(((Convert.ToInt16(msg[26], 16) << 8 + Convert.ToInt16(msg[25], 16)) >> 3) & 1);
                Indicators[CollectionIndex].TLowBatteryValue = Convert.ToBoolean(((Convert.ToInt16(msg[26], 16) << 8 + Convert.ToInt16(msg[25], 16)) >> 4) & 1);
                Indicators[CollectionIndex].TLEDIndicationStatus = Convert.ToBoolean(Convert.ToInt16(msg[28], 16));
                Indicators[CollectionIndex].Information = "рыба";
                Indicators[CollectionIndex].TIndicatorProtectionState = "рыба2";
            }
            IndicatorConfirm = "";
            //Thread.Sleep(100);
        }
        //Dispatcher.Thread.Interrupt();

        public void CheckAvailibleIndicators()
        {
            ushort crc;
            int oldFunc;

            oldFunc = FunctionCallNumStart;
            //FunctionCallNumStart = 1;
            FunctionCallNumEnd = 0;
            if (_blackListIndesxes.Contains(DeviceFamilyNum))
            {
                Messages.AddMessage("Этот тип индикаторов пока нельзя использовать");
                return;
            }
            if (!Sender.Port.IsOpen)
            {
                Messages.AddMessage("Порт не открыт, не удалось отправить сообщение");
                return;
            }
            Request =
                new string[] { $"{_namingProtocol.ElementAt(DeviceCommunicationProtocolNum).Value:X2}",
                    $"{_namingFamily.ElementAt(DeviceFamilyNum).Value:X2}",
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
                    $"{CallAdress:X2}",
                    "00",
                    $"{1:X2}",
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
            if (_blackListIndesxes.Contains(DeviceFamilyNum))
            {
                Messages.AddMessage("Этот тип индикаторов пока нельзя использовать");
                return;
            }
            if (!Sender.Port.IsOpen)
            {
                Messages.AddMessage("Порт не открыт, не удалось отправить сообщение");
                return;
            }
            if (Indicators.Count < 1)
            {
                Messages.AddMessage("Информация не считана, ни одного индикатора КЗ нет в списке");
                return;
            }
            //TODO: вынести в отдельный поток
            new Thread(() =>
            {
                foreach (var indicator in Indicators)
                {
                    //MAC = indicator.MACAdress;

                    var splittedMac = indicator.MACAdress.Split('-');
                    //MAC = $"{msg[7]}-{msg[6]}-{msg[5]}-{msg[4]}";
                    Request =
                    new string[] { $"{_namingProtocol.ElementAt(DeviceCommunicationProtocolNum).Value:X2}",
                    $"{_namingFamily.ElementAt(DeviceFamilyNum).Value:X2}",
                    "15",
                    "25",
                    $"{CallTime:X2}",
                    $"{WaitTime:X2}",
                    "00",
                    $"{CallFrequency:X2}",
                    "00",
                    $"{splittedMac[6]:X2}",
                    $"{splittedMac[5]:X2}",
                    $"{splittedMac[4]:X2}",
                    $"{splittedMac[3]:X2}",
                    $"{splittedMac[2]:X2}",
                    $"{CallAdress:X2}",
                    $"{0:X2}",
                    $"{(FunctionCallNumStart + 1):X2}",
                    $"{FunctionCallNumEnd:X2}",
                    "00",
                    "00",
                    $"{Trailer:X2}" };

                    crc = checksum.CheckSum_CRC(Request, 2, Request.Length);
                    Request[^2] = string.Join("", (crc & 255).ToString("X2"));
                    Request[^3] = string.Join("", (crc >> 8).ToString("X2"));
                    //orderNumber++;

                    Sender.SendHEXMessage(string.Join(" ", Request));
                    Messages.AddSentMessage(string.Join(" ", Request));

                    Thread.Sleep(500);
                }
            }).Start();
        }

        public void WriteIndicatorParameters()
        {
            ushort crc;
            int funcInMemory, subNumber;

            funcInMemory = FunctionCallNumStart + 1;
            //FunctionCallNumStart = 1;
            //FunctionCallNumEnd = 0;
            if (_blackListIndesxes.Contains(DeviceFamilyNum))
            {
                Messages.AddMessage("Этот тип индикаторов пока нельзя использовать");
                return;
            }
            if (!Sender.Port.IsOpen)
            {
                Messages.AddMessage("Порт не открыт, не удалось отправить сообщение");
                return;
            }
            if (Indicators.Count < 1)
            {
                Messages.AddMessage("Информация не считана, ни одного индикатора КЗ нет в списке");
                return;
            }
            //TODO: вынести в отдельный поток
            foreach (var indicator in Indicators)
            {
                //MAC = indicator.MACAdress;

                var splittedMac = indicator.MACAdress.Split('-');
                //MAC = $"{msg[7]}-{msg[6]}-{msg[5]}-{msg[4]}";
                Request =
                new string[] { $"{_namingProtocol.ElementAt(DeviceCommunicationProtocolNum).Value:X2}",
                    $"{_namingFamily.ElementAt(DeviceFamilyNum).Value:X2}",
                    "34",
                    "26",
                    $"{CallTime:X2}",
                    $"{WaitTime:X2}",
                    "00",
                    $"{CallFrequency:X2}",
                    "00",
                    $"{splittedMac[6]:X2}",
                    $"{splittedMac[5]:X2}",
                    $"{splittedMac[4]:X2}",
                    $"{splittedMac[3]:X2}",
                    $"{splittedMac[2]:X2}",
                    $"{splittedMac[1]:X2}",
                    $"{splittedMac[0]:X2}",
                    $"{funcInMemory:X2}",
                    $"{FunctionCallNumEnd:X2}",
                    "00", "00", "00", "00",
                    "00", "00", "00", "00",
                    "00", "00", "00", "00",
                    "00", "00", "00", "00",
                    "00", "00", "00", "00",
                    "00", "00", "00", "00",
                    "00", "00", "00", "00",
                    "00", "00", "00", "00",
                    "00",
                    $"{Trailer:X2}" };

                switch (funcInMemory)
                {
                    case 2:
                        Request[18] = "18";
                        Request[19] = "F0";
                        Request[20] = "0A";
                        Request[21] = $"{indicator.FieldThreshold & 255:X2}";
                        Request[22] = $"{(indicator.FieldThreshold >> 8) & 255:X2}";
                        Request[23] = $"{(indicator.CurrentThreshold * 10) & 255:X2}";
                        Request[24] = "8C";
                        Request[25] = "00";
                        Request[26] = "06";
                        Request[27] = "19";
                        Request[28] = $"{indicator.UploadT1 & 255:X2}";
                        Request[29] = $"{(indicator.UploadT1 >> 8) & 255:X2}";
                        Request[30] = $"{indicator.UploadT2 & 255:X2}";
                        Request[31] = $"{(indicator.UploadT2 >> 8) & 255:X2}";
                        Request[32] = $"{(indicator.UploadT2 >> 16) & 255:X2}";
                        Request[33] = $"{(indicator.UploadT2 >> 24) & 255:X2}";
                        Request[34] = $"{indicator.HeartBeat & 255:X2}";
                        Request[35] = $"{(indicator.HeartBeat >> 8) & 255:X2}";
                        Request[36] = $"{(indicator.CurrentIValue * 10) & 255:X2}";
                        Request[37] = $"{indicator.CurrentRelativeValue & 255:X2}";
                        Request[38] = $"{(indicator.CurrentAbsoluteValue * 10) & 255:X2}";
                        Request[39] = $"{((indicator.CurrentAbsoluteValue * 10) >> 8) & 255:X2}";
                        Request[40] = "05";
                        Request[41] = "02";
                        Request[42] = "02";
                        Request[43] = "00";
                        break;
                    case 3:
                        Request[18] = "15";
                        Request[19] = $"{indicator.TimeDelay & 255:X2}";
                        Request[20] = $"{(indicator.TimeDelay >> 8) & 255:X2}";
                        Request[21] = $"{(indicator.TimeDelay >> 16) & 255:X2}";
                        Request[22] = $"{(indicator.TimeDelay >> 24) & 255:X2}";
                        Request[23] = $"{indicator.RepowerDelay & 255:X2}";
                        Request[24] = $"{(indicator.RepowerDelay >> 8) & 255:X2}";
                        Request[25] = $"{(indicator.ShortCurrent * 10) & 255:X2}";
                        Request[26] = $"{((indicator.ShortCurrent * 10) >> 8) & 255:X2}";
                        Request[27] = "00";
                        Request[28] = "00";
                        Request[29] = "64";
                        Request[30] = "00";
                        Request[31] = "0A";
                        Request[32] = "00";
                        Request[33] = "DC";
                        Request[34] = "05";
                        Request[35] = "96";
                        Request[36] = "00";
                        Request[37] = "01";
                        Request[38] = "00";
                        Request[39] = "08";
                        Request[40] = "00";
                        break;
                    case 4:
                        Request[18] = "0F";
                        Request[19] = "00";
                        Request[20] = "00";
                        Request[21] = "00";
                        Request[22] = "00";
                        Request[23] = "00";
                        Request[24] = "00";
                        Request[25] = "00";
                        Request[26] = "00";
                        Request[27] = "00";
                        Request[28] = "00";
                        Request[29] = "1E";
                        Request[30] = "00";
                        Request[31] = $"{indicator.TransientFieldDrop & 255:X2}";
                        Request[32] = $"{indicator.FieldDropDelay & 255:X2}";
                        Request[33] = $"{(indicator.FieldDropDelay >> 8) & 255:X2}";
                        break;
                }

                crc = checksum.CheckSum_CRC(Request, 2, Request.Length);
                Request[^2] = string.Join("", (crc & 255).ToString("X2"));
                Request[^3] = string.Join("", (crc >> 8).ToString("X2"));
                //orderNumber++;

                Sender.SendHEXMessage(string.Join(" ", Request));
                Messages.AddSentMessage(string.Join(" ", Request));

                Thread.Sleep(200);
            }

        }

        public void Restart()
        {
            if (_blackListIndesxes.Contains(DeviceFamilyNum))
            {
                Messages.AddMessage("Этот тип индикаторов пока нельзя использовать");
                return;
            }
            if (!Sender.Port.IsOpen)
            {
                Messages.AddMessage("Порт не открыт, не удалось отправить сообщение");
                return;
            }
            if (Indicators.Count < 1)
            {
                Messages.AddMessage("Информация не считана, ни одного индикатора КЗ нет в списке");
                return;
            }
            foreach (var indicator in Indicators)
            {
                //MAC = indicator.MACAdress;
                int crc;
                var splittedMac = indicator.MACAdress.Split('-');

                Request =
                    new string[] { $"{_namingProtocol.ElementAt(DeviceCommunicationProtocolNum).Value:X2}",
                    $"{_namingFamily.ElementAt(DeviceFamilyNum).Value:X2}",
                    "15",
                    "2B",                   //restart action command
                    $"{CallTime:X2}",
                    $"{WaitTime:X2}",
                    "00",
                    $"{CallFrequency:X2}",
                    "00",
                    $"{splittedMac[6]:X2}",
                    $"{splittedMac[5]:X2}",
                    $"{splittedMac[4]:X2}",
                    $"{splittedMac[3]:X2}",
                    $"{splittedMac[2]:X2}",
                    $"{splittedMac[1]:X2}",
                    $"{splittedMac[0]:X2}",
                    $"00",
                    $"{FunctionCallNumEnd:X2}",
                    "00",
                    "00",
                    $"{Trailer:X2}" };

                crc = checksum.CheckSum_CRC(Request, 2, Request.Length);
                Request[^2] = string.Join("", (crc & 255).ToString("X2"));
                Request[^3] = string.Join("", (crc >> 8).ToString("X2"));
                //orderNumber++;

                Sender.SendHEXMessage(string.Join(" ", Request));
                Messages.AddSentMessage(string.Join(" ", Request));

                Thread.Sleep(200);
            }
        }

        public void ReadTelemetry()
        {
            if (_blackListIndesxes.Contains(DeviceFamilyNum))
            {
                Messages.AddMessage("Этот тип индикаторов пока нельзя использовать");
                return;
            }
            if (!Sender.Port.IsOpen)
            {
                Messages.AddMessage("Порт не открыт, не удалось отправить сообщение");
                return;
            }
            if (Indicators.Count < 1)
            {
                Messages.AddMessage("Информация не считана, ни одного индикатора КЗ нет в списке");
                return;
            }
            new Thread(() =>
            {
                foreach (var indicator in Indicators)
                {
                    //MAC = indicator.MACAdress;
                    int crc;
                    var splittedMac = indicator.MACAdress.Split('-');

                    Request =
                        new string[] { $"{_namingProtocol.ElementAt(DeviceCommunicationProtocolNum).Value:X2}",
                                $"{_namingFamily.ElementAt(DeviceFamilyNum).Value:X2}",
                                "15",
                                "23",                   //telemetry read command
                                $"{CallTime:X2}",
                                $"{0:X2}",
                                "00",
                                $"{CallFrequency:X2}",
                                "00",
                                $"{splittedMac[6]:X2}",
                                $"{splittedMac[5]:X2}",
                                $"{splittedMac[4]:X2}",
                                $"{splittedMac[3]:X2}",
                                $"{splittedMac[2]:X2}",
                                $"{splittedMac[1]:X2}",
                                $"{splittedMac[0]:X2}",
                                $"01",
                                $"{FunctionCallNumEnd:X2}",
                                "00",
                                "00",
                                $"{Trailer:X2}"
                        };
                    crc = checksum.CheckSum_CRC(Request, 2, Request.Length);
                    Request[^2] = string.Join("", (crc & 255).ToString("X2"));
                    Request[^3] = string.Join("", (crc >> 8).ToString("X2"));
                    //orderNumber++;

                    Sender.SendHEXMessage(string.Join(" ", Request));
                    Messages.AddSentMessage(string.Join(" ", Request));


                    Thread.Sleep(500);
                }
            }).Start();
        }

        public void LEDOff()
        {
            if (_blackListIndesxes.Contains(DeviceFamilyNum))
            {
                Messages.AddMessage("Этот тип индикаторов пока нельзя использовать");
                return;
            }
            if (!Sender.Port.IsOpen)
            {
                Messages.AddMessage("Порт не открыт, не удалось отправить сообщение");
                return;
            }
            if (Indicators.Count < 1)
            {
                Messages.AddMessage("Информация не считана, ни одного индикатора КЗ нет в списке");
                return;
            }
            foreach (var indicator in Indicators)
            {
                //MAC = indicator.MACAdress;
                int crc;
                var splittedMac = indicator.MACAdress.Split('-');

                Request =
                    new string[] { $"{_namingProtocol.ElementAt(DeviceCommunicationProtocolNum).Value:X2}",
                    $"{_namingFamily.ElementAt(DeviceFamilyNum).Value:X2}",
                    "15",
                    "28",                   //control action command
                    $"{CallTime:X2}",
                    $"{WaitTime:X2}",
                    "00",
                    $"{CallFrequency:X2}",
                    "00",
                    $"{splittedMac[6]:X2}",
                    $"{splittedMac[5]:X2}",
                    $"{splittedMac[4]:X2}",
                    $"{splittedMac[3]:X2}",
                    $"{splittedMac[2]:X2}",
                    $"{splittedMac[1]:X2}",
                    $"{splittedMac[0]:X2}",
                    $"{CallAdress:X2}",   //выбор цвета led индикатора
                    $"{LEDControl:X2}",
                    "00",
                    "00",
                    $"{Trailer:X2}" };

                crc = checksum.CheckSum_CRC(Request, 2, Request.Length);
                Request[^2] = string.Join("", (crc & 255).ToString("X2"));
                Request[^3] = string.Join("", (crc >> 8).ToString("X2"));
                //orderNumber++;

                Sender.SendHEXMessage(string.Join(" ", Request));
                Messages.AddSentMessage(string.Join(" ", Request));

                Thread.Sleep(200);
            }
        }

        public void LEDOn()
        {
            if (_blackListIndesxes.Contains(DeviceFamilyNum))
            {
                Messages.AddMessage("Этот тип индикаторов пока нельзя использовать");
                return;
            }
            if (!Sender.Port.IsOpen)
            {
                Messages.AddMessage("Порт не открыт, не удалось отправить сообщение");
                return;
            }
            if (Indicators.Count < 1)
            {
                Messages.AddMessage("Информация не считана, ни одного индикатора КЗ нет в списке");
                return;
            }
            foreach (var indicator in Indicators)
            {
                //MAC = indicator.MACAdress;
                int crc;
                var splittedMac = indicator.MACAdress.Split('-');

                Request =
                    new string[] { $"{_namingProtocol.ElementAt(DeviceCommunicationProtocolNum).Value:X2}",
                    $"{_namingFamily.ElementAt(DeviceFamilyNum).Value:X2}",
                    "15",
                    "27",                   //control action command
                    $"{CallTime:X2}",
                    $"{WaitTime:X2}",
                    "00",
                    $"{CallFrequency:X2}",
                    "00",
                    $"{splittedMac[6]:X2}",
                    $"{splittedMac[5]:X2}",
                    $"{splittedMac[4]:X2}",
                    $"{splittedMac[3]:X2}",
                    $"{splittedMac[2]:X2}",
                    $"{splittedMac[1]:X2}",
                    $"{splittedMac[0]:X2}",
                    $"{splittedMac[2]:X2}",   //выбор цвета led индикатора
                    $"{LEDControl:X2}",
                    "00",
                    "00",
                    $"{Trailer:X2}" };

                crc = checksum.CheckSum_CRC(Request, 2, Request.Length);
                Request[^2] = string.Join("", (crc & 255).ToString("X2"));
                Request[^3] = string.Join("", (crc >> 8).ToString("X2"));
                //orderNumber++;

                Sender.SendHEXMessage(string.Join(" ", Request));
                Messages.AddSentMessage(string.Join(" ", Request));

                Thread.Sleep(200);
            }
        }

        public void SoftwareVersionParameter()
        {
            if (_blackListIndesxes.Contains(DeviceFamilyNum))
            {
                Messages.AddMessage("Этот тип индикаторов пока нельзя использовать");
                return;
            }
            if (!Sender.Port.IsOpen)
            {
                Messages.AddMessage("Порт не открыт, не удалось отправить сообщение");
                return;
            }
            if (Indicators.Count < 1)
            {
                Messages.AddMessage("Информация не считана, ни одного индикатора КЗ нет в списке");
                return;
            }
            new Thread(() =>
            {
                foreach (var indicator in Indicators)
                {
                    //MAC = indicator.MACAdress;
                    int crc;
                    var splittedMac = indicator.MACAdress.Split('-');

                    Request =
                        new string[] { $"{_namingProtocol.ElementAt(DeviceCommunicationProtocolNum).Value:X2}",
                    $"{_namingFamily.ElementAt(DeviceFamilyNum).Value:X2}",
                    "15",
                    "25",       //software info command
                    $"{CallTime:X2}",
                    $"{WaitTime:X2}",
                    "00",
                    $"{CallFrequency:X2}",
                    "00",
                    $"{splittedMac[6]:X2}",
                    $"{splittedMac[5]:X2}",
                    $"{splittedMac[4]:X2}",
                    $"{splittedMac[3]:X2}",
                    $"{splittedMac[2]:X2}",
                    $"{splittedMac[1]:X2}",
                    $"{splittedMac[0]:X2}",
                    $"{5:X2}",   //software info command
                    $"{FunctionCallNumEnd:X2}",
                    "00",
                    "00",
                    $"{Trailer:X2}" };

                    crc = checksum.CheckSum_CRC(Request, 2, Request.Length);
                    Request[^2] = string.Join("", (crc & 255).ToString("X2"));
                    Request[^3] = string.Join("", (crc >> 8).ToString("X2"));
                    //orderNumber++;

                    Sender.SendHEXMessage(string.Join(" ", Request));
                    Messages.AddSentMessage(string.Join(" ", Request));

                    Thread.Sleep(500);
                }
            }).Start();
        }

        public void ShortCircuitCheck()
        {
            if (_blackListIndesxes.Contains(DeviceFamilyNum))
            {
                Messages.AddMessage("Этот тип индикаторов пока нельзя использовать");
                return;
            }
            if (!Sender.Port.IsOpen)
            {
                Messages.AddMessage("Порт не открыт, не удалось отправить сообщение");
                return;
            }
            if (Indicators.Count < 1)
            {
                Messages.AddMessage("Информация не считана, ни одного индикатора КЗ нет в списке");
                return;
            }
            new Thread(() =>
            {
                foreach (var indicator in Indicators)
                {
                    //MAC = indicator.MACAdress;
                    int crc;
                    var splittedMac = indicator.MACAdress.Split('-');

                    Request =
                        new string[] { $"{_namingProtocol.ElementAt(DeviceCommunicationProtocolNum).Value:X2}",
                    $"{_namingFamily.ElementAt(DeviceFamilyNum).Value:X2}",
                    "15",
                    "2A",
                    $"{CallTime:X2}",
                    $"{WaitTime:X2}",
                    "00",
                    $"{CallFrequency:X2}",
                    "00",
                    $"{splittedMac[6]:X2}",
                    $"{splittedMac[5]:X2}",
                    $"{splittedMac[4]:X2}",
                    $"{splittedMac[3]:X2}",
                    $"{splittedMac[2]:X2}",
                    $"{splittedMac[1]:X2}",
                    $"{splittedMac[0]:X2}",
                    $"{1:X2}",
                    $"{FunctionCallNumEnd:X2}",
                    "00",
                    "00",
                    $"{Trailer:X2}" };

                    crc = checksum.CheckSum_CRC(Request, 2, Request.Length);
                    Request[^2] = string.Join("", (crc & 255).ToString("X2"));
                    Request[^3] = string.Join("", (crc >> 8).ToString("X2"));
                    //orderNumber++;

                    Sender.SendHEXMessage(string.Join(" ", Request));
                    Messages.AddSentMessage(string.Join(" ", Request));

                    Thread.Sleep(500);
                }
            }).Start();
        }

        public void GroundShortCircuitCheck()
        {
            if (_blackListIndesxes.Contains(DeviceFamilyNum))
            {
                Messages.AddMessage("Этот тип индикаторов пока нельзя использовать");
                return;
            }
            if (!Sender.Port.IsOpen)
            {
                Messages.AddMessage("Порт не открыт, не удалось отправить сообщение");
                return;
            }
            if (Indicators.Count < 1)
            {
                Messages.AddMessage("Информация не считана, ни одного индикатора КЗ нет в списке");
                return;
            }
            new Thread(() =>
            {
                foreach (var indicator in Indicators)
                {
                    //MAC = indicator.MACAdress;
                    int crc;
                    var splittedMac = indicator.MACAdress.Split('-');

                    Request =
                        new string[] { $"{_namingProtocol.ElementAt(DeviceCommunicationProtocolNum).Value:X2}",
                    $"{_namingFamily.ElementAt(DeviceFamilyNum).Value:X2}",
                    "15",
                    "2A",
                    $"{CallTime:X2}",
                    $"{WaitTime:X2}",
                    "00",
                    $"{CallFrequency:X2}",
                    "00",
                    $"{splittedMac[6]:X2}",
                    $"{splittedMac[5]:X2}",
                    $"{splittedMac[4]:X2}",
                    $"{splittedMac[3]:X2}",
                    $"{splittedMac[2]:X2}",
                    $"{CallAdress:X2}",
                    $"{0:X2}",
                    $"{2:X2}",
                    $"{FunctionCallNumEnd:X2}",
                    "00",
                    "00",
                    $"{Trailer:X2}" };

                    crc = checksum.CheckSum_CRC(Request, 2, Request.Length);
                    Request[^2] = string.Join("", (crc & 255).ToString("X2"));
                    Request[^3] = string.Join("", (crc >> 8).ToString("X2"));
                    //orderNumber++;

                    Sender.SendHEXMessage(string.Join(" ", Request));
                    Messages.AddSentMessage(string.Join(" ", Request));

                    Thread.Sleep(500);
                }
            }).Start();
        }

        public void ControlActionParameter()
        {
            ushort orderNumber;

            //_functionCallNumStart = 2;
            //_functionCallNumEnd = 0;
            orderNumber = 4;
            if (_blackListIndesxes.Contains(DeviceFamilyNum))
            {
                Messages.AddMessage("Этот тип индикаторов пока нельзя использовать");
                return;
            }
            if (!Sender.Port.IsOpen)
            {
                Messages.AddMessage("Порт не открыт, не удалось отправить сообщение");
                return;
            }
            if (Indicators.Count < 1)
            {
                Messages.AddMessage("Информация не считана, ни одного индикатора КЗ нет в списке");
                return;
            }
            foreach (var indicator in Indicators)
            {
                //MAC = indicator.MACAdress;
                int crc;
                var splittedMac = indicator.MACAdress.Split('-');

                Request =
                    new string[] { $"{_namingProtocol.ElementAt(DeviceCommunicationProtocolNum).Value:X2}",
                    $"{_namingFamily.ElementAt(DeviceFamilyNum).Value:X2}",
                    "15",
                    "27",                   //control action command
                    $"{CallTime:X2}",
                    $"{WaitTime:X2}",
                    "00",
                    $"{CallFrequency:X2}",
                    "00",
                    $"{splittedMac[6]:X2}",
                    $"{splittedMac[5]:X2}",
                    $"{splittedMac[4]:X2}",
                    $"{splittedMac[3]:X2}",
                    $"{splittedMac[2]:X2}",
                    $"{CallAdress:X2}",
                    $"{0:X2}",
                    $"{orderNumber += 1:X2}",   //control action phase check
                    $"{1:X2}",
                    "00",
                    "00",
                    $"{Trailer:X2}" };

                crc = checksum.CheckSum_CRC(Request, 2, Request.Length);
                Request[^2] = string.Join("", (crc & 255).ToString("X2"));
                Request[^3] = string.Join("", (crc >> 8).ToString("X2"));
                //orderNumber++;

                Sender.SendHEXMessage(string.Join(" ", Request));
                Messages.AddSentMessage(string.Join(" ", Request));

                Thread.Sleep(250);
            }
        }

        public void ControlReturnParameter()
        {
            ushort orderNumber;
            orderNumber = 4;
            if (_blackListIndesxes.Contains(DeviceFamilyNum))
            {
                Messages.AddMessage("Этот тип индикаторов пока нельзя использовать");
                return;
            }
            if (!Sender.Port.IsOpen)
            {
                Messages.AddMessage("Порт не открыт, не удалось отправить сообщение");
                return;
            }
            if (Indicators.Count < 1)
            {
                Messages.AddMessage("Информация не считана, ни одного индикатора КЗ нет в списке");
                return;
            }
            foreach (var indicator in Indicators)
            {
                //MAC = indicator.MACAdress;
                int crc;
                var splittedMac = indicator.MACAdress.Split('-');

                Request =
                    new string[] { $"{_namingProtocol.ElementAt(DeviceCommunicationProtocolNum).Value:X2}",
                    $"{_namingFamily.ElementAt(DeviceFamilyNum).Value:X2}",
                    "15",
                    "28",                  //control return command
                    $"{CallTime:X2}",
                    $"{WaitTime:X2}",
                    "00",
                    $"{CallFrequency:X2}",
                    "00",
                    $"{splittedMac[6]:X2}",
                    $"{splittedMac[5]:X2}",
                    $"{splittedMac[4]:X2}",
                    $"{splittedMac[3]:X2}",
                    $"{splittedMac[2]:X2}",
                    $"{CallAdress:X2}",
                    $"{0:X2}",
                    $"{orderNumber += 1:X2}",   //control action phase check
                    $"{1:X2}",
                    "00",
                    "00",
                    $"{Trailer:X2}" };

                crc = checksum.CheckSum_CRC(Request, 2, Request.Length);
                Request[^2] = string.Join("", (crc & 255).ToString("X2"));
                Request[^3] = string.Join("", (crc >> 8).ToString("X2"));
                //orderNumber++;

                Sender.SendHEXMessage(string.Join(" ", Request));
                Messages.AddSentMessage(string.Join(" ", Request));

                Thread.Sleep(250);
            }
        }

        public void LEDConf()
        {
            ushort orderNumber;

            //_functionCallNumStart = 2;
            //_functionCallNumEnd = 0;
            orderNumber = 4;
            if (_blackListIndesxes.Contains(DeviceFamilyNum))
            {
                Messages.AddMessage("Этот тип индикаторов пока нельзя использовать");
                return;
            }
            if (!Sender.Port.IsOpen)
            {
                Messages.AddMessage("Порт не открыт, не удалось отправить сообщение");
                return;
            }
            if (Indicators.Count < 1)
            {
                Messages.AddMessage("Информация не считана, ни одного индикатора КЗ нет в списке");
                return;
            }
            new Thread(() =>
            {
                foreach (var indicator in Indicators)
                {
                    //MAC = indicator.MACAdress;
                    int crc;
                    var splittedMac = indicator.MACAdress.Split('-');

                    Request =
                        new string[] { $"{_namingProtocol.ElementAt(DeviceCommunicationProtocolNum).Value:X2}",
                    $"{_namingFamily.ElementAt(DeviceFamilyNum).Value:X2}",
                    "15",
                    $"{39+LEDCommand:X2}",                   //led action command
                    $"{CallTime:X2}",
                    $"{WaitTime:X2}",
                    "00",
                    $"{CallFrequency:X2}",
                    "00",
                    $"{splittedMac[6]:X2}",
                    $"{splittedMac[5]:X2}",
                    $"{splittedMac[4]:X2}",
                    $"{splittedMac[3]:X2}",
                    $"{splittedMac[2]:X2}",
                    $"{CallAdress:X2}",
                    $"{0:X2}",
                    $"{orderNumber += 1:X2}",   //led action phase check
                    $"{LEDControl:X2}",
                    "00",
                    "00",
                    $"{Trailer:X2}" };

                    crc = checksum.CheckSum_CRC(Request, 2, Request.Length);
                    Request[^2] = string.Join("", (crc & 255).ToString("X2"));
                    Request[^3] = string.Join("", (crc >> 8).ToString("X2"));
                    //orderNumber++;

                    Sender.SendHEXMessage(string.Join(" ", Request));
                    Messages.AddSentMessage(string.Join(" ", Request));

                    Thread.Sleep(300);
                }
            }).Start();
        }

        public void UpdateModelContent()
        {
            //TODO: Исправить баг с выбором при отсчёте с 0
            DeviceModel = _namingModel[DeviceModelNum];
        }

        public void UpdateFamilyContent()
        {
            //TODO: Исправить баг с выбором при отсчёте с 0
            DeviceFamily = _namingFamily.ElementAt(DeviceFamilyNum).Key;
        }

        public void UpdateCommunicationContent()
        {
            //TODO: Исправить баг с выбором при отсчёте с 0
            DeviceCommunicationProtocol = _namingProtocol.ElementAt(DeviceCommunicationProtocolNum - 1).Key;
        }

        public void ClearDataGridContent()
        {
            Indicators.Clear();
            ButtonCheckAnyIndicators = false;
        }

        public void ClearAllContent()
        {
            DeviceFamily = "";
            DeviceModel = "";
            DeviceCommunicationProtocol = "";
            DeviceModelNum = 0;
            DeviceFamilyNum = 0;
            DeviceCommunicationProtocolNum = 0;
            //SerialPort.CloseAll();
            SerialPort.Settings.CanEditControls = true;
        }
    }
}

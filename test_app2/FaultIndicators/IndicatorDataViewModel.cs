using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRFramework.Utilities;

namespace test_app2.FaultIndicators
{
    public class IndicatorDataViewModel : BaseViewModel
    {
        private string[] _namingModel = {
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
            "JYZ-LH-LoRa",
            ""
        };

        private string[] _namingProtocol =
        {
            "JYZ-FF",
            "JYZ-HW",
            "JYZ-HW V2.0"
        };

        private string[] _namingFamily =
        {
            "JYZ-FF",
            "JYZ-HW",
            "JYZ-HW V2.0"
        };

        private int _deviceModelNum;
        private int _deviceFamilyNum;
        private int _deviceCommunicationProtocolNum;
        private string _deviceModel;
        private string _deviceFamily;
        private string _deviceCommunicationProtocol;
        private int _callAdress;
        private int _callFrequency;
        private int _callTime;
        private int _waitTime;
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

        public Command UpdateModelContentCommand { get; }

        public Command UpdateFamilyContentCommand { get; }

        public Command UpdateCommunicationProtocolCommand { get; }

        public Command ClearAllContentCommand { get; }

        public ObservableCollection<FaultIndicatorViewModel> Indicators { get; set; }

        public IndicatorDataViewModel()
        {
            Indicators = new ObservableCollection<FaultIndicatorViewModel>
            {
                new FaultIndicatorViewModel() { CallAdress = 25, _callFrequency = 30 }
            };

            UpdateModelContentCommand = new Command(UpdateModelContent);
            UpdateFamilyContentCommand = new Command(UpdateFamilyContent);
            ClearAllContentCommand = new Command(ClearAllContent);
            UpdateCommunicationProtocolCommand = new Command(ClearAllContent);
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

        public void ClearAllContent()
        {
            DeviceFamily = "";
            DeviceModel = "";
            DeviceModelNum = 0;
            DeviceFamilyNum = 0;
        }
    }
}

using System.Data;
using TheRFramework.Utilities;

namespace test_app2.FaultIndicators
{
    public class FaultIndicatorViewModel : BaseViewModel
    {
        private string[] _naming = { 
            "JYL - FF - FI",
            "JYL - FF - CN - HP",
            "JYZ(W) - FF - FI V1.0",
            "JYZ(W) - FF - FI V2.0 (JYZ - HW - FI V1.0)",
        };

        private int _deviceModelNum;
        private int _deviceFamilyNum;
        private string _deviceModel;
        private string _deviceFamily;
        private int _callAdress;
        private int _callFrequency;
        private int _callTime;
        private int _waitTime;
        private string _MACAddr;

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

        public Command UpdateContentCommand { get; }

        public FaultIndicatorViewModel()
        {
            UpdateContentCommand = new Command(UpdateContent);
        }

        public void UpdateContent()
        {
            DeviceModel = _naming[DeviceModelNum];
        }
    }
}

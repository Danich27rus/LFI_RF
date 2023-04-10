using System.Data;
using System.ComponentModel;
using TheRFramework.Utilities;
using System;
using System.Windows.Input;
using Accessibility;

namespace test_app2.FaultIndicators
{
    public class FaultIndicatorViewModel : BaseViewModel, IEditableObject
    {
        private string _macAdress;
        private string _macAdressShow;
        private string _information;
        private string _status;
        //общие
        private int _fieldThreshold;
        private int _currentThreshold;
        private int _uploadT1;
        private int _uploadT2;
        private int _heartBeat;
        private int _currentIValue;
        private int _currentRelativeValue;
        private int _currentAbsoluteValue;
        //межфазные
        private int _timeDelay;
        private int _repowerDelay;
        private int _shortCurrent;
        //земля
        private int _transientFieldDrop;
        private int _fieldDropDelay;
        //ВРЕМЕННО ДЛЯ ТЕСТА ВАЛИДАЦИИ
        //private int _callAdress;
        //public int _callFrequency;
        //----------------------------
        //private int _callTime;
        //private int _waitTime;
        //private string _MACAddr;

        public string MACAdress
        {
            get => _macAdress;
            set => RaisePropertyChanged(ref _macAdress, value);
        }

        public string MACAdressShow
        {
            get => _macAdressShow;
            set => RaisePropertyChanged(ref _macAdressShow, value);
        }

        public string Information
        {
            get => _information;
            set => RaisePropertyChanged(ref _information, value);
        }

        public string Status
        {
            get => _status;
            set => RaisePropertyChanged(ref _status, value);
        }

        public int FieldThreshold
        {
            get => _fieldThreshold;
            set => RaisePropertyChanged(ref _fieldThreshold, value);
        }

        public int CurrentThreshold
        {
            get => _currentThreshold;
            set => RaisePropertyChanged(ref _currentThreshold, value);
        }

        public int UploadT1
        {
            get => _uploadT1;
            set => RaisePropertyChanged(ref _uploadT1, value);
        }

        public int UploadT2
        {
            get => _uploadT2;
            set => RaisePropertyChanged(ref _uploadT2, value);
        }

        public int HeartBeat
        {
            get => _heartBeat;
            set => RaisePropertyChanged(ref _heartBeat, value);
        }

        public int CurrentIValue
        {
            get => _currentIValue;
            set => RaisePropertyChanged(ref _currentIValue, value);
        }

        public int CurrentRelativeValue
        {
            get => _currentRelativeValue; 
            set => RaisePropertyChanged(ref _currentRelativeValue, value);
        }

        public int CurrentAbsoluteValue
        {
            get => _currentAbsoluteValue; 
            set => RaisePropertyChanged(ref _currentAbsoluteValue, value);
        }

        public int TimeDelay
        {
            get => _timeDelay;
            set => RaisePropertyChanged(ref _timeDelay, value);
        }

        public int RepowerDelay
        {
            get => _repowerDelay;
            set => RaisePropertyChanged(ref _repowerDelay, value);
        }

        public int ShortCurrent
        {
            get => _shortCurrent;
            set => RaisePropertyChanged(ref _shortCurrent, value);
        }

        public int TransientFieldDrop
        {
            get => _transientFieldDrop;
            set => RaisePropertyChanged(ref _transientFieldDrop, value);
        }

        public int FieldDropDelay
        {
            get => _fieldDropDelay;
            set => RaisePropertyChanged(ref _fieldDropDelay, value);
        }

        #region IEditableObject

        private FaultIndicatorViewModel backupCopy;
        private bool _inEdit;
        void IEditableObject.BeginEdit()
        {
            if (_inEdit) return;
            _inEdit = true;
            backupCopy = this.MemberwiseClone() as FaultIndicatorViewModel;
        }

        void IEditableObject.CancelEdit()
        {
            if (!_inEdit) return;
            _inEdit = false;
            //this._callAdress = backupCopy._callAdress;
            //this._callFrequency = backupCopy._callFrequency;
        }

        void IEditableObject.EndEdit()
        {
            if (!_inEdit) return;
            _inEdit = false;
            backupCopy = null;
        }
        #endregion

        //public Command UpdateContentCommand { get; }

        public FaultIndicatorViewModel()
        {
            //UpdateContentCommand = new Command(UpdateContent);
        }

        public void UpdateContent()
        {
            //DeviceModel = _naming[DeviceModelNum];
        }
    }
}

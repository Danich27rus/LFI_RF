﻿using System.Data;
using System.ComponentModel;
using TheRFramework.Utilities;
using System;
using System.Windows.Input;

namespace test_app2.FaultIndicators
{
    public class FaultIndicatorViewModel : BaseViewModel, IEditableObject
    {
        private string _deviceModel;
        private string _deviceFamily;
        //ВРЕМЕННО ДЛЯ ТЕСТА ВАЛИДАЦИИ
        private int _callAdress;
        public int _callFrequency;
        //----------------------------
        private int _callTime;
        private int _waitTime;
        //private string _MACAddr;
        public int CallAdress
        {
            get => _callAdress;
            set => RaisePropertyChanged(ref _callAdress, value);
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
            this._callAdress = backupCopy._callAdress;
            this._callFrequency = backupCopy._callFrequency;
        }

        void IEditableObject.EndEdit()
        {
            if (!_inEdit) return;
            _inEdit = false;
            backupCopy = null;
        }
        #endregion

        public Command UpdateContentCommand { get; }

        public FaultIndicatorViewModel()
        {
            UpdateContentCommand = new Command(UpdateContent);
        }

        public void UpdateContent()
        {
            //DeviceModel = _naming[DeviceModelNum];
        }
    }
}
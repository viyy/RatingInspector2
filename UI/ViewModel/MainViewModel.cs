using System;
using System.ComponentModel;
using Common;
using GalaSoft.MvvmLight;
using Interfaces;
using UI.Properties;

namespace UI.ViewModel
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private IInfo _info;
        public MainViewModel(IInfo info)
        {
            _info = info;
            MessengerInstance.Register<string>(this, NotifyMe);
        }

        private void NotifyMe(string msg)
        {
            if (msg != Ri2Constants.Notifications.DbUpdated) return;
            RaisePropertyChanged(nameof(LastUpdate));
            RaisePropertyChanged(nameof(Version));
            RaisePropertyChanged(nameof(ProfilesCount));
            RaisePropertyChanged(nameof(RcfCount));
            RaisePropertyChanged(nameof(FideCount));
        }

        public string LastUpdate => _info.LastUpdate.ToShortDateString();
        public string Version => _info.Version;
        public string UiVersion => Resources.UiVersion;

        public int ProfilesCount => _info.ProfilesCount;

        public int RcfCount => _info.RcfCount;

        public int FideCount => _info.FideCount;
    }
}
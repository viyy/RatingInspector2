using System.ComponentModel;
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
        }

        public string LastUpdate => _info.LastUpdate.ToShortDateString();
        public int Count => _info.ProfilesCount;
        public string Version => _info.Version;
        public string UiVersion => Resources.UiVersion;
    }
}
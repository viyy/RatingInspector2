using System.ComponentModel;
using System.Windows.Input;
using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Interfaces;
using UI.Properties;
using UI.Utils;

namespace UI.ViewModel
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {

        public ICommand CloseCommand { get; }

        private IInfo _info;
        public MainViewModel(IInfo info)
        {
            _info = info;
            MessengerInstance.Register<string>(this, NotifyMe);
            CloseCommand = new RelayCommand(CloseApp, StateMachine.CurrentState==ProcessStates.Idle);
            Logger.Log("MainVM initialized");
        }

        private void CloseApp()
        {
            Logger.Log("Shutdown...");
            if (StateMachine.CurrentState != ProcessStates.Idle)
            {
                Logger.Log("Some tasks not completed. Abort.");
                return;
            }
            MessengerInstance.Send(Ri2Constants.Notifications.Exit);
            App.Current.Shutdown();
        }

        private void NotifyMe(string msg)
        {
            if (msg != Ri2Constants.Notifications.DbUpdated && msg!=Ri2Constants.Notifications.ProfilesUpdated) return;
            Logger.Log("MainVM", "Updating view");
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
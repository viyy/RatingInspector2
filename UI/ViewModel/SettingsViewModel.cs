using System;
using System.Linq;
using System.Windows.Input;
using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MaterialDesignThemes.Wpf;
using UI.Utils;

namespace UI.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            SaveCommand = new RelayCommand(()=>Settings.Current.Save());
            CancelCommand = new RelayCommand(() =>
            {
                Settings.Current.Reload();
                RaisePropertyChanged(nameof(FideUrl));
                RaisePropertyChanged(nameof(RcfUrl));
                RaisePropertyChanged(nameof(Year));
                RaisePropertyChanged(nameof(DeleteFiles));
                RaisePropertyChanged(nameof(Filter));
            });
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public string FideUrl
        {
            get => Settings.Current.FideUrl;
            set
            {
                if (Settings.Current.FideUrl == value) return;
                Settings.Current.FideUrl = value;
                RaisePropertyChanged(nameof(FideUrl));
            }
        }

        public string RcfUrl
        {
            get => Settings.Current.RcfUrl;
            set
            {
                if (Settings.Current.RcfUrl == value) return;
                Settings.Current.RcfUrl = value;
                RaisePropertyChanged(nameof(RcfUrl));
            }
        }

        public int Year
        {
            get => Settings.Current.BirthCutoff;
            set
            {
                if (Settings.Current.BirthCutoff == value) return;
                Settings.Current.BirthCutoff = value;
                RaisePropertyChanged(nameof(Year));
            }
        }

        public bool DeleteFiles
        {
            get => Settings.Current.RemoveTmpFiles;
            set
            {
                if (Settings.Current.RemoveTmpFiles == value) return;
                Settings.Current.RemoveTmpFiles = value;
                RaisePropertyChanged(nameof(DeleteFiles));
            }
        }

        public string Filter
        {
            get => string.Join(" ",Settings.Current.Filter);
            set
            {
                if (string.Join(" ", Settings.Current.Filter)==value) return;
                Settings.Current.Filter = value.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries).ToList();
                RaisePropertyChanged(nameof(Filter));
            }
        }
    }
}
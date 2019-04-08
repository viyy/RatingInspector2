using System.Collections.ObjectModel;
using System.Windows.Input;
using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Interfaces;
using UI.Properties;

namespace UI.ViewModel
{
    public class AboutViewModel : ViewModelBase
    {
        public string AboutText { get; } = Resources.AboutText;

        public ObservableCollection<IPlugin> Plugins { get; } = new ObservableCollection<IPlugin>(PluginManager.GetPlugins<IPlugin>());
        
        public string LicenseType => License.GetData("type");

        public string LicenseOwner => License.GetData("owner");

        private string _key;

        public AboutViewModel()
        {
            InstallCommand = new RelayCommand(()=>
            {
                License.Set(Key);
                RaisePropertyChanged(nameof(LicenseType));
                RaisePropertyChanged(nameof(LicenseOwner));
                Key = string.Empty;
                RaisePropertyChanged(nameof(Key));
            }, ()=>!string.IsNullOrEmpty(Key));
        }

        public string Key
        {
            get => _key;
            set => Set(ref _key, value);
        }
        
        public ICommand InstallCommand { get; }
    }
}
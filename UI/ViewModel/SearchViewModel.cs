using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Interfaces;
using Models;

namespace UI.ViewModel
{
    public class SearchViewModel : ViewModelBase
    {
        public ICommand LoadProfilesOnLoadCommand { get; }
        public ICommand RemoveFilterCommand { get; }
        public ICommand ApplyFilterCommand { get; }
        public ICommand OpenRcfUrlCommand { get; }
        public ICommand OpenFideUrlCommand { get; }

        private bool _panelVisible = false;
        private readonly IProfileManager _pm;
        public SearchViewModel(IProfileManager pm)
        {
            _pm = pm;

            LoadProfilesOnLoadCommand = new RelayCommand(LoadProfilesOnLoad);
            RemoveFilterCommand = new RelayCommand(() => { SearchCriteria = null; ApplyFilter(null);}, ()=>!string.IsNullOrEmpty(SearchCriteria));
            ApplyFilterCommand = new RelayCommand(()=>ApplyFilter(SearchCriteria));
            OpenRcfUrlCommand = new RelayCommand(OpenRcfUrl,()=>SelectedProfile?.RcfProfile!=null);
            OpenFideUrlCommand = new RelayCommand(OpenFideUrl, () => SelectedProfile?.FideProfile != null);
        }

        private void LoadProfilesOnLoad()
        {
            if (!_panelVisible)
            {
                Profiles = new ObservableCollection<Profile>(_pm.GetProfiles());
                _panelVisible = true;
            }
            else
            {
                _panelVisible = false;
            }
        }

        public ObservableCollection<Group> Groups => new ObservableCollection<Group>(_pm.GetGroups());

        private ObservableCollection<Profile> _profiles;
        public ObservableCollection<Profile> Profiles
        {
            get => _profiles;
            set => Set(ref _profiles, value);
        }

        private Profile _selectedProfile;

        public Profile SelectedProfile
        {
            get => _selectedProfile;
            set => Set(ref _selectedProfile, value);
        }

        private string _searchCriteria;

        public string SearchCriteria
        {
            get => _searchCriteria;
            set => Set(ref _searchCriteria, value);
        }

        private void ApplyFilter(string needle)
        {
            Profiles = new ObservableCollection<Profile>(_pm.GetProfiles(needle));
        }

        private void OpenRcfUrl()
        {
            if (SelectedProfile.RcfProfile==null) return;
            Process.Start(Ri2Constants.Urls.RcfUrl + SelectedProfile.RcfProfile.RcfId);
        }

        private void OpenFideUrl()
        {
            if (SelectedProfile.FideProfile == null) return;
            Process.Start(Ri2Constants.Urls.FideUrl + SelectedProfile.FideProfile.FideId);
        }
    }
}
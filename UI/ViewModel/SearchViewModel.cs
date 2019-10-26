using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
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
        private readonly IProfileManager _pm;

        private FideProfile _fide2Merge;

        private int _fideSearch;

        private ObservableCollection<Group> _groups;

        private bool _isSnackBarVisible;

        private Visibility _panelVisibility = Visibility.Collapsed;

        private ObservableCollection<Profile> _profiles;

        private RcfProfile _rcf2Merge;

        private int _rcfSearch;

        private string _searchCriteria;

        private Profile _selectedProfile;

        public SearchViewModel(IProfileManager pm)
        {
            _pm = pm;
            RemoveFilterCommand = new RelayCommand(() =>
            {
                SearchCriteria = null;
                ApplyFilter(null);
            }, () => !string.IsNullOrEmpty(SearchCriteria));
            ApplyFilterCommand = new RelayCommand(() => ApplyFilter(SearchCriteria));
            OpenRcfUrlCommand = new RelayCommand(OpenRcfUrl, () => SelectedProfile?.RcfProfile != null);
            OpenFideUrlCommand = new RelayCommand(OpenFideUrl, () => SelectedProfile?.FideProfile != null);
            CopyToClipboardCommand = new RelayCommand(CopyToClipboard, () => SelectedProfile != null && License.GetData("copy")=="true");
            DismissSnackbarCommand = new RelayCommand(() => { IsSnackBarVisible = false; });
            DeleteProfileCommand = new RelayCommand(DeleteProfile, () => SelectedProfile != null);
            SaveCurrentProfileCommand = new RelayCommand(SaveProfile);
            Groups = new ObservableCollection<Group>(_pm.GetGroups());
            MessengerInstance.Register<string>(this, NotifyMe);
            SearchFideCommand = new RelayCommand(() => Fide2Merge = _pm.SearchFideProfile(FideSearch));
            SearchRcfCommand = new RelayCommand(() => Rcf2Merge = _pm.SearchRcfProfile(RcfSearch));
            MergeCommand = new RelayCommand(RunMerge);
            Logger.Log("SearchVM initialized");
        }

        public ICommand RemoveFilterCommand { get; }
        public ICommand ApplyFilterCommand { get; }
        public ICommand OpenRcfUrlCommand { get; }
        public ICommand OpenFideUrlCommand { get; }
        public ICommand CopyToClipboardCommand { get; }
        public ICommand DismissSnackbarCommand { get; }
        public ICommand DeleteProfileCommand { get; }
        public ICommand SaveCurrentProfileCommand { get; }
        public ICommand SearchFideCommand { get; }
        public ICommand SearchRcfCommand { get; }
        public ICommand MergeCommand { get; }

        public bool IsSnackBarVisible
        {
            get => _isSnackBarVisible;
            set => Set(ref _isSnackBarVisible, value);
        }

        public ObservableCollection<Group> Groups
        {
            get => _groups;
            set => Set(ref _groups, value);
        }

        public ObservableCollection<Profile> Profiles
        {
            get => _profiles;
            set => Set(ref _profiles, value);
        }

        public Profile SelectedProfile
        {
            get => _selectedProfile;
            set => Set(ref _selectedProfile, value);
        }

        public string SearchCriteria
        {
            get => _searchCriteria;
            set => Set(ref _searchCriteria, value);
        }

        public Visibility PanelVisibility
        {
            get => _panelVisibility;
            set
            {
                if (_panelVisibility != value && value == Visibility.Visible)
                    Profiles = new ObservableCollection<Profile>(_pm.GetProfiles());
                _panelVisibility = value;
            }
        }

        public int RcfSearch
        {
            get => _rcfSearch;
            set => Set(ref _rcfSearch, value);
        }

        public int FideSearch
        {
            get => _fideSearch;
            set => Set(ref _fideSearch, value);
        }

        public FideProfile Fide2Merge
        {
            get => _fide2Merge;
            set => Set(ref _fide2Merge, value);
        }

        public RcfProfile Rcf2Merge
        {
            get => _rcf2Merge;
            set => Set(ref _rcf2Merge, value);
        }

        private void RunMerge()
        {
            _pm.MergeProfiles(SelectedProfile, Rcf2Merge, Fide2Merge);
            Profiles = new ObservableCollection<Profile>(_pm.GetProfiles(SearchCriteria));
        }

        private void NotifyMe(string msg)
        {
            switch (msg)
            {
                case Ri2Constants.Notifications.GroupsUpdated:
                    Groups = new ObservableCollection<Group>(_pm.GetGroups());
                    return;
                case Ri2Constants.Notifications.ProfilesUpdated:
                    Profiles = new ObservableCollection<Profile>(_pm.GetProfiles());
                    return;
            }
        }

        private void SaveProfile()
        {
            _pm.SaveProfile(SelectedProfile);
            //MessengerInstance.Send(Ri2Constants.Notifications.ProfilesUpdated);
        }

        private void CopyToClipboard()
        {
            Logger.Log("SearchVM", "Copying profile");
            try
            {
                //License block++
                if (License.GetData("copy") != "true")
                {
                    throw new OutOfLicenseLimitException(
                        "Search system: You can not copy profile with your current license");
                }

                //License block--
                var str = "Имя:".PadRight(10) + SelectedProfile.RcfProfile?.Name + Environment.NewLine;
                str += "Имя(En):".PadRight(10) + SelectedProfile.FideProfile?.Name + Environment.NewLine;
                str += "Г.р.:".PadRight(10) + SelectedProfile.Birth + Environment.NewLine;
                str += "РШФ Id:".PadRight(10) + SelectedProfile.RcfProfile?.RcfId + Environment.NewLine;
                str += "Fide Id:".PadRight(10) + SelectedProfile.FideProfile?.FideId + Environment.NewLine;
                str += "РШФ Std:".PadRight(10) + SelectedProfile.RcfProfile?.Std + Environment.NewLine;
                str += "РШФ Rpd:".PadRight(10) + SelectedProfile.RcfProfile?.Rpd + Environment.NewLine;
                str += "РШФ Blz:".PadRight(10) + SelectedProfile.RcfProfile?.Blz + Environment.NewLine;
                str += "FIDE Std:".PadRight(10) + SelectedProfile.FideProfile?.Std + Environment.NewLine;
                str += "FIDE Rpd:".PadRight(10) + SelectedProfile.FideProfile?.Rpd + Environment.NewLine;
                str += "FIDE Blz:".PadRight(10) + SelectedProfile.FideProfile?.Blz + Environment.NewLine;
                Clipboard.SetText(str);
                IsSnackBarVisible = true;
            }
            catch (Exception ex)
            {
                Logger.Log("SearchVM", $"Unable to copy: {ex.Message}", LogLevel.Error);
            }
        }

        private void ApplyFilter(string needle)
        {
            Profiles = new ObservableCollection<Profile>(_pm.GetProfiles(needle));
        }

        private void OpenRcfUrl()
        {
            if (SelectedProfile.RcfProfile == null) return;
            Process.Start(Ri2Constants.Urls.RcfUrl + SelectedProfile.RcfProfile.RcfId);
        }

        private void OpenFideUrl()
        {
            if (SelectedProfile.FideProfile == null) return;
            Process.Start(Ri2Constants.Urls.FideUrl + SelectedProfile.FideProfile.FideId);
        }

        private void DeleteProfile()
        {
            _pm.DeleteProfile(SelectedProfile.Id);
            Profiles = new ObservableCollection<Profile>(_pm.GetProfiles(SearchCriteria));
        }
    }
}
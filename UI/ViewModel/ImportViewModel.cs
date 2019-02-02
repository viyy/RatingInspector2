using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Interfaces;
using Microsoft.Win32;
using Models;
using UI.Utils;

namespace UI.ViewModel
{
    public class ImportViewModel : ViewModelBase
    {
        private readonly IImportService _import;

        private int _currentId;
        private bool _fide;

        private ObservableCollection<int> _ids = new ObservableCollection<int>();
        private bool _rcf = true;

        private Group _selectedGroup;

        public ImportViewModel(IImportService import)
        {
            _import = import;
            ClearCommand = new RelayCommand(() => Ids.Clear(), () => Ids.Count > 0);
            ImportCommand = new AsyncCommand(async () =>
            {
                await Task.Run(() =>
                    _import.ImportAsync(Ids.ToList(), SelectedGroup, Rcf ? ProfileType.Rcf : ProfileType.Fide));
                //await _import.ImportAsync(Ids.ToList(), SelectedGroup, Rcf ? ProfileType.Rcf : ProfileType.Fide)
                //.ConfigureAwait(false);
                MessengerInstance.Send(Ri2Constants.Notifications.DbUpdated);
                MessengerInstance.Send(Ri2Constants.Notifications.ProfilesUpdated);
            });
            AddIdCommand = new RelayCommand(() =>
            {
                if (CurrentId != 0)
                    Ids.Add(CurrentId);
                CurrentId = 0;
            });
            SelectFileCommand = new RelayCommand(SelectFile);
            DeleteIdCommand = new RelayCommand<int>(i => Ids.Remove(i), Ids.Count > 0);
            MessengerInstance.Register<string>(this, msg =>
            {
                if (msg == Ri2Constants.Notifications.GroupsUpdated)
                    RaisePropertyChanged(nameof(Groups));
            });
        }

        public ObservableCollection<Group> Groups => new ObservableCollection<Group>(_import.GetGroups());

        public Group SelectedGroup
        {
            get => _selectedGroup;
            set => Set(ref _selectedGroup, value);
        }

        public int CurrentId
        {
            get => _currentId;
            set => Set(ref _currentId, value);
        }

        public ObservableCollection<int> Ids
        {
            get => _ids;
            set => Set(ref _ids, value);
        }

        public ICommand ClearCommand { get; }
        public IAsyncCommand ImportCommand { get; }
        public ICommand AddIdCommand { get; }
        public ICommand SelectFileCommand { get; }
        public ICommand DeleteIdCommand { get; }

        public bool Rcf
        {
            get => _rcf;
            set => Set(ref _rcf, value);
        }

        public bool Fide
        {
            get => _fide;
            set => Set(ref _fide, value);
        }

        private void SelectFile()
        {
            var dlg = new OpenFileDialog
            {
                Filter = _import.GetFilters(),
                Multiselect = false,
                CheckFileExists = true
            };
            var path = dlg.ShowDialog() == true ? dlg.FileName : "";
            if (path == "") return;

            foreach (var i in _import.LoadFromFile(path)) Ids.Add(i);
        }
    }
}
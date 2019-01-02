using System.Collections.ObjectModel;
using System.Linq;
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
        private bool _rcf = true;
        private bool _fide = false;
        private readonly IImportService _import;

        public ObservableCollection<Group> Groups => new ObservableCollection<Group>(_import.GetGroups());

        private Group _selectedGroup;

        public Group SelectedGroup
        {
            get => _selectedGroup;
            set => Set(ref _selectedGroup, value);
        }

        private int _currentId;
        public int CurrentId
        {
            get => _currentId;
            set => Set(ref _currentId, value);
        }

        private string _path;

        public string Path
        {
            get => _path;
            set => Set(ref _path, value);
        }

        private ObservableCollection<int> _ids = new ObservableCollection<int>();

        public ObservableCollection<int> Ids
        {
            get => _ids;
            set => Set(ref _ids, value);
        }

        public ICommand ClearCommand { get; }
        public IAsyncCommand ImportCommand { get; }
        public ICommand AddIdCommand { get; }
        public ICommand SelectFileCommand { get; }


        public ImportViewModel(IImportService import)
        {
            _import = import;
            ClearCommand = new RelayCommand(()=>Ids.Clear(), ()=>Ids.Count>0);
            ImportCommand = new AsyncCommand(() => _import.ImportAsync(Ids.ToList(),SelectedGroup,Rcf?ProfileType.Rcf:ProfileType.Fide));
            AddIdCommand = new RelayCommand(()=>
            {
                Ids.Add(CurrentId);
                CurrentId = 0;
            }, ()=>CurrentId!=0);
            SelectFileCommand = new RelayCommand(()=>SelectFile());
        }

        private string SelectFile()
        {
            var dlg = new OpenFileDialog
            {
                Filter = _import.GetFilters(),
                Multiselect = false,
                CheckFileExists = true
            };
            return dlg.ShowDialog() == true ? dlg.FileName : "";

        }
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
    }
}
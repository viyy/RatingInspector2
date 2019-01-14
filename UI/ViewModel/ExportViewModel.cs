using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Interfaces;
using Models;

namespace UI.ViewModel
{
    public class ExportViewModel : ViewModelBase
    {
        public ICommand GroupSelectionChangedCommand { get; }

        private readonly IExportService _exporter;
        private readonly ExportSettings _settings = new ExportSettings();
        public ExportViewModel(IExportService exporter)
        {
            GroupSelectionChangedCommand = new RelayCommand<IEnumerable<object>>(SetListSelection);
            _exporter = exporter;
        }

        public ObservableCollection<Group> Groups => new ObservableCollection<Group>(_exporter.GetGroups());

        private ObservableCollection<Group> _selectedGroups = new ObservableCollection<Group>();

        public ObservableCollection<Group> SelectedGroups
        {
            get => _selectedGroups;
            set => Set(ref _selectedGroups, value);
        }

        private void SetListSelection(IEnumerable<object> groups)
        {
            SelectedGroups = new ObservableCollection<Group>();
            foreach (var group in groups)
            {
                if (!(group is Group g)) return;
                SelectedGroups.Add(g);
            }
        }

        public bool Rcf
        {
            get => _settings.Rcf;
            set
            {
                if (_settings.Rcf==value) return;
                _settings.Rcf = value;
                RaisePropertyChanged(nameof(Rcf));
            }
        }
        public bool Fide
        {
            get => _settings.Rcf;
            set
            {
                if (_settings.Fide == value) return;
                _settings.Fide = value;
                RaisePropertyChanged(nameof(Fide));
            }
        }
        public bool Birth
        {
            get => _settings.Birth;
            set
            {
                if (_settings.Birth == value) return;
                _settings.Birth = value;
                RaisePropertyChanged(nameof(Birth));
            }
        }
        public bool Group
        {
            get => _settings.Groups;
            set
            {
                if (_settings.Groups == value) return;
                _settings.Groups = value;
                RaisePropertyChanged(nameof(Group));
            }
        }
    }
}
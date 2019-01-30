using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Interfaces;
using Models;
using PluginShared;

namespace UI.ViewModel
{
    public class ExportViewModel : ViewModelBase
    {
        private readonly IExportService _exporter;
        private readonly ExportSettings _settings = new ExportSettings();

        private ObservableCollection<IFileExporter> _plugins;

        private ObservableCollection<Group> _selectedGroups = new ObservableCollection<Group>();

        public ExportViewModel(IExportService exporter)
        {
            GroupSelectionChangedCommand = new RelayCommand<IEnumerable<object>>(SetListSelection);
            ExportCommand =
                new RelayCommand<Guid>(guid => _exporter.ExportAsync(SelectedGroups.ToList(), _settings, guid));
            _exporter = exporter;
            _plugins = new ObservableCollection<IFileExporter>(_exporter.GetPlugins());
        }

        public ICommand GroupSelectionChangedCommand { get; }
        public ICommand ExportCommand { get; }

        public ObservableCollection<IFileExporter> Plugins
        {
            get => _plugins;
            set => Set(ref _plugins, value);
        }

        public ObservableCollection<Group> Groups => new ObservableCollection<Group>(_exporter.GetGroups());

        public ObservableCollection<Group> SelectedGroups
        {
            get => _selectedGroups;
            set => Set(ref _selectedGroups, value);
        }

        public bool Rcf
        {
            get => _settings.Rcf;
            set
            {
                if (_settings.Rcf == value) return;
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

        private void SetListSelection(IEnumerable<object> groups)
        {
            SelectedGroups = new ObservableCollection<Group>();
            foreach (var group in groups)
            {
                if (!(group is Group g)) return;
                SelectedGroups.Add(g);
            }
        }
    }
}
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Interfaces;
using Models;

namespace UI.ViewModel
{
    public class GroupManagerViewModel : ViewModelBase
    {
        private readonly IGroupManager _gm;

        private Group _selectedGroup;

        private Group _sourceGroup;

        private Group _targetGroup;

        public GroupManagerViewModel(IGroupManager gm)
        {
            _gm = gm;
            AddCommand = new RelayCommand(CreateGroup);
            DeleteCommand = new RelayCommand(DeleteGroup, () => SelectedGroup != null);
            UpdateGroupCommand = new RelayCommand(UpdateGroup, () => SelectedGroup != null);
            ConfirmMergeCommand = new RelayCommand(RunMerge, () => SourceGroup != null && TargetGroup != null);
            CancelMergeCommand = new RelayCommand(CancelMerge);
            MergeCommand = new RelayCommand(AddToMerge, () => SelectedGroup != null);
            Logger.Log("GroupManagerVM initialized");
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand MergeCommand { get; }
        public ICommand CancelMergeCommand { get; }
        public ICommand ConfirmMergeCommand { get; }
        public ICommand UpdateGroupCommand { get; }

        public ObservableCollection<Group> GroupList => new ObservableCollection<Group>(_gm.GetGroups());

        public Group SelectedGroup
        {
            get => _selectedGroup;
            set => Set(ref _selectedGroup, value);
        }

        public Group SourceGroup
        {
            get => _sourceGroup;
            set => Set(ref _sourceGroup, value);
        }

        public Group TargetGroup
        {
            get => _targetGroup;
            set => Set(ref _targetGroup, value);
        }

        #region Actions

        private void CreateGroup()
        {
            Logger.Log("GroupManagerVM", "Creating new group");
            _gm.CreateGroup(new Group {Id = 0, Name = "<New Group>"});
            RaisePropertyChanged(nameof(GroupList));
            SelectedGroup = GroupList.Last();
            MessengerInstance.Send(Ri2Constants.Notifications.GroupsUpdated);
        }

        private void DeleteGroup()
        {
            Logger.Log("GroupManagerVM", $"Deleting group [{SelectedGroup.Id}] {SelectedGroup.Name}");
            _gm.DeleteGroup(SelectedGroup);
            SelectedGroup = null;
            RaisePropertyChanged(nameof(GroupList));
            RaisePropertyChanged(nameof(SelectedGroup));
            RaisePropertyChanged(nameof(SourceGroup));
            RaisePropertyChanged(nameof(TargetGroup));
            MessengerInstance.Send(Ri2Constants.Notifications.GroupsUpdated);
        }

        private void UpdateGroup()
        {
            Logger.Log("GroupManagerVM", $"Updating group [{SelectedGroup.Id}] {SelectedGroup.Name}");
            _gm.UpdateGroup(SelectedGroup);
            RaisePropertyChanged(nameof(GroupList));
            MessengerInstance.Send(Ri2Constants.Notifications.GroupsUpdated);
        }

        private void AddToMerge()
        {
            if (SourceGroup == null)
            {
                SourceGroup = SelectedGroup;
                return;
            }

            if (TargetGroup == null && SourceGroup != SelectedGroup)
                TargetGroup = SelectedGroup;
        }

        private void RunMerge()
        {
            if (SourceGroup == null || TargetGroup == null)
                return;
            Logger.Log("GroupManagerVM", $"Merging groups: [{SourceGroup.Id}] {SourceGroup.Name}, [{TargetGroup.Id}] {TargetGroup.Name}");
            _gm.MergeGroups(SourceGroup, TargetGroup);
            SourceGroup = null;
            TargetGroup = null;
            MessengerInstance.Send(Ri2Constants.Notifications.GroupsUpdated);
        }

        private void CancelMerge()
        {
            SourceGroup = null;
            TargetGroup = null;
        }

        #endregion
    }
}
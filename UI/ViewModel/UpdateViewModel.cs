using System;
using System.IO;
using System.Threading.Tasks;
using Common;
using GalaSoft.MvvmLight;
using Interfaces;
using UI.Properties;
using UI.Utils;

namespace UI.ViewModel
{
    /// <summary>
    ///     The update view model.
    /// </summary>
    public class UpdateViewModel : ViewModelBase
    {
        private readonly IUpdateService _upd;

        private bool _busy;

        private string _report;

        public UpdateViewModel(IUpdateService updateService)
        {
            _upd = updateService;
            RunUpdateCommand = new AsyncCommand(UpdateAsync);
            MessengerInstance.Register<string>(this, (msg) =>
            {
                if (msg!=Ri2Constants.Notifications.Exit) return;
                _upd.CleanUp();
            });
        }

        public IAsyncCommand RunUpdateCommand { get; }

        public bool Busy
        {
            get => _busy;
            set => Set(ref _busy, value);
        }

        public string Report
        {
            get => _report;
            set => Set(ref _report, value);
        }

        private async Task UpdateAsync()
        {
            try
            {
                var pr = new Progress<string>(s => Report = s);
                Busy = true;
                if (!StateMachine.TrySetState(ProcessStates.Busy, this)) return;
                await _upd.UpdateAsync(pr).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                File.AppendAllText("log.txt",
                    string.Format(Resources.UpdateViewModel_UpdateAsync_Error, DateTime.Now.ToLongDateString(),
                        e.Message));
            }
            finally
            {
                if (!StateMachine.TrySetState(ProcessStates.Idle, this)) throw new Exception("Cannot unlock process");
                Busy = false;
                MessengerInstance.Send(Ri2Constants.Notifications.DbUpdated);
            }
        }
    }
}
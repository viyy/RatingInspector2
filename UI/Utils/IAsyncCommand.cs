using System.Threading.Tasks;
using System.Windows.Input;

namespace UI.Utils
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }
}
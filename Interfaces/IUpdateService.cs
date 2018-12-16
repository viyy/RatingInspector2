using System.Threading.Tasks;

namespace Interfaces
{
    public interface IUpdateService
    {
        Task UpdateAsync();
        void Update();
    }
}
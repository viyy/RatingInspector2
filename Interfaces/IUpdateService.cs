using System;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IUpdateService
    {
        Task UpdateAsync(IProgress<string> progress = null);
        void CleanUp();
    }
}
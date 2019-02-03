using System;

namespace UI.Utils
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
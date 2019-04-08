using System;

namespace Common
{
    public class OutOfLicenseLimitException : Exception
    {
        public OutOfLicenseLimitException()
        {
        }

        public OutOfLicenseLimitException(string message)
            : base(message)
        {
        }

        public OutOfLicenseLimitException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
using System;

namespace SaveSystem
{
    public class SaveSystemException : Exception
    {
        public SaveSystemException() : base() { }

        public SaveSystemException(string message) : base(message) { }

        public SaveSystemException(string message, Exception innerException) : base(message, innerException) { }
    }
}
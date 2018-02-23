using System;

namespace Faore.CFService
{
    public class NotConfiguredException : Exception
    {
        public NotConfiguredException() : base() { }
        public NotConfiguredException(string message) : base(message) { }
        public NotConfiguredException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class BadConfigurationException : Exception
    {
        public BadConfigurationException() : base() { }
        public BadConfigurationException(string message) : base(message) { }
        public BadConfigurationException(string message, Exception innerException) : base(message, innerException) { }
    }
}


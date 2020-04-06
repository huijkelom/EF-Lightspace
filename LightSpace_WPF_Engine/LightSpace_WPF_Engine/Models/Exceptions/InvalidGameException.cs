using System;
using System.Runtime.Serialization;

namespace LightSpace_WPF_Engine.Models.Exceptions
{
    [Serializable]
    internal class InvalidGameException : Exception
    {
        public InvalidGameException()
        {
        }

        public InvalidGameException(string message, Exception innerException) : base (message, innerException)
        {
        }

        protected InvalidGameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LightSpace_WPF_Engine.Models.Exceptions
{
    [Serializable]
    internal class InvalidConversionException : Exception
    {
        public InvalidConversionException()
        {
        }

        public InvalidConversionException(string message, Exception innerException) : base (message, innerException)
        {
        }

        public InvalidConversionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

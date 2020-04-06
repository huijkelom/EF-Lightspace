using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LightSpace_WPF_Engine.Models.Exceptions
{
    [Serializable]
    internal class NoException :Exception
    {
        public NoException()
        {
        }

        public NoException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

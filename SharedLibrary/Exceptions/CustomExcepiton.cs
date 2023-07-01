using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Exceptions
{
    public class CustomExcepiton : Exception
    {
        public CustomExcepiton()
        {
        }

        public CustomExcepiton(string message) : base(message)
        {
        }

        public CustomExcepiton(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CustomExcepiton(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

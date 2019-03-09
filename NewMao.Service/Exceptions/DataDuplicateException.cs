using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Service.Exceptions
{
    [Serializable]
    public class DataDuplicateException : Exception
    {
        public DataDuplicateException()
        {

        }

        public DataDuplicateException(string message) : base(message)
        {

        }

        public DataDuplicateException(string message, Exception inner) : base(message, inner)
        {

        }

        protected DataDuplicateException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {

        }
    }
}

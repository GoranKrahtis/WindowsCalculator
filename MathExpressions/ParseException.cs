using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressions
{
    [Serializable]
    public class ParseException : Exception
    {
        public ParseException()
            : base()
        { }

        public ParseException(string message)
            : base(message)
        { }

        public ParseException(string message, Exception innerException)
           : base(message, innerException)
        { }

        protected ParseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}

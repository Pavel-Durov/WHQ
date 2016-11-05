using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHQ.Core.Exceptions
{
    public class UnsupportedOperationException : Exception
    {
        public UnsupportedOperationException(string message) : base(message)
        {
        }
    }
}

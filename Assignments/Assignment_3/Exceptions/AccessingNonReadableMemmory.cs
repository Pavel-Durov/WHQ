using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_3.Exceptions
{
    class AccessingNonReadableMemmory : Exception
    {
        public AccessingNonReadableMemmory(string message) : base(message)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_3.Exceptions
{
    class AccessingToNonReadableMemmory : Exception
    {
        public AccessingToNonReadableMemmory(string message) : base(message)
        {
        }
    }
}

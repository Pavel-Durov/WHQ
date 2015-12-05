using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Exceptions
{
    class AccessingNonReadableMemmory : Exception
    {
        public AccessingNonReadableMemmory(string message) : base(message)
        {
        }
    }
}

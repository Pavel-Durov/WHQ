using System;

namespace Assignments.Core.Exceptions
{
    class AccessingNonReadableMemmory : Exception
    {
        public AccessingNonReadableMemmory(string message) : base(message)
        {
        }
    }
}

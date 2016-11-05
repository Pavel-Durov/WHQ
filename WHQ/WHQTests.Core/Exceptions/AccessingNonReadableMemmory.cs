using System;

namespace WHQ.Core.Exceptions
{
    class AccessingNonReadableMemmory : Exception
    {
        public AccessingNonReadableMemmory(string message) : base(message)
        {
        }
    }
}

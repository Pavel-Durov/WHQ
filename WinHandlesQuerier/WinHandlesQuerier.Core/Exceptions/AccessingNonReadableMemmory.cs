using System;

namespace WinHandlesQuerier.Core.Exceptions
{
    class AccessingNonReadableMemmory : Exception
    {
        public AccessingNonReadableMemmory(string message) : base(message)
        {
        }
    }
}

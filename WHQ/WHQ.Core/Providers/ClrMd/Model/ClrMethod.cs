using System;

namespace WHQ.Core.Providers.ClrMd.Model
{
    public class ClrMethod
    {
        public ulong NativeCode { get; internal set; }

        internal string GetFullSignature()
        {
            throw new NotImplementedException();
        }
    }
}
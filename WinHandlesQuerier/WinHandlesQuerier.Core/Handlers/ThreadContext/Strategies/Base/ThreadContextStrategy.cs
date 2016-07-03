using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;
using System;
using WinHandlesQuerier.Core.msos;
using WinNativeApi;

namespace WinHandlesQuerier.Core.Handlers.ThreadContext.Strategies
{
    internal abstract class ThreadContextStrategy
    {
        #region Constants

        protected const uint CONTEXT_SIZE_AMD64 = 0x4d0;
        protected const uint CONTEXT_SIZE_X86 = 0x2d0;
        protected const uint CONTEXT_SIZE_ARM = 0x1a0;

        #endregion

        public ThreadContextStrategy(uint contextSize)
        {
            ContextSize = contextSize;
        }

        public uint ContextSize { get; protected set; }

        public abstract bool GetThreadContext(ThreadInfo threadInfo, IDebugAdvanced debugClient, IDataReader dataReader);

        protected uint SetCurrentThreadId(uint engineThreadId, IDebugAdvanced debugClient)
        {
            IDebugSystemObjects sytemObject = ((IDebugSystemObjects)debugClient);
            uint set_h_result = (uint)sytemObject.SetCurrentThreadId(engineThreadId);

            if (set_h_result == DbgEng.E_NOINTERFACE)
            {
                throw new InvalidOperationException("No thread with the specified ID was found.");
            }
            return set_h_result;
        }

        protected uint GetThreadContextSize(IDataReader dataReader)
        {
            uint result = 0;
            var plat = dataReader.GetArchitecture();

            if (plat == Architecture.Amd64)
                result = CONTEXT_SIZE_AMD64;
            else if (plat == Architecture.X86)
                result = CONTEXT_SIZE_X86;
            else if (plat == Architecture.Arm)
                result = CONTEXT_SIZE_ARM;
            else
                throw new InvalidOperationException("Unexpected architecture.");

            return result;
        }
    }
}

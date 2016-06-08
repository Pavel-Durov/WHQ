using System;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;
using WinHandlesQuerier.Core.msos;
using System.Runtime.InteropServices;
using WinNativeApi;

namespace Assignments.Core.Handlers.ThreadContext.Strategies
{
    class ThreadContext_x86_Strategy : ThreadContextStrategy
    {
        public ThreadContext_x86_Strategy() : base(CONTEXT_SIZE_X86)
        {

        }

        public override bool GetThreadContext(ThreadInfo threadInfo, IDebugAdvanced debugClient, IDataReader dataReader)
        {
            bool result = false;
            var plat = dataReader.GetArchitecture();

            if (plat != Architecture.Amd64)
            {
                throw new InvalidOperationException("Unexpected architecture.");
            }

            byte[] contextBytes = new byte[GetThreadContextSize(dataReader)];

            if (SetCurrentThreadId(threadInfo.EngineThreadId, debugClient) == DbgEng.S_OK)
            {
                var gch = GCHandle.Alloc(contextBytes, GCHandleType.Pinned);

                var h_result = ((IDebugAdvanced)debugClient).GetThreadContext(gch.AddrOfPinnedObject(), (uint)contextBytes.Length);

                if (h_result == DbgEng.S_OK)
                {
                    try
                    {
                        var structure = Marshal.PtrToStructure<CONTEXT>(gch.AddrOfPinnedObject());
                        threadInfo.ContextStruct = new Model.UnifiedThreadContext(structure, threadInfo);
                        result = true;
                    }
                    finally
                    {
                        gch.Free();
                    }
                }

            }

            return result;
        }
    }
}

using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;
using System;
using System.Runtime.InteropServices;
using WinHandlesQuerier.Core.msos;
using WinNativeApi;
using WinNativeApi.WinNT;

namespace Assignments.Core.Handlers
{
    public class ThreadContextHandler
    {
        const uint CONTEXT_SIZE_AMD64 = 0x4d0;
        const uint CONTEXT_SIZE_X86 = 0x2d0;
        const uint CONTEXT_SIZE_ARM = 0x1a0;


        public static unsafe bool GetThreadContext(ThreadInfo threadInfo, IDebugAdvanced debugClient, IDataReader dataReader)
        {
            bool result = false;
            var plat = dataReader.GetArchitecture();

            //Currently supporting only 64Bit 
            //TODO: Create the sa,me logic for 32Bit
            if (plat == Architecture.Amd64)
            {
                byte[] contextBytes = new byte[GetThreadContextSize(dataReader)];

                if (SetCurrentThreadId(threadInfo.EngineThreadId, debugClient) == DbgEng.S_OK)
                {
                    var gch = GCHandle.Alloc(contextBytes, GCHandleType.Pinned);

                    var h_result = ((IDebugAdvanced)debugClient).GetThreadContext(gch.AddrOfPinnedObject(), (uint)contextBytes.Length);

                    if (h_result == DbgEng.S_OK)
                    {
                        try
                        {
                            var structure = Marshal.PtrToStructure<CONTEXT_AMD64>(gch.AddrOfPinnedObject());
                            threadInfo.ContextStruct = new Model.UnifiedThreadContext(structure);
                            result = true;
                        }
                        finally
                        {
                            gch.Free();
                        }
                    }
                }
            }

            

            return result;
        }

        private static uint SetCurrentThreadId(uint engineThreadId, IDebugAdvanced debugClient)
        {
            IDebugSystemObjects sytemObject = ((IDebugSystemObjects)debugClient);
            uint set_h_result = (uint)sytemObject.SetCurrentThreadId(engineThreadId);

            if (set_h_result == DbgEng.E_NOINTERFACE)
            {
                throw new InvalidOperationException("No thread with the specified ID was found.");
            }
            return set_h_result;
        }


        private static uint GetThreadContextSize(IDataReader dataReader)
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

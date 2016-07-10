using System.Collections.Generic;
using WinHandlesQuerier.Core.Model.Unified;
using WinHandlesQuerier.Core.msos;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;
using System;
using WinHandlesQuerier.Core.Handlers.MiniDump;
using System.Threading.Tasks;

namespace WinHandlesQuerier.Core.Handlers.StackAnalysis.Strategies
{
    internal class DumpFileQuerierStrategy : ProcessQuerierStrategy
    {
        public DumpFileQuerierStrategy(string dumpFilePath, ClrRuntime runtime, IDebugClient debugClient, IDataReader dataReader) 
            : base(debugClient, dataReader, runtime)
        {
            _miniDump = new MiniDump.MiniDumpHandler(dumpFilePath);
        }

        MiniDump.MiniDumpHandler _miniDump;


        public MiniDumpSystemInfo SystemInfo => _miniDump.GetSystemInfo().Result;

        public override CPUArchitecture CPUArchitechture
        {
            get
            {
                CPUArchitecture architechture;

                var systemInfo = SystemInfo;

                if (systemInfo.ProcessorArchitecture == DbgHelp.MiniDumpProcessorArchitecture.PROCESSOR_ARCHITECTURE_INTEL)
                {
                    architechture = CPUArchitecture.x86;
                }
                else if (systemInfo.ProcessorArchitecture == DbgHelp.MiniDumpProcessorArchitecture.PROCESSOR_ARCHITECTURE_AMD64)
                {
                    architechture = CPUArchitecture.x64;
                }
                else
                {
                    throw new InvalidOperationException("Unexpected architecture.");
                }
                return architechture;
            }
        }

        public override async Task<List<UnifiedBlockingObject>> GetUnmanagedBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            var handles = await _miniDump.GetHandles();
            return _unmanagedBlockingObjectsHandler.GetUnmanagedBlockingObjects(thread, unmanagedStack, runtime, handles);
        }


    }
}

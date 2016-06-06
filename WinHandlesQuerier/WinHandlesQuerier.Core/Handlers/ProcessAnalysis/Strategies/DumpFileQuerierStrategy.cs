using System.Collections.Generic;
using WinHandlesQuerier.Core.Model.Unified;
using WinHandlesQuerier.Core.msos;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;
using System;

namespace WinHandlesQuerier.Core.Handlers.StackAnalysis.Strategies
{
    public class DumpFileQuerierStrategy : ProcessQuerierStrategy
    {
        public DumpFileQuerierStrategy(string dumpFilePath, ClrRuntime runtime, IDebugClient debugClient, IDataReader dataReader) 
            : base(debugClient, dataReader, runtime)
        {
            _miniDump = new MiniDump.MiniDumpHandler(dumpFilePath);
        }

        MiniDump.MiniDumpHandler _miniDump;

        public override CPUArchitecture CPUArchitechture
        {
            get
            {
                CPUArchitecture architechture;

                var systemInfo = _miniDump.GetSystemInfo();

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

        public override List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            var miniDumpHandles = _miniDump.GetHandles();

            List<UnifiedBlockingObject> result = new List<UnifiedBlockingObject>();

            result.AddRange(base.GetUnmanagedBlockingObjects(unmanagedStack));

            foreach (var item in miniDumpHandles)
            {
                result.Add(new UnifiedBlockingObject(item));
            }

            CheckForCriticalSections(result, unmanagedStack, runtime);

            return result;
        }


    }
}

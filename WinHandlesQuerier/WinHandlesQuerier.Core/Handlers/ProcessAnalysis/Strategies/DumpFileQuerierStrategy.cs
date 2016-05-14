using System.Collections.Generic;
using WinHandlesQuerier.Core.Model.Unified;
using WinHandlesQuerier.Core.msos;
using Microsoft.Diagnostics.Runtime;

namespace WinHandlesQuerier.Core.Handlers.StackAnalysis.Strategies
{
    public class DumpFileQuerierStrategy : ProcessQuerierStrategy
    {

        ClrRuntime _runtime;

        public DumpFileQuerierStrategy(string dumpFilePath, ClrRuntime runtime)
        {
            _miniDump = new MiniDump.MiniDumpHandler(dumpFilePath);
            _runtime = runtime;
        }

        MiniDump.MiniDumpHandler _miniDump;

        public override List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            var miniDumpHandles = _miniDump.GetHandles();

            List<UnifiedBlockingObject> result = null;

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

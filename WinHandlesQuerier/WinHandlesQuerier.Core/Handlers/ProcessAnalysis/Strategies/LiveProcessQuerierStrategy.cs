using System.Collections.Generic;
using WinHandlesQuerier.Core.Model.Unified;
using WinHandlesQuerier.Core.msos;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.WCT;
using Microsoft.Diagnostics.Runtime.Interop;
using System;
using System.Threading.Tasks;

namespace WinHandlesQuerier.Core.Handlers.StackAnalysis.Strategies
{
    internal class LiveProcessQuerierStrategy : ProcessQuerierStrategy
    {
        public LiveProcessQuerierStrategy(IDebugClient debugClient, IDataReader dataReader, ClrRuntime runtime)
            : base(debugClient, dataReader, runtime)
        {
            _wctApi = new WctHandler();
        }

        WctHandler _wctApi;

        public override CPUArchitecture CPUArchitechture
        {
            get
            {
                CPUArchitecture architecture;
                var plat = _dataReader.GetArchitecture();

                if (plat == Architecture.Amd64)
                    architecture = CPUArchitecture.x64;
                else if (plat == Architecture.X86)
                    architecture = CPUArchitecture.x86;
                else
                    throw new InvalidOperationException("Unexpected architecture.");

                return architecture;
            }
        }

        public override async Task<List<UnifiedBlockingObject>> GetUnmanagedBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            return await Task<List<UnifiedBlockingObject>>.Run(() =>
            {
                ThreadWCTInfo wct_threadInfo = _wctApi.GetBlockingObjects(thread.OSThreadId);
                return _unmanagedBlockingObjectsHandler.GetUnmanagedBlockingObjects(wct_threadInfo, unmanagedStack);
            });
        }

        public override void Dispose()
        {
            //WctHandler is stateless, nothing to dispose here
        }
    }
}

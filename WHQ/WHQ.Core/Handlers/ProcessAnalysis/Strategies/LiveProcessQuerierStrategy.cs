using System.Collections.Generic;
using WHQ.Core.Model.Unified;
using WHQ.Core.msos;
using WHQ.Core.Model.WCT;
using System;
using System.Threading.Tasks;
using WHQ.Core.Providers.ClrMd.Model;
using WHQ.Providers.ClrMd.Model;

namespace WHQ.Core.Handlers.StackAnalysis.Strategies
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

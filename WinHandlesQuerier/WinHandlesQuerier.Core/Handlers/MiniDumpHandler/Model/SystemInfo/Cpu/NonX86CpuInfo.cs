using WinHandlesQuerier.Core.WinApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHandlesQuerier.Core.Handlers.MiniDumpHandler.SystemInfo.Cpu
{
    public class NonX86CpuInfo
    {
        private DbgHelp.CPU_INFORMATION _cpuInfo;
        private ulong[] _processorFeatures;

        internal unsafe NonX86CpuInfo(DbgHelp.CPU_INFORMATION cpuInfo)
        {
            _cpuInfo = cpuInfo;

            _processorFeatures = new ulong[2];
            _processorFeatures[0] = cpuInfo.ProcessorFeatures[0];
            _processorFeatures[1] = cpuInfo.ProcessorFeatures[1];
        }

        public ulong[] ProcessorFeatures { get { return this._processorFeatures; } }
    }

}

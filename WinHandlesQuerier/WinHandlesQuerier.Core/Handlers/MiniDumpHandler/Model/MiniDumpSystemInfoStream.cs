using WinHandlesQuerier.Core.Handlers.MiniDumpHandler.SystemInfo.Cpu;
using WinHandlesQuerier.Core.WinApi;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WinHandlesQuerier.Core.Handlers.MiniDump
{
    public class MiniDumpSystemInfo
    {
        private DbgHelp.MINIDUMP_SYSTEM_INFO _systemInfo;
        public X86CpuInfo X86CpuInfo { get; private set; }
        public NonX86CpuInfo OtherCpuInfo { get; private set; }
        public bool IsX86 { get; set; }

        internal MiniDumpSystemInfo(DbgHelp.MINIDUMP_SYSTEM_INFO systemInfo)
        {
            _systemInfo = systemInfo;

            IsX86 = this.ProcessorArchitecture == DbgHelp.MiniDumpProcessorArchitecture.PROCESSOR_ARCHITECTURE_INTEL;
            if (IsX86)
            {   
                X86CpuInfo = new X86CpuInfo(_systemInfo.Cpu);
            }
            else
            {
                OtherCpuInfo = new NonX86CpuInfo(_systemInfo.Cpu);
            }
        }

        public DbgHelp.MiniDumpProcessorArchitecture ProcessorArchitecture { get { return (DbgHelp.MiniDumpProcessorArchitecture)_systemInfo.ProcessorArchitecture; } }

        public ushort ProcessorLevel { get { return _systemInfo.ProcessorLevel; } }

        public ushort ProcessorRevision { get { return _systemInfo.ProcessorRevision; } }

        public DbgHelp.MiniDumpProductType ProductType { get { return (DbgHelp.MiniDumpProductType)_systemInfo.ProductType; } }

        public uint MajorVersion { get { return _systemInfo.MajorVersion; } }

        public uint MinorVersion { get { return _systemInfo.MinorVersion; } }

        public uint BuildNumber { get { return _systemInfo.BuildNumber; } }

        public DbgHelp.MiniDumpPlatform PlatformId { get { return (DbgHelp.MiniDumpPlatform)_systemInfo.PlatformId; } }

    }
}

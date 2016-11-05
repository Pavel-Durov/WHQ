using System;
using DbgHelp;

namespace WHQ.Core.Model.MiniDump
{
    public class MiniDumpHandleInfo
    {
        public MiniDumpHandleInfo(MINIDUMP_HANDLE_OBJECT_INFORMATION info)
        {
            this.NextInfoRva = info.NextInfoRva;
            this.SizeOfInfo = info.SizeOfInfo;
            this.InfoType = info.InfoType;
        }

        public uint NextInfoRva { get; private set; }
        public MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE InfoType { get; private set; }
        public UInt32 SizeOfInfo { get; private set; }
    }
}

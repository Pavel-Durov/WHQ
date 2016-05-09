using WinHandlesQuerier.Core.WinApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHandlesQuerier.Core.Model.MiniDump
{
    public class MiniDumpHandleInfo
    {
        public MiniDumpHandleInfo(DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION info)
        {
            this.NextInfoRva = info.NextInfoRva;
            this.SizeOfInfo = info.SizeOfInfo;
            this.InfoType = info.InfoType;
        }

        public uint NextInfoRva { get; private set; }
        public DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE InfoType { get; private set; }
        public UInt32 SizeOfInfo { get; private set; }
    }
}

using System;
using DbgHelp;

namespace WinHandlesQuerier.Core.Model.MiniDump
{
    public class DumpHandleInfo
    {
        internal DumpHandleInfo(MINIDUMP_HANDLE_OBJECT_INFORMATION info, uint rva)
        {
            this.SizeOfInfo = info.SizeOfInfo;
            this.InfoType = (DumpHandleType)info.InfoType;
            this.Rva = rva;
        }

        /// <summary>
        /// Rva address of MINIDUMP_HANDLE_OBJECT_INFORMATION  structure
        /// </summary>
        public uint Rva { get; private set; }
        /// <summary>
        /// Type of object 
        /// </summary>
        internal DumpHandleType InfoType { get; private set; }

        /// <summary>
        /// Size of object information
        /// </summary>
        internal UInt32 SizeOfInfo { get; private set; }
    }
}

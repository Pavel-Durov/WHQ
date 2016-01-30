﻿using Assignments.Core.WinApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Model.MiniDump
{
    public class MiniDumpHandleInfo
    {
        public MiniDumpHandleInfo(DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION info)
        {
            this.NextInfoRva = info.NextInfoRva;
            this.SizeOfInfo = info.SizeOfInfo;
            this.InfoType = (DbgHelp.MiniDumpHandleObjectInformationType)info.InfoType;
        }

        public uint NextInfoRva { get; private set; }
        public DbgHelp.MiniDumpHandleObjectInformationType InfoType { get; private set; }
        public UInt32 SizeOfInfo { get; private set; }
    }
}

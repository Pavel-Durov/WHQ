using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.WinApi;

namespace Assignments.Core.Model.MiniDump
{
    public class MiniDumpHandle
    {
        public MiniDumpHandle(DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR handleDescriptor)
        {
            Handle = handleDescriptor.Handle;
            HandleCount = handleDescriptor.HandleCount;
            ObjectNameRva = handleDescriptor.ObjectNameRva;
            PointerCount = handleDescriptor.PointerCount;
            TypeNameRva = handleDescriptor.TypeNameRva;
            Attributes = handleDescriptor.Attributes;
            GrantedAccess = handleDescriptor.GrantedAccess;
        }

        public MiniDumpHandle(DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR_2 handleDescriptor)
        {
            Handle = handleDescriptor.Handle;
            HandleCount = handleDescriptor.HandleCount;
            ObjectNameRva = handleDescriptor.ObjectNameRva;
            PointerCount = handleDescriptor.PointerCount;
            TypeNameRva = handleDescriptor.TypeNameRva;
            Attributes = handleDescriptor.Attributes;
            GrantedAccess = handleDescriptor.GrantedAccess;
        }

        public ulong Handle { get; private set; }
        public uint HandleCount { get; private set; }
        public uint ObjectNameRva { get; private set; }
        public uint PointerCount { get; private set; }
        public uint TypeNameRva { get; private set; }
        public uint Attributes { get; private set; }
        public uint GrantedAccess { get; private set; }


    }
}

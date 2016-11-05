using System;
using WinHandlesQuerier.Core.Model.Unified;
using DbgHelp;
using System.Collections.Generic;

namespace WinHandlesQuerier.Core.Model.MiniDump
{
    public enum DumpHandleType
    {
        NONE,
        THREAD,
        MUTEX1,
        MUTEX2,
        PROCESS1,
        PROCESS2,
        EVENT,
        SECTION,
        TYPE_MAX
    }


    /// <summary>
    /// Represents a dump handle wnd its information
    /// </summary>
    public class DumpHandle
    {
        /// <summary>
        /// Construct unified minidump descriptor using MINIDUMP_HANDLE_DESCRIPTOR structure
        /// </summary>
        /// <param name="handleDescriptor">MiniDump handle structure</param>
        internal DumpHandle(MINIDUMP_HANDLE_DESCRIPTOR handleDescriptor)
        {
            Handle = handleDescriptor.Handle;
            HandleCount = handleDescriptor.HandleCount;
            ObjectNameRva = handleDescriptor.ObjectNameRva;
            PointerCount = handleDescriptor.PointerCount;
            TypeNameRva = handleDescriptor.TypeNameRva;
            Attributes = handleDescriptor.Attributes;
            GrantedAccess = handleDescriptor.GrantedAccess;
        }

        /// <summary>
        ///  Construct unified minidump descriptor using MINIDUMP_HANDLE_DESCRIPTOR_2 structure
        /// </summary>
        /// <param name="handleDescriptor">MiniDump handle structure</param>
        internal DumpHandle(MINIDUMP_HANDLE_DESCRIPTOR_2 handleDescriptor)
        {
            Handle = handleDescriptor.Handle;
            HandleCount = handleDescriptor.HandleCount;
            ObjectNameRva = handleDescriptor.ObjectNameRva;
            PointerCount = handleDescriptor.PointerCount;
            TypeNameRva = handleDescriptor.TypeNameRva;
            Attributes = handleDescriptor.Attributes;
            GrantedAccess = handleDescriptor.GrantedAccess;
            ObjectInfoRva = handleDescriptor.ObjectInfoRva;
        }

        internal DumpHandle(MINIDUMP_HANDLE_DESCRIPTOR_2 handleDescriptor, string objectName, string typeName) : this(handleDescriptor)
        {
            ObjectName = objectName;
            TypeName = typeName;
        }

        internal DumpHandle(MINIDUMP_HANDLE_DESCRIPTOR handleDescriptor, string objectName, string typeName) : this(handleDescriptor)
        {
            this.ObjectName = objectName;
            this.TypeName = typeName;
        }

        /// <summary>
        /// Name of Minidump handle object 
        /// </summary>
        public string ObjectName { get; private set; }
        /// <summary>
        /// Name of Minidump handle type object 
        /// </summary>
        public string TypeName { get; private set; }
        /// <summary>
        /// Handle address
        /// </summary>
        public ulong Handle { get; private set; }
        /// <summary>
        /// Amount of handles
        /// </summary>
        public uint HandleCount { get; private set; }
        /// <summary>
        /// An RVA to a MINIDUMP_STRING structure that specifies the object name of the handle. This member can be 0.
        /// </summary>
        public Int32 ObjectNameRva { get; private set; }
        /// <summary>
        /// This is the number kernel references to the object that this handle refers to. 
        /// </summary>
        public uint PointerCount { get; private set; }
        /// <summary>
        /// An RVA to a MINIDUMP_STRING structure that specifies the object type name of the handle. This member can be 0.
        /// </summary>
        public Int32 TypeNameRva { get; private set; }
        /// <summary>
        /// /The attributes for the handle, this corresponds to OBJ_INHERIT, OBJ_CASE_INSENSITIVE, etc.
        /// </summary>
        public uint Attributes { get; private set; }
        /// <summary>
        /// The meaning of this member depends on the handle type and the operating system.
        /// </summary>
        public uint GrantedAccess { get; private set; }
        /// <summary>
        ///  An RVA to a MINIDUMP_HANDLE_OBJECT_INFORMATION sttructure
        /// </summary>
        public Int32 ObjectInfoRva { get; private set; }
        /// <summary>
        /// Type of MiniDump handle
        /// </summary>
        public DumpHandleType Type { get; set; }
        /// <summary>
        /// </summary>
        public uint OwnerProcessId { get; internal set; }
        /// <summary>
        /// Handle owner thread id
        /// </summary>
        public uint OwnerThreadId { get; internal set; }
        /// <summary>
        /// Unknown mutex fields
        /// </summary>
        public MutexUnknownFields MutexUnknown { get; internal set; }

        /// <summary>
        /// Additional minidump handle
        /// </summary>
        public List<DumpHandleInfo> HandleInfoList { get; private set; }

        internal void AddInfo(MINIDUMP_HANDLE_OBJECT_INFORMATION info, uint rva)
        {
            if (HandleInfoList == null)
            {
                HandleInfoList = new List<DumpHandleInfo>();
            }
            HandleInfoList.Add(new DumpHandleInfo(info, rva));
        }
    }
    public class MutexUnknownFields
    {
        public object Field1 { get; internal set; }
        public object Field2 { get; internal set; }
    }
}

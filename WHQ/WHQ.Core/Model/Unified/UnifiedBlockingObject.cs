using WHQ.Core.Model.Unified.Thread;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using WHQ.Core.Model.WCT;
using WHQ.Core.Model.MiniDump;
using System;
using WinBase;

namespace WHQ.Core.Model.Unified
{
    public enum UnifiedBlockingType
    {
        WaitChainInfoObject, ClrBlockingObject, DumpHandle, CriticalSectionObject, UnmanagedHandleObject
    }

    public enum OriginSource
    {
        WCT, MiniDump, ClrMD, StackWalker, ThreadContextRegisters
    }

    public class UnifiedBlockingObject
    {
        private UnifiedBlockingObject(OriginSource source)
        {
            Origin = source;
        }

        public UnifiedBlockingObject(BlockingObject obj) : this(OriginSource.ClrMD)
        {

            SetOwners(obj);
            SetWaiters(obj);

            Reason = (UnifiedBlockingReason)((int)obj.Reason);
            RecursionCount = obj.RecursionCount;
            ManagedObjectAddress = obj.Object;
            KernelObjectName = null;

            Type = UnifiedBlockingType.ClrBlockingObject;

        }

        internal UnifiedBlockingObject(WaitChainInfoObject obj) : this(OriginSource.WCT)
        {
            KernelObjectName = obj.ObjectName;
            Reason = obj.UnifiedType;
            Type = UnifiedBlockingType.WaitChainInfoObject;
        }

        internal UnifiedBlockingObject(MiniDumpHandle handle)
            : this(OriginSource.MiniDump)
        {
            KernelObjectName = handle.ObjectName;
            KernelObjectTypeName = handle.TypeName;
            //TODO: Convertion
            Reason = ConvertToUnified(handle.Type);
            Type = UnifiedBlockingType.DumpHandle;
            Handle = handle.Handle;
        }

        internal UnifiedBlockingObject(CRITICAL_SECTION section, ulong handle)
            : this(OriginSource.StackWalker)
        {
            Owners = new List<UnifiedThread>();
            Owners.Add(new UnifiedThread((uint)section.OwningThread));
            Reason = UnifiedBlockingReason.CriticalSection;
            Type = UnifiedBlockingType.CriticalSectionObject;
            Handle = handle;
        }

        public UnifiedBlockingObject(ulong handle, string objectName, string objectType)
            : this(OriginSource.StackWalker)
        {
            Owners = new List<UnifiedThread>();
            Handle = handle;
            KernelObjectName = objectName;
            KernelObjectTypeName = objectType;
            Type = UnifiedBlockingType.UnmanagedHandleObject;
            Reason = ConvertToUnified(objectType);
        }

        public UnifiedBlockingObject(ulong handle, UnifiedBlockingType type)
            : this(OriginSource.ThreadContextRegisters)
        {
            Handle = handle;
            Type = type;
        }

        private void SetWaiters(BlockingObject item)
        {
            if (item.Waiters?.Count > 0)
            {
                Owners = new List<UnifiedThread>();
                foreach (var waiter in item.Waiters)
                {
                    this.Owners.Add(new UnifiedManagedThread(waiter));
                }
            }
        }

        private void SetOwners(BlockingObject item)
        {
            if (item.Owners?.Count > 0)
            {
                Owners = new List<UnifiedThread>();
                foreach (var owner in item.Owners)
                {
                    if (owner != null)
                    {
                        this.Owners.Add(new UnifiedManagedThread(owner));
                    }
                }
            }
        }

        public OriginSource Origin { get; private set; }
        public UnifiedBlockingType Type { get; private set; }

        internal List<UnifiedThread> Owners { get; private set; }

        public bool HasOwnershipInformation { get { return Owners != null && Owners.Count > 0; } }

        public UnifiedBlockingReason Reason { get; private set; } = UnifiedBlockingReason.Unknown;

        internal List<UnifiedThread> Waiters { get; private set; }

        public int RecursionCount { get; private set; }

        public ulong ManagedObjectAddress { get; private set; }

        public string KernelObjectName { get; private set; }

        public string KernelObjectTypeName { get; private set; }
        public ulong Handle { get; private set; }

        public const int BLOCK_REASON_WCT_SECTION_START_INDEX = 9;

        private static UnifiedBlockingReason ConvertToUnified(string objectType)
        {
            UnifiedBlockingReason result = UnifiedBlockingReason.Unknown;

            switch (objectType)
            {
                case "Thread": result = UnifiedBlockingReason.Thread; break;
                case "Job": result = UnifiedBlockingReason.Job; break;
                case "File": result = UnifiedBlockingReason.File; break;
                case "Semaphore": result = UnifiedBlockingReason.Semaphore; break;
                case "Mutex": result = UnifiedBlockingReason.Mutex; break;
                case "Section": result = UnifiedBlockingReason.CriticalSection; break;
                case "Mutant": result = UnifiedBlockingReason.Mutex; break;
                case "ALPC Port": result = UnifiedBlockingReason.Alpc; break;
                case "Process": result = UnifiedBlockingReason.ProcessWait; break;
                case "Unknown": result = UnifiedBlockingReason.Unknown; break;
                case "None": result = UnifiedBlockingReason.None; break;
                case "Timer": result = UnifiedBlockingReason.Timer; break;
                case "Event": result = UnifiedBlockingReason.Event; break;
                    //case "Callback": break;
                    //case "Desktop": break;
                    //case "Key": break;
                    //case "IoCompletion": break;
                    //case "Directory": break;
                    //case "WindowStation": break;
                    //case "WaitCompletionPacket": break;
                    //case "TpWorkerFactory": break;
                    //case "Timer": break;
            }
            return result;
        }

        UnifiedBlockingReason ConvertToUnified(MiniDumpHandleType type)
        {
            UnifiedBlockingReason result = UnifiedBlockingReason.Unknown;

            switch (type)
            {
                case MiniDumpHandleType.NONE: result = UnifiedBlockingReason.None; break;
                case MiniDumpHandleType.THREAD: result = UnifiedBlockingReason.Thread; break;
                case MiniDumpHandleType.MUTEX1: result = UnifiedBlockingReason.Mutex; break;
                case MiniDumpHandleType.MUTEX2: result = UnifiedBlockingReason.Mutex; break;
                case MiniDumpHandleType.PROCESS1: result = UnifiedBlockingReason.ProcessWait; break;
                case MiniDumpHandleType.PROCESS2: result = UnifiedBlockingReason.ProcessWait; break;
                case MiniDumpHandleType.EVENT: result = UnifiedBlockingReason.ThreadWait; break;
                case MiniDumpHandleType.SECTION: result = UnifiedBlockingReason.MemorySection; break;
            }

            return result;
        }

    }


    public enum UnifiedBlockingReason
    {
        //Based on ClrThread BlockingReason Enumerations
        None = 0,
        Unknown = 1,
        Monitor = 2,
        MonitorWait = 3,
        WaitOne = 4,
        WaitAll = 5,
        WaitAny = 6,
        ThreadJoin = 7,
        ReaderAcquired = 8,
        WriterAcquired = 9,

        //Based on WCT_OBJECT_TYPE Enumerations
        CriticalSection = 20,
        SendMessage = 11,
        Mutex = 12,
        Alpc = 13,
        Com = 14,
        ThreadWait = 15,
        ProcessWait = 16,
        Thread = 17,
        ComActivation = 18,
        UnknownType = Unknown,
        File = 19,
        Job = 20,
        Semaphore = 21,

        Event = 22,        //An object which encapsulates some information, to be used for notifying processes of something.
        Timer = 23,
        MemorySection = 24
    }


}

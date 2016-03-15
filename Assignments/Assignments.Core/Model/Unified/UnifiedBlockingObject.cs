using Assignments.Core.Model.Unified.Thread;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Model.WCT;
using Assignments.Core.Model.MiniDump;

namespace Assignments.Core.Model.Unified
{
    public enum UnifiedBlockingType
    {
        WaitChainInfoObject, ClrBlockingObject, MiniDumpHandle
    }
    public enum OriginSource
    {
        WCT, MiniDump, ClrMD
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

            WaitReason = (UnifiedBlockingReason)((int)obj.Reason);
            RecursionCount = obj.RecursionCount;
            ManagedObjectAddress = obj.Object;
            KernelObjectName = null;

            Type = UnifiedBlockingType.ClrBlockingObject;

        }


        public UnifiedBlockingObject(WaitChainInfoObject obj) : this(OriginSource.WCT)
        {
            KernelObjectName = obj.ObjectName;
            WaitReason = obj.UnifiedType;
            Type = UnifiedBlockingType.WaitChainInfoObject;
        }

        public UnifiedBlockingObject(MiniDumpHandle handle) : this(OriginSource.MiniDump)
        {
            KernelObjectName = handle.ObjectName;
            KernelObjectTypeName = handle.TypeName;
            WaitReason = handle.UnifiedType;
            Type = UnifiedBlockingType.MiniDumpHandle;
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

        public List<UnifiedThread> Owners { get; private set; }

        public bool HasOwnershipInformation { get { return Owners != null && Owners.Count > 0; } }

        public UnifiedBlockingReason WaitReason { get; private set; }

        public List<UnifiedThread> Waiters { get; private set; }

        public int RecursionCount { get; private set; }

        public ulong ManagedObjectAddress { get; private set; }

        public string KernelObjectName { get; private set; }

        public string KernelObjectTypeName { get; private set; }

        public const int BLOCK_REASON_WCT_SECTION_START_INDEX = 9;
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
        CriticalSectionType = 20,
        SendMessageType = 11,
        MutexType = 12,
        AlpcType = 13,
        ComType = 14,
        ThreadWaitType = 15,
        ProcessWaitType = 16,
        ThreadType = 17,
        ComActivationType = 18,
        UnknownType = 19
    }
}

using Assignments.Core.Model.Unified.Thread;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using System;
using Assignments.Core.Model.WCT;
using Assignments.Core.Model.MiniDump;

namespace Assignments.Core.Model.Unified
{
    public enum UnifiedBlockingType
    {
        WaitChainInfoObject, ClrBlockingObject, MiniDumpHandle
    }

    public class UnifiedBlockingObject
    {
        public UnifiedBlockingObject(BlockingObject obj)
        {
            SetOwners(obj);
            SetWaiters(obj);

            WaitReason = (UnifiedBlockingReason)((int)obj.Reason);
            RecursionCount = obj.RecursionCount;
            ManagedObjectAddress = obj.Object;
            KernelObjectName = null;

            Type = UnifiedBlockingType.ClrBlockingObject;
        }


        public UnifiedBlockingObject(WaitChainInfoObject obj)
        {
            ManagedObjectAddress = 0;
            KernelObjectName = obj.ObjectName;

            var wctIndex = (int)obj.ObjectType;
            WaitReason = (UnifiedBlockingReason)(BLOCK_REASON_WCT_SECTION_START_INDEX + wctIndex);

            Type = UnifiedBlockingType.WaitChainInfoObject;
        }
        
        public UnifiedBlockingObject(MiniDumpHandle item)
        {
            //item.Handle
            KernelObjectName = item.ObjectName;
            KernelObjectTypeName = item.TypeName;

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


        public UnifiedBlockingType Type { get; private set; }

        public List<UnifiedThread> Owners { get; private set; }

        public bool HasOwnershipInformation { get { return Owners != null && Owners.Count > 0; } }

        public UnifiedBlockingReason WaitReason { get; private set; }

        public List<UnifiedThread> Waiters { get; private set; }

        public int RecursionCount { get; private set; }

        public ulong ManagedObjectAddress { get; private set; }

        public string KernelObjectName { get; private set; }

        public string KernelObjectTypeName { get; private set; }

        const int BLOCK_REASON_WCT_SECTION_START_INDEX = 9;
    }


    public enum UnifiedBlockingReason
    {
        //ClrThread BlockingReason Enumerations
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

        // WCT_OBJECT_TYPE Enumerations
        WctCriticalSectionType = 20,
        WctSendMessageType = 11,
        WctMutexType = 12,
        WctAlpcType = 13,
        WctComType = 14,
        WctThreadWaitType = 15,
        WctProcessWaitType = 16,
        WctThreadType = 17,
        WctComActivationType = 18,
        WctUnknownType = 19
    }
}

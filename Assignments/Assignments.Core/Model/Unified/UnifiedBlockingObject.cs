using Assignments.Core.Model.Unified.Thread;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using System;
using Assignments.Core.Model.WCT;

namespace Assignments.Core.Model.Unified
{
    public class UnifiedBlockingObject
    {
        public UnifiedBlockingObject(BlockingObject obj, string name)
        {
            SetOwners(obj);
            SetWaiters(obj);

            WaitReason = (UnifiedBlockingReason)((int)obj.Reason);
            RecursionCount = obj.RecursionCount;
            ManagedObjectAddress = obj.Object;
            KernelObjectName = name;
        }


        public UnifiedBlockingObject(WaitChainInfoObject obj)
        {
            ManagedObjectAddress = 0;
            KernelObjectName = obj.ObjectName;

            var wctIndex = (int)obj.ObjectType;
            WaitReason = (UnifiedBlockingReason)(BLOCK_REASON_WCT_SECTION_START_INDEX + wctIndex);
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
                if (item.HasSingleOwner)
                {
                    Owners.Add(new UnifiedManagedThread(item.Owner));
                }
                else
                {
                    foreach (var owner in item.Owners)
                    {
                        this.Owners.Add(new UnifiedManagedThread(owner));
                    }
                }
            }
        }


        

        public List<UnifiedThread> Owners { get; set; }

        public bool HasOwnershipInformation { get { return Owners != null && Owners.Count > 0; } }

        public UnifiedBlockingReason WaitReason { get; set; }

        public List<UnifiedThread> Waiters { get; set; }

        public int RecursionCount { get; set; }

        public ulong ManagedObjectAddress { get; set; }

        public string KernelObjectName { get; set; }

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

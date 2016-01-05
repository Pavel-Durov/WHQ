using Assignments.Core.Model.Unified.Thread;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using System;
using Assignments.Core.Model.WCT;

namespace Assignments.Core.Model.Unified
{
    public class UnifiedBlockingObject
    {
        public UnifiedBlockingObject(BlockingObject item)
        {
            SetOwners(item);

            SetWaiters(item);

            this.RecursionCount = item.RecursionCount;
            this.ManagedObjectAddress = item.Object;
            //this.KernelObjectName = item.
            this.ManagedObjectAddress = item.Object;

            //this.KernelObjectName = kernelObjectName;
            //TODO: Get Kernel object name
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
                this.HasOwnershipInformation = true;
            }
        }
        



        public UnifiedBlockingObject(WaitChainInfoObject blockingObj)
        {
            //TODO: Complete gathering the inforamtion...

            KernelObjectName = blockingObj.ObjectName;

            //TODO: Get
            ManagedObjectAddress = 0;
            KernelObjectName = blockingObj.ObjectName;
            
        }




        public List<UnifiedThread> Owners { get; set; }

        bool HasOwnershipInformation { get; set; }

        UnifiedBlockingReason WaitReason { get; set; }

        List<UnifiedThread> Waiters { get; set; }

        int RecursionCount { get; set; }

        ulong ManagedObjectAddress { get; set; }

        string KernelObjectName { get; set; }
    }
}

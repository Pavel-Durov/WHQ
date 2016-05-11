﻿using WinHandlesQuerier.Core.Model.Unified.Thread;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.WCT;
using WinHandlesQuerier.Core.Model.MiniDump;
using WinHandlesQuerier.Core.WinApi;
using System;

namespace WinHandlesQuerier.Core.Model.Unified
{
    public enum UnifiedBlockingType
    {
        WaitChainInfoObject, ClrBlockingObject, MiniDumpHandle, CriticalSectionObject, UnmanagedHandleObject
    }

    public enum OriginSource
    {
        WCT, MiniDump, ClrMD, StackWalker
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
            Handle = handle.Handle;
        }

        public UnifiedBlockingObject(WinBase.CRITICAL_SECTION section, uint handle) : this(OriginSource.StackWalker)
        {
            Owners = new List<UnifiedThread>();
            Owners.Add(new UnifiedThread((uint)section.OwningThread));
            WaitReason = UnifiedBlockingReason.CriticalSectionType;
            Type = UnifiedBlockingType.CriticalSectionObject;
            Handle = handle;
        }

        public UnifiedBlockingObject(uint handle, string objectName, string objectType) : this(OriginSource.StackWalker)
        {
            Owners = new List<UnifiedThread>();
            Handle = handle;
            KernelObjectName = objectName;
            KernelObjectTypeName = objectType;
            Type = UnifiedBlockingType.UnmanagedHandleObject;
            WaitReason = ConvertToUnified(objectType);
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

        public UnifiedBlockingReason WaitReason { get; private set; } = UnifiedBlockingReason.Unknown;

        public List<UnifiedThread> Waiters { get; private set; }

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
                case "Thread":
                    result = UnifiedBlockingReason.ThreadType;
                    break;
                case "Job":
                    result = UnifiedBlockingReason.Job;
                    break;
                case "File":
                    result = UnifiedBlockingReason.File;
                    break;
                case "Semaphore":
                    result = UnifiedBlockingReason.Semaphore;
                    break;
                case "Mutex":
                    result = UnifiedBlockingReason.MutexType;
                    break;
                case "Section":
                    result = UnifiedBlockingReason.CriticalSectionType;
                    break;
                case "Mutant":
                    result = UnifiedBlockingReason.MutexType;
                    break;
                case "ALPC Port":
                    result = UnifiedBlockingReason.AlpcType;
                    break;
                case "Process":
                    result = UnifiedBlockingReason.ProcessWaitType;
                    break;
                case "Unknown":
                    result = UnifiedBlockingReason.Unknown;
                    break;
                case "None":
                    result = UnifiedBlockingReason.None;
                    break;
                case "Timer":
                    result = UnifiedBlockingReason.Timer;
                    break;
                case "Event":
                    result = UnifiedBlockingReason.Event;
                    break;
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
        UnknownType = Unknown,
        File = 19,
        Job = 20,
        Semaphore = 21,

        Event = 22,        //An object which encapsulates some information, to be used for notifying processes of something.
        Timer = 23
    }


}

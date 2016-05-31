using Assignments.Core.Infra;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;
using System;
using System.Collections.Generic;
using WinHandlesQuerier.Core.Handlers;
using WinHandlesQuerier.Core.Model.Unified;

namespace Assignments.Core.Handlers.UnmanagedStackFrame.Strategies.Base
{
    internal abstract class UnmanagedStackWalkerStrategy
    {
        public const string WAIT_FOR_SINGLE_OBJECTS_FUNCTION_NAME = "WaitForSingleObject";
        public const string WAIT_FOR_MULTIPLE_OBJECTS_FUNCTION_NAME = "WaitForMultipleObjects";
        public const string ENTER_CRITICAL_SECTION_FUNCTION_NAME = "EnterCriticalSection";

        protected const int ENTER_CRITICAL_SECTION_FUNCTION_PARAM_COUNT = 1;
        protected const int WAIT_FOR_SINGLE_OBJECT_PARAM_COUNT = 2;
        protected const int WAIT_FOR_MULTIPLE_OBJECTS_PARAM_COUNT = 4;

        internal List<UnifiedStackFrame> ConvertToUnified(DEBUG_STACK_FRAME[] stackFrames, uint framesFilled, 
            ClrRuntime runtime, IDebugClient debugClient, IntPtr osThreadId, uint pid = Constants.INVALID_PID)
        {
            List<UnifiedStackFrame> stackTrace = new List<UnifiedStackFrame>();
            for (uint i = 0; i < framesFilled; ++i)
            {
                var frame = new UnifiedStackFrame(stackFrames[i], (IDebugSymbols2)debugClient, osThreadId);
                Inpsect(frame, runtime, pid);
                stackTrace.Add(frame);
            }
            return stackTrace;
        }

        protected void Inpsect(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {

            if (CheckForWinApiCalls(frame, WAIT_FOR_SINGLE_OBJECTS_FUNCTION_NAME))
            {
                DealWithSingle(frame, runtime, pid);
            }
            else if (CheckForWinApiCalls(frame, WAIT_FOR_MULTIPLE_OBJECTS_FUNCTION_NAME))
            {
                DealWithMultiple(frame, runtime, pid);
            }
            else
            {
                //CheckForCriticalSections(frame, runtime, result);
            }
        }

        protected UnifiedHandle GenerateUnifiedHandle(uint handleUint, uint pid)
        {
            UnifiedHandle result;

            if (pid != Constants.INVALID_PID)
            {
                var typeName = NtQueryHandler.GetHandleType((IntPtr)handleUint, pid);
                var handleName = NtQueryHandler.GetHandleObjectName((IntPtr)handleUint, pid);

                result = new UnifiedHandle(handleUint, typeName, handleName);
            }
            else
            {
                result = new UnifiedHandle(handleUint);
            }
            return result;
        }

        public bool CheckForWinApiCalls(UnifiedStackFrame c, string key)
        {
            bool result = c != null
                && !String.IsNullOrEmpty(c.Method)
                && c.Method != null && c.Method.Contains(key);

            return result;
        }

        public bool CheckForCriticalSectionCalls(UnifiedStackFrame frame, ClrRuntime runtime, out UnifiedBlockingObject blockingObject)
        {
            bool result = false;

            if (CheckForWinApiCalls(frame, ENTER_CRITICAL_SECTION_FUNCTION_NAME))
            {
                blockingObject = ReadCriticalSectionData(frame, runtime);
                result = blockingObject != null;
            }
            else
            {
                blockingObject = null;
            }

            return result;
        }


        protected uint Convert(byte[] bits)
        {
            return BitConverter.ToUInt32(bits, 0);
        }

        #region Abstract Methods

        protected abstract void DealWithSingle(UnifiedStackFrame frame, ClrRuntime runtime, uint pid);

        abstract protected void DealWithMultiple(UnifiedStackFrame frame, ClrRuntime runtime, uint pid);

        public abstract List<byte[]> GetNativeParams(UnifiedStackFrame stackFrame, ClrRuntime runtime, int paramCount);

        public abstract List<byte[]> ReadFromMemmory(uint startAddress, uint count, ClrRuntime runtime);

        protected abstract UnifiedBlockingObject ReadCriticalSectionData(UnifiedStackFrame frame, ClrRuntime runtime);

        #endregion

    }
}

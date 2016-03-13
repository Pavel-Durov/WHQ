using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DWORD = System.UInt32;

namespace Assignments.Core.WinApi
{

#if !(_WIN64)

    public struct THREAD_ADDITIONAL_INFO
    {
        public DWORD Unknown1;
        public DWORD Unknown2;
        public DWORD ProcessId;
        public DWORD ThreadId;
        public DWORD Unknown3;
        public DWORD Priority;
        public DWORD Unknown4;
    };

    public struct MUTEX_ADDITIONAL_INFO_1
    {
        public DWORD Unknown1;
        public DWORD Unknown2;
    };

    public struct MUTEX_ADDITIONAL_INFO_2
    {
        public DWORD OwnerProcessId;
        public DWORD OwnerThreadId;
    };

    public struct PROCESS_ADDITIONAL_INFO_2
    {
        public DWORD Unknown1;
        public DWORD Unknown2;
        public DWORD Unknown3;
        public DWORD Unknown4;
        public DWORD Unknown5;
        public DWORD ProcessId;
        public DWORD ParentProcessId;
        public DWORD Unknown6;
    };

#else

    struct MUTEX_ADDITIONAL_INFO_2
    {
        DWORD OwnerProcessId;
        DWORD Unknown1;
        DWORD OwnerThreadId;
        DWORD Unknown2;
    };

    struct MUTEX_ADDITIONAL_INFO_1
    {
        DWORD Unknown1;
        DWORD Unknown2;
    };

    struct THREAD_ADDITIONAL_INFO
    {
        DWORD Unknown1;
        DWORD Unknown2;
        DWORD Unknown3;
        DWORD Unknown4;
        DWORD ProcessId;
        DWORD Unknown5;
        DWORD ThreadId;
        DWORD Unknown6;
        DWORD Unknown7;
        DWORD Unknown8;
        DWORD Priority;
        DWORD Unknown9;
    };

    struct PROCESS_ADDITIONAL_INFO_2
    {
        DWORD Unknown1;
        DWORD Unknown2;
        DWORD Unknown3;
        DWORD Unknown4;
        DWORD Unknown5;
        DWORD Unknown6;
        DWORD Unknown7;
        DWORD Unknown8;
        DWORD BasePriority;
        DWORD Unknown10;
        DWORD ProcessId;
        DWORD Unknown12;
        DWORD ParentProcessId;
        DWORD Unknown14;
        DWORD Unknown15;
        DWORD Unknown16;
    };

#endif


}

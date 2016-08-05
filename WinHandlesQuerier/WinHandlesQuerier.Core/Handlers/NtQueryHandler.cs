using System;
using System.Runtime.InteropServices;
using NtDll;

namespace WinHandlesQuerier.Core.Handlers
{
    internal class NtQueryHandler
    {
        /// <summary>
        /// Gets Handle Type (String type name) using NtQueryObject NtDll function
        /// Doc: https://msdn.microsoft.com/en-us/library/bb432383(v=vs.85).aspx
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static unsafe string GetHandleType(IntPtr handle, uint pid)
        {
            string result = null;

            IntPtr handleDuplicate = IntPtr.Zero;

            if (DuplicateHandle(handle, pid, out handleDuplicate))
            {
                int length;

                NtStatus stat = Functions.NtQueryObject(handleDuplicate,
                    OBJECT_INFORMATION_CLASS.ObjectTypeInformation, IntPtr.Zero, 0, out length);

                if (stat != NtStatus.InvalidHandle)
                {
                    IntPtr pointer = default(IntPtr);
                    try
                    {
                        pointer = Marshal.AllocHGlobal((int)length);

                        NtStatus status = Functions.NtQueryObject(handleDuplicate,
                           OBJECT_INFORMATION_CLASS.ObjectTypeInformation, pointer, length, out length);

                        if (status == NtStatus.Success)
                        {
                            var res = Marshal.PtrToStructure<PUBLIC_OBJECT_TYPE_INFORMATION>(pointer);
                            result = res.TypeName.ToString();
                        }
                    }
                    finally
                    {
                        if (pointer != default(IntPtr))
                        {
                            Marshal.FreeHGlobal(pointer);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets Handle Object name using NtQueryObject NtDll function
        /// Doc: https://msdn.microsoft.com/en-us/library/bb432383(v=vs.85).aspx
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static unsafe string GetHandleObjectName(IntPtr handle, uint pid)
        {
            string result = null;

            IntPtr duplicatedHandle = default(IntPtr);

            if (DuplicateHandle(handle, pid, out duplicatedHandle))
            {
                int length;

                NtStatus stat = Functions.NtQueryObject(duplicatedHandle,
                    OBJECT_INFORMATION_CLASS.ObjectNameInformation, IntPtr.Zero, 0, out length);

                if (stat != NtStatus.InvalidHandle)
                {
                    IntPtr pointer = default(IntPtr);
                    try
                    {
                        pointer = Marshal.AllocHGlobal((int)length);

                        NtStatus status = Functions.NtQueryObject(duplicatedHandle,
                               OBJECT_INFORMATION_CLASS.ObjectNameInformation, pointer, length, out length);

                        if (status == NtStatus.Success)
                        {
                            var res = Marshal.PtrToStructure<OBJECT_NAME_INFORMATION>(pointer);
                            result = res.Name.ToString();
                        }
                    }
                    finally
                    {
                        if (pointer != default(IntPtr))
                        {
                            Marshal.FreeHGlobal(pointer);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Perform handle duplication
        /// </summary>
        /// <param name="handle">Handle which needed to be duplicated</param>
        /// <param name="pid">Process PID (handle owner)</param>
        /// <param name="result">Duplicated Handle</param>
        /// <returns></returns>
        private static bool DuplicateHandle(IntPtr handle, uint pid, out IntPtr result)
        {
            result = default(IntPtr);

            var processHandle = Kernel32.Functions.OpenProcess(Kernel32.ProcessAccessFlags.All, true, pid);

            var process = Kernel32.Functions.GetCurrentProcess();
            var options = Kernel32.DuplicateOptions.DUPLICATE_SAME_ACCESS;

            return Kernel32.Functions.DuplicateHandle(processHandle, handle, process, out result, 0, false, options);
        }
    }
}

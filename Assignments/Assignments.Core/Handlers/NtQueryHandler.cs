using Assignments.Core.WinApi.NtDll;
using System;
using System.Runtime.InteropServices;

namespace Assignments.Core.Handlers
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
            IntPtr handleDuplicate = IntPtr.Zero;

            if(!DuplicateHandle(handle, pid, out handleDuplicate))
            {
                return null;
            }

            int length;

            NtDll.NtStatus stat = NtDll.NtQueryObject(handleDuplicate,
                NtDll.OBJECT_INFORMATION_CLASS.ObjectTypeInformation, IntPtr.Zero, 0, out length);

            if (stat == NtDll.NtStatus.InvalidHandle)
                return null;

            return ExecuteSafe<string>(length, (pointer) =>
            {
                string result = string.Empty;

                NtDll.NtStatus status = NtDll.NtQueryObject(handleDuplicate,
                    NtDll.OBJECT_INFORMATION_CLASS.ObjectTypeInformation, pointer, length, out length);

                if (status == NtDll.NtStatus.Success)
                {
                    var res = Marshal.PtrToStructure<NtDll.PUBLIC_OBJECT_TYPE_INFORMATION>(pointer);
                    result = res.TypeName.ToString();
                }
                return result;
            });
        }

        private static bool DuplicateHandle(IntPtr handle, uint pid, out IntPtr handleDuplicate)
        {
            bool result = true;

            var sourceProcessHandle = WinApi.Kernel32.OpenProcess(WinApi.Kernel32.ProcessAccessFlags.All, true, pid);
            if (NtDll.DuplicateHandle(sourceProcessHandle, (IntPtr)handle, WinApi.Kernel32.GetCurrentProcess(), out handleDuplicate, 0, false, NtDll.DuplicateOptions.DUPLICATE_SAME_ACCESS))
            {
                
            }
            else
            {
                result = false;
                handleDuplicate = IntPtr.Zero;
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
            IntPtr handleDuplicate = IntPtr.Zero;

            if (!DuplicateHandle(handle, pid, out handleDuplicate))
            {
                return null;
            }

            int length;

            NtDll.NtStatus stat = NtDll.NtQueryObject(handleDuplicate,
                NtDll.OBJECT_INFORMATION_CLASS.ObjectNameInformation, IntPtr.Zero, 0, out length);

            if (stat == NtDll.NtStatus.InvalidHandle)
                return null;

            return ExecuteSafe<string>(length, (pointer) =>
            {
                string result = string.Empty;

                NtDll.NtStatus status = NtDll.NtQueryObject(handleDuplicate,
                    NtDll.OBJECT_INFORMATION_CLASS.ObjectNameInformation, pointer, length, out length);

                if (status == NtDll.NtStatus.Success)
                {
                    var res = Marshal.PtrToStructure<NtDll.OBJECT_NAME_INFORMATION>(pointer);
                    result = res.Name.ToString();
                }
                return result;
            });
        }

        private static T ExecuteSafe<T>(int length, Func<IntPtr, T> func)
        {
            T result = default(T);
            IntPtr ptr = IntPtr.Zero;

            try
            {
                ptr = Marshal.AllocHGlobal(length);
                result = func(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return result;
        }
    }
}

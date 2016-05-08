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
        public static unsafe string GetHandleType(IntPtr handle)
        {
            int length;

            NtDll.NtStatus stat = NtDll.NtQueryObject(handle,
                NtDll.OBJECT_INFORMATION_CLASS.ObjectTypeInformation, IntPtr.Zero, 0, out length);

            if (stat == NtDll.NtStatus.InvalidHandle)
                return null;

            return ExecuteSafe<string>(length, (pointer) =>
            {
                string result = string.Empty;

                NtDll.NtStatus status = NtDll.NtQueryObject(handle,
                    NtDll.OBJECT_INFORMATION_CLASS.ObjectTypeInformation, pointer, length, out length);

                if (status == NtDll.NtStatus.Success)
                {
                    var res = Marshal.PtrToStructure<NtDll.PUBLIC_OBJECT_TYPE_INFORMATION>(pointer);
                    result = res.TypeName.ToString();
                }
                return result;
            });
        }

        /// <summary>
        /// Gets Handle Object name using NtQueryObject NtDll function
        /// Doc: https://msdn.microsoft.com/en-us/library/bb432383(v=vs.85).aspx
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static unsafe string GetHandleObjectName(IntPtr handle)
        {
            int length;

            NtDll.NtStatus stat = NtDll.NtQueryObject(handle,
                NtDll.OBJECT_INFORMATION_CLASS.ObjectNameInformation, IntPtr.Zero, 0, out length);

            if (stat == NtDll.NtStatus.InvalidHandle)
                return null;

            return ExecuteSafe<string>(length, (pointer) =>
            {
                string result = string.Empty;

                NtDll.NtStatus status = NtDll.NtQueryObject(handle,
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

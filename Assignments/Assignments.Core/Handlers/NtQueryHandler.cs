
using Assignments.Core.WinApi.NtDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

                if(status == NtDll.NtStatus.Success)
                {
                    var res = (NtDll.PUBLIC_OBJECT_TYPE_INFORMATION)Marshal.PtrToStructure(pointer, typeof(NtDll.PUBLIC_OBJECT_TYPE_INFORMATION));
                    result = res.TypeName.ToString();
                    res.TypeName.Dispose();
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

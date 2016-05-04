
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

                //TODO: Read struct TypeName
                var res = (NtDll.PUBLIC_OBJECT_TYPE_INFORMATION)Marshal.PtrToStructure(pointer, typeof(NtDll.PUBLIC_OBJECT_TYPE_INFORMATION));

                //var ObjectName = Marshal.PtrToStringUni((IntPtr)res.TypeName  + 0x60);

                switch (status)
                {
                    case NtDll.NtStatus.Success:
                        result = Marshal.PtrToStringUni((IntPtr)((int)pointer + 0x60));
                        break;
                    case NtDll.NtStatus.InvalidHandle:
                        break;
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

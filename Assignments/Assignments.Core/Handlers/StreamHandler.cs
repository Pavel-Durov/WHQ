using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Handlers
{
    public class StreamHandler
    {

        public static unsafe T[] ReadArray<T>(IntPtr absoluteAddress, int count, SafeMemoryMappedViewHandle safeHandle) where T : struct
        {
            T[] readItems = new T[count];

            try
            {
                byte* baseOfView = null;
                safeHandle.AcquirePointer(ref baseOfView);
                ulong offset = (ulong)absoluteAddress - (ulong)baseOfView;
                safeHandle.ReadArray<T>(offset, readItems, 0, count);
            }
            finally
            {
                safeHandle.ReleasePointer();
            }

            return readItems;
        }

        public static unsafe T ReadStruct<T>(uint rva, IntPtr streamPtr, SafeMemoryMappedViewHandle safeHandle) where T : struct
        {
            T result = default(T);

            try
            {
                byte* baseOfView = null;
                safeHandle.AcquirePointer(ref baseOfView);
                ulong offset = (ulong)streamPtr - (ulong)baseOfView;
                result = safeHandle.Read<T>(offset);
            }
            finally
            {
                safeHandle.ReleasePointer();
            }

            return result;
        }


    }
}

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Handlers
{
    public class StreamHandler
    {
        public static unsafe string ReadString(uint rva, uint length, SafeMemoryMappedViewHandle safeHandle)
        {
            try
            {
                byte* baseOfView = null;
                safeHandle.AcquirePointer(ref baseOfView);
                IntPtr positionToReadFrom = new IntPtr(baseOfView + rva);
                int len = Marshal.ReadInt32(positionToReadFrom) / 2;

                positionToReadFrom += (int)length;

                return Marshal.PtrToStringUni(positionToReadFrom);
                
            }
            finally
            {
                safeHandle.ReleasePointer();
            }
        }

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

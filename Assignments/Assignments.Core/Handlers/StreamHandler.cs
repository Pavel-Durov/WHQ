using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace Assignments.Core.Handlers
{
    public class StreamHandler
    {
        public static unsafe string ReadString(uint rva, uint length, SafeMemoryMappedViewHandle safeHandle)
        {
            return RunSafe<string>(() =>
            {
                byte* baseOfView = null;
                safeHandle.AcquirePointer(ref baseOfView);
                IntPtr positionToReadFrom = new IntPtr(baseOfView + rva);
                positionToReadFrom += (int)length;

                return Marshal.PtrToStringUni(positionToReadFrom);

            }, safeHandle);
        }

        public static unsafe T[] ReadArray<T>(IntPtr absoluteAddress, 
            int count, SafeMemoryMappedViewHandle safeHandle) where T : struct
        {
            return RunSafe<T>(() =>
            {
                T[] readItems = new T[count];

                byte* baseOfView = null;
                safeHandle.AcquirePointer(ref baseOfView);
                ulong offset = (ulong)absoluteAddress - (ulong)baseOfView;
                safeHandle.ReadArray<T>(offset, readItems, 0, count);
                return readItems;

            }, safeHandle, count);
        }

        public static unsafe T ReadStruct<T>(uint rva, IntPtr streamPtr, SafeMemoryMappedViewHandle safeHandle) where T : struct
        {
            return RunSafe<T>(() =>
            {
                byte* baseOfView = null;
                safeHandle.AcquirePointer(ref baseOfView);
                ulong offset = (ulong)streamPtr - (ulong)baseOfView;
                return safeHandle.Read<T>(offset);

            }, safeHandle);
        }

        private static T RunSafe<T>(Func<T> func, SafeMemoryMappedViewHandle safeHandle)
        {
            T result = default(T);
            try
            {
                result = func();
            }
            finally
            {
                safeHandle.ReleasePointer();
            }
            return result;
        }

        private static T[] RunSafe<T>(Func<T[]> function, SafeMemoryMappedViewHandle safeHandle, int count) where T : struct
        {
            T[] result = new T[count];
            try
            {
                return result = function();
            }
            finally
            {
                safeHandle.ReleasePointer();
            }
            return result;
        }

    }
}

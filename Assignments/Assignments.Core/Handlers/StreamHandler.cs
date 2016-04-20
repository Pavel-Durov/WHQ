using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using Assignments.Core.WinApi;

namespace Assignments.Core.Handlers
{
    public class StreamHandler
    {
        public static unsafe bool ReadMiniDumpStream<T>(SafeMemoryMappedViewHandle safeHandle ,DbgHelp.MINIDUMP_STREAM_TYPE streamToRead, out T streamData, out IntPtr streamPointer, out uint streamSize)
        {
            bool result = false;

            DbgHelp.MINIDUMP_DIRECTORY directory = new DbgHelp.MINIDUMP_DIRECTORY();
            streamPointer = IntPtr.Zero;
            streamSize = 0;

            try
            {
                byte* baseOfView = null;
                safeHandle.AcquirePointer(ref baseOfView);

                if (baseOfView == null)
                    throw new Exception("Unable to aquire pointer to memory mapped view");

                if (!DbgHelp.MiniDumpReadDumpStream((IntPtr)baseOfView, streamToRead, ref directory, ref streamPointer, ref streamSize))
                {
                    int lastError = Marshal.GetLastWin32Error();
                    result = false;
                }

                streamData = (T)Marshal.PtrToStructure(streamPointer, typeof(T));

            }
            finally
            {
                safeHandle.ReleasePointer();
            }

            return result;
        }


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

        public static unsafe T ReadStruct<T>(uint rva) where T : struct
        {
            IntPtr positionToReadFrom = new IntPtr((int)(rva));
            return Marshal.PtrToStructure<T>(positionToReadFrom);
        }


        public static T RunSafe<T>(Func<T> func, SafeMemoryMappedViewHandle safeHandle)
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

       
        public static T[] RunSafe<T>(Func<T[]> function, SafeMemoryMappedViewHandle safeHandle, int count) where T : struct
        {
            T[] result = new T[count];
            try
            {
                result = function();
            }
            finally
            {
                safeHandle.ReleasePointer();
            }
            return result;
        }

        public static void RunSafe(Action action, SafeMemoryMappedViewHandle safeHandle)
        {
            try
            {
                action();
            }
            finally
            {
                safeHandle.ReleasePointer();
            }
        }
    }
}

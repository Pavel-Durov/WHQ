using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using DbgHelp;
using System.Threading.Tasks;

namespace WHQ.Core.Handlers
{
    public class SafeMemoryMappedViewStreamHandler
    {

        public static unsafe bool ReadStream<T>(MINIDUMP_STREAM_TYPE streamType, out T streamData, out IntPtr streamPointer, out uint streamSize, SafeMemoryMappedViewHandle safeMemoryMappedViewHandle)
        {
            IntPtr viewBase;
            return ReadStream<T>(streamType, out streamData, out streamPointer, out streamSize, safeMemoryMappedViewHandle, out viewBase);
        }

        public static unsafe bool ReadStream<T>(MINIDUMP_STREAM_TYPE streamType, out T streamData, out IntPtr streamPointer, out uint streamSize, SafeMemoryMappedViewHandle safeMemoryMappedViewHandle, out IntPtr viewBase)
        {
            bool result = false;
            MINIDUMP_DIRECTORY directory = new MINIDUMP_DIRECTORY();
            streamData = default(T);
            streamPointer = IntPtr.Zero;
            streamSize = 0;

            try
            {
                byte* baseOfView = null;
                safeMemoryMappedViewHandle.AcquirePointer(ref baseOfView);

                result = Functions.MiniDumpReadDumpStream((IntPtr)baseOfView, streamType, ref directory, ref streamPointer, ref streamSize);
                viewBase = (IntPtr)baseOfView;
                if (result)
                {
                    streamData = (T)Marshal.PtrToStructure(streamPointer, typeof(T));
                }
            }
            finally
            {
                safeMemoryMappedViewHandle.ReleasePointer();
            }

            return result;
        }


        public static async Task<string> ReadString(Int32 rva, SafeMemoryMappedViewHandle safeHandle)
        {
            return await RunSafe<string>(() =>
            {
                unsafe
                {
                    byte* baseOfView = null;

                    safeHandle.AcquirePointer(ref baseOfView);

                    IntPtr positionToReadFrom = new IntPtr(baseOfView + rva);
                    int len = Marshal.ReadInt32(positionToReadFrom) / 2;
                    positionToReadFrom += 4;

                    // Read and marshal the string
                    return Marshal.PtrToStringUni(positionToReadFrom, len);
                }
            }, safeHandle);
        }

        public static async Task<string> ReadString(int rva, uint length, SafeMemoryMappedViewHandle safeHandle)
        {
            return await RunSafe<string>(() =>
            {
                unsafe
                {
                    byte* baseOfView = null;
                    safeHandle.AcquirePointer(ref baseOfView);
                    IntPtr positionToReadFrom = new IntPtr(baseOfView + rva);
                    positionToReadFrom += (int)length;

                    return Marshal.PtrToStringUni(positionToReadFrom);
                }
            }, safeHandle);
        }

        public static unsafe Task<T[]> ReadArray<T>(IntPtr absoluteAddress,
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

        public static async Task<T> ReadStruct<T>(Int32 rva, IntPtr streamPtr, SafeMemoryMappedViewHandle safeHandle) where T : struct
        {
            return await RunSafe<T>(() =>
            {
                unsafe
                {
                    byte* baseOfView = null;
                    safeHandle.AcquirePointer(ref baseOfView);
                    ulong offset = (ulong)streamPtr - (ulong)baseOfView;
                    return safeHandle.Read<T>(offset);
                }

            }, safeHandle);
        }

        public static unsafe T ReadStruct<T>(IntPtr rva) where T : struct
        {
            return Marshal.PtrToStructure<T>(rva);
        }


        public static async Task<T> RunSafe<T>(Func<T> func, SafeMemoryMappedViewHandle safeHandle)
        {
            return await Task<T>.Run(() =>
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
            });
        }


        public static async Task<T[]> RunSafe<T>(Func<T[]> function, SafeMemoryMappedViewHandle safeHandle, int count) where T : struct
        {
            return await Task<T[]>.Run(() =>
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
            });
        }

        public static async Task RunSafe(Action action, SafeMemoryMappedViewHandle safeHandle)
        {
            await Task.Run(() =>
            {
                try
                {
                    action();
                }
                finally
                {
                    safeHandle.ReleasePointer();
                }
            });
        }
    }
}

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using Assignments.Core.WinApi;

namespace Assignments.Core.Handlers
{
    public class SafeMemoryMappedViewStreamHandler
    {

        public static unsafe bool ReadStream<T>(DbgHelp.MINIDUMP_STREAM_TYPE streamType, out T streamData, out IntPtr streamPointer, out uint streamSize, SafeMemoryMappedViewHandle safeMemoryMappedViewHandle)
        {
            IntPtr viewBase;
            return ReadStream<T>(streamType, out streamData, out streamPointer, out streamSize, safeMemoryMappedViewHandle, out viewBase);
        }

        public static unsafe bool ReadStream<T>(DbgHelp.MINIDUMP_STREAM_TYPE streamType, out T streamData, out IntPtr streamPointer, out uint streamSize, SafeMemoryMappedViewHandle safeMemoryMappedViewHandle, out IntPtr viewBase)
        {
            bool result = false;
            DbgHelp.MINIDUMP_DIRECTORY directory = new DbgHelp.MINIDUMP_DIRECTORY();
            streamData = default(T);
            streamPointer = IntPtr.Zero;
            streamSize = 0;

            try
            {
                byte* baseOfView = null;
                safeMemoryMappedViewHandle.AcquirePointer(ref baseOfView);


                result = DbgHelp.MiniDumpReadDumpStream((IntPtr)baseOfView, streamType, ref directory, ref streamPointer, ref streamSize);
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


        public static unsafe string ReadString(uint rva, SafeMemoryMappedViewHandle safeHandle)
        {
            return RunSafe<string>(() =>
            {
                byte* baseOfView = null;

                safeHandle.AcquirePointer(ref baseOfView);

                IntPtr positionToReadFrom = new IntPtr(baseOfView + rva);
                int len = Marshal.ReadInt32(positionToReadFrom) / 2;
                positionToReadFrom += 4;

                // Read and marshal the string
                return Marshal.PtrToStringUni(positionToReadFrom, len);
            }, safeHandle);
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

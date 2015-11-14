using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using HANDLE = System.IntPtr;

namespace Assignment_3.DumpTest
{
    class Program
    {
        //Run this project in orer to take a dump
        //Wait functions -> https://msdn.microsoft.com/en-us/library/windows/desktop/ms687069(v=vs.85).aspx
        static void Main(string[] args)
        {

            Console.WriteLine("Calling native methods... :)");

            HANDLE p = CreateEvent(HANDLE.Zero, false, true, null);
            WaitForSingleObject(p, INFINITE);
            //CloseHandle(p);

        }


        [DllImport("kernel32.dll")]
        static extern uint WaitForMultipleObjects(uint nCount, IntPtr[] lpHandles, bool bWaitAll, uint dwMilliseconds);

        [DllImport("coredll.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        public static extern HANDLE CreateEvent(HANDLE lpEventAttributes, [In, MarshalAs(UnmanagedType.Bool)] bool bManualReset, [In, MarshalAs(UnmanagedType.Bool)] bool bIntialState, [In, MarshalAs(UnmanagedType.BStr)] string lpName);

        [DllImport("coredll.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(HANDLE hObject);

        //Sigle WAit////////////////////////////////////////////////////////////
        //http://www.pinvoke.net/default.aspx/coredll/WaitForSingleObject.html
        ////////////////////////////////////////////////////////////////////////
        [DllImport("coredll.dll", SetLastError = true)]
        public static extern Int32 WaitForSingleObject(IntPtr Handle, Int32 Wait);

        public const Int32 INFINITE = -1;
        public const Int32 WAIT_ABANDONED = 0x80;
        public const Int32 WAIT_OBJECT_0 = 0x00;
        public const Int32 WAIT_TIMEOUT = 0x102;
        public const Int32 WAIT_FAILED = -1;

    }
}

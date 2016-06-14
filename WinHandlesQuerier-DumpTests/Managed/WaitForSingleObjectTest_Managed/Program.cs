using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HANDLE = System.IntPtr;

namespace WaitForSingleObjectTest_Managed
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("DEBUG");
#else
            Console.WriteLine("RELEASE");
#endif
            Console.WriteLine("Test started");
            Console.WriteLine($"Is64BitProcess : {Environment.Is64BitProcess}{Environment.NewLine}");

            var proc = Process.GetCurrentProcess();
            Console.WriteLine("PID : " + proc.Id);

            IntPtr hEvent = CreateEvent(IntPtr.Zero, true, false, "SomethingSomething");
            
            uint waitTime = 1234567890;
            Console.WriteLine($"WaitForSingleObject : handle {hEvent}, 0x{hEvent.ToString("X")}, waitTime: {waitTime}, 0x{waitTime.ToString("X")}");

            WaitForSingleObject(hEvent, waitTime);

            Console.WriteLine("Done Waiting");

            Console.ReadKey();
        }

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        public static extern HANDLE CreateEvent(HANDLE lpEventAttributes, [In, MarshalAs(UnmanagedType.Bool)] bool bManualReset, [In, MarshalAs(UnmanagedType.Bool)] bool bIntialState, [In, MarshalAs(UnmanagedType.BStr)] string lpName);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Int32 WaitForSingleObject(HANDLE Handle, uint Wait);

    }
}

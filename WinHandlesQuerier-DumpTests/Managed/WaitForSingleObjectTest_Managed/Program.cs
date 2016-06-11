using System;
using System.Diagnostics;

namespace WaitForSingleObjectTest_Managed
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test started");
            Console.WriteLine($"Is64BitProcess : {Environment.Is64BitProcess}{Environment.NewLine}");

            var proc = Process.GetCurrentProcess();
            Console.WriteLine("PID : " + proc.Id);

            IntPtr hEvent = Kernel32.Functions.CreateEvent(IntPtr.Zero, true, false, "SomethingSomething");

            Console.WriteLine($"WaitForSingleObject : handle {hEvent}, 0x{hEvent.ToString("X")}");
            Kernel32.Functions.WaitForSingleObject(hEvent, Kernel32.Const.INFINITE);

            Console.WriteLine("Done Waiting");

            Console.ReadKey();
        }
    }
}

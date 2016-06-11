using Kernel32;
using System;
using System.Diagnostics;
using System.Threading;

namespace WaitForMultipleObjectsTest_Managed
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Test started");
            Console.WriteLine($"Is64BitProcess : {Environment.Is64BitProcess}{Environment.NewLine}");

            var proc = Process.GetCurrentProcess();
            Console.WriteLine("PID : " + proc.Id);


            IntPtr[] arr = new IntPtr[19];
            for (int i = 0; i < arr.Length; i++)
            {
                var loopAutoEvent = new AutoResetEvent(false);
                arr[i] = loopAutoEvent.Handle;
            }

            Console.WriteLine($"WaitForMultipleObjects - Count: 19, bWaitAll : true, wait: {int.MaxValue} / {int.MaxValue.ToString("X")}");
            var mulRes2 = Functions.WaitForMultipleObjects(19, arr, true, int.MaxValue);


        }
    }
}

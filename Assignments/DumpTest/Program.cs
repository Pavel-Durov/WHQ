using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using HANDLE = System.IntPtr;
using System.Threading.Tasks;
using System.Threading;
using DumpTest.Tests;

namespace DumpTest
{
    public class Program
    {

        const String PID_FILE_PATH = @"./../../../dump_pid.txt";

        static void Main(string[] args)
        {

            Console.WriteLine("Test started");
        

            DealWithPID();

            Kernel32Calls.Run();

            Console.ReadLine();
        }

        private static void DealWithPID()
        {
            var proc = Process.GetCurrentProcess();
            Console.WriteLine("PID : " + proc.Id);
            File.WriteAllText(PID_FILE_PATH, proc.Id.ToString());
            Console.WriteLine("Pid wrote to shared txt file");
        }
    }
}

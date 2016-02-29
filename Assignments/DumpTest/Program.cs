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

            var kernel32Task = Task.Run(() => {
                Kernel32Calls.Run();
            });

           var mutexWaitRun = Task.Run(async () => {
                await MutexWait.Run();
            });

            var threadEventWaitTask = Task.Run(() => {
                ThreadEventWait.Run();
            });

            var threadMonitorWaitTask = Task.Run(() => {
                ThreadMonitorWait.Run();
            });

            var result = Task.WaitAny(kernel32Task, mutexWaitRun, threadEventWaitTask, threadMonitorWaitTask);

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

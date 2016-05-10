#define LIVE_PID_DEBUG


using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DumpTest.Tests;
using Microsoft.Win32;

namespace DumpTest
{
    public class Program
    {
        const String PID_FILE_PATH = @"./../../../dump_pid.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Test started");

            DealWithPID();

            var kernel32Task = Task.Run(() =>
            {
                Kernel32Calls.Run();
            });

            var mutexWaitRun = Task.Run(async () =>
            {
                await MutexWait.Run();
            });

            var threadEventWaitTask = Task.Run(() =>
            {
                ThreadEventWait.Run();
            });

            var threadMonitorWaitTask = Task.Run(() =>
            {
                ThreadMonitorWait.Run();
            });

            var result = Task.WaitAny(kernel32Task, mutexWaitRun, threadEventWaitTask, threadMonitorWaitTask);

            Console.ReadLine();
        }

        private static void DealWithPID()
        {
            var proc = Process.GetCurrentProcess();
            Console.WriteLine("PID : " + proc.Id);

#if LIVE_PID_DEBUG
            Registry.CurrentUser.SetValue("my-ruthles-pid-key", proc.Id);
#endif

            Console.WriteLine("Pid wrote to shared txt file");
        }


    }
}

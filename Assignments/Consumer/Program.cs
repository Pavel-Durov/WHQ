
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core;
using System.IO;
using Assignments.Core.Handlers;
using Microsoft.Diagnostics.Runtime;

//TODO: post build -> start ../../../Assignment_3.DumpTest\bin\x86\Debug\Assignment_3.DumpTest.exe

namespace Consumer
{
    class Program
    {
        const string SOME_86_DUMP = @"C:\temp\Dumps\Assignment_3.dmp";
        const int PID_NOT_FOUND = -1;
        const int ATTACH_TO_PPROCESS_TIMEOUT = 999999;

        static void Main(string[] args)
        {


            int pid = GetPidFromDumpProcessTextFile();
            if (pid != PID_NOT_FOUND)
            {
                Console.WriteLine("PID found in Assignment_3.DumpTest file :) ");
            }
            else
            {
                Console.WriteLine("--- Assignment_4 C# project ----");
                Console.WriteLine("Please enter a PID: ");

                pid = int.Parse(Console.ReadLine());
            }

            //Global.Test.Run(pid);
            DumpFile64Bit.Test.Run(pid);
            Console.ReadKey();
        }

        private static int GetPidFromDumpProcessTextFile()
        {
            int pid = PID_NOT_FOUND;

            var fileContent = File.ReadAllText(@"./../../../dump_pid.txt");
            if (!String.IsNullOrEmpty(fileContent))
            {
                var success = int.TryParse(fileContent, out pid);
            }

            return pid;
        }

      
    }
}

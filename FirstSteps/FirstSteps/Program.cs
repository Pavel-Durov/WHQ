using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FirstSteps
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"C:\Users\Pavel\AppData\Local\Temp\Taskmgr.DMP";
            if (File.Exists(path))
            {


                using (var info = DataTarget.LoadCrashDump(path))
                {
                    foreach (ClrInfo version in info.ClrVersions)
                    {
                        var runtime = version.CreateRuntime();
                    }
                }
            }
            else
            {
                Console.Beep();
            }
        }
    }
}

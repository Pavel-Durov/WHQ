
using System;
using Microsoft.Diagnostics.Runtime;
using System.IO;

namespace GettingStarted.Handlers
{
    class DumpHandler
    {
        const string KALSEFER_86_DUMP = @"C:\Git\sela-opensource\clrmd\src\Dumps\Kalsefer.dmp";
            //@"~/../../Dumps/Kalsefer.dmp";
        public DumpHandler()
        {

        }


        public void Start(string path = KALSEFER_86_DUMP)
        {

            if (File.Exists(path))
            {
                //Loading a crash dump
                using (DataTarget target = DataTarget.LoadCrashDump(path))
                {
                    foreach (ClrInfo version in target.ClrVersions)
                    {
                        Console.WriteLine("Found CLR Version:" + version.Version.ToString());

                        // This is the data needed to request the dac from the symbol server:
                        ModuleInfo dacInfo = version.DacInfo;
                        Console.WriteLine("Filesize:  {0:X}", dacInfo.FileSize);
                        Console.WriteLine("Timestamp: {0:X}", dacInfo.TimeStamp);
                        Console.WriteLine("Dac File:  {0}", dacInfo.FileName);


                        ClrRuntime runtime = version.CreateRuntime();

                        Console.WriteLine("List of AppDomains : ");

                        foreach (ClrAppDomain domain in runtime.AppDomains)
                        {
                            Console.WriteLine("ID:      {0}", domain.Id);
                            Console.WriteLine("Name:    {0}", domain.Name);
                            Console.WriteLine("Address: {0}", domain.Address);
                        }

                        foreach (ClrThread thread in runtime.Threads)
                        {
                            //Checking if thread is alive
                           
                        }

                        var heap = runtime.GetHeap();
                        foreach (var handle in runtime.EnumerateHandles())
                        {
                            string objectType = heap.GetObjectType(handle.Object).Name;
                            Console.WriteLine("{0,12:X} {1,12:X} {2,12} {3}", handle.Address, handle.Object, handle.Type.ToString(), objectType);
                        }

                    }
                }
            }
        }
    }
}

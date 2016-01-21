using Assignments.Core.Handlers;
using Assignments.Core.WinApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Consumer.DumpFile64Bit
{
    class Test
    {
        public static void Run(uint pid)
        {
            MiniDumpHandler handler = new MiniDumpHandler();
            handler.Handle(pid);
        }
    }
}

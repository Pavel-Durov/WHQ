using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingStarted
{
    class Program
    {
        static void Main(string[] args)
        {
            Handlers.DumpHandler dumpH = new Handlers.DumpHandler();
            dumpH.Start();
            Console.ReadKey();
        }
    }
}

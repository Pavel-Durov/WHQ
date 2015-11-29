using Assignment_4.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_4
{
    class Program
    {
        static void Main(string[] args)
        {
            WctApi api = new WctApi();

            api.TestRun();

            Console.ReadKey();
        }
    }
}

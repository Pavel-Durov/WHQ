using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHandlesQuerier.Core.Handlers.MiniDump;
using WinHandlesQuerier.Core.Handlers.StackAnalysis.Strategies;

namespace Assignments.Core.Infra
{

    class Config
    {

        static Config _singletone;
        static object _sync = new object();

        public CPUArchitecture CpuArchitecture { get; set; }

        public static Config GetInstance()
        {
            lock (_sync)
            {
                if (_singletone == null)
                {
                    _singletone = new Config();
                }
            }

            return _singletone;
        }

        public void Init(CPUArchitecture cpuArchitecture)
        {
            CpuArchitecture = cpuArchitecture;
        }

        public CPUArchitecture SystemArchitecture { get; private set; }
    }
}

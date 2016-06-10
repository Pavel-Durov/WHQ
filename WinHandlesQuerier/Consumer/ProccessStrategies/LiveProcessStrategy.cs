using Assignments.Core.Infra;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHandlesQuerier.Core.Handlers;
using WinHandlesQuerier.Core.Model.Unified.Thread;

namespace Consumer.ProccessStrategies
{
    internal class LiveProcessStrategy : ProcessStrategy
    {
        public LiveProcessStrategy(uint pid) : base(pid)
        {

        }

        public override List<UnifiedThread> Run()
        {
            List<UnifiedThread> result = null;
            if (_pid == Constants.INVALID_PID)
            {
                Console.WriteLine("--- Assignment_4 C# project ----");
                Console.WriteLine("Please enter a PID: ");

                _pid = uint.Parse(Console.ReadLine());
            }

            using (DataTarget target = DataTarget.AttachToProcess((int)_pid, Constants.MAX_ATTACH_TO_PPROCESS_TIMEOUT))
            {
                if (Environment.Is64BitProcess && target.Architecture != Architecture.Amd64)
                {
                    throw new InvalidOperationException($"Unexpected architecture. Process runs as x64");
                }

                Console.WriteLine("Attached To Process Successfully");
                result = DoAnaytics(target, _pid);

            }


            return result;
        }

        private static List<UnifiedThread> DoAnaytics(DataTarget target, uint pid)
        {
            var clrVer = target.ClrVersions[0];

            //Live process handler
            var runtime = clrVer.CreateRuntime();
            ProcessAnalyzer handler = new ProcessAnalyzer(target, runtime, pid);

            return handler.Handle();
        }
    }
}

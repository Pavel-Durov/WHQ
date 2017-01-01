using WHQ.Core.Model;
using System;
using System.Threading.Tasks;
using WHQ.Core.Handlers;
using System.IO;
using WHQ.Providers.ClrMd.Model;

namespace WHQ.ProcessStrategies
{
    public class DumpFileProcessStrategy : ProcessStrategy
    {
        public DumpFileProcessStrategy(string filePath) : base(filePath)
        {

        }

        public string FilePath => _filePath;

        public override async Task<ProcessAnalysisResult> Run()
        {
            ProcessAnalysisResult result = null;

            if (File.Exists(_filePath))
            {
                try
                {

                    using (DataTarget target = DataTarget.LoadCrashDump(_filePath))
                    {
                        if (CheckTargetBitness(target))
                        {
                            Console.WriteLine("Dump file loaded successfully.");

                            ClrRuntime runtime = target.CreateRuntime();

                            using (ProcessAnalyzer handler = new ProcessAnalyzer(target, runtime, _filePath))
                            {
                                result = await handler.Handle();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = SetError(ex.Message);
                }
            }
            else
            {
                result = SetError("File Not Found");
            }

            return result;
        }
    }
}

using DllRefProvider.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHQ.Core.Providers.ClrMd.Model;

namespace WHQ.Providers.ClrMd.Model
{
    public enum Architecture
    {
        Unknown = 0,
        X86 = 1,
        Amd64 = 2,
        Arm = 3
    }

    public class DataTarget : IDisposable
    {
        private static ClrMdBase _clrMd;
        public Architecture Architecture { get; }
        internal IDebugClient DebuggerInterface { get; set; }
        internal IDataReader DataReader { get; set; }

        internal DataTarget(ClrMdBase clrMd)
        {
            _clrMd = clrMd;
        }

        public static DataTarget LoadCrashDump(string fileName)
        {
            return new DataTarget(_clrMd);
        }

        public static DataTarget AttachToProcess(int _pid, int mAX_ATTACH_TO_PPROCESS_TIMEOUT)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _clrMd.Dispose();
        }

        public ClrRuntime CreateRuntime()
        {
            //ClrRuntime runtime = target.ClrVersions[0].CreateRuntime();
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHQ.Core.Providers.ClrMd.Model
{
    interface IDebugSystemObjects
    {
        int GetNumberThreads(out uint _numThreads);
        int GetThreadIdsByIndex(uint threadIndex, int v, uint[] engineThreadIds, uint[] osThreadIds);
        int SetCurrentThreadId(uint engineThreadId);
    }
}

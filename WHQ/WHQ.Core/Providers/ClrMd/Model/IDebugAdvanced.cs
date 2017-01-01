using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHQ.Core.Providers.ClrMd.Model
{
    interface IDebugAdvanced
    {
        uint GetThreadContext(IntPtr intPtr, uint length);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;

namespace Assignments.Core.Model.Unified.Thread
{
    public class UnifiedManagedThread : UnifiedThread
    {
        public UnifiedManagedThread(ThreadInfo info) : base (info)
        {

        }
        public UnifiedManagedThread(ClrThread thread)//, string detail)
        {
  
        }

        public ClrThread Thread { get; private set; }

    }
}

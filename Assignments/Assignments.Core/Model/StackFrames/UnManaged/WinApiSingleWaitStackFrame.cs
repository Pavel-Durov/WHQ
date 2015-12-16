using Assignments.Core.Model.StackFrames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Model.StackFrames.UnManaged
{
    public class WinApiSingleWaitStackFrame : WinApiStackFrame
    {
        public const string FUNCTION_NAME = "WaitForSingleObject";
    }
}

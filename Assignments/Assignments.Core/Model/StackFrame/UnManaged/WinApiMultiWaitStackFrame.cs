using System.Text;
using Assignments.Core.Extentions;

namespace Assignments.Core.Model.StackFrames.UnManaged
{
    public class WinApiMultiWaitStackFrame : WinApiStackFrame
    {
        public const string FUNCTION_NAME = "WaitForMultipleObjects";

        public uint HandlesCunt { get; internal set; }
        public uint WaitallFlag { get; internal set; }
    }
}

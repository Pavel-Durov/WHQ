using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_3.PrintHandles.Factory
{
    public enum StackFrameHandlerTypes
    {
        WaitForSingleObject,
        WaitForMultipleObjects
    }
    class StackFrameHandlersFactory
    {

        public static StackFrameHandler GetHandler(StackFrameHandlerTypes type)
        {
            StackFrameHandler result = null;
            switch (type)
            {
                case StackFrameHandlerTypes.WaitForSingleObject:
                    result = new SingleWaitStackFrameHandler();
                    break;
                case StackFrameHandlerTypes.WaitForMultipleObjects:
                    result = new MultiWaitStackFrameHandler();
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}

using Assignment_3.Exceptions;
using Assignment_3.msos;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_3.PrintHandles.Factory
{
   // public delegate void PrintDel(UInt32 handleAddress, UInt32 handlesCunt, ClrRuntime runtime);

    abstract class StackFrameHandler
    {
        public abstract void Print(UnifiedStackFrame item, ClrRuntime runtime);

        public virtual List<byte[]> GetNativeParams(UnifiedStackFrame stackFrame, ClrRuntime runtime, int paramCount)
        {
            List<byte[]> result = new List<byte[]>();

            var offset = stackFrame.FrameOffset; //Base Pointer - % EBP
            byte[] paramBuffer;
            int bytesRead = 0;
            offset += 4;

            for (int i = 0; i < paramCount; i++)
            {
                paramBuffer = new byte[4];
                offset += 4;
                if (runtime.ReadMemory(offset, paramBuffer, 4, out bytesRead))
                {
                    result.Add(paramBuffer);
                }
            }

            Console.WriteLine();

            return result;
        }

        protected void Print(UInt32 handleAddress, UInt32 handlesCunt, ClrRuntime runtime)
        {
            //Reading n times from memmory, advansing by 4 bytes each time
            byte[] readedBytes = null;
            int count = 0;
            for (int i = 0; i < handlesCunt; i ++)
            {
                readedBytes = new byte[4];

                if (runtime.ReadMemory(handleAddress, readedBytes, 4, out count))
                {
                    uint byteValue = BitConverter.ToUInt32(readedBytes, 0);
                    Console.Write("handle {0}=0x{1:x}  ", i, byteValue);
                }
                else
                {
                    throw new AccessingNonReadableMemmory(string.Format("Accessing Unreadable memorry at {0}", handleAddress));
                    //Print(ConsoleColor.Red, "Unreadable memorry");
                }
                //Advancing the pointer by 4 (32-bit system)
                handleAddress += 4;
            }
            Console.WriteLine();
        }

    }
}

using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignment_3.msos;

namespace Assignment_3.PrintHandles
{
    class ThreadStackAnalyzer
    {
        const string key0 = "WaitForSingleObject";
        const string key1 = "WaitForMultipleObjects";

        /// <summary>
        /// Iterates over thread Stack and searches fot two Windows API calls - 
        /// WaitForSingleObject(Ex), WaitForMultipleObjects(Ex). 
        /// </summary>
        /// <param name="stackTrace"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private static bool InspectStackForWindowsApiCalls(IEnumerable<UnifiedStackFrame> stackTrace,
            ref IEnumerable<UnifiedStackFrame> list)
        {

            if (stackTrace != null)
            {
                list = from c in stackTrace
                       where CheckSome(c)
                       select c;
            }

            return list != null && list.Any();
        }

        private static bool CheckSome(UnifiedStackFrame c)
        {
            return c != null
                && !String.IsNullOrEmpty(c.Method)
                && c.Method != null && (c.Method.Contains(key0) || c.Method.Contains(key1));
        }

        
        public static void PrintStackTrace(IEnumerable<UnifiedStackFrame> stackTrace, ClrThread thread, ClrRuntime runtime, bool isNativeStack = false)
        {
            bool hasBlockingObjects = false;
            if (!isNativeStack)
            {
                if (thread.BlockingObjects != null)
                {
                    foreach (var bObj in thread.BlockingObjects)
                    {
                        Print(bObj);
                    }

                    hasBlockingObjects = thread?.BlockingObjects.Count > 0;
                }
            }

            if (!hasBlockingObjects)
            {
                IEnumerable<UnifiedStackFrame> list = null;
                if (InspectStackForWindowsApiCalls(stackTrace, ref list))
                {
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            var nativeParams = GetNativeParams(item, runtime);
                            Console.WriteLine("-- Native method handles : ");

                            var prevColor = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.Red;

                            Console.WriteLine(" << {0} >> " , item.Method);

                            PrintParams(nativeParams);
                            Console.ForegroundColor = prevColor;
                        }
                    }
                }
            }

            foreach (var frame in stackTrace)
            {
                if (frame.Type == UnifiedStackFrameType.Special)
                {
                    Console.WriteLine("{0,-10}", "Special");
                    continue;
                }
                if (String.IsNullOrEmpty(frame.SourceFileName))
                {
                    Console.WriteLine("{0,-10} {1,-20:x16} {2}!{3}+0x{4:x}",
                        frame.Type, frame.InstructionPointer,
                        frame.Module, frame.Method, frame.OffsetInMethod);
                }
                else
                {
                    Console.WriteLine("{0,-10} {1,-20:x16} {2}!{3} [{4}:{5},{6}]",
                        frame.Type, frame.InstructionPointer,
                        frame.Module, frame.Method, frame.SourceFileName,
                        frame.SourceLineNumber, frame.SourceColumnNumber);
                }

                if (isNativeStack)
                {
                    var prevColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Green;
       
                    List<byte[]> parms = GetNativeParams(frame, runtime);
                    PrintParams(parms);

                    Console.ForegroundColor = prevColor;

                }
            }

            
        }


        private static List<byte[]> GetNativeParams(UnifiedStackFrame stackFrame, ClrRuntime runtime)
        {

            List<byte[]> result = new List<byte[]>();

            var offset = stackFrame.FrameOffset; //Base Pointer - % EBP
            byte[] paramBuffer;
            int bytesRead = 0;
            offset += 4;

            for (int i = 0; i < 4; i++)
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




        #region Printer funcs

        private static bool Print(BlockingObject bObj)
        {
            bool result = false;
            if (bObj != null)
            {
                Console.WriteLine("Associated Object : {0}", bObj.Object);

                if (bObj.HasSingleOwner && bObj.Taken)
                {
                    Console.WriteLine("Single Owner : {0}", bObj.Owner);
                }
                else
                {
                    int ownerCounter = 0;

                    Console.WriteLine("-- Owners -- ");

                    PrintThreadsIds(bObj.Owners);
                    foreach (var owner in bObj.Owners)
                    {
                        Console.WriteLine("{0}) Owner: {1}", ++ownerCounter, owner?.OSThreadId);
                    }
                }

                Console.WriteLine("Block Reason : {0}", bObj.Reason);
                Console.WriteLine("Taken : {0}", bObj.Taken);
                Console.WriteLine(" -- Witers List -- ");
                PrintThreadsIds(bObj.Owners);
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        private static void PrintThreadsIds(IList<ClrThread> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine("{0}) Owner: {1}", i, list[i]?.OSThreadId);
            }
        }

        private static void PrintParams(List<byte[]> nativeParams)
        {

            for (int i = 0; i < nativeParams.Count; i++)
            {
                //string hexData = BitConverter.ToString(nativeParams[i]);
                uint byteValue = BitConverter.ToUInt32(nativeParams[i], 0);
                Console.Write("p{0}=0x{1:x}  ", i, byteValue);
            }
            Console.WriteLine();
        }





        #endregion
    }
}

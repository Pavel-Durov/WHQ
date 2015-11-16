using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignment_3.msos;
using Assignment_3.Exceptions;

namespace Assignment_3.PrintHandles
{
    class ThreadStackAnalyzer
    {
        const string key0 = "WaitForSingleObject";
        const string key1 = "WaitForMultipleObjects";

        public static void PrintSyncObjects(IEnumerable<UnifiedStackFrame> stackTrace,
            ClrThread thread, ClrRuntime runtime, bool isNativeStack = false)
        {
            if (isNativeStack)
            {
                PrintBlockingWinApiCalls(stackTrace, runtime);
            }
            else
            {
                if (thread.BlockingObjects != null && thread?.BlockingObjects?.Count > 0)
                {
                    foreach (var bObj in thread.BlockingObjects)
                    {
                        Print(bObj);
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
                    PrintBytesAsHex(ConsoleColor.Green, WinApiCallsInspector.GetNativeParams(frame, runtime));
                }
            }
        }


        private static void PrintBlockingWinApiCalls(IEnumerable<UnifiedStackFrame> stackTrace, ClrRuntime runtime)
        {
            IEnumerable<UnifiedStackFrame> singleList = null;
            IEnumerable<UnifiedStackFrame> multipleList = null;

            var any = WinApiCallsInspector.InspectStackForWindowsApiCalls(stackTrace, ref singleList, ref multipleList);
            if (any)
            {
                if (singleList?.Count() > 0)
                {
                    foreach (var item in singleList)
                    {
                        Console.WriteLine("-- Native method handles {0}: ", item.Method);
                        PrintSingleWait(item, runtime);
                    }
                }

                if (multipleList?.Count() > 0)
                {
                    foreach (var item in multipleList)
                    {
                        Console.WriteLine("-- Native method handles {0}: ", item.Method);
                        PrintMultiWaitHandles(item, runtime);
                    }
                }
            }
        }


        private static void PrintSingleWait(UnifiedStackFrame item, ClrRuntime runtime)
        {
            var nativeParams = WinApiCallsInspector.GetNativeParams(item, runtime);

            if (nativeParams != null && nativeParams.Count > 0)
            {
                var handleAddress = BitConverter.ToUInt32(nativeParams[0], 0);
                var dwMilliseconds = BitConverter.ToUInt32(nativeParams[1], 0);

                Print(handleAddress, 1, runtime);
            }
        }

        private static void PrintMultiWaitHandles(UnifiedStackFrame item, ClrRuntime runtime)
        {
            var nativeParams = WinApiCallsInspector.GetNativeParams(item, runtime);

            if (nativeParams != null && nativeParams.Count > 0)
            {
                var handlesCunt = BitConverter.ToUInt32(nativeParams[0], 0);
                var handleAddress = BitConverter.ToUInt32(nativeParams[1], 0);
                var waitallFlag = BitConverter.ToUInt32(nativeParams[2], 0);
                var waitTimeout = BitConverter.ToUInt32(nativeParams[3], 0);

                if (handlesCunt > 0)
                {
                    Print(handleAddress, handlesCunt, runtime);
                }
            }
        }



        private static void Print(UInt32 handleAddress, UInt32 handlesCunt, ClrRuntime runtime)
        {
            //Reading n times from memmory, advansing by 4 bytes each time
            byte[] readedBytes = null;
            int count = 0;
            for (int i = 0; i < handlesCunt; i += 4)
            {
                readedBytes = new byte[4];

                if (runtime.ReadMemory(handleAddress, readedBytes, 4, out count))
                {
                    uint byteValue = BitConverter.ToUInt32(readedBytes, 0);
                    Console.Write("hander {0}=0x{1:x}  ", i, byteValue);
                }
                else
                {
                    throw new AccessingNonReadableMemmory(string.Format("Accessing Unreadable memorry at {0}", handleAddress));
                    //Print(ConsoleColor.Red, "Unreadable memorry");
                }
                //Advancing the pointer by 4 (32-bit system)
                handleAddress += 4;
            }
        }

        private static void Print(ConsoleColor color, string toPrint)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.WriteLine(toPrint);

            Console.ForegroundColor = prevColor;
        }

        private static void PrintBytesAsHex(ConsoleColor color, List<byte[]> parms)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < parms.Count; i++)
            {
                int byteValue = BitConverter.ToInt32(parms[i], 0);

                var msg = String.Format("p{0}= 0x{1:x}", i, byteValue);
                sb.Append(msg);
            }

            Print(color, sb.ToString());
        }

       

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


                PrintThreadsIds(bObj.Waiters);

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

    }
}

using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.msos;
using Assignments.Core.Exceptions;
using Assignments.Core.PrintHandles.Factory;

namespace Assignments.Core.PrintHandles
{
    public class ThreadStackAnalyzer
    {
        public static void PrintSyncObjects(IEnumerable<UnifiedStackFrame> stackTrace,
            ClrThread thread, ClrRuntime runtime, bool isNativeStack = false)
        {


            if (!isNativeStack)
            {
                if (thread.BlockingObjects != null && thread?.BlockingObjects?.Count > 0)
                {
                    foreach (var bObj in thread.BlockingObjects)
                    {
                        Print(bObj);
                    }
                }
            }
            //PrintBlockingWinApiCalls(stackTrace, runtime);
            //return;
            var multi = StackFrameHandlersFactory.GetHandler(StackFrameHandlerTypes.WaitForMultipleObjects);
            var single = StackFrameHandlersFactory.GetHandler(StackFrameHandlerTypes.WaitForSingleObject);

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
                    PrintBytesAsHex(ConsoleColor.Green, WinApiCallsInspector.GetNativeParams(frame, runtime, 4));

                    DealWithNativeFrame(frame, runtime, multi, multi);
                    Console.WriteLine();
                }
            }
        }

        private static void DealWithNativeFrame(UnifiedStackFrame frame,ClrRuntime runtime, StackFrameHandler multiHandler, StackFrameHandler singleHandler)
        {
            if (WinApiCallsInspector.CheckForWinApiCalls(frame, WinApiCallsInspector.WAIT_FOR_SINGLE_OBJECT_KEY))
            {
                singleHandler.Print(frame, runtime);
            }
            else if(WinApiCallsInspector.CheckForWinApiCalls(frame, WinApiCallsInspector.WAIT_FOR_MULTIPLE_OBJECTS_KEY))
            {
                multiHandler.Print(frame, runtime);
            }
            else
            {
                //Do nothing...
            }
        }

        private static void PrintBlockingWinApiCalls(IEnumerable<UnifiedStackFrame> stackTrace, ClrRuntime runtime)
        {
            IEnumerable<UnifiedStackFrame> singleList = null;
            IEnumerable<UnifiedStackFrame> multipleList = null;

            var any = WinApiCallsInspector.InspectStackForWindowsApiCalls(stackTrace, ref singleList, ref multipleList);
            if (any)
            {
                var l0 = singleList.ToList();
                var l1 = multipleList.ToList();

                if (multipleList?.Count() > 0)
                {
                    var multi = StackFrameHandlersFactory.GetHandler(StackFrameHandlerTypes.WaitForMultipleObjects);
                    foreach (var item in multipleList)
                    {
                        multi.Print(item, runtime);
                    }
                }

                if (singleList?.Count() > 0)
                {
                    var single = StackFrameHandlersFactory.GetHandler(StackFrameHandlerTypes.WaitForSingleObject);
                    foreach (var item in singleList)
                    {
                        single.Print(item, runtime);
                    }
                }

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

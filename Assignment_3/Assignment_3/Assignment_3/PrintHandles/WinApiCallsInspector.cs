using Assignment_3.msos;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_3.PrintHandles
{
    class WinApiCallsInspector
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
        public static bool InspectStackForWindowsApiCalls(IEnumerable<UnifiedStackFrame> stackTrace, ref IEnumerable<UnifiedStackFrame> singleObjectList, ref IEnumerable<UnifiedStackFrame> multipleObjectList)
        {

            if (stackTrace != null)
            {
                singleObjectList = from frame in stackTrace
                       where CheckForWinApiCalls(frame, key0)
                       select frame;

                multipleObjectList = from frame in stackTrace
                                   where CheckForWinApiCalls(frame, key1)
                                   select frame;

            }

            return (singleObjectList != null && singleObjectList.Any()
                || (multipleObjectList != null && multipleObjectList.Any()));
        }

        private static bool CheckForWinApiCalls(UnifiedStackFrame c, string key)
        {
            bool result = c != null
                && !String.IsNullOrEmpty(c.Method)
                && c.Method != null && c.Method.Contains(key);

            return result;
        }

        public static List<byte[]> GetNativeParams(UnifiedStackFrame stackFrame, ClrRuntime runtime, int paramCount)
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

    }

    /*
        DWORD WINAPI WaitForSingleObject(
          _In_ HANDLE hHandle,
          _In_ DWORD  dwMilliseconds
        );


        DWORD WINAPI WaitForMultipleObjects(
          _In_       DWORD  nCount,
          _In_ const HANDLE *lpHandles,
          _In_       BOOL   bWaitAll,
          _In_       DWORD  dwMilliseconds
        );
    
    */
}

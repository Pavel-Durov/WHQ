using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.PrintHandles
{
    public  class WinApiCallsInspector
    {
        public const string WAIT_FOR_SINGLE_OBJECT_KEY = "WaitForSingleObject";
        public const string WAIT_FOR_MULTIPLE_OBJECTS_KEY = "WaitForMultipleObjects";

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
                       where CheckForWinApiCalls(frame, WAIT_FOR_SINGLE_OBJECT_KEY)
                       select frame;

                multipleObjectList = from frame in stackTrace
                                   where CheckForWinApiCalls(frame, WAIT_FOR_MULTIPLE_OBJECTS_KEY)
                                   select frame;

            }

            return (singleObjectList != null && singleObjectList.Any()
                || (multipleObjectList != null && multipleObjectList.Any()));
        }

        public static bool CheckForWinApiCalls(UnifiedStackFrame c, string key)
        {
            bool result = c != null
                && !String.IsNullOrEmpty(c.Method)
                && c.Method != null && c.Method.Contains(key);

            return result;
        }
    }
}

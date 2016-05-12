using Kernel32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DumpTest.Tests
{
    class Kernel32Calls
    {
        public static void Run()
        {
            MutexCalls();
            CriticalSectionCalls();
            WaitCalls();
        }

        private static async void MutexCalls()
        {
            SECURITY_ATTRIBUTES secAttribs = new SECURITY_ATTRIBUTES();
            secAttribs.nLength = Marshal.SizeOf(secAttribs);
            secAttribs.bInheritHandle = 1;

            var mutexAttributesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(secAttribs));
            var mutexName = "This-My-Mutex-Man";

            Console.WriteLine($"Creating Mutex with name : '{mutexName}'");

            var hMutex = Functions.CreateMutex(mutexAttributesPtr, true, mutexName);
            var lastError = Marshal.GetLastWin32Error();
            Console.WriteLine($"Win32 Error : {lastError}");

 
            while (true)
            {
                Console.WriteLine("OpenMutex");
                
                IntPtr openMutex = Functions.OpenMutex((uint)(SyncObjectAccess.MUTEX_MODIFY_STATE), false, mutexName);

                Console.WriteLine("CreateMutex");

                Functions.CreateMutex(mutexAttributesPtr, true, mutexName);

                var lastError2 = Marshal.GetLastWin32Error();
                Console.WriteLine($"Win32 Error : {lastError2}");
                await Task.Delay(1000);
            }
        }

        public static IntPtr section;
        private static async void CriticalSectionCalls()
        {
            //Console.WriteLine(" - Kernel32Calls IntPtr ");

            Functions.InitializeCriticalSection(out section);
            Console.WriteLine($"InitializeCriticalSection id: {Task.CurrentId} ");

            Functions.EnterCriticalSection(ref section);

            bool canenter = Functions.TryEnterCriticalSection(ref section);
            var onerror = Marshal.GetLastWin32Error();

            Console.WriteLine($"id: {Task.CurrentId}, can: {canenter }, error: {onerror }");


            await Task.Run(() =>
            {

                bool enter = Functions.TryEnterCriticalSection(ref section);
                Functions.LeaveCriticalSection(ref section);
                var error = Marshal.GetLastWin32Error();
                //Console.WriteLine($"id: {Task.CurrentId}, can: {enter}, error: {error}");

            });

            await Task.Run(() =>
            {
                bool enter = Functions.TryEnterCriticalSection(ref section);
                Functions.EnterCriticalSection(ref section);
                Functions.EnterCriticalSection(ref section);
 
                var error = Marshal.GetLastWin32Error();
                //Console.WriteLine($"id: {Task.CurrentId}, can: {enter}, error: {error}");
            });
          
        }

        private static void WaitCalls()
        {
            IntPtr[] arr = new IntPtr[3];
            for (int i = 0; i < 3; i++)
            {
                var loopAutoEvent = new AutoResetEvent(false);
                arr[i] = loopAutoEvent.Handle;
            }

            Console.WriteLine(" - Kernel32Calls WaitForMultipleObjects");
            var mulRes0 = Functions.WaitForMultipleObjects(3, arr, true, 0);
            var mulRes1 = Functions.WaitForMultipleObjects(3, arr, false, 0);
            var mulRes2 = Functions.WaitForMultipleObjects(3, arr, true, int.MaxValue);
        }
       
        public const Int32 INFINITE = -1;
        public const Int32 WAIT_ABANDONED = 0x80;
        public const Int32 WAIT_OBJECT_0 = 0x00;
        public const Int32 WAIT_TIMEOUT = 0x102;
        public const Int32 WAIT_FAILED = -1;
    }

   
}

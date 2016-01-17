using System;
using System.Runtime.InteropServices;
using HANDLE = System.IntPtr;

namespace Assignments.Core.WinApi
{
    public class Kernel32
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern HANDLE CreateEvent(SECURITY_ATTRIBUTES lpSecurityAttributes, bool isManualReset, bool initialState, string name);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool Beep(uint dwFreq, uint dwDuration);

        [DllImport("kernel32.dll")]
        static extern uint WaitForMultipleObjects(uint nCount, IntPtr[] lpHandles, bool bWaitAll, uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        public static extern HANDLE CreateEvent(HANDLE lpEventAttributes, [In, MarshalAs(UnmanagedType.Bool)] bool bManualReset, [In, MarshalAs(UnmanagedType.Bool)] bool bIntialState, [In, MarshalAs(UnmanagedType.BStr)] string lpName);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(HANDLE hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Int32 WaitForSingleObject(IntPtr Handle, uint Wait);



        /// <summary>
        /// </summary>
        /// <param name="dwDesiredAccess">Handle</param>
        /// <param name="bInheritHandle">If this value is TRUE, processes created by this process will inherit the handle. Otherwise, the processes do not inherit this handle.</param>
        /// <param name="processId"></param>
        /// <returns></returns>
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern HANDLE OpenProcess(Int64 dwDesiredAccess, bool bInheritHandle, Int32 processId);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/aa446619(v=vs.85).aspx
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool SetPrivilege(HANDLE token, PROCESS_PRIVILEGE Privilege, bool EnablePrivilege);

        public enum PROCESS_PRIVILEGE : long
        {
            SDELETE = 0x00010000L,
            /*Required to delete the object. */
            READ_CONTROL = 0x00020000L,
            /*Required to read information in the security descriptor for the object, not including the information in the SACL.To read or write the SACL, you must request the ACCESS_SYSTEM_SECURITY access right.For more information, see SACL Access Right. */
            SYNCHRONIZE = 0x00100000L,
            /*The right to use the object for synchronization. This enables a thread to wait until the object is in the signaled state.*/
            WRITE_DAC = 0x00040000L,
            /*Required to modify the DACL in the security descriptor for the object.*/
            WRITE_OWNER = 0x00080000L
            /*Required to change the owner in the security descriptor for the object.*/
        }

        #region Constants

        public const Int32 INFINITE = -1;
        public const Int32 WAIT_ABANDONED = 0x80;
        public const Int32 WAIT_OBJECT_0 = 0x00;
        public const Int32 WAIT_TIMEOUT = 0x102;
        public const Int32 WAIT_FAILED = -1;

        #endregion


        #region Structs

        public struct SECURITY_ATTRIBUTES
        {
            Int32 nLength;
            IntPtr lpSecurityDescriptor;
            bool bInheritHandle;
        };

        #endregion

    }
}



namespace Assignments.Core.Handlers.WCT
{
    /// <summary>
    /// Doc: https://msdn.microsoft.com/en-us/library/windows/desktop/ms681388(v=vs.85).aspx
    /// </summary>
    public enum SYSTEM_ERROR_CODES
    {
        /// <summary>
        /// Overlapped I/O operation is in progress. (997 (0x3E5))
        /// </summary>
        ERROR_IO_PENDING = 997 
    }

    public enum WCT_SESSION_OPEN_FLAGS
    {
        WCT_SYNC_OPEN_FLAG = 0,
        WCT_ASYNC_OPEN_FLAG = 1
    }

    class WctApiConst
    {
        //Consts doc:
        //http://winappdbg.sourceforge.net/doc/v1.4/reference/winappdbg.win32.advapi32-module.html
        public const int WCT_MAX_NODE_COUNT = 16;
        public const uint WCTP_GETINFO_ALL_FLAGS = 7;
        public const int WCT_OBJNAME_LENGTH = 128;
        
    }
}

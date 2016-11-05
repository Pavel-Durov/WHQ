using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinNativeApi
{        
    //DbgEng.h
    public class DbgEng
    {
        #region Constants

        public const uint S_OK = 0x00000000; /*Operation successful*/
        public const uint E_ABORT = 0x80004004;//Operation aborted 
        public const uint E_ACCESSDENIED = 0x80070005;//General access denied error 
        public const uint E_FAIL = 0x80004005;//Unspecified failure 
        public const uint E_HANDLE = 0x80070006;//Handle that is not valid 
        public const uint E_INVALIDARG = 0x80070057;//One or more arguments are not valid 
        public const uint E_NOINTERFACE = 0x80004002;//No such interface supported 
        public const uint E_NOTIMPL = 0x80004001;//Not implemented 
        public const uint E_OUTOFMEMORY = 0x8007000E;//Failed to allocate necessary memory 
        public const uint E_POINTER = 0x80004003;//Pointer that is not valid 
        public const uint E_UNEXPECTED = 0x8000FFFF;//Unexpected failure 

        #endregion
    }
}

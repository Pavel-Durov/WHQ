using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Assignments.Core.Utils
{
    public class StringUtil
    {
        public static unsafe String ConvertCStringToString(ushort* ptr, Encoding encoding)
        {
            if (ptr == null) return "";

            char* unsafeCString = (char*)ptr;
            return ConvertCStringToString(unsafeCString, encoding);
        }

        public static unsafe String ConvertCStringToString(char* ptr, Encoding encoding)
        {
            
            //TODO: Check the string length calculation....

            if (ptr == null) return "";

            char* unsafeCString = (char*)ptr;

            int lengthOfCString = 0;
            while (unsafeCString[lengthOfCString] != '\0')
            {
                lengthOfCString++;
            }

            // now that we have the length of the string, let's get its size in bytes
            int lengthInBytes = encoding.GetByteCount(unsafeCString, lengthOfCString);
            byte[] asByteArray = new byte[lengthInBytes];

            fixed (byte* ptrByteArray = asByteArray)
            {
                encoding.GetBytes(unsafeCString, lengthOfCString, ptrByteArray, lengthInBytes);
            }

            // now get the string
            return encoding.GetString(asByteArray);
        }
    }
}
   
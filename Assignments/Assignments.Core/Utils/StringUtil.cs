using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Utils
{
    public class StringUtil
    {

        public static unsafe String MarshalUnsafeCStringToString(ushort* ptr, Encoding encoding)
        {

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

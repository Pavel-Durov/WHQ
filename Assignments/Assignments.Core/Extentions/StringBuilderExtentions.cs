using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Extentions
{
    public static class StringBuilderExtentions
    {
        public static StringBuilder AppendWithNewLine(this StringBuilder sb, string str)
        {
            sb.Append($"{ str}{Environment.NewLine}");
            return sb;
        }
    }
}

using System;
using System.Text;

namespace Assignments.Core.Extentions
{
    public static class StringBuilderExtentions
    {
        public static StringBuilder AppendWithNewLine(this StringBuilder sb, String str = "")
        {
            sb.Append($"{str}{Environment.NewLine}");
            return sb;
        }
    }
}

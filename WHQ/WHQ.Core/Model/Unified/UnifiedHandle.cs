using System;

namespace WHQ.Core.Model.Unified
{
    public class UnifiedHandle
    {
        public UnifiedHandle(ulong uid, string type = null, string objectName = null)
        {
            Id = uid;
            Type = type;
            ObjectName = objectName;
        }

        public ulong Id { get; private set; }
        public string Type { get; private set; }
        public string ObjectName { get; private set; }
    }
}

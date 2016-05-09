using System;

namespace WinHandlesQuerier.Core.Model.Unified
{
    public class UnifiedHandle
    {
        public UnifiedHandle(uint uid, string type = null, string objectName = null)
        {
            Id = uid;
            Type = type;
            ObjectName = objectName;
        }

        public uint Id { get; private set; }
        public string Type { get; private set; }
        public string ObjectName { get; private set; }
    }
}

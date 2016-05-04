using System;

namespace Assignments.Core.Model.Unified
{
    public class UnifiedHandle
    {
        public UnifiedHandle(uint uid, string type = null)
        {
            Id = uid;
            Type = type;
        }

        public uint Id { get; private set; }
        public string Type { get; private set; }
    }
}

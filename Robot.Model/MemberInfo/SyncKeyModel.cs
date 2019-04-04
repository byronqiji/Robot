using System.Collections.Generic;

namespace Robot.Model.MemberInfo
{
    public class SyncKeyModel
    {
        public int Count { get; set; }

        public List<SyncKeyItem> List { get; set; }
    }

    public class SyncKeyItem
    {
        public uint Key { get; set; }

        public ulong Val { get; set; }
    }
}

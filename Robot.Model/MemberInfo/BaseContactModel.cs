using System.Collections.Generic;

namespace Robot.Model.MemberInfo
{
    public abstract class BaseContactModel
    {
        protected Dictionary<string, MemberModel> memberDic;

        public int Uin { get; set; } //0,

        public string UserName { get; set; } //@@9cde8f179ec49687b3b1c74e13bf4e0fcf10baa3b0d949ae6653594d737d957f",

        public string NickName { get; set; } //技术吃货群",

        public string HeadImgUrl { get; set; } ///cgi-bin/mmwebwx-bin/webwxgetheadimg?seq=0&username=@@9cde8f179ec49687b3b1c74e13bf4e0fcf10baa3b0d949ae6653594d737d957f&skey=@crypt_4af0643b_a71ee7afdd07832e286b5ea4b5dcba10",

        public int MemberCount { get; set; } //147,

        public List<MemberModel> MemberList { get; set; }

        public abstract int ContactFlag { get; set; }

        internal void Initial()
        {
            if (MemberList == null || MemberList.Count <= 0)
                return;

            memberDic = new Dictionary<string, MemberModel>();
            foreach (MemberModel member in MemberList)
            {
                memberDic.Add(member.UserName, member);
            }
        }

        internal void SetMember(List<MemberModel> memberList)
        {
            foreach (MemberModel member in memberList)
            {
                if (!memberDic.ContainsKey(member.UserName))
                    memberDic.Add(member.UserName, member);
            }
        }
    }
}

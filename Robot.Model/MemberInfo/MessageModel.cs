using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.Model.MemberInfo
{
    public class MessageModel
    {
        public string MsgId { get; set; }

        public string FromUserName { get; set; }

        public string ToUserName { get; set; }

        public int MsgType { get; set; }

        public string Content { get; set; }
    }
}

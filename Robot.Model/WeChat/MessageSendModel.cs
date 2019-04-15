namespace Robot.Model.WeChat
{
    public class MessageSendModel
    {
        public string ClientMsgId { get; set; }

        /// <summary>
        /// 发送的内容
        /// </summary>
        public string Content { get; set; }

        public string FromUserName { get; set; }

        public string LocalID { get; set; }

        public string ToUserName { get; set; }

        public int Type { get; set; }
    }
}

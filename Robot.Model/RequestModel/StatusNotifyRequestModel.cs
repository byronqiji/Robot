using Robot.Model.WeChat;

namespace Robot.Model.RequestModel
{
    public class StatusNotifyRequestModel
    {
        public BaseRequestModel BaseRequest { get; set; }

        //ClientMsgId: 1554473523742
        //Code: 3
        //FromUserName: "@e2dd3c6ee71c80018abbfbf9075b5187"
        //ToUserName: "@e2dd3c6ee71c80018abbfbf9075b5187"
        public ulong ClientMsgId { get; set; }

        public int Code { get; set; } = 3;

        public string FromUserName { get; set; }

        public string ToUserName { get; set; }

        public static StatusNotifyRequestModel Create()
        {
            //ClientMsgId: 1554473523742
            //Code: 3
            //FromUserName: "@e2dd3c6ee71c80018abbfbf9075b5187"
            //ToUserName: "@e2dd3c6ee71c80018abbfbf9075b5187"
            StatusNotifyRequestModel model = new StatusNotifyRequestModel()
            {
                BaseRequest = BaseRequestModel.Create(),
                ClientMsgId = UserInfo.Instance.DateTimeDelt,
                FromUserName = UserInfo.Instance.User.UserName,
                ToUserName = UserInfo.Instance.User.UserName
            };

            return model;
        }
    }
}

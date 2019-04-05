using Robot.Model.RequestModel;

namespace Robot.Model.WeChat
{
    public class SendModel
    { 
        public BaseRequestModel BaseRequest { get; set; }

        public MessageModel Msg { get; set; }

        public int Scene { get; set; }
    }
}

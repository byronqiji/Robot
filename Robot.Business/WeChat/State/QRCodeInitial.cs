using Robot.Request;

namespace Robot.Business.WeChat.State
{
    public class QRCodeInitial : UserState
    {
        public QRCodeInitial(UserManager um)
            : base(um)
        { 
        }

        protected override string StateName
        {
            get
            {
                return "初始化二维码";
            }
        }

        public override void Start()
        {
            string value = HttpHelper.GetResponseValue(userManager.UserInfo.QRInitialUrl);

            if (value != string.Empty)
            {
                userManager.UserInfo.UUID = value.Split('\"')[1];
                userManager.State = userManager.ReadyScan;
            }
        }
    }
}

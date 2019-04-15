using Robot.Model.WeChat;
using Robot.Request;

namespace Robot.Business.WeChat.State
{
    public class ReadyScan : UserState
    {
        public ReadyScan(UserManager um)
            : base(um)
        { 
        }

        protected override string StateName
        {
            get
            {
                return "待扫码";
            }
        }

        public override string Monitor()
        {
            string value = HttpHelper.GetResponseValue(userManager.UserInfo.LoginUrl);

            if (value.Split('=')[1].Split(';')[0] == "201")
                userManager.State = userManager.Scaned;

            return value;
        }
    }
}

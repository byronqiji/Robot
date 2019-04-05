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
            //                          https://login.wx.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid=Yd5gXvu2mQ==&tip=1&r=1181519240&_=1497762066796
            string url = string.Format("https://login.wx.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid={0}&tip={2}&_={1}", UserInfo.Instance.UUID, UserInfo.Instance.RequestCount, UserInfo.Instance.TIP);

            //window.code=408
            //window.code=201;
            string value = HttpHelper.GetResponseValue(url);

            if (value.Split('=')[1].Split(';')[0] == "201")
                userManager.State = userManager.Scaned;

            return value;
        }
    }
}

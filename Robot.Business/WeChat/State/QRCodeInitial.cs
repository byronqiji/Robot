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
            //https://login.wx.qq.com/jslogin?appid=wx782c26e4c19acffb&redirect_uri=https%3A%2F%2Fwx.qq.com%2Fcgi-bin%2Fmmwebwx-bin%2Fwebwxnewloginpage&fun=new&lang=zh_CN&_=1497712047909
            string url = string.Format("https://login.wx.qq.com/jslogin?appid=wx782c26e4c19acffb&redirect_uri=https%3A%2F%2Fwx.qq.com%2Fcgi-bin%2Fmmwebwx-bin%2Fwebwxnewloginpage&fun=new&lang=zh_CN&_={0}", userManager.User.RequestCount);
            
            string value = HttpHelper.GetResponseValue(url);

            if (value != string.Empty)
            {
                //window.QRLogin.code = 200; window.QRLogin.uuid = "AbT66_9SIw==";
                userManager.User.UUID = value.Split('\"')[1];
                userManager.State = userManager.ReadyScan;
            }
        }
    }
}

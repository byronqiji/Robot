using Robot.Request;
using System.Text;
using System.Web;

namespace Robot.Business.WeChat.State
{
    public class Login : UserState
    {
        public Login(UserManager um)
            : base(um)
        {
        }

        protected override string StateName
        {
            get
            {
                return "已登录";
            }
        }

        public override string Monitor()
        {
            //https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?r=1498915088960&skey=%40crypt_4af0643b_9f615fbce9ddb2be3fce910f3e285ee8&sid=E5pvocibbKBAWjXh&
            //uin =1126851900&deviceid=e336493130571149&synckey=1_658954891%7C2_658955296%7C3_658955084%7C11_658955257%7C13_658700011%7C201_1498915083%7C203_1498913594%7C1000_1498903382%7C1001_1498903412&_=1498911897174

            string url = $"https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?r={userManager.User.RequestCount}&skey={HttpUtility.UrlEncode(userManager.User.SKey, Encoding.UTF8)}&sid={userManager.User.SID}&" +
                $"uin={HttpUtility.UrlEncode(userManager.User.UIN, Encoding.UTF8)}&deviceid={userManager.User.DeviceID}&synckey={HttpUtility.UrlEncode(userManager.User.SyncKey, Encoding.UTF8)}&_={userManager.User.RequestCount}";

            string value = HttpHelper.GetResponseValue(url, userManager.User.Cookies);

            return base.Monitor();
        }
    }
}

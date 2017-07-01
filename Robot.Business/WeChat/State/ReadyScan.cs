using Robot.Business.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //https://login.wx.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid=Yd5gXvu2mQ==&tip=1&r=1181519240&_=1497762066796
            string url = string.Format("https://login.wx.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid={0}==&tip={2}&_={1}", userManager.User.UUID, userManager.User.RequestCount, userManager.User.TIP);

            string value = HttpHelper.GetResponseValue(url);

            return value;
        }
    }
}

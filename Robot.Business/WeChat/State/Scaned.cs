using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Robot.Model.MemberInfo;
using Robot.Model.RequestModel;
using Robot.Model.WeChat;
using Robot.Request;
using System;
using System.IO;
using System.Net;
using System.Xml;

namespace Robot.Business.WeChat.State
{
    public class Scaned : UserState
    {
        public Scaned(UserManager um)
            :base (um)
        { 
        }

        protected override string StateName
        {
            get
            {
                return "已扫码";
            }
        }

        public override string Monitor()
        {
            //                          https://login.wx.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid=oboytYOOQA==&tip=0&r=474606286&_=1554303542497
            //                          https://login.wx.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid=Yd5gXvu2mQ==&tip=1&r=1181519240&_=1497762066796
            //                          https://login.wx.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid=4a4qFGUpiw==&tip=0&r=475841793&_=1554302308495
            string url = string.Format("https://login.wx.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid={0}&tip={2}&_={1}", UserInfo.Instance.UUID, UserInfo.Instance.RequestCount, UserInfo.Instance.TIP);
            
            string value = HttpHelper.GetResponseValue(url);
            //window.code=201;window.redirect_uri="https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxnewloginpage?ticket=A5ncNUM2NJBYNOpJ49Jd38m2@qrticket_0&uuid=Ia7HTPkEdQ==&lang=zh_CN&scan=1485320697"
            if (value.Split('=')[1].Split(';')[0] == "200")
            {
                SetUserInfo(value.Split('\"')[1] + "&fun=new&version=v2");
                InitialWeChat();

                userManager.State = userManager.Login;
            }

            return value;
        }

        private void InitialWeChat()
        {
            // https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxinit?r=475852637&lang=zh_CN&pass_ticket=A2i9g98QReOimAFpX1XuB6jQ%252Fqtm%252F3DFsg72UzzWHtHMhVevsKux9pWpp%252B7NvBFL
            string url = $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxinit?r={UserInfo.Instance.BackTimeSpan}&lang=zh_CN&pass_ticket={UserInfo.Instance.PassTicket}";

            InitialTree(url, true);
            
            //     https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxstatusnotify?lang=zh_CN&pass_ticket=dIHhIDH%252FB164MLcnXXvg6C6eduvLod3Ub2wnAsYF6JLLDy5s4UYOu7MWsJGKlDwM
            url = $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxstatusnotify?lang=zh_CN&pass_ticket={UserInfo.Instance.PassTicket}";


            StatusNotifyRequestModel snrm = StatusNotifyRequestModel.Create();

            string value = HttpHelper.GetResponseValue(url, HttpMethod.POST, JsonConvert.SerializeObject(snrm));

            using (StreamWriter sw = new StreamWriter(UrlFilePath, true))
                sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}\t{url}\t{JsonConvert.SerializeObject(snrm)}");

            //using (StreamWriter sw = new StreamWriter(Path + "\\getcontact.txt", true))
            //    sw.Write(value);

            url = $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact?lang=zh_CN&pass_ticket={UserInfo.Instance.PassTicket}&r={UserInfo.Instance.DateTimeDelt}&seq=0&skey=@{UserInfo.Instance.SKey}";
            value = HttpHelper.GetResponseValue(url, HttpMethod.GET, null);
            
            using (StreamWriter sw = new StreamWriter(UrlFilePath, true))
                sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}\t{url}");

            using (StreamWriter sw = new StreamWriter(Path + "\\getcontact.txt", true))
                sw.Write(value);

            //url = $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r={UserInfo.Instance.DateTimeDelt}&lang=zh_CN&pass_ticket={UserInfo.Instance.PassTicket}";

            //InitialTree(url, false);
        }

        private void InitialTree(string url, bool setSyncKeyList)
        {
            var postData = new { BaseRequest = new { Uin = UserInfo.Instance.UIN, Sid = UserInfo.Instance.SID, Skey = UserInfo.Instance.SKey, DeviceID = UserInfo.Instance.DeviceID } };

            string value = HttpHelper.GetResponseValue(url, HttpMethod.POST, JsonConvert.SerializeObject(postData));

            using (StreamWriter sw = new StreamWriter(InitialFilePath, true))
                sw.Write(value);

            using (StreamWriter sw = new StreamWriter(UrlFilePath, true))
            {
                sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}\t{url}");
                sw.WriteLine(JsonConvert.SerializeObject(postData));
            }

            InitialContactTree tree = JsonConvert.DeserializeObject<InitialContactTree>(value);
            tree.Initial();
            UserInfo.Instance.SetContact(tree.ContactList);

            if (setSyncKeyList)
            {
                UserInfo.Instance.SyncKeyInfo = tree.SyncKey;
                UserInfo.Instance.User = tree.User;
            }
        }

        private void SetUserInfo(string loginUrl)
        {
            WebResponse response = HttpHelper.GetResponse(loginUrl, null);

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(HttpHelper.GetResponseValue(response));
            XmlNode root = xd.SelectSingleNode("error");

            UserInfo.Instance.SKey = root.ChildNodes[2].InnerText;
            UserInfo.Instance.SID = root.ChildNodes[3].InnerText;
            UserInfo.Instance.UIN = Convert.ToUInt64(root.ChildNodes[4].InnerText);
            UserInfo.Instance.PassTicket = root.ChildNodes[5].InnerText;

            UserInfo.Instance.Cookies = ((HttpWebResponse)response).Cookies;
        }
    }
}

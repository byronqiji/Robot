﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private const string SyncKeyString = "SyncKey";
        private const string ListString = "List";

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
            //                          https://login.wx.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid=Yd5gXvu2mQ==&tip=1&r=1181519240&_=1497762066796
            string url = string.Format("https://login.wx.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid={0}&tip={2}&_={1}", userManager.User.UUID, userManager.User.RequestCount, userManager.User.TIP);
            
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
            // https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxinit?r=31689187&pass_ticket=7tGcP7LNCPegHGPMhV7Ltvq2vt%252FOpkSapS%252FJVprdMzMDlkN0yq3GUZdGSxciK4W5
            string url = $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxinit?r={userManager.User.DateTimeDelt}&pass_ticket={userManager.User.PassTicket}";

            var postData = new { BaseRequest = new { Uin = userManager.User.UIN, Sid = userManager.User.SID, Skey = userManager.User.SKey, DeviceID = userManager.User.DeviceID } };

            string value = HttpHelper.GetResponseValue(url, HttpMethod.POST, JsonConvert.SerializeObject(postData));

            using (StreamWriter sw = new StreamWriter(Path + "\\initial.txt", false))
            {
                sw.Write(value);
            }

            var info = JsonConvert.DeserializeObject(value) as JObject;

            for (int i = 0; i < (int)(info[SyncKeyString]["Count"]); ++i)
            {
                userManager.User.AddSyncKey(new SyncKeyInfo() { Key = info[SyncKeyString][ListString][i]["Key"].ToString(), Val = info[SyncKeyString][ListString][i]["Val"].ToString() });
            }
        }

        private void SetUserInfo(string loginUrl)
        {
            WebResponse response = HttpHelper.GetResponse(loginUrl, null);

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(HttpHelper.GetResponseValue(response));
            XmlNode root = xd.SelectSingleNode("error");

            userManager.User.SKey = root.ChildNodes[2].InnerText;
            userManager.User.SID = root.ChildNodes[3].InnerText;
            userManager.User.UIN = root.ChildNodes[4].InnerText;
            userManager.User.PassTicket = root.ChildNodes[5].InnerText;

            userManager.User.Cookies = ((HttpWebResponse)response).Cookies;
        }
    }
}

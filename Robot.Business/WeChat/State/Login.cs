using Newtonsoft.Json;
using Robot.Model.MemberInfo;
using Robot.Model.RequestModel;
using Robot.Model.WeChat;
using Robot.Request;
using System;
using System.IO;
using System.Net;
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
            string msgs = string.Empty;
            string url = $"https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?r={UserInfo.Instance.DateTimeDelt}&skey={HttpUtility.UrlEncode(UserInfo.Instance.SKey, Encoding.UTF8)}&sid={UserInfo.Instance.SID}&" +
                $"uin={HttpUtility.UrlEncode(UserInfo.Instance.UIN.ToString(), Encoding.UTF8)}&deviceid={UserInfo.Instance.DeviceID}&synckey={HttpUtility.UrlEncode(UserInfo.Instance.SyncKey, Encoding.UTF8)}&_={UserInfo.Instance.RequestCount}";

            string value = HttpHelper.GetResponseValue(url, UserInfo.Instance.Cookies);

            using (StreamWriter sw = new StreamWriter(Path + "\\sync.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + value);
            }

            using (StreamWriter sw = new StreamWriter(UrlFilePath, true))
            {
                sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}: {url}");
            }
            
            string[] tempArr = value.Trim().Split('=');

            if (tempArr.Length >= 2)
            {
                SyncCheck sc = JsonConvert.DeserializeObject<SyncCheck>(tempArr[1]);
                if (sc.retcode == "0" && sc.selector != "0")
                {
                    var data = new
                    {
                        BaseRequest = new
                        {
                            Uin = UserInfo.Instance.UIN,
                            Sid = UserInfo.Instance.SID,
                        },

                        SyncKey = UserInfo.Instance.SyncKeyInfo,
                        rr = UserInfo.Instance.DateTimeDelt,
                    };

                    string jsonData = JsonConvert.SerializeObject(data);

                    using (StreamWriter sw = new StreamWriter(DataFilePath, true))
                    {
                        sw.WriteLine(value);
                    }

                    string u = $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsync?sid={UserInfo.Instance.SID}&skey={UserInfo.Instance.SKey}&lang=zh_CN&pass_ticket={UserInfo.Instance.PassTicket}";

                    //value = HttpHelper.GetResponseValue(u, HttpMethod.POST, jsonData);

                    WebResponse webResponse = HttpHelper.CreateRequest(url, HttpMethod.POST, jsonData).GetResponse();
                    value = HttpHelper.GetResponseValue(webResponse);

                    CookieCollection cookies = ((HttpWebResponse)webResponse).Cookies;
                    if (cookies != null && cookies.Count > 0)
                        UserInfo.Instance.Cookies = ((HttpWebResponse)webResponse).Cookies;

                    using (StreamWriter sw = new StreamWriter(UrlFilePath, true))
                    {
                        sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}: {u}");
                        sw.WriteLine(jsonData);
                    }

                    using (StreamWriter sw = new StreamWriter(MessageFilePath, true))
                    {
                        sw.WriteLine(value);
                    }

                    try
                    {
                        MessageContactTree mct = JsonConvert.DeserializeObject<MessageContactTree>(value);
                        mct.Initial();

                        //UserInfo.Instance.SetContact(mct.ContactList);
                        if (mct.SyncKey.Count > 0)
                            UserInfo.Instance.SyncKeyInfo = mct.SyncKey;

                        if (mct.AddMsgCount > 0)
                        {
                            foreach (var msg in mct.AddMsgList)
                            {
                                string content = msg.Content.Replace("<br />", "<br/>");

                                if (string.IsNullOrEmpty(content) || !content.Contains("<br/>"))
                                    continue;

                                int s = content.LastIndexOf("<br/>") + "<br/>".Length;
                                string message = msg.Content.Substring(s, msg.Content.Length - s);
                                string sendUser = msg.Content.Substring(0, s - ":<br/>".Length);


                                BaseContactModel bcm = UserInfo.Instance[msg.FromUserName];
                                if (bcm == null)
                                {
                                    GetContact(msg.FromUserName);
                                }

                                bcm = UserInfo.Instance[msg.FromUserName];

                                if (bcm != null)
                                {
                                    MemberModel mm = bcm[sendUser];

                                    if (string.IsNullOrEmpty(mm.NickName) && string.IsNullOrEmpty(mm.DisplayName))
                                    {
                                        GetContact(msg.FromUserName);
                                    }

                                    mm = UserInfo.Instance[msg.FromUserName][sendUser];

                                    if (mm != null)
                                    {
                                        using (StreamWriter sw = new StreamWriter(Path + $"\\{bcm.NickName}_{DateTime.Now.ToString("yyyyMMdd")}.txt", true))
                                        {
                                            sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {(string.IsNullOrEmpty(mm.DisplayName) ? mm.NickName : mm.DisplayName)}: {message}");
                                        }
                                    }
                                }

                                msgs += message + "<br />";
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return msgs;
        }

        private void GetContact(string userName)
        {
            string url = $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r={UserInfo.Instance.DateTimeDelt}&lang=zh_CN&pass_ticket={UserInfo.Instance.PassTicket}";

            BatchRequestModel brm = BatchRequestModel.Create();
            brm.Count = 1;
            brm.List.AddLast(new BatchItem { UserName = userName });

            string json = JsonConvert.SerializeObject(brm);
            using (StreamWriter sw = new StreamWriter(Path + "\\batch.txt", true))
            {
                sw.WriteLine(json);
            }

            string value = HttpHelper.GetResponseValue(url, HttpMethod.POST, json);
            using (StreamWriter sw = new StreamWriter(Path + "\\member.txt", true))
            {
                sw.WriteLine(value);
            }
            InitialContactTree tree = JsonConvert.DeserializeObject<InitialContactTree>(value);
            tree.Initial();
            UserInfo.Instance.SetContact(tree.ContactList);
        }
    }
}

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
            //string url = $"https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?r={userManager.UserInfo.DateTimeDelt}&skey={HttpUtility.UrlEncode(userManager.UserInfo.SKey, Encoding.UTF8)}&sid={userManager.UserInfo.SID}&" +
            //    $"uin={HttpUtility.UrlEncode(userManager.UserInfo.UIN.ToString(), Encoding.UTF8)}&deviceid={userManager.UserInfo.DeviceID}&synckey={HttpUtility.UrlEncode(userManager.UserInfo.SyncKey, Encoding.UTF8)}&_={userManager.UserInfo.RequestCount}";

            string value = HttpHelper.GetResponseValue(userManager.UserInfo.SyncCheckUrl, userManager.UserInfo.Cookies);

            using (StreamWriter sw = new StreamWriter(Path + "\\sync.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + value);
            }

            //using (StreamWriter sw = new StreamWriter(UrlFilePath, true))
            //{
            //    sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}: {url}");
            //}
            
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
                            Uin = userManager.UserInfo.UIN,
                            Sid = userManager.UserInfo.SID,
                        },

                        SyncKey = userManager.UserInfo.SyncKeyInfo,
                        rr = userManager.UserInfo.DateTimeDelt,
                    };

                    string jsonData = JsonConvert.SerializeObject(data);

                    using (StreamWriter sw = new StreamWriter(DataFilePath, true))
                    {
                        sw.WriteLine(value);
                    }

                    //string u = $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsync?sid={userManager.UserInfo.SID}&skey={userManager.UserInfo.SKey}&lang=zh_CN&pass_ticket={userManager.UserInfo.PassTicket}";

                    //value = HttpHelper.GetResponseValue(u, HttpMethod.POST, jsonData);

                    WebResponse webResponse = HttpHelper.CreateRequest(userManager.UserInfo.SyncUrl, HttpMethod.POST, jsonData, userManager.UserInfo.Cookies).GetResponse();
                    value = HttpHelper.GetResponseValue(webResponse);

                    CookieCollection cookies = ((HttpWebResponse)webResponse).Cookies;
                    if (cookies != null && cookies.Count > 0)
                        userManager.UserInfo.Cookies = ((HttpWebResponse)webResponse).Cookies;

                    //using (StreamWriter sw = new StreamWriter(UrlFilePath, true))
                    //{
                    //    sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}: {u}");
                    //    sw.WriteLine(jsonData);
                    //}

                    using (StreamWriter sw = new StreamWriter(MessageFilePath, true))
                    {
                        sw.WriteLine(value);
                    }

                    try
                    {
                        MessageContactTree mct = JsonConvert.DeserializeObject<MessageContactTree>(value);
                        mct.Initial();

                        //userManager.UserInfo.SetContact(mct.ContactList);
                        if (mct.SyncKey.Count > 0)
                            userManager.UserInfo.SyncKeyInfo = mct.SyncKey;

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


                                BaseContactModel bcm = userManager.UserInfo[msg.FromUserName];
                                if (bcm == null)
                                {
                                    GetContact(msg.FromUserName);
                                }

                                bcm = userManager.UserInfo[msg.FromUserName];

                                if (bcm != null)
                                {
                                    MemberModel mm = bcm[sendUser];

                                    if (string.IsNullOrEmpty(mm.NickName) && string.IsNullOrEmpty(mm.DisplayName))
                                    {
                                        GetContact(msg.FromUserName);
                                    }

                                    mm = userManager.UserInfo[msg.FromUserName][sendUser];

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
            //string url = $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r={userManager.UserInfo.DateTimeDelt}&lang=zh_CN&pass_ticket={userManager.UserInfo.PassTicket}";

            BatchRequestModel brm = BatchRequestModel.Create(userManager.UserInfo);
            brm.Count = 1;
            brm.List.AddLast(new BatchItem { UserName = userName });

            string json = JsonConvert.SerializeObject(brm);
            using (StreamWriter sw = new StreamWriter(Path + "\\batch.txt", true))
            {
                sw.WriteLine(json);
            }

            string value = HttpHelper.GetResponseValue(userManager.UserInfo.BatchContactUrl, HttpMethod.POST, json, userManager.UserInfo.Cookies);
            using (StreamWriter sw = new StreamWriter(Path + "\\member.txt", true))
            {
                sw.WriteLine(value);
            }
            InitialContactTree tree = JsonConvert.DeserializeObject<InitialContactTree>(value);
            tree.Initial();
            userManager.UserInfo.SetContact(tree.ContactList);
        }

        private void SendMessage()
        {
        }
    }
}

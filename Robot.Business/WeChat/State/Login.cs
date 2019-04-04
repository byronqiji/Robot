using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Robot.Model.MemberInfo;
using Robot.Model.WeChat;
using Robot.Request;
using System;
using System.IO;
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
            string url = $"https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?r={userManager.User.DateTimeDelt}&skey={HttpUtility.UrlEncode(userManager.User.SKey, Encoding.UTF8)}&sid={userManager.User.SID}&" +
                $"uin={HttpUtility.UrlEncode(userManager.User.UIN, Encoding.UTF8)}&deviceid={userManager.User.DeviceID}&synckey={HttpUtility.UrlEncode(userManager.User.SyncKey, Encoding.UTF8)}&_={userManager.User.RequestCount}";

            string value = HttpHelper.GetResponseValue(url, userManager.User.Cookies);

            using (StreamWriter sw = new StreamWriter(Path + "\\sync.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + value);
            }

            using (StreamWriter sw = new StreamWriter(UrlFilePath, true))
            {
                sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}: {url}");
            }
            
            string[] tempArr = value.Trim().Split('=');

            SyncCheck sc = JsonConvert.DeserializeObject<SyncCheck>(tempArr[1]);
            if (sc.retcode == "0" && sc.selector == "2")
            {
                var data = new
                {
                    BaseRequest = new
                    {
                        Uin = userManager.User.UIN,
                        Sid = userManager.User.SID,
                    },

                    AccountModel.Instance.SyncKey,
                    rr = userManager.User.DateTimeDelt,
                };

                string jsonData = JsonConvert.SerializeObject(data);

                using (StreamWriter sw = new StreamWriter(DataFilePath, true))
                {
                    sw.WriteLine(value);
                }

                string u = $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsync?sid={userManager.User.SID}&skey={userManager.User.SKey}&lang=zh_CN&pass_ticket={userManager.User.PassTicket}";

                value = HttpHelper.GetResponseValue(u, HttpMethod.POST, jsonData);

                using (StreamWriter sw = new StreamWriter(UrlFilePath, true))
                {
                    sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}: {u}");
                    sw.WriteLine(jsonData);
                }

                using (StreamWriter sw = new StreamWriter(MessageFilePath, true))
                {
                    sw.WriteLine(value);
                }

                MessageContactTree mct = JsonConvert.DeserializeObject<MessageContactTree>(value);
                mct.Initial();

                AccountModel.Instance.SetContact(mct.ContactList);
                AccountModel.Instance.SyncKey = mct.SyncKey;

                userManager.User.SyncKeyList = mct.SyncKey.List;

                if (mct.AddMsgCount > 0)
                {
                    foreach (var msg in mct.AddMsgList)
                    {
                        string content = msg.Content.Replace("<br />", "<br/>");

                        if (string.IsNullOrEmpty(content) || !content.Contains("<br/>"))
                            continue;

                        int s = content.LastIndexOf("<br/>") + "<br/>".Length;
                        msgs += msg.Content.Substring(s, msg.Content.Length - s) + "<br />";
                    }
                }
            }

            return msgs;
        }
    }
}

using Robot.Model.MemberInfo;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace Robot.Model.WeChat
{
    public class UserInfo
    {
        //private static UserInfo single;

        //public static UserInfo Instance
        //{
        //    get
        //    {
        //        if (single != null)
        //            return single;

        //        UserInfo temp = new UserInfo();
        //        Interlocked.CompareExchange(ref single, temp, null);

        //        return single;
        //    }
        //}
        
        private static readonly DateTime standardDateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 8, 0, 0));

        int tip;
        private ulong requestCount;

        private Dictionary<string, BaseContactModel> contactDic;

        public SyncKeyModel SyncKeyInfo { get; set; }

        public UserInfo()
        {
            requestCount = DateTimeDelt;

            tip = 1;
            contactDic = new Dictionary<string, BaseContactModel>();
        }

        public string UUID { get; set; }

        public int TIP 
        { 
            get 
            {
                if (tip == 1)
                {
                    tip--;
                    return 1;
                }
                else
                    return tip;
            }
        }

        public uint BackTimeSpan
        {
            get
            {
                return (uint)~DateTimeDelt;
            }
        }

        public ulong DateTimeDelt 
        {
            get
            {
                return (ulong)(DateTime.Now - standardDateTime).TotalMilliseconds;
            }
        }

        public ulong LocalID
        {
            get
            {
                Random r = new Random();
                return (ulong)(DateTime.Now - standardDateTime).TotalMilliseconds * 10000 + (ulong)r.Next(0, 1000);
            }
        }

        public ulong RequestCount
        {
            get
            {
                return requestCount++;
            }
        }

        public string SKey { get; set; }

        public string SID { get; set; }

        public ulong UIN { get; set; }

        public string PassTicket { get; set; }

        public CookieCollection Cookies { get; set; }

        public string DeviceID
        {
            get
            {
                Random r = new Random();
                return $"e{r.Next(1000000, 9999999).ToString()}{r.Next(10000000, 99999999).ToString()}";
            }
        }

        public string SyncKey
        {
            get
            {
                string temp = string.Empty;
                if (SyncKeyInfo?.Count > 0)
                {
                    foreach (SyncKeyItem syncKey in SyncKeyInfo.List)
                    {
                        temp += $"{syncKey.Key}_{syncKey.Val}|";
                    }
                    temp = temp.TrimEnd('|');
                }

                return temp;
            }
        }

        public void SetContact<T>(List<T> contactList) where T : BaseContactModel
        {
            foreach (T contact in contactList)
            {
                if (!contactDic.ContainsKey(contact.UserName))
                {
                    contactDic.Add(contact.UserName, contact);
                }
                else
                {
                    if (contact.MemberCount > 0)
                    {
                        contactDic[contact.UserName].SetMember(contact.MemberList);
                    }
                }
            }
        }

        public BaseContactModel this[string userName]
        {
            get
            {
                if (contactDic != null && contactDic.ContainsKey(userName))
                    return contactDic[userName];

                return null;
            }
        }

        public MemberModel User { get; set; }

        //string url = string.Format("https://login.wx.qq.com/jslogin?appid=wx782c26e4c19acffb&redirect_uri=https%3A%2F%2Fwx.qq.com%2Fcgi-bin%2Fmmwebwx-bin%2Fwebwxnewloginpage&fun=new&lang=zh_CN&_={0}", userManager.UserInfo.RequestCount);
        public string QRInitialUrl => $"https://login.wx.qq.com/jslogin?appid=wx782c26e4c19acffb&redirect_uri=https%3A%2F%2Fwx.qq.com%2Fcgi-bin%2Fmmwebwx-bin%2Fwebwxnewloginpage&fun=new&lang=zh_CN&_={RequestCount}";

        public string LoginUrl => $"https://login.wx.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid={UUID}&tip={RequestCount}&_={TIP}"; //, UserInfo.Instance.UUID, UserInfo.Instance.RequestCount, UserInfo.Instance.TIP);

        public string InitialUrl => $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxinit?r={BackTimeSpan}&lang=zh_CN&pass_ticket={PassTicket}";

        public string StatusNotifyUrl => $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxstatusnotify?lang=zh_CN&pass_ticket={PassTicket}";

        public string ContactUrl => $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact?pass_ticket={PassTicket}&r={DateTimeDelt}&seq=0&skey=@{SKey}";

        public string BatchContactUrl => $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r={DateTimeDelt}&lang=zh_CN&pass_ticket={PassTicket}";

        public string SyncCheckUrl => $"https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?r={DateTimeDelt}&skey={HttpUtility.UrlEncode(SKey, Encoding.UTF8)}&sid={SID}&uin={HttpUtility.UrlEncode(UIN.ToString(), Encoding.UTF8)}&deviceid={DeviceID}&synckey={HttpUtility.UrlEncode(SyncKey, Encoding.UTF8)}&_={RequestCount}";

        public string SyncUrl => $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsync?sid={SID}&skey={SKey}&lang=zh_CN&pass_ticket={PassTicket}";

        public string SendUrl => $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?lang=zh_CN&pass_ticket={PassTicket}";

        public string GetGroupContact()
        {
            string groupInfo = string.Empty;
            if (contactDic?.Count > 0)
            {
                foreach (KeyValuePair<string, BaseContactModel> keyValue in contactDic)
                {
                    if (keyValue.Value.MemberCount > 0)
                        groupInfo += $"{keyValue.Value.NickName}&nbsp;&nbsp;{keyValue.Key} <br />";
                }
            }

            return groupInfo;
        }
    }
}

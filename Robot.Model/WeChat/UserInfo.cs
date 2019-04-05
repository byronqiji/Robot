using Robot.Model.MemberInfo;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Robot.Model.WeChat
{
    public class UserInfo
    {
        private static UserInfo single;

        public static UserInfo Instance
        {
            get
            {
                if (single != null)
                    return single;

                UserInfo temp = new UserInfo();
                Interlocked.CompareExchange(ref single, temp, null);

                return single;
            }
        }
        
        private static readonly DateTime standardDateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 8, 0, 0));

        int tip;
        private ulong requestCount;

        private Dictionary<string, BaseContactModel> contactDic;

        public SyncKeyModel SyncKeyInfo { get; set; }

        private UserInfo()
        {
            requestCount = DateTimeDelt;

            tip = 1;
            contactDic = new Dictionary<string, BaseContactModel>();

            //SyncKeyList = new List<SyncKeyItem>();
        }

        //public List<SyncKeyItem> SyncKeyList { get; set; }

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
    }
}

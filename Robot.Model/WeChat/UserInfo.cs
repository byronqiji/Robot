using System;
using System.Collections.Generic;
using System.Net;

namespace Robot.Model.WeChat
{
    public class UserInfo : IDisposable
    {
        private static DateTime standardDateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 8, 0, 0));

        int tip;

        private List<SyncKeyInfo> syncKeyList;

        private ulong requestCount;
        public UserInfo()
        {
            requestCount = DateTimeDelt;

            tip = 1;

            syncKeyList = new List<SyncKeyInfo>();
        }

        public List<SyncKeyInfo> SyncKeyList { get { return syncKeyList; } }

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

        public string UIN { get; set; }

        public string PassTicket { get; set; }

        public CookieCollection Cookies { get; set; }

        public object DeviceID
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
                foreach (SyncKeyInfo syncKey in syncKeyList)
                {
                    temp += $"{syncKey.Key}_{syncKey.Val}|";
                }
                temp = temp.TrimEnd('|');

                return temp;
            }
        }

        public int SyncKeyCount
        {
            get
            {
                return syncKeyList.Count;
            }
        }

        public void AddSyncKey(SyncKeyInfo syncKey)
        {
            syncKeyList.Add(syncKey);
        }

        public void Dispose()
        {
        }
    }
}

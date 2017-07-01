using System;

namespace Robot.Model.WeChat
{
    public class UserInfo : IDisposable
    {
        private static DateTime standardDateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

        int tip;

        private ulong requestCount;
        public UserInfo()
        {
            requestCount = DateTimeDelt;

            tip = 1;
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

        public void Dispose()
        {
        }
    }
}

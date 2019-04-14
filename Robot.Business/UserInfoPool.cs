using Robot.Business.WeChat;
using System.Collections.Generic;
using System.Threading;

namespace Robot.Business
{
    public class UserPool
    {
        private static UserPool single;

        public void AddUser(UserManager userManger)
        {
            if (!userMap.ContainsKey(userManger.UserInfo.UUID))
            {
                userMap.Add(userManger.UserInfo.UUID, userManger);
            }
        }

        public static UserPool Instance
        {
            get
            {
                if (single != null)
                    return single;

                UserPool temp = new UserPool();
                Interlocked.CompareExchange(ref single, temp, null);

                return single;
            }
        }

        Dictionary<string, UserManager> userMap;

        private UserPool()
        {
            userMap = new Dictionary<string, UserManager>();
        }

        public UserManager this[string uuid]
        {
            get
            {
                if (userMap.ContainsKey(uuid))
                    return userMap[uuid];

                return null;
            }
        }
    }
}

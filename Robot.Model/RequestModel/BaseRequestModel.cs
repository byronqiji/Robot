﻿using Robot.Model.WeChat;

namespace Robot.Model.RequestModel
{
    public class BaseRequestModel
    {
//DeviceID: "e555725698519022"
//Sid: "+EofzwmYa0pz7JD1"
//Skey: "@crypt_4af0643b_2074fbc30df1a9d963ef86902afc111a"
//Uin: 1126851900

        public string DeviceId { get; set; }

        public string Sid { get; set; }

        public string Skey { get; set; }

        public ulong Uin { get; set; }

        public static BaseRequestModel Create(UserInfo userInfo)
        {
            return new BaseRequestModel()
            {
                Uin = userInfo.UIN,
                Sid = userInfo.SID,
                Skey = userInfo.SKey,
                DeviceId = userInfo.DeviceID
            };
        }
    }
}

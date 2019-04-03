using Newtonsoft.Json;
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
            //https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?r=1498915088960&skey=%40crypt_4af0643b_9f615fbce9ddb2be3fce910f3e285ee8&sid=E5pvocibbKBAWjXh&
            //https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?r=1554282435522&skey=%40crypt_4af0643b_4cae09263ff05eaab459aa29f65399b1&sid=ebXHPrVwoOvZ5rlB&uin=1126851900&deviceid=e430041265548391&synckey=1_683279947%7C2_683279978%7C3_683279510%7C11_683279603%7C201_1554282384%7C203_1554276597%7C1000_1554280922%7C1001_1554280995&_=1554282339966
            //uin =1126851900&deviceid=e336493130571149&synckey=1_658954891%7C2_658955296%7C3_658955084%7C11_658955257%7C13_658700011%7C201_1498915083%7C203_1498913594%7C1000_1498903382%7C1001_1498903412&_=1498911897174

            //https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?r=1554256679463&skey=%40crypt_4af0643b_ab0721cd782bf72a62b1cd17927c9dcb&sid=e/2Ko03IrAH1CC4M&uin=1126851900&deviceid=e813083841604741&synckey=1_683279947%7c2_683280061%7c3_683279510%7c1000_1554280922&_=1554256555488
            //https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?r=1554282435522&skey=%40crypt_4af0643b_4cae09263ff05eaab459aa29f65399b1&sid=ebXHPrVwoOvZ5rlB&uin=1126851900&deviceid=e430041265548391&synckey=1_683279947%7C2_683279978%7C3_683279510%7C11_683279603%7C201_1554282384%7C203_1554276597%7C1000_1554280922%7C1001_1554280995&_=1554282339966
            string url = $"https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?r={userManager.User.DateTimeDelt}&skey={HttpUtility.UrlEncode(userManager.User.SKey, Encoding.UTF8)}&sid={userManager.User.SID}&" +
                $"uin={HttpUtility.UrlEncode(userManager.User.UIN, Encoding.UTF8)}&deviceid={userManager.User.DeviceID}&synckey={HttpUtility.UrlEncode(userManager.User.SyncKey, Encoding.UTF8)}&_={userManager.User.RequestCount}";

            string value = HttpHelper.GetResponseValue(url, userManager.User.Cookies);

            using (StreamWriter sw = new StreamWriter(Path + "\\sync.txt", true))
            using (StreamWriter x = new StreamWriter(Path + "\\url.txt", true))
            {
                x.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: {url}");
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + value);
            }

            //window.synccheck={retcode:"0",selector:"0"}
            string[] tempArr = value.Trim().Split('=');

            SyncCheck sc = JsonConvert.DeserializeObject<SyncCheck>(tempArr[1]);
            if (sc.retcode == "0" && sc.selector == "2")
            {
                //"BaseRequest" : { "Uin":2545437902,"Sid":"QfLp+Z+FePzvOFoG"},
                //"SyncKey" : { "Count":4,"List":[{"Key":1,"Val":620310295},{"Key":2,"Val":620310303},{"Key":3,"Val":620310285},{"Key":1000,"Val":1377479086}]},
                //"rr" :1377482079876};

                var data = new
                {
                    BaseRequest = new
                    {
                        Uin = userManager.User.UIN,
                        Sid = userManager.User.SID,
                    },

                    SyncKey = new
                    {
                        Count = userManager.User.SyncKeyCount,
                        List = userManager.User.SyncKeyList
                    },
                    rr = userManager.User.DateTimeDelt,
                };

                string jsonData = JsonConvert.SerializeObject(data);

                using (StreamWriter sw = new StreamWriter(Path + "\\ + data.txt", true))
                {
                    sw.WriteLine(value);
                }
                //https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsync?sid=ebXHPrVwoOvZ5rlB&skey=@crypt_4af0643b_4cae09263ff05eaab459aa29f65399b1&lang=zh_CN&pass_ticket=TL6offdixrMXke%252BaENCe2VJ%252BzirYrviM%252BPHkUnD7BlfgAuz1dEXez7dsP3UmVAGE
                //https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsync?sid=XVUX2odG2zc6R02i&skey=@crypt_4af0643b_78ac88387a52679170de6c7e0413f767&lang=zh_CN&pass_ticket=GvuWUeqTBjeVFiCgj0BIPM08%252FAt0v1n9b0cd4mb%252B9yxr%252F2rG7Bh5z%252Fja6A7vVKnV
                //https://webpush.wx.qq.com   /cgi-bin/mmwebwx-bin/webwxsync?sid=QfLp+Z+FePzvOFoG&r=1377482079876
                string u = $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsync?sid={userManager.User.SID}&skey={userManager.User.SKey}&lang=zh_CN&pass_ticket={userManager.User.PassTicket}";
                
                value = HttpHelper.GetResponseValue(u, HttpMethod.POST, jsonData);

                using (StreamWriter sw = new StreamWriter(Path + "\\ + message.txt", true))
                using (StreamWriter x = new StreamWriter(Path + "\\url.txt", true))
                {
                    x.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: {u}");
                    sw.WriteLine(value);
                }
            }

            return value;
        }
    }
}

using Newtonsoft.Json;
using Robot.Business;
using Robot.Business.WeChat;
using Robot.Model.RequestModel;
using Robot.Model.WeChat;
using Robot.Request;
using System.IO;
using System.Web.Mvc;

namespace Robot.Web.Controllers
{
    public class WeChatController : Controller
    {
        // GET: WeChat
        public ActionResult Index()
        {
            UserManager userManager = new UserManager();
            userManager.Start();

            ViewBag.UUID = userManager.UserInfo.UUID;

            UserPool.Instance.AddUser(userManager);

            return View();
        }

        [HttpPost]
        public void Send(SendRequest req)
        {
            if (string.IsNullOrEmpty(req.UUID))
                return;

            UserManager um = UserPool.Instance[req.UUID];
            if (um != null && um.UserInfo != null)
            {
                //             https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?lang=zh_CN&pass_ticket=t3sjJCoGGKrvZyulNu8a5dRsrf0C1Zg0AX0PaGhpMsp%252Ff18EBwpHQzpJRY1sJ3xT
                //string url = $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?lang=zh_CN&pass_ticket={UserInfo.Instance.PassTicket}";

                SendModel sm = new SendModel()
                {
                    BaseRequest = BaseRequestModel.Create(um.UserInfo),
                    Msg = new MessageModel()
                    {
                        FromUserName = um.UserInfo.User.UserName,
                        ToUserName = req.ToUserName,
                        Content = req.Message,
                        Type = 1,
                        LocalID = um.UserInfo.LocalID.ToString(),
                    },
                    Scene = 0
                };

                sm.Msg.ClientMsgId = sm.Msg.LocalID;

                string j = JsonConvert.SerializeObject(sm);

                string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\data\\send.txt";
                using (StreamWriter sw = new StreamWriter(path, true))
                {
                    sw.WriteLine(j);
                }

                string value = HttpHelper.GetResponseValue(um.UserInfo.SendUrl, HttpMethod.POST, j, um.UserInfo.Cookies);
            }
        }

        public string GroupInfo(string uuid)
        {
            if (string.IsNullOrEmpty(uuid))
                return string.Empty;

            UserManager um = UserPool.Instance[uuid];
            if (um != null && um.UserInfo != null)
            {
                try
                {
                    return um.UserInfo.GetGroupContact();
                }
                catch
                {
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        public string Monitor(string uuid)
        {
            if (string.IsNullOrEmpty(uuid))
                return string.Empty;

            try
            {
                UserManager um = UserPool.Instance[uuid];
                if (um != null && um.State != null)
                    return um.State.Monitor();

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
using Newtonsoft.Json;
using Robot.Business.WeChat;
using Robot.Model.RequestModel;
using Robot.Model.WeChat;
using Robot.Request;
using System.IO;
using System.Threading;
using System.Web.Mvc;

namespace Robot.Web.Controllers
{
    public class WeChatController : Controller
    {
        // GET: WeChat
        public ActionResult Index()
        {
            UserManager.Single.Start();

            ViewBag.UUID = UserInfo.Instance.UUID;

            return View();
        }

        [HttpPost]
        public void Send(SendRequest req)
        {
            //             https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?lang=zh_CN&pass_ticket=t3sjJCoGGKrvZyulNu8a5dRsrf0C1Zg0AX0PaGhpMsp%252Ff18EBwpHQzpJRY1sJ3xT
            string url = $"https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?lang=zh_CN&pass_ticket={UserInfo.Instance.PassTicket}";

            SendModel sm = new SendModel()
            {
                BaseRequest = BaseRequestModel.Create(),
                Msg = new MessageModel()
                {
                    FromUserName = UserInfo.Instance.User.UserName,
                    ToUserName = req.ToUserName,
                    Content = req.Message,
                    Type = 1,
                    LocalID = UserInfo.Instance.LocalID.ToString(),
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

            string value = HttpHelper.GetResponseValue(url, HttpMethod.POST, j);
        }

        public string GroupInfo()
        {
            if (UserInfo.Instance == null)
                Thread.Sleep(1000);

            try
            {
                return UserInfo.Instance.GetGroupContact();
            }
            catch
            {
                return string.Empty;
            }
        }

        public string Monitor()
        {
            if (UserManager.Single.State == null)
                Thread.Sleep(1000);

            try
            {
                return UserManager.Single.State.Monitor();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
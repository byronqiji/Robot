using Newtonsoft.Json;
using Robot.Business;
using Robot.Business.WeChat;
using Robot.Model.MemberInfo;
using Robot.Model.RequestModel;
using Robot.Model.WeChat;
using Robot.Request;
using System.Collections.Generic;
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
                SendModel sm = new SendModel()
                {
                    BaseRequest = BaseRequestModel.Create(um.UserInfo),
                    Msg = new MessageSendModel()
                    {
                        FromUserName = um.UserInfo.User.UserName,
                        Content = req.Message,
                        Type = 1,
                        LocalID = um.UserInfo.LocalID.ToString(),
                    },
                    Scene = 0
                };
                sm.Msg.ClientMsgId = sm.Msg.LocalID;

                foreach (KeyValuePair<string, BaseContactModel> item in um.UserInfo)
                {
                    sm.Msg.ToUserName = item.Key;

                    string j = JsonConvert.SerializeObject(sm);

                    string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\data\\send.txt";
                    using (StreamWriter sw = new StreamWriter(path, true))
                    {
                        sw.WriteLine(j);
                    }

                    string value = HttpHelper.GetResponseValue(um.UserInfo.SendUrl, HttpMethod.POST, j, um.UserInfo.Cookies);
                }
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

        [HttpPost]
        public void SetSend(SetSendRrequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UUID))
                return;

            UserManager um = UserPool.Instance[request.UUID];
            if (um != null && um.State != null)
                um.UserInfo.SetSendGroup(request);

        }
    }
}
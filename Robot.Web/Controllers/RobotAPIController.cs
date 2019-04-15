using Newtonsoft.Json;
using Robot.Business;
using Robot.Business.WeChat;
using Robot.Model.MemberInfo;
using Robot.Model.RequestModel;
using Robot.Model.WeChat;
using Robot.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Http;

namespace Robot.Web.Controllers
{
    public class RobotAPIController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetUserNameByNickName(string nickName)
        {
            UserManager um = UserPool.Instance.GetUserManagerByNickName(nickName);

            if (um != null && um.UserInfo != null && um.UserInfo.User != null)
                return Json(new { UUID = um.UserInfo.UUID });
            else
                return Json(new { UUID = string.Empty });
        }

        [HttpPost]
        public IHttpActionResult SendMessage(SendRequest req)
        {
            if (string.IsNullOrEmpty(req.UUID))
                return Json(new { Code = 1, Message = "UUID为空"});

            if (string.IsNullOrWhiteSpace(req.Message))
                return Json(new { Code = 2, Message = "发送内容为空" });

            try
            {
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

                return Json(new { Code = 0, Message = "消息发送成功" });
            }
            catch (Exception ex)
            {
                return Json(new { Code = -1, Message = "消息发送出现异常" });
            }
        }
    }
}
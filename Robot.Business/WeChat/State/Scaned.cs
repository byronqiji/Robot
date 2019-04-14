using Newtonsoft.Json;
using Robot.Model.MemberInfo;
using Robot.Model.RequestModel;
using Robot.Model.WeChat;
using Robot.Request;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;

namespace Robot.Business.WeChat.State
{
    public class Scaned : UserState
    {
        public Scaned(UserManager um)
            :base (um)
        { 
        }

        protected override string StateName
        {
            get
            {
                return "已扫码";
            }
        }

        public override string Monitor()
        {
            string value = HttpHelper.GetResponseValue(userManager.UserInfo.LoginUrl);
            if (value.Split('=')[1].Split(';')[0] == "200")
            {
                SetUserInfo(value.Split('\"')[1] + "&fun=new&version=v2");
                InitialWeChat();

                userManager.State = userManager.Login;
            }

            return value;
        }

        private void InitialWeChat()
        {
            InitialTree(userManager.UserInfo.InitialUrl, true);
            StatusNotifyRequestModel snrm = StatusNotifyRequestModel.Create(userManager.UserInfo);

            string value = HttpHelper.GetResponseValue(userManager.UserInfo.StatusNotifyUrl, HttpMethod.POST, JsonConvert.SerializeObject(snrm), userManager.UserInfo.Cookies);
            
            value = HttpHelper.GetResponseValue(userManager.UserInfo.ContactUrl, HttpMethod.GET, null, userManager.UserInfo.Cookies);

            using (StreamWriter sw = new StreamWriter(Path + "\\getcontact.txt", true))
                sw.Write(value);

            MemberTree memberTree = JsonConvert.DeserializeObject<MemberTree>(value);

            BatchRequestModel brm = BatchRequestModel.Create(userManager.UserInfo);
            for (int i = 0; i < memberTree.MemberCount; ++i)
            {
                brm.Count++;
                brm.List.AddLast(new BatchItem { UserName = memberTree.MemberList[i].UserName });
                if (i > 0 && i % 50 == 0)
                {
                    string json = JsonConvert.SerializeObject(brm);
                    using (StreamWriter sw = new StreamWriter(Path + "\\batch.txt", true))
                    {
                        sw.WriteLine(json);
                    }

                    value = HttpHelper.GetResponseValue(userManager.UserInfo.BatchContactUrl, HttpMethod.POST, json, userManager.UserInfo.Cookies);
                    InitialContactTree tree = JsonConvert.DeserializeObject<InitialContactTree>(value);
                    tree.Initial();
                    userManager.UserInfo.SetContact(tree.ContactList);

                    brm.Count = 0;
                    brm.List.Clear();

                    Thread.Sleep(100);
                }
            }

            if (brm.Count > 0)
            {
                string json = JsonConvert.SerializeObject(brm);
                using (StreamWriter sw = new StreamWriter(Path + "\\batch.txt", true))
                {
                    sw.WriteLine(json);
                }

                value = HttpHelper.GetResponseValue(userManager.UserInfo.BatchContactUrl, HttpMethod.POST, json, userManager.UserInfo.Cookies);
                using (StreamWriter sw = new StreamWriter(Path + "\\member.txt", true))
                {
                    sw.WriteLine(value);
                }
                InitialContactTree tree = JsonConvert.DeserializeObject<InitialContactTree>(value);
                tree.Initial();
                userManager.UserInfo.SetContact(tree.ContactList);
            }
        }

        private void InitialTree(string url, bool setSyncKeyList)
        {
            var postData = new { BaseRequest = new { Uin = userManager.UserInfo.UIN, Sid = userManager.UserInfo.SID, Skey = userManager.UserInfo.SKey, DeviceID = userManager.UserInfo.DeviceID } };

            string value = HttpHelper.GetResponseValue(url, HttpMethod.POST, JsonConvert.SerializeObject(postData), userManager.UserInfo.Cookies);

            using (StreamWriter sw = new StreamWriter(InitialFilePath, true))
                sw.Write(value);

            using (StreamWriter sw = new StreamWriter(UrlFilePath, true))
            {
                sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}\t{url}");
                sw.WriteLine(JsonConvert.SerializeObject(postData));
            }

            InitialContactTree tree = JsonConvert.DeserializeObject<InitialContactTree>(value);
            tree.Initial();
            userManager.UserInfo.SetContact(tree.ContactList);

            if (setSyncKeyList)
            {
                userManager.UserInfo.SyncKeyInfo = tree.SyncKey;
                userManager.UserInfo.User = tree.User;
            }
        }

        private void SetUserInfo(string loginUrl)
        {
            WebResponse response = HttpHelper.GetResponse(loginUrl, null);

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(HttpHelper.GetResponseValue(response));
            XmlNode root = xd.SelectSingleNode("error");

            userManager.UserInfo.SKey = root.ChildNodes[2].InnerText;
            userManager.UserInfo.SID = root.ChildNodes[3].InnerText;
            userManager.UserInfo.UIN = Convert.ToUInt64(root.ChildNodes[4].InnerText);
            userManager.UserInfo.PassTicket = root.ChildNodes[5].InnerText;

            userManager.UserInfo.Cookies = ((HttpWebResponse)response).Cookies;
        }
    }
}

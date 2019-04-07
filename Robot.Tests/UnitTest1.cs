using System;
using System.IO;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Model.MemberInfo;
using Robot.Model.WeChat;

namespace Robot.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestInitialContactTree()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string json = string.Empty;

            using (StreamReader sr = new StreamReader(path + "\\data\\Tree.json"))
            {
                json = sr.ReadToEnd();
            }

            InitialContactTree tree = JsonConvert.DeserializeObject<InitialContactTree>(json);

            tree.Initial();
        }

        [TestMethod]
        public void TestContactTree()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string json = string.Empty;

            using (StreamReader sr = new StreamReader(path + "\\data\\addMessage.json"))
            {
                json = sr.ReadToEnd();
            }

            MessageContactTree tree = JsonConvert.DeserializeObject<MessageContactTree>(json);

            tree.Initial();

            UserInfo.Instance.SetContact(tree.ContactList);

            UserInfo.Instance.SyncKeyInfo = tree.SyncKey;

            foreach (var msg in tree.AddMsgList)
            {
                int s = msg.Content.Replace("<br />", "<br/>").LastIndexOf("<br/>") + "<br/>".Length;
                string content = msg.Content.Substring(s, msg.Content.Length - s);
            }
        }

        [TestMethod]
        public void TestMemberTree()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string json = string.Empty;

            using (StreamReader sr = new StreamReader(path + "\\data\\MemberTree.json"))
            {
                json = sr.ReadToEnd();
            }

            MemberTree tree = JsonConvert.DeserializeObject<MemberTree>(json);
        }
    }
}

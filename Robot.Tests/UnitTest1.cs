using System;
using System.IO;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Model.MemberInfo;

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

            using (StreamReader sr = new StreamReader(path + "\\data\\Message.json"))
            {
                json = sr.ReadToEnd();
            }

            ModContactTree tree = JsonConvert.DeserializeObject<ModContactTree>(json);

            tree.Initial();

            AccountModel.Instance.SetContact(tree.ContactList);

            AccountModel.Instance.SyncKey = tree.SyncKey;
        }
    }
}

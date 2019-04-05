using System;
using System.IO;

namespace Robot.Business.WeChat.State
{
    public abstract class UserState : IUserState
    {
        protected const string SyncKeyString = "SyncKey";
        protected const string ListString = "List";

        protected string Path { get; private set; }

        protected virtual string StateName { get { return "未知"; } }

        protected UserManager userManager;

        public UserState(UserManager um)
        {
            userManager = um;

            Path = AppDomain.CurrentDomain.BaseDirectory + "\\Data";

            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
        }

        public string UrlFilePath => Path + $"\\url{DateTime.Now.ToString("yyyyMMdd")}.txt";

        public string MessageFilePath => Path + $"\\message{DateTime.Now.ToString("yyyyMMdd")}.txt";

        public string DataFilePath => Path + $"\\data{DateTime.Now.ToString("yyyyMMdd")}.txt";

        public string InitialFilePath => Path + $"\\initial{DateTime.Now.ToString("yyyyMMdd")}.txt";

        public virtual string Monitor()
        {
            return StateName;
        }

        public virtual void Start()
        { 
        }
    }
}

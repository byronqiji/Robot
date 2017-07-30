using System;
using System.IO;

namespace Robot.Business.WeChat.State
{
    public abstract class UserState : IUserState
    {
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

        public virtual string Monitor()
        {
            return StateName;
        }

        public virtual void Start()
        { 
        }
    }
}

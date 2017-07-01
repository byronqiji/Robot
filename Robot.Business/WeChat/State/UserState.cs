
namespace Robot.Business.WeChat.State
{
    public abstract class UserState : IUserState
    {
        protected virtual string StateName { get { return "未知"; } }

        protected UserManager userManager;

        public UserState(UserManager um)
        {
            userManager = um;
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

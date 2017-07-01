namespace Robot.Business.WeChat.State
{
    public class Login : UserState
    {
        public Login(UserManager um)
            : base(um)
        {
        }

        public override string Monitor()
        {
            return base.Monitor();
        }
    }
}

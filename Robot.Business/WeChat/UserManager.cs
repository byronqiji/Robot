using Robot.Business.WeChat.State;
using Robot.Model.WeChat;

namespace Robot.Business.WeChat
{
    public class UserManager
    {
        public IUserState State { get; internal set; }
        internal IUserState QRCodeInitial { get; private set; }
        internal IUserState ReadyScan { get; private set; }
        internal IUserState Scaned { get; private set; }
        internal IUserState Login { get; private set; }

        public UserInfo UserInfo { get; set; }

        public UserManager()
        {
            UserInfo = new UserInfo();

            QRCodeInitial = new QRCodeInitial(this);
            ReadyScan = new ReadyScan(this);
            Scaned = new Scaned(this);
            Login = new Login(this);
        }

        public void Start()
        {
            State = QRCodeInitial;
            State.Start();
        }

        public string Monitor()
        {
            return State.Monitor();
        }
    }
}

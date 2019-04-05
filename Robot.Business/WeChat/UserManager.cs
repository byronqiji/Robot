using Robot.Business.WeChat.State;
using Robot.Model.WeChat;
using System.Threading;

namespace Robot.Business.WeChat
{
    public class UserManager
    {
        private static UserManager single;
        public static UserManager Single
        {
            get
            {
                if (single != null)
                    return single;

                UserManager temp = new UserManager();
                Interlocked.CompareExchange(ref single, temp, null);

                return single;
            }
        }

        public IUserState State { get; internal set; }
        internal IUserState QRCodeInitial { get; private set; }
        internal IUserState ReadyScan { get; private set; }
        internal IUserState Scaned { get; private set; }
        internal IUserState Login { get; private set; }

        //public UserInfo User { get; private set; }

        private UserManager()
        {
            QRCodeInitial = new QRCodeInitial(this);
            ReadyScan = new ReadyScan(this);
            Scaned = new Scaned(this);
            Login = new Login(this);

            //User = new UserInfo();
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

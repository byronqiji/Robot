using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return base.Monitor();
        }
    }
}

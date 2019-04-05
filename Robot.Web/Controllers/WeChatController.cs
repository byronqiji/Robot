using Robot.Business.WeChat;
using Robot.Model.WeChat;
using System.Threading;
using System.Web.Mvc;

namespace Robot.Web.Controllers
{
    public class WeChatController : Controller
    {
        // GET: WeChat
        public ActionResult Index()
        {
            UserManager.Single.Start();

            ViewBag.UUID = UserInfo.Instance.UUID;

            return View();
        }

        public string Monitor()
        {
            if (UserManager.Single.State == null)
                Thread.Sleep(1000);

            return UserManager.Single.State.Monitor();
        }
    }
}
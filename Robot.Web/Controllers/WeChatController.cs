using Robot.Business.WeChat;
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

            ViewBag.UUID = UserManager.Single.User.UUID;

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
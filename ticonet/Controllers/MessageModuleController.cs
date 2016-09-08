using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ticonet.Controllers
{
    [Authorize]
    public class MessageModuleController : Controller
    {
        // GET: MessageModule
        public ActionResult Index()
        {
            return View();
        }
    }
}
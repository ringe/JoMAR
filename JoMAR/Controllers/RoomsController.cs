using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JoMAR.Controllers
{
    public class RoomsController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Chat()
        {
            JodADataContext db = new JodADataContext();
            var users = db.aspnet_Users.ToList();

            return View(users);
        }
    }
}

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

        public ActionResult PublicRooms()
        {
            JodADataContext db = new JodADataContext();
            var rooms = db.ChatRooms.ToList();

            return View(rooms);
        }

        public ActionResult edit()
        {
            JodADataContext db = new JodADataContext();
            var rooms = db.ChatRooms.First();


            return View(rooms);
        
        }

        public ActionResult getRoom(ChatRoom cr)
        {
            return View(cr);
            
        }
    }
}

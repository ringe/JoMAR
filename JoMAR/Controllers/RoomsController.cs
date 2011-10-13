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
            ViewBag.Message = "Welcome to JoMAR's online Chat!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

       

        public ActionResult PublicRooms()
        {
            JodADataContext db = new JodADataContext();
            var rooms = db.ChatRooms.ToList();
            
            return View(rooms);
        }

        public ActionResult edit(string returnUrl)
        {
          
                JodADataContext db = new JodADataContext();
                ChatRoom room = (from p in db.ChatRooms
                                 where p.RoomID.ToString() == Url.RequestContext.RouteData.Values.Last().Value
                                 select p).First(); 

                return View(room);
        }

        public ActionResult create()
        {
            JodADataContext db = new JodADataContext();
            
            var rooms = new ChatRoom();


            return View(rooms);
        }

        [HttpPost]
        public ActionResult create(ChatRoom model, string returnUrl)
        {
            JodADataContext db = new JodADataContext();


            if (ModelState.IsValid)
            {
                model.UserID = (from p in db.aspnet_Users
                                where p.UserName == User.Identity.Name
                                select p).First().UserId;
                db.ChatRooms.InsertOnSubmit(model);
                model.RoomID = Guid.NewGuid();

                db.SubmitChanges();

                return Redirect("/Chat/" + model.Name);
            }
            return View(model);
        }

        public ActionResult MyRooms()
        {
            JodADataContext db = new JodADataContext();
            var rooms = db.ChatRooms.ToList();

            return View(rooms);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JoMAR.Controllers
{
    public class ChatController : Controller
    {
        //
        // GET: /Chat/

        public ActionResult Index(string name)
        {
            if (!Request.IsAuthenticated)
            {
                Session["jomarmessage"] = "Log in to chat!";
                return Redirect("/Rooms/Public");
            }

            JodADataContext db = new JodADataContext();
            ChatRoom room = (from p in db.ChatRooms
                       where p.Name == name
                       select p).First();

            var model = new JoMAR.Models.ChatModel(room, db);
            ViewBag.Title = model.Name;

            aspnet_User user = (from p in db.aspnet_Users
                                        where p.UserName == User.Identity.Name
                                        select p).First();

            // Public eller medlem?
            if (!model.Users.Contains(user) && room.isPrivate)
            {
                Session["jomarmessage"] = "The room you tried to access is private, members only!";
                return Redirect("/Rooms");
            }

            if (!model.Users.Contains(user))
            {
                UserRoom r = new UserRoom();
                r.UserID = user.UserId;
                r.RoomID = room.RoomID;
                db.UserRooms.InsertOnSubmit(r);
                db.SubmitChanges();
                model = new JoMAR.Models.ChatModel(room, db);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(string name, string id, FormCollection collection)
        {
            JodADataContext db = new JodADataContext();
            ChatRoom room = (from p in db.ChatRooms
                             where p.Name == name
                             select p).First();


            // Prepare message for submit
            ChatMessage message = new ChatMessage();
            message.Date = DateTime.Now;
            message.MessageID = Guid.NewGuid();
            message.UserID = (from p in db.aspnet_Users
                                where p.UserName == User.Identity.Name
                                select p).First().UserId;
            message.RoomID = room.RoomID;
            message.Text = collection["Message"];


            // Submit message to DB
            db.ChatMessages.InsertOnSubmit(message);
            db.SubmitChanges();

            var model = new JoMAR.Models.ChatModel(room, db);

            return View(model);
        }

        public ActionResult Profile()
        {
            JodADataContext db = new JodADataContext();
            var users = (from p in db.aspnet_Users
                         where p.UserName == User.Identity.Name
                         select p).First();

            return View(users);
        }

    }
}

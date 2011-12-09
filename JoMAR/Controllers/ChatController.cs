using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JoMAR.Models;

namespace JoMAR.Controllers
{
    public class ChatController : Controller
    {
        //
        // GET: /Chat/

        [Authorize]
        public ActionResult Index(string name)
        {
            if (name == User.Identity.Name)
            {
                Session["jomarmessage"] = "Schizophrenia!";
                return Redirect("/Rooms/Public");
            }

            JodADataContext db = new JodADataContext();
            ChatRoom room = null;

            Profile user = JoMAR.Models.Profile.GetProfile(User.Identity.Name);
            Profile user2 = JoMAR.Models.Profile.GetProfile(name);

            // Does the second user exist?
            if (user2 != null) // find private room
                room = Rooms.Private(user, user2, db);
            else // Find public room
                room = Rooms.Public(name, db);

            // No room found?
            if (room == null)
            {
                Session["jomarmessage"] = "No room found by that name!";
                return Redirect("/Rooms");
            }

            var model = new ChatModel(room, db);
            ViewBag.Title = model.Name;

            // Is the user member while the room is private?
            if (!model.Users.Contains(user.User) && room.isPrivate)
            {
                Session["jomarmessage"] = "The room you tried to access is private, members only!";
                return Redirect("/Rooms");
            }

            // Add the user to the room if necessary
            if (!model.Users.Contains(user.User))
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

        [Authorize]
        [HttpPost]
        public ActionResult Index(string name, string id, FormCollection collection)
        {
            if (name == User.Identity.Name)
            {
                Session["jomarmessage"] = "Schizophrenia!";
                return Redirect("/Rooms/Public");
            }

            JodADataContext db = new JodADataContext();
            ChatRoom room = null;

            Profile user = JoMAR.Models.Profile.GetProfile(User.Identity.Name);
            Profile user2 = JoMAR.Models.Profile.GetProfile(name);

            // Does the second user exist?
            if (user2 != null) // find private room
                room = Rooms.Private(user, user2, db);
            else // Find public room
                room = Rooms.Public(name, db);

            // No room found?
            if (room == null)
            {
                Session["jomarmessage"] = "No room found by that name!";
                return Redirect("/Rooms");
            }

            // Room members
            List<aspnet_User> users = (from u in db.aspnet_Users
                           join m2m in db.UserRooms on user.UserId equals m2m.UserID
                           where m2m.RoomID == room.RoomID
                           select u).ToList();

            // Public eller medlem?
            if (!users.Contains(user.User) && room.isPrivate)
            {
                Session["jomarmessage"] = "The room you tried to access is private, members only!";
                return Redirect("/Rooms");
            }

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

            var model = new ChatModel(room, db);
            return View(model);
        }

        public ActionResult Profile()
        {
            //JodADataContext db = new JodADataContext();
            //var user = (from p in db.aspnet_Users
            //             where p.UserName == User.Identity.Name
            //             select p).First();
            JoMAR.Models.Profile user = JoMAR.Models.Profile.GetProfile(User.Identity.Name);
            return View(user);
        }

        [HttpPost]
        public ActionResult Profile(FormCollection collection)
        {
            JodADataContext db = new JodADataContext();

            // Prepare message for submit
            aspnet_User usr = (from p in db.aspnet_Users
                              where p.UserName == User.Identity.Name
                              select p).First();

            aspnet_Membership ma = (from m in db.aspnet_Memberships
                                   where m.UserId == usr.UserId
                                   select m).First();
            ma.Email = collection["Email"];

            // Submit message to DB
            db.SubmitChanges();

            JoMAR.Models.Profile user = JoMAR.Models.Profile.GetProfile(User.Identity.Name);

            user.FirstName = collection["FirstName"];
            user.LastName = collection["LastName"];

            user.CellPhone = collection["CellPhone"];
            user.Save();
            return View(user);
        }

    }
}

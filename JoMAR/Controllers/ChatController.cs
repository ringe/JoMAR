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
            if (model.IsMember(user.UserId, db))
            {
                return View(model);
            }
            else
            {
                Session["jomarmessage"] = "The room you tried to access is private, members only!";
                return Redirect("/Rooms");
            }
        }

        [Authorize, HttpPost]
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

            var model = new ChatModel(room, db);
            ViewBag.Title = model.Name;

            // Is the user member while the room is private?
            if (model.IsMember(user.UserId, db))
            {
                Rooms.Post(collection["Message"], user.UserId, room.RoomID, db);
                return View(model);
            }
            else
            {
                Session["jomarmessage"] = "The room you tried to access is private, members only!";
                return Redirect("/Rooms");
            }
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

        public ActionResult UploadFile()
        {
            return View();
        }

    }
}

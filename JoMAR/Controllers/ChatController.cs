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
            aspnet_User user = (from p in db.aspnet_Users
                                where p.UserName == User.Identity.Name
                                select p).First();

            aspnet_User[] privUser = (from p in db.aspnet_Users
                                      where p.UserName == name
                                      select p).ToArray();

            List<ChatRoom> rooms;
            if (privUser.Length != 0)
            {
                rooms = (from r in db.ChatRooms
                                    where (r.Name == (User.Identity.Name + name) || r.Name == (name + User.Identity.Name) ) && r.isPrivate && !r.isPublic && r.UserRooms.Count == 2
                                    select r).ToList();
                ChatRoom newr = new ChatRoom();
                newr.UserID = user.UserId;
                newr.RoomID = Guid.NewGuid();
                newr.isPrivate = true;
                newr.Name = (User.Identity.Name + name);
                db.ChatRooms.InsertOnSubmit(newr);

                UserRoom nr = new UserRoom();
                nr.UserID = user.UserId;
                nr.RoomID = newr.RoomID;

                UserRoom nr2 = new UserRoom();
                nr2.UserID = privUser.First().UserId;
                nr2.RoomID = newr.RoomID;

                db.UserRooms.InsertOnSubmit(nr);
                db.UserRooms.InsertOnSubmit(nr2);
                db.SubmitChanges();

                rooms.Add(newr);

            } else {
                rooms = (from p in db.ChatRooms
                                 where p.Name == name
                                 select p).ToList();
            }

            // Public eller medlem?
            if (rooms.Count == 0)
            {
                Session["jomarmessage"] = "No room found by that name!";
                return Redirect("/Rooms");
            }

            // Return the first room found
            ChatRoom room = rooms[0];

            var model = new JoMAR.Models.ChatModel(room, db);
            ViewBag.Title = model.Name;

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
            aspnet_User user = (from p in db.aspnet_Users
                                where p.UserName == User.Identity.Name
                                select p).First();

            aspnet_User[] privUser = (from p in db.aspnet_Users
                                      where p.UserName == name
                                      select p).ToArray();

            List<ChatRoom> rooms;
            if (privUser.Length != 0)
            {
                rooms = (from r in db.ChatRooms
                         where (r.Name == (User.Identity.Name + name) || r.Name == (name + User.Identity.Name)) && r.isPrivate && !r.isPublic && r.UserRooms.Count == 2
                         select r).ToList();
            }
            else
            {
                rooms = (from p in db.ChatRooms
                         where p.Name == name
                         select p).ToList();
            }

            if (rooms.Count == 0)
            {
                Session["jomarmessage"] = "No room found by that name!";
                return Redirect("/Rooms");
            }

            

            // Return the first room found
            ChatRoom room = rooms[0];


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

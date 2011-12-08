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

        public ActionResult Public()
        {
            JodADataContext db = new JodADataContext();
            var rooms = db.ChatRooms.ToList();

            return View(rooms);
        }


        public ActionResult edit(Guid id)
        {
            if (!Request.IsAuthenticated)
            {
                Session["jomarmessage"] = "Log in to chat!";
                return Redirect("/Rooms/Public");
            }

            JodADataContext db = new JodADataContext();
            ChatRoom room = (from p in db.ChatRooms
                             where p.RoomID.ToString() == (String)Url.RequestContext.RouteData.Values.Last().Value
                             select p).First();

            return View(room);
        }

        [HttpPost]
        public ActionResult edit(Guid id, FormCollection collection)
        {
            if (!Request.IsAuthenticated)
            {
                Session["jomarmessage"] = "Log in to chat!";
                return Redirect("/Rooms/Public");
            }

            JodADataContext db = new JodADataContext();
            ChatRoom editChat = (from p in db.ChatRooms
                                 where p.RoomID == id
                                 select p).First();

            aspnet_User user = (from p in db.aspnet_Users
                                where p.UserName == User.Identity.Name
                                select p).First();

            if (editChat.aspnet_User != user)
            {
                Session["jomarmessage"] = "You can't edit a room you don't own.";
                return Redirect("/");
            }

            if (ModelState.IsValid)
            {
                UpdateModel(editChat);
                db.SubmitChanges();

                return Redirect("/Chat/" + editChat.Name);
            }
            return View(editChat);
        }

        public ActionResult create()
        {
            if (!Request.IsAuthenticated)
            {
                Session["jomarmessage"] = "Log in to create a room!";
                return Redirect("/Rooms/Public");
            }

            var rooms = new ChatRoom();
            return View(rooms);
        }

        [HttpPost]
        public ActionResult create(ChatRoom model, string returnUrl)
        {
            if (!Request.IsAuthenticated)
            {
                Session["jomarmessage"] = "Log in to create a room!";
                return Redirect("/Rooms/Public");
            }

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

        public ActionResult delete(Guid id)
        {
            if (!Request.IsAuthenticated)
            {
                Session["jomarmessage"] = "Log in to delete a room!";
                return Redirect("/Rooms/Public");
            }

            JodADataContext db = new JodADataContext();
            ChatRoom room = (from p in db.ChatRooms
                             where p.RoomID.ToString() == (String)Url.RequestContext.RouteData.Values.Last().Value
                             select p).First();

            return View(room);
        }

        [HttpPost]
        public ActionResult delete(Guid id, FormCollection collection)
        {
            if (!Request.IsAuthenticated)
            {
                Session["jomarmessage"] = "Log in to delete a room!";
                return Redirect("/Rooms/Public");
            }

            JodADataContext db = new JodADataContext();
            ChatRoom deleteChat = (from p in db.ChatRooms
                                 where p.RoomID == id
                                 select p).First();

            aspnet_User user = (from p in db.aspnet_Users
                                        where p.UserName == User.Identity.Name
                                        select p).First();
            if (deleteChat.aspnet_User != user)
            {
                Session["jomarmessage"] = "You can't delete a room you don't own.";
                return Redirect("/");
            }

            if (ModelState.IsValid)
            {
                db.UserRooms.DeleteAllOnSubmit(deleteChat.UserRooms);
                db.ChatMessages.DeleteAllOnSubmit(deleteChat.ChatMessages);
                db.ChatRooms.DeleteOnSubmit(deleteChat);
                db.SubmitChanges();

                return Redirect("/");
            }
            return View(deleteChat);
        }

        public ActionResult Private()
        {
            if (!Request.IsAuthenticated)
            {
                Session["jomarmessage"] = "Log in to see my rooms!";
                return Redirect("/Rooms/Public");
            }

            JodADataContext db = new JodADataContext();
            aspnet_User user = (from p in db.aspnet_Users
                                where p.UserName == User.Identity.Name
                                select p).First();

            Models.MyRoom model = new Models.MyRoom(user, db);

            return View(model);
        }

        [HttpPost]
        public ActionResult Private(FormCollection collection)
        {
            if (!Request.IsAuthenticated)
            {
                Session["jomarmessage"] = "Log in to see my rooms!";
                return Redirect("/Rooms/Public");
            }

            JodADataContext db = new JodADataContext();
            aspnet_User newMember = (from p in db.aspnet_Users
                                where p.UserId.ToString() == collection["SelectedValue"]
                                select p).First();
            ChatRoom room = (from p in db.ChatRooms
                                where p.RoomID.ToString() == collection["item.RoomID"]
                                select p).First();
            aspnet_User user = (from p in db.aspnet_Users
                                where p.UserName == User.Identity.Name
                                select p).First();

            if (room.UserID != user.UserId)
            {
                Session["jomarmessage"] = "You can't add users to a room you don't own.";
                return Redirect("/Rooms/Private");
            }

            List<Guid> mmm = new List<Guid>();
            foreach (var s in room.UserRooms)
                mmm.Add(s.UserID);
            
            if (!mmm.Contains(newMember.UserId)) {
                UserRoom ur = new UserRoom();
                ur.RoomID = room.RoomID;
                ur.UserID = newMember.UserId;
                db.UserRooms.InsertOnSubmit(ur);
                db.SubmitChanges();
            }

            Models.MyRoom model = new Models.MyRoom(user, db);
            return View(model);
        }
    }
}

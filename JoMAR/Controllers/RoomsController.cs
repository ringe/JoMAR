﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JoMAR.Models;

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
            return View(new Invite());
        }

        [HttpPost]
        public ActionResult About(FormCollection collection)
        {
            Profile p = Profile.GetProfile(User.Identity.Name);
            string text = "You are invited to join JoMAR chat by: " + ((p == null) ? "" : p.Name);
            SMS.Send(text, "JoMAR", collection["CellPhone"]);
            ViewBag.Thanks = "Thank you!";
            return View(new Invite());
        }

        public ActionResult Public()
        {
            JodADataContext db = new JodADataContext();
            var rooms = db.ChatRooms.ToList();

            return View(rooms);
        }

        [Authorize]
        public ActionResult edit(Guid id)
        {
            JodADataContext db = new JodADataContext();
            ChatRoom room = (from p in db.ChatRooms
                             where p.RoomID.ToString() == (String)Url.RequestContext.RouteData.Values.Last().Value
                             select p).First();

            return View(room);
        }

        [HttpPost,Authorize]
        public ActionResult edit(Guid id, FormCollection collection)
        {
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
                return Redirect(Url.Action("", "Rooms"));
            }

            if (ModelState.IsValid)
            {
                UpdateModel(editChat);
                db.SubmitChanges();

                return Redirect(Url.Action(editChat.Name, "Chat"));
            }
            return View(editChat);
        }

        [Authorize]
        public ActionResult create()
        {
            var rooms = new ChatRoom();
            return View(rooms);
        }

        [HttpPost,Authorize]
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

                return Redirect(Url.Action(model.Name, "Chat"));
            }
            return View(model);
        }

        [Authorize]
        public ActionResult delete(Guid id)
        {
            JodADataContext db = new JodADataContext();
            ChatRoom room = (from p in db.ChatRooms
                             where p.RoomID.ToString() == (String)Url.RequestContext.RouteData.Values.Last().Value
                             select p).First();

            return View(room);
        }

        [HttpPost, Authorize]
        public ActionResult delete(Guid id, FormCollection collection)
        {
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
                return Redirect(Url.Action("", "Rooms"));
            }

            if (ModelState.IsValid)
            {
                db.UserRooms.DeleteAllOnSubmit(deleteChat.UserRooms);
                db.ChatMessages.DeleteAllOnSubmit(deleteChat.ChatMessages);
                db.ChatRooms.DeleteOnSubmit(deleteChat);
                db.SubmitChanges();

                return Redirect(Url.Action("", "Rooms"));
            }
            return View(deleteChat);
        }

        [Authorize]
        public ActionResult Private()
        {
            JodADataContext db = new JodADataContext();
            aspnet_User user = (from p in db.aspnet_Users
                                where p.UserName == User.Identity.Name
                                select p).First();

            Models.MyRoom model = new Models.MyRoom(user, db);

            return View(model);
        }

        [HttpPost,Authorize]
        public ActionResult Private(FormCollection collection)
        {
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
                return Redirect(Url.Action("Private", "Rooms"));
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

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
            JodADataContext db = new JodADataContext();
            ChatRoom room = (from p in db.ChatRooms
                       where p.Name == name
                       select p).First();

            var model = new JoMAR.Models.ChatModel();
            model.MessageBoard = room.ChatMessages.ToArray();


            model.Users = (from user in db.aspnet_Users
                                              join m2m in db.UserRooms on user.UserId equals m2m.UserID
                                              where m2m.RoomID == room.RoomID
                                              select user).ToList();

            if (!model.Users.Contains(room.aspnet_User))
                model.Users.Add(room.aspnet_User);

            ViewBag.Title = model.Name = name;
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(Models.ChatModel model, string returnUrl)
        {
            JodADataContext db = new JodADataContext();
            ChatRoom room = (from p in db.ChatRooms
                             where p.Name == model.Name
                             select p).First();

            // Prepare message for submit
            ChatMessage message = new ChatMessage();
            message.Date = DateTime.Now;
            message.MessageID = Guid.NewGuid();
            message.UserID = (from p in db.aspnet_Users
                                where p.UserName == User.Identity.Name
                                select p).First().UserId;
            message.RoomID = room.RoomID;
            message.Text = model.Message;

            // Submit message to DB
            db.ChatMessages.InsertOnSubmit(message);
            db.SubmitChanges();

            model.MessageBoard = room.ChatMessages.ToArray();


            model.Users = (from user in db.aspnet_Users
                           join m2m in db.UserRooms on user.UserId equals m2m.UserID
                           where m2m.RoomID == room.RoomID
                           select user).ToList();

            if (!model.Users.Contains(room.aspnet_User))
                model.Users.Add(room.aspnet_User);

            return View(model);
        }

    }
}

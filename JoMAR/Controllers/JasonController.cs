using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Diagnostics;
using System.Web.Script.Serialization;
using JoMAR.Models;

namespace JoMAR.Controllers
{
    public class JasonController : Controller
    {
        //
        // GET: /Jason/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult getMessages(Guid id)
        {

            JodADataContext db = new JodADataContext();
            List<dynamic> jasons = new List<dynamic>();

            ChatRoom room = (from p in db.ChatRooms
                                 where p.RoomID == id
                                 select p).First();

            ChatMessage[] msg = room.ChatMessages.OrderByDescending(d => d.Date).ToArray();
            
            foreach (var message in msg)
            {
                bool missing = !System.IO.File.Exists(Server.MapPath("~/uploads/" + message.Image));
                string file = (missing ? null : message.Image);
                var js = new
                {
                    date = message.Date,
                    user = message.aspnet_User.UserName,
                    text = message.Text,
                    image = file,
                    missing = missing
                };

                jasons.Add(js);
            }
            
            return Json(jasons, JsonRequestBehavior.AllowGet);

        }

        public JsonResult getRooms()
        {
            JodADataContext db = new JodADataContext();
            List<dynamic> rooms = new List<dynamic>();

            ChatRoom[] rA = (from p in db.ChatRooms
                            where p.isPublic
                            select p).ToArray();

            foreach (var r in rA)
            {
                var js = new
                {
                    name = r.Name,
                    owner = r.aspnet_User.UserName,
                    isPrivate = r.isPrivate
                };
                rooms.Add(js);
            }

            return Json(rooms, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getUsersOnRoom(Guid id)
        {
            JodADataContext db = new JodADataContext();
            List<Jason> userList = new List<Jason>();

            ChatRoom room = (from p in db.ChatRooms where p.RoomID == id select p).First();

            foreach (UserRoom ur in room.UserRooms)
            {
                Profile p = Profile.GetProfile(ur.aspnet_User.UserName);
                Jason jj = new Jason();
                jj.Gravatar = p.Image;
                jj.Email = p.Email;
                jj.UserId = p.UserId;
                jj.UserName = p.UserName;
                userList.Add(jj);
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            
            return Json(js.Serialize(userList), JsonRequestBehavior.AllowGet);

        }

    }
}

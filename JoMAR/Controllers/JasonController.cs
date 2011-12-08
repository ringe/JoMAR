using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            List<string> messages = new List<string>();
            int i = 0;

            ChatRoom room = (from p in db.ChatRooms
                                 where p.RoomID == id
                                 select p).First();

            ChatMessage[] msg = room.ChatMessages.ToArray();

            foreach (var message in msg)
            {
                messages.Add(message.Date + " " + message.aspnet_User.UserName + " said: " + message.Text);
                i++;
            }
            messages.Sort();
            return Json(messages.ToArray(), JsonRequestBehavior.AllowGet);

        }

        public JsonResult getRooms()
        {
            JodADataContext db = new JodADataContext();
            List<string> rooms = new List<string>();

            ChatRoom[] room = (from p in db.ChatRooms
                             where p.isPublic
                             select p).ToArray();

            foreach (var r in room)
            {
                rooms.Add("Name: " + r.Name + " Owner: " + r.aspnet_User.UserName);
            }

            return Json(rooms.ToArray(), JsonRequestBehavior.AllowGet);
        }

    }
}

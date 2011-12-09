using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            List<string> messages = new List<string>();
            int i = 0;

            ChatRoom room = (from p in db.ChatRooms
                                 where p.RoomID == id
                                 select p).First();

            ChatMessage[] msg = room.ChatMessages.OrderBy(d => d.Date).ToArray();
            
            foreach (var message in msg)
            {
                string str = message.Date + " " + message.aspnet_User.UserName + " said: " + message.Text;
                str += "(<a href=\"/uploads/" + message.Image + "\">Click to get file </a>)";
                messages.Add(str) ;
                i++;
            }
            
            return Json(messages.ToArray(), JsonRequestBehavior.AllowGet);

        }

        public JsonResult getRooms()
        {
            //Fiks problem seinar


            JodADataContext db = new JodADataContext();
            //List<ChatRoom> rooms = new List<ChatRoom>();

            ChatRoom[] rooms = (from p in db.ChatRooms
                        where p.isPublic
                        select p).ToArray();

            //foreach (var r in room)
            //    rooms.Add("Room: " + r.Name + " Owner: " + r.aspnet_User.UserName);
            Debug.WriteLine(rooms[0].Name);
            Debug.WriteLine(rooms[0].aspnet_User.UserName);
            JsonResult m = Json(rooms.ToArray(), JsonRequestBehavior.AllowGet);

            Debug.WriteLine(m.Data);

            return m;
        }

        public JsonResult getUsersOnRoom(Guid id)
        {
            JodADataContext db = new JodADataContext();
            List<Jason> userList = new List<Jason>();

            ChatRoom room = (from p in db.ChatRooms where p.RoomID == id select p).First();
            //aspnet_User[] users = (from user in db.aspnet_Users
            //                       join m2m in db.UserRooms on user.UserId equals m2m.UserID
            //                       where m2m.RoomID == id
            //                       select user).ToArray();
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

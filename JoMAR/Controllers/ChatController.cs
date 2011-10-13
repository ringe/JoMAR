using System;
using System.Collections;
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
            model.Users = new aspnet_User[room.UserRooms.Count];

            int i=0;
            foreach (UserRoom r in room.UserRooms)
            {
                model.Users[i] = r.aspnet_User;
                i++;
            }

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
            ChatMessage message = model.Message;

            // Prepare message for submit
            message.MessageID = Guid.NewGuid();
            message.UserID = (from p in db.aspnet_Users
                                where p.UserName == User.Identity.Name
                                select p).First().UserId;
            message.Text = model.Message.Text;

            // Submit message to DB
            db.ChatMessages.InsertOnSubmit(model.Message);
            db.SubmitChanges();

            room.ChatMessages.Add(message);

            model.MessageBoard = room.ChatMessages.ToArray();

            return View(model);
        }

    }
}

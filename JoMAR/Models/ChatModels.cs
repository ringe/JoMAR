using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using System.Web;

namespace JoMAR.Models
{
    public class MyRoom
    {
        public MyRoom(aspnet_User user, JodADataContext db)
        {
            Users = db.aspnet_Users.Select(x => new SelectListItem
            {
                Text = x.UserName,
                Value = x.UserId.ToString()
            }).ToList();

            Memberships = (from room in db.ChatRooms
                     join m2m in db.UserRooms on room.RoomID equals m2m.RoomID
                     where m2m.UserID == user.UserId
                     select room).ToList();

            MyRooms = user.ChatRooms;
            foreach (ChatRoom room in MyRooms)
                Memberships.Remove(room);
        }

        [Display(Name = "My Rooms")]
        public IEnumerable<ChatRoom> MyRooms { get; set; }

        [Display(Name = "Memberships")]
        public List<ChatRoom> Memberships { get; set; }

        [Display(Name = "Users")]
        public IEnumerable<SelectListItem> Users { get; set; }
        public string SelectedValue { get; set; }
    }

    public class ChatModel
    {
        public ChatModel(ChatRoom room, JodADataContext db)
        {
            MessageBoard = room.ChatMessages.ToArray();

            Users = (from user in db.aspnet_Users
                           join m2m in db.UserRooms on user.UserId equals m2m.UserID
                           where m2m.RoomID == room.RoomID
                           select user).ToList();

            if (!Users.Contains(room.aspnet_User))
                Users.Add(room.aspnet_User);

            Name = room.Name;
            RoomID = room.RoomID;
            Private = room.isPrivate;
        }

        public string Messages() {
            List<string> messages = new List<string>();
            int i = 0;
            foreach (var message in MessageBoard) {
                messages.Add(message.Date + " " + message.aspnet_User.UserName + " said: " + message.Text + "\n");
                i++;
            }
            messages.Sort();
            return string.Join("", messages.ToArray());
        }

        /// <summary>
        /// Is the given user a member in this room?
        /// </summary>
        /// <param name="user">user id</param>
        /// <param name="db">JodADataContext</param>
        /// <returns>true/false</returns>
        public bool IsMember(Guid user, JodADataContext db)
        {
            // See if user is a member already
            foreach (aspnet_User u in Users)
                if (u.UserId == user) return true;

            if (Private) // Private room
                return false;
            else
            {
                UserRoom r = new UserRoom();
                r.UserID = user;
                r.RoomID = RoomID;
                db.UserRooms.InsertOnSubmit(r);
                db.SubmitChanges();
                return true;
            }
        }

        public string UrlName()
        {
            return HttpUtility.UrlEncode(Name);
        }

        public Guid RoomID;

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "MessageBoard")]
        public ChatMessage[] MessageBoard { get; set; }

        [Display(Name = "Users")]
        public List<aspnet_User> Users { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }

        [Display(Name = "Private")]
        public bool Private { get; set; }
    }
}
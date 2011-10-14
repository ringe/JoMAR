using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;

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

            MyRooms = user.ChatRooms;
        }

        [Display(Name = "My Rooms")]
        public IEnumerable<ChatRoom> MyRooms { get; set; }

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

        public Guid RoomID;

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "MessageBoard")]
        public ChatMessage[] MessageBoard { get; set; }

        [Display(Name = "Users")]
        public List<aspnet_User> Users { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace JoMAR.Models
{
    public class ChatModel
    {
        public string Messages() {
            string messages ="";
            foreach (var message in MessageBoard) {
                messages += message.Date + " " + message.aspnet_User.UserName + " said: " + message.Text + "\n";
            }
            return messages;
        }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "MessageBoard")]
        public ChatMessage[] MessageBoard { get; set; }

        [Required]
        [Display(Name = "Users")]
        public List<aspnet_User> Users { get; set; }
        //public aspnet_User[] Users { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }
    }
}
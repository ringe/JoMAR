﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace JoMAR.Models
{
    public class ChatModel
    {
        [Required]
        [Display(Name = "MessageBoard")]
        public ChatMessage[] MessageBoard { get; set; }

        [Required]
        [Display(Name = "Users")]
        public aspnet_User[] Users { get; set; }

        [Display(Name = "Message")]
        public ChatMessage Message { get; set; }
    }
}
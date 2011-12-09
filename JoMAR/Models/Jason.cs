using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace JoMAR.Models
{
    public class Jason
    {
        [Display(Name = "UserId")]
        public virtual Guid UserId { get; set; }

        [Display(Name = "UserName")]
        public virtual string UserName { get; set; }

        [Display(Name = "Email")]
        public virtual string Email { get; set; }

        [Display(Name = "Gravatar")]
        public virtual string Gravatar { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace JoMAR.Models
{
    public class Invite
    {
        [Display(Name = "Invite by SMS:")]
        public virtual string CellPhone { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Security;

namespace JoMAR.Models
{
    public class Profile : ProfileBase
    {
        [Display(Name = "First Name")]
        public virtual string FirstName
        {
            get
            {
                return (this.GetPropertyValue("FirstName").ToString());
            }
            set
            {
                this.SetPropertyValue("FirstName", value);
            }
        }

        [Display(Name = "Last Name")]
        public virtual string LastName
        {
            get
            {
                return (this.GetPropertyValue("LastName").ToString());
            }
            set
            {
                this.SetPropertyValue("LastName", value);
            }
        }

        [Display(Name = "Cell Phone")]
        public virtual string CellPhone
        {
            get
            {
                return (this.GetPropertyValue("CellPhone").ToString());
            }
            set
            {
                this.SetPropertyValue("CellPhone", value);
            }
        }

        public static Profile GetProfile(string username)
        {
            return Create(username) as Profile;
        }
    }
}
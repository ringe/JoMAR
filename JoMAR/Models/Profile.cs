using System;
using System.Linq;
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
        private string email;
        private string image;

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

        [Display(Name = "Image")]
        public virtual string Image { get { return (this.image); } }

        [Display(Name = "Email")]
        public virtual string Email { get { return (this.email); } }

        /// <summary>
        /// Get a Profile for the given user
        /// </summary>
        /// <param name="username">username</param>
        /// <returns></returns>
        public static Profile GetProfile(string username)
        {
            Profile me = Create(username) as Profile;
            JodADataContext db = new JodADataContext();
            IQueryable<aspnet_User> users = (from p in db.aspnet_Users
                                 where p.UserName == username
                                   select p);
            if (users.Count() == 0) return null;
            aspnet_User user = users.First();
            me.image = Tools.Gravatar(user.aspnet_Membership.LoweredEmail, me.FirstName + " " + me.LastName);
            me.email = user.aspnet_Membership.Email;
            return me;
        }
    }
}
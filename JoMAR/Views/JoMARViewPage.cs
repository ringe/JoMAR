using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using JoMAR.Models;

namespace JoMAR.Views
{
    /// <summary>
    /// Custom default WebViewPage for JoMAR Chat.
    /// </summary>
    public abstract class JoMARViewPage<T> : WebViewPage<T>
    {
        protected override void InitializePage()
        {
            DefaultViewBag();
            base.InitializePage();
        }

        /// <summary>
        /// Prepare default ViewBag properties
        /// </summary>
        private void DefaultViewBag()
        {
            ViewBag.Profile = (Request.IsAuthenticated) ? Profile.GetProfile(User.Identity.Name) : null;
        }
    }
}
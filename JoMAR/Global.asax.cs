using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Caching;
using JoMAR.Models;
using System.Diagnostics;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace JoMAR
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static CacheItemRemovedCallback OnCacheRemove = null;

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Profile",
                "Chat/Profile",
                new { controller = "Chat", action = "Profile", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Chat",
                "Chat/{name}",
                new { controller = "Chat", action = "Index", name = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Rooms", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            // Add task to read SMS's
            AddTask("ReadSMS", 30);
        }

        /// <summary>
        /// Add periodic task to the cache, run in (expire in) given seconds
        /// </summary>
        /// <param name="name">Name of task to run</param>
        /// <param name="seconds">Number of seconds between runs</param>
        private void AddTask(string name, int seconds)
        {
            OnCacheRemove = new CacheItemRemovedCallback(CacheItemRemoved);
            HttpRuntime.Cache.Insert(name, seconds, null,
                DateTime.Now.AddSeconds(seconds), Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable, OnCacheRemove);
        }

        /// <summary>
        /// Remove task from cache, perform task and insert task again for next run
        /// </summary>
        public void CacheItemRemoved(string k, object v, CacheItemRemovedReason r)
        {
            // Read SMS's from server
            if (k == "ReadSMS")
            {
                JodADataContext db = new JodADataContext();
                IQueryable<string> users = (from p in db.aspnet_Users select p.UserName.ToLower());
                IQueryable<string> rooms = (from p in db.ChatRooms select p.Name.ToLower());

                foreach (string user in users)
                {
                    // Fetch user profile
                    Profile p = Profile.GetProfile(user);

                    // If user has a cell phone
                    if (p.CellPhone.Length == 10)
                    {
                        // Get phone number
                        long nr = Int64.Parse(p.CellPhone);

                        // Retrieve all SMS from this user
                        while (true)
                        {
                            SMS sms = SMS.Get(nr);
                            if (sms.status == "NoSuchMessage")
                                break;
                            else
                            {
                                string[] text = Regex.Split(sms.txt, @"\W+");
                                bool found = false;

                                // If UserName found, post to private room
                                if (users.Contains(text[0].ToLower()))
                                {
                                    found = true;
                                    Profile user1 = Profile.GetProfile(user);
                                    Profile user2 = Profile.GetProfile(user);
                                    ChatRoom room = Rooms.Private(user1, user2, db);

                                    // Prepare message
                                    string msg = "";
                                    foreach (string s in text)
                                        if (s != text[0])
                                            msg += s + " ";

                                    // Post message to room
                                    Rooms.Post(msg, user1.UserId, room.RoomID, db);

                                    // Forward message to the other user
                                    SMS.Send(user1.UserName + " said: " + msg, "JoMAR", user2.CellPhone);
                                    //Debug.WriteLine("Found message to user '" + text[0] + "'");
                                }

                                // If Room found, post.
                                if (rooms.Contains(text[0].ToLower()))
                                {
                                    found = true;
                                    //Debug.WriteLine("Found message to room '" + text[0] + "'");
                                }

                                // Reply with not found if no room found.
                                if (!found)
                                    SMS.Send("Thanks for you interest in us, but we didn't find the user or room you seek.", "JoMAR", sms.snd);
                            }
                                
                        }
                    }
                }
            }

            AddTask(k, Convert.ToInt32(v));
        }
    }
}
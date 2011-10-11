using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JoMAR.Controllers
{
    public class ChatController : Controller
    {
        //
        // GET: /Chat/

        public ActionResult Index(string name)
        {
            ViewBag.Title = name;

            new JoMAR.Models.ChatModel();

            return View();
        }

    }
}

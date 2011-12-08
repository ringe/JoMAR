using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Net;
using System.IO;
using JoMAR.Models;
using Newtonsoft.Json;

namespace JoMAR.Controllers
{
    public class EssemessController : Controller
    {
        //
        // GET: /Essemess/

        public ActionResult Index()
        {
            //ViewBag.mama = SMS.Send("mama speaking", 4797177229, 4741800072);
            String jsonresult = SMS.Get(4799104626);

            SMS sms = JsonConvert.DeserializeObject<SMS>(jsonresult);

            return View(sms);
        }

    }
}

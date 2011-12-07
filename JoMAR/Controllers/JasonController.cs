using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JoMAR.Controllers
{
    public class JasonController : Controller
    {
        //
        // GET: /Jason/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult getMessages(Guid id)
        {
            List<dynamic> list = new List<dynamic>();
            JodADataContext db = new JodADataContext();

            dynamic msg = (from p in db.ChatMessages
                           where p.RoomID == id
                           select p.Text).ToList();

            //foreach (string o in msg)
            //{
            //    var myOp = new
            //    {
            //        text = o.Text

            //    };
            //    list.Add(myOp);
            //}
            return Json(msg, JsonRequestBehavior.AllowGet);

        }

    }
}

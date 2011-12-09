using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using JoMAR.Models;



namespace JoMAR.Controllers
{
    public class UploadController : Controller
    {
        [Authorize, HttpPost]
        public void Index(Guid room, HttpPostedFileBase file)
        {
            Profile user = JoMAR.Models.Profile.GetProfile(User.Identity.Name);
            JodADataContext db = new JodADataContext();

            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                // extract only the fielname
                var fileName = Path.GetFileName(file.FileName);
                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/uploads"), fileName);
                file.SaveAs(path);

                Rooms.Post("File uploaded by "+ user.UserName, user.UserId, room, db, fileName);
            }
        }
    }
}
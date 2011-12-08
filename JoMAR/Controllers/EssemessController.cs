using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Net;
using System.IO;

namespace JoMAR.Controllers
{
    public class EssemessController : Controller
    {
        //
        // GET: /Essemess/

        public ActionResult Index()
        {
            // used to build entire input
            StringBuilder sb = new StringBuilder();

            // used on each read operation
            byte[] buf = new byte[8192];

            // prepare the web page we will be asking for
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create("http://malmen.hin.no/pswin/sms/sendsms");

            string parameter = "RCV=4747281944&";
            parameter += "SND=4797177229&";
            parameter += "TXT=JoMAR ja JoMAR hahahaha";
            byte[] byteArray = Encoding.UTF8.GetBytes(parameter);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            // POST
            request.Method = "POST";

            // Say who it is
            request.UserAgent = "JoMAR Chat in ASP.NET";

            // prepare request
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            // execute the request
            HttpWebResponse response = (HttpWebResponse)
                request.GetResponse();

            // we will read data via the response stream
            Stream resStream = response.GetResponseStream();

            string tempString = null;
            int count = 0;

            do
            {
                // fill the buffer with data
                count = resStream.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count != 0)
                {
                    // translate from bytes to ASCII text
                    tempString = Encoding.ASCII.GetString(buf, 0, count);

                    // continue building the string
                    sb.Append(tempString);
                }
            }
            while (count > 0); // any more data to read?

            ViewBag.mama = sb.ToString();

            resStream.Close();
            response.Close();

            return View();
        }

    }
}

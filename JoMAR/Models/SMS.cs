using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace JoMAR.Models
{
    /// <summary>
    /// The SMS class takes care of handling SMS sending and receiving.
    /// </summary>
    public class SMS
    {
        //private int id;
        //private long snd;
        //private long rcv;
        //private string txt;
        //private DateTime date;
        //private string status;

        public string id { get; set; }
        public string snd { get; set; }
        public string rcv { get; set; }
        public string txt { get; set; }
        public string date { get; set; }
        public string status { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SMS() {}

        /// <summary>
        /// Any logging here?
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="sender">The sender's phone number</param>
        /// <param name="receiver">The receiver's phone number</param>
        public static Boolean Send(String message, string sender, string receiver) {
            // used to build entire input
            StringBuilder sb = new StringBuilder();

            // used on each read operation
            byte[] buf = new byte[8192];

            // prepare the web page we will be asking for
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create("http://malmen.hin.no/pswin/sms/sendsms");

            // Prepare request parameters
            string parameter = "RCV=" + receiver;
            parameter += "&SND=" + sender;
            parameter += "&TXT=" + message;
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
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();

            // we will read data via the response stream
            Stream resStream = response.GetResponseStream();

            string tempString = null;
            int count = 0;

            if (HttpStatusCode.OK == response.StatusCode)
            {
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
            }
            else return false;

            // Close streams
            resStream.Close();
            response.Close();

            // Success
            return true;
        }

        /// <summary>
        /// Fetch all SMS's sent by the given number
        /// </summary>
        /// <param name="sender">The number to look up</param>
        /// <returns></returns>
        public static SMS Get(long sender)
        {
            // used to build entire input
            StringBuilder sb = new StringBuilder();

            // used on each read operation
            byte[] buf = new byte[8192];

            // prepare the web page we will be asking for
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://malmen.hin.no/pswin/SMS/ChatInNumber/" + sender);

            // Say who it is
            request.UserAgent = "JoMAR Chat in ASP.NET";

            // execute the request
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();

            // we will read data via the response stream
            Stream resStream = response.GetResponseStream();

            string tempString = null;
            int count = 0;

            if (HttpStatusCode.OK == response.StatusCode)
            {
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
            }
            else return null;

            // Close streams
            resStream.Close();
            response.Close();

            // Convert result to SMS
            SMS sms = (SMS)JsonConvert.DeserializeObject(sb.ToString(), typeof(SMS));
            

            // Return the SMS found
            return sms;
        }
    }
}
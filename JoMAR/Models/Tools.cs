using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Net;
using System.IO;

namespace JoMAR.Models
{
    public class Tools
    {
        public static string GetCode() { 
            string randomUrl = "http://www.random.org/strings/?num=1&len=5&digits=on&upperalpha=on&loweralpha=on&unique=on&format=plain&rnd=new";
            // used to build entire input
            StringBuilder sb = new StringBuilder();

            // used on each read operation
            byte[] buf = new byte[8192];

            // prepare the web page we will be asking for
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(randomUrl);

            // execute the request
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();

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

            // return
            return  sb.ToString();
        }

        public static string Gravatar(string Email, string Alt)
        {
            string imageUrl = "http://www.gravatar.com/avatar/";

            if (!string.IsNullOrEmpty(Email))
            {
                imageUrl += MD5Hash(Email);
                imageUrl += "?d=wavatar";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<a href=\"http://www.gravatar.com\" target=\"_blank\" title=\"Get your Free Gravatar\">");
            sb.Append("<img src=\"");
            sb.Append(imageUrl);
            sb.Append("\"");
            if (!string.IsNullOrEmpty(Alt)) sb.Append(" alt=\"" + Alt + "\"");
            sb.Append(" />");
            sb.Append("</a>");

            return sb.ToString();
        }

        private static string MD5Hash(string Email)
        {
            // build up image url, including MD5 hash for supplied email:
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            UTF8Encoding encoder = new UTF8Encoding();
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(Email.ToLower()));
            StringBuilder sb = new StringBuilder(hashedBytes.Length * 2);
            for (int i = 0; i < hashedBytes.Length; i++)
            {
                sb.Append(hashedBytes[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
    }
}
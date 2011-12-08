using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;

namespace JoMAR.Models
{
    public class Gravatar
    {
        public static string Image(string Email, string Alt)
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
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices
{
    class Util
    {
        /// <summary>
        /// Make a name from a forename and surname
        /// </summary>
        /// <param name="forename">The forename part of the the name</param>
        /// <param name="surname">The surname part of the name</param>
        /// <returns></returns>
        public static string MakeName(string forename, string surname)
        {
            return surname + ", " + forename;
        }

        /// <summary>
        /// Get a username/id from a emil address
        /// </summary>
        /// <param name="userName">The email address</param>
        /// <returns>A username/id</returns>
        public static string TrimEmail(string userName)
        {
            // If user has included their email address, get rid of it
            if(userName.Contains('@'))
            {
                string[] name = userName.Split('@');
                return name[0];
            }
            else
                return userName;
        }

        /// <summary>
        /// Format a date time object
        /// </summary>
        /// <param name="date">The date time object to format</param>
        /// <returns>Date as dd MMM yyyy</returns>
        public static string FormatDate(DateTime date)
        {
            return date.ToString("dd MMMM, yyyy");
        }

        /// <summary>
        /// Convert a string to a byte array - From: http://stackoverflow.com/a/5664400
        /// </summary>
        /// <param name="str">The string to convert</param>
        /// <returns>A byte array</returns>
        public static byte[] ConvertToBinary(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        /// <summary>
        /// Convert a binary object to a string
        /// </summary>
        /// <param name="bytes">The binary object to convert</param>
        /// <returns>A string</returns>
        public static string ConvertToString(Binary bytes)
        {

            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        /// <summary>
        /// Check if the given file type is valid
        /// </summary>
        /// <param name="mimeType">The files mime type</param>
        /// <returns>True if valid, false if not</returns>
        public static bool ValidFileType(string mimeType)
        {
            // TODO: replace with a dictionary
            string[] validTypes = { "image/gif", "image/jpeg", "image/x-png", "video/mpeg", "application/pdf", "application/x-pdf", "application/zip", "application/x-tar", "application/mspowerpoint", "application/msword" };
            if (validTypes.Contains(mimeType))
                return true;
            else return false;
        }

        /// <summary>
        /// Generate a new result set from a given list of results
        /// </summary>
        /// <typeparam name="T">The type of result</typeparam>
        /// <param name="myResults">The list of results</param>
        /// <param name="perPage">The number of results per page</param>
        /// <returns>A result set of type T</returns>
        public static List<List<T>> GenerateResultSet<T>(List<T> myResults, int perPage)
        {
            try
            {
                var count = 0;
                var resultSet = new List<T>();
                var pages = new List<List<T>>();
                foreach (var myResult in myResults)
                {
                    if (count > perPage)
                    {
                        pages.Add(resultSet);
                        resultSet = new List<T>();
                        count = 0;
                    }
                    else
                    {
                        resultSet.Add(myResult);
                        count++;
                    }
                }
                // Add any leftover results
                pages.Add(resultSet);
                return pages;
            }
            catch
            {
                return new List<List<T>>();
            }
        }

        /// <summary>
        /// A generic error message to display
        /// </summary>
        public static string GenericError = "Sorry, something went wrong. Try again, and if the error persists please contact the site admin";
        
        /// <summary>
        /// The base url of the website
        /// </summary>
        public static string BaseUrl = "localhost"; 
    }
}

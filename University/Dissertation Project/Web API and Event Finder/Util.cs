using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Xml.Linq;

namespace ImageServer
{
    public class Util
    {
        public static string GenericError = "Something went wrong.";

        public static bool CheckAuth()
        {
            //check header authentication 
            return true;
        }

        /// <summary>
        /// Replace . with , to allow coordinate to be used in a url
        /// </summary>
        /// <param name="coordinate">The coordinate to encapsulate</param>
        /// <returns>A string with the URL safe coordinate</returns>
        public static string DecapsulateCoordinate(string coordinate)
        {
            return coordinate.Replace(',', '.');
        }

        /// <summary>
        /// Converts an array of bytes to a given result object
        /// </summary>
        /// <typeparam name="T">The result object type</typeparam>
        /// <param name="bytes">The byte array to convert</param>
        /// <returns>The requested result type</returns>
        public static T ByteArrayToObject<T>(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }

        /// <summary>
        /// Geocode a set of coordinates to a postcode string
        /// </summary>
        /// <param name="latlng">The lat,lng to geocode</param>
        /// <returns>A string containing a postcode</returns>
        public static string GetPostcode(string latlng)
        {
            string[] coords = latlng.Split(',');
            var geoRequestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false", coords[0], coords[1]);

            var geoRequest = WebRequest.Create(geoRequestUri);
            var geoResponse = geoRequest.GetResponse();
            var geoXDoc = XDocument.Load(geoResponse.GetResponseStream());

            var geoResult = geoXDoc.Element("GeocodeResponse").Element("result");
            foreach (var addressComp in geoResult.Elements("address_component"))
            {
                if (addressComp.Element("type").Value == "postal_code")
                    return addressComp.Element("short_name").Value;
            }
            return "";
            //var imgAddress = geoResult.Element("formatted_address");
            //var imgPostcode = geoResult.Element("address_components").Descendants("postcode"); //not sure if this is right
            //return "a postcode";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;

namespace TwitchServices
{
    public class Util
    {
        /// <summary>
        /// Common SQL statement used to join twitch tables
        /// </summary>
        public static string TwitchJoins = " JOIN mp115.Twitch_Species ON mp115.Twitch_Twitch.id_species=mp115.Twitch_Species.id_species JOIN mp115.Twitch_User ON mp115.Twitch_Twitch.id_user=mp115.Twitch_User.id_user LEFT JOIN mp115.Twitch_Image ON mp115.Twitch_Twitch.id_twitch=mp115.Twitch_Image.id_twitch";

        /// <summary>
        /// Get the lat and lng coordinates from a location string
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static string[] GetCoordinates(string location)
        {
            // Split string and de-capsulate
            string[] coords = location.Split(';');
            foreach(string coord in coords)
                coord.Replace(',', '.');
            return coords;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace TwitchServices
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")] //stuweb.mp115
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class TwitchService : System.Web.Services.WebService
    {
        private Database mDb = new Database();

        [WebMethod]
        public XmlDocument AllTwitches()
        {
            SqlDataReader dataTwitch = mDb.ExecuteQuery("SELECT * FROM mp115.Twitch_Twitch" + Util.TwitchJoins + ";");
            XmlDocument res = CreateTwitchResult(dataTwitch);
            mDb.CloseConn();
            return res;
        }

        [WebMethod]
        public XmlDocument TwitchByID(string id)
        {
            SqlDataReader dataTwitch = mDb.ExecuteQuery("SELECT * FROM mp115.Twitch_Twitch" + Util.TwitchJoins + " WHERE uid_twitch = '" + id + "';");
            XmlDocument res = CreateTwitchResult(dataTwitch);
            mDb.CloseConn();
            return res;
        }

        [WebMethod]
        public XmlDocument TwitchBySpecies(string name)
        {
            SqlDataReader dataTwitch = mDb.ExecuteQuery("SELECT * FROM mp115.Twitch_Twitch" + Util.TwitchJoins + " WHERE name LIKE '%" + name + "%';");
            XmlDocument res = CreateTwitchResult(dataTwitch);
            mDb.CloseConn();
            return res;
        }

        [WebMethod]
        public XmlDocument TwitchByImage(int hasImage)
        {
            SqlDataReader dataTwitch;
            switch (hasImage)
            {
                case 0:
                    dataTwitch = mDb.ExecuteQuery("SELECT * FROM mp115.Twitch_Twitch" + Util.TwitchJoins + " WHERE hasImage = '0';");
                    break;
                case 1:
                    dataTwitch = mDb.ExecuteQuery("SELECT * FROM mp115.Twitch_Twitch" + Util.TwitchJoins + " WHERE hasImage = '1';");
                    break;
                default:
                    dataTwitch = mDb.ExecuteQuery("SELECT * FROM mp115.Twitch_Twitch" + Util.TwitchJoins + ";");
                    break;
            }
            XmlDocument res = CreateTwitchResult(dataTwitch);
            mDb.CloseConn();
            return res;
        }

        [WebMethod]
        public XmlDocument TwitchByCoord(string lat, string lng)
        {
            SqlDataReader dataTwitch = mDb.ExecuteQuery("SELECT * FROM mp115.Twitch_Twitch" + Util.TwitchJoins + " WHERE lat = '" + lat + "' AND lng = '" + lng + "';");
            XmlDocument res = CreateTwitchResult(dataTwitch);
            mDb.CloseConn();
            return res;
        }

        [WebMethod]
        public XmlDocument TwitchByLocation(string location)
        {
            SqlDataReader dataTwitch = mDb.ExecuteQuery("SELECT * FROM mp115.Twitch_Twitch" + Util.TwitchJoins + " WHERE location LIKE '%" + location + "%';");
            XmlDocument res = CreateTwitchResult(dataTwitch);
            mDb.CloseConn();
            return res;
        }

        // No overloads in 3.5, so each of these will have to be declared
        [WebMethod]
        public XmlDocument TwitchSearch(string species, string location, int hasImage)
        {
            string query;
            if (hasImage == 2)
                query = "SELECT * FROM mp115.Twitch_Twitch" + Util.TwitchJoins + " WHERE name LIKE '%" + species + "%' AND location LIKE '%" + location + "%';";
            else
                query = "SELECT * FROM mp115.Twitch_Twitch" + Util.TwitchJoins + " WHERE name LIKE '%" + species + "%' AND location LIKE '%" + location + "%' AND hasImage = " + hasImage + ";";
            SqlDataReader dataTwitch = mDb.ExecuteQuery(query);
            XmlDocument res = CreateTwitchResult(dataTwitch);
            mDb.CloseConn();
            return res;
        }

        [WebMethod]
        public XmlDocument TwitchSearchXSLT(string species, string location, int hasImage, string xslt)
        {
            string query;
            if (hasImage == 2)
                query = "SELECT * FROM mp115.Twitch_Twitch" + Util.TwitchJoins + " WHERE name LIKE '%" + species + "%' AND location LIKE '%" + location + "%';";
            else
                query = "SELECT * FROM mp115.Twitch_Twitch" + Util.TwitchJoins + " WHERE name LIKE '%" + species + "%' AND location LIKE '%" + location + "%' AND hasImage = " + hasImage + ";";
            SqlDataReader dataTwitch = mDb.ExecuteQuery(query);
            XmlDocument res = CreateTwitchResult(dataTwitch, xslt);
            mDb.CloseConn();
            return res;
        }

        [WebMethod]
        public XmlDocument GetSpecies(string name)
        {
            string query = "SELECT * FROM mp115.Twitch_Twitch" + Util.TwitchJoins + " WHERE name LIKE '%" + name + "%';";
            SqlDataReader dataTwitch = mDb.ExecuteQuery(query);
            XmlDocument res = CreateNameResult(dataTwitch, "species");
            mDb.CloseConn();
            return res;
        }

        [WebMethod]
        public XmlDocument GetLocation(string location)
        {
            string query = "SELECT * FROM mp115.Twitch_Twitch" + Util.TwitchJoins + " WHERE location LIKE '%" + location + "%';";
            SqlDataReader dataTwitch = mDb.ExecuteQuery(query);
            XmlDocument res = CreateNameResult(dataTwitch, "location");
            mDb.CloseConn();
            return res;
        }

        /// <summary>
        /// Create a new BWML document from the query results
        /// </summary>
        /// <param name="dataTwitch">The results to construct the BWML from</param>
        /// <returns>A BWML document</returns>
        private XmlDocument CreateTwitchResult(SqlDataReader dataTwitch)
        {
            return CreateTwitchResult(dataTwitch, null);
        }

        /// <summary>
        /// Create a new BWML document from the query results
        /// </summary>
        /// <param name="dataTwitch">The results to construct the BWML from</param>
        /// <param name="xslt">Whether to include server side XSLT</param>
        /// <returns>A BWML document</returns>
        private XmlDocument CreateTwitchResult(SqlDataReader dataTwitch, string xslt)
        {
            // Instantiate an XML document
            XmlDocument xmlDom = new XmlDocument();
            xmlDom.AppendChild(xmlDom.CreateElement("", "twitches", ""));
            XmlElement xmlRoot = xmlDom.DocumentElement;

            // Create local variables
            XmlElement xmlTwitch, xmlSpecies, xmlSpeciesName, xmlAge, xmlSex, xmlFree_Text, xmlDate, xmlAddress, xmlHouse, xmlStreet, xmlTown, xmlCity, xmlPostcode, xmlLng, xmlLat, xmlDistrict, xmlUser, xmlUsername, xmlEmail, xmlPassword, xmlAct_Code, xmlImageSet, xmlImage;
            XmlAttribute attrId, attrFile, attrDesc;
            XmlText xmlText;
            string twitchId, speciesId, speciesName, age, sex, free_text, house, street, town, city, postcode, lng, lat, district, username, password, email, act_code, userId, date;

            // Loop over all of the records in the data set
            while (dataTwitch.Read())
            {
                // Retrieve the records from the data set
                twitchId = dataTwitch["uid_twitch"].ToString();
                age = dataTwitch["age"].ToString();
                sex = dataTwitch["sex"].ToString();
                speciesId = dataTwitch["id_species"].ToString();
                house = dataTwitch["house"].ToString();
                street = dataTwitch["street"].ToString();
                town = dataTwitch["town"].ToString();
                city = dataTwitch["city"].ToString();
                postcode = dataTwitch["postcode"].ToString();
                lng = dataTwitch["lng"].ToString();
                lat = dataTwitch["lat"].ToString();
                district = dataTwitch["district"].ToString();
                speciesName = dataTwitch["name"].ToString();
                userId = dataTwitch["uid_user"].ToString();
                username = dataTwitch["username"].ToString();
                email = dataTwitch["email"].ToString();
                act_code = dataTwitch["act_code"].ToString();
                date = dataTwitch["date"].ToString();
                free_text = dataTwitch["free_text"].ToString();

                // Create XML elements for the records and append them to the DOM
                xmlTwitch = xmlDom.CreateElement("twitch");
                attrId = xmlDom.CreateAttribute("id");
                attrId.Value = twitchId;
                xmlTwitch.Attributes.Append(attrId);
                xmlSpecies = xmlDom.CreateElement("species");
                xmlText = xmlDom.CreateTextNode(speciesName);
                xmlSpecies.AppendChild(xmlText);
                xmlTwitch.AppendChild(xmlSpecies);
                xmlAge = xmlDom.CreateElement("age");
                xmlText = xmlDom.CreateTextNode(age);
                xmlAge.AppendChild(xmlText);
                xmlTwitch.AppendChild(xmlAge);
                xmlFree_Text = xmlDom.CreateElement("free_text");
                xmlText = xmlDom.CreateTextNode(free_text);
                xmlFree_Text.AppendChild(xmlText);
                xmlTwitch.AppendChild(xmlFree_Text);
                xmlSex = xmlDom.CreateElement("sex");
                xmlText = xmlDom.CreateTextNode(sex);
                xmlSex.AppendChild(xmlText);
                xmlTwitch.AppendChild(xmlSex);
                xmlDate = xmlDom.CreateElement("date");
                xmlText = xmlDom.CreateTextNode(date);
                xmlDate.AppendChild(xmlText);
                xmlTwitch.AppendChild(xmlDate);
                xmlAddress = xmlDom.CreateElement("address");
                xmlHouse = xmlDom.CreateElement("house");
                xmlText = xmlDom.CreateTextNode(house);
                xmlHouse.AppendChild(xmlText);
                xmlAddress.AppendChild(xmlHouse);
                xmlStreet = xmlDom.CreateElement("street");
                xmlText = xmlDom.CreateTextNode(street);
                xmlStreet.AppendChild(xmlText);
                xmlAddress.AppendChild(xmlStreet);
                xmlTown = xmlDom.CreateElement("town");
                xmlText = xmlDom.CreateTextNode(town);
                xmlTown.AppendChild(xmlText);
                xmlAddress.AppendChild(xmlTown);
                xmlCity = xmlDom.CreateElement("city");
                xmlText = xmlDom.CreateTextNode(city);
                xmlCity.AppendChild(xmlText);
                xmlAddress.AppendChild(xmlCity);
                xmlPostcode = xmlDom.CreateElement("postcode");
                xmlText = xmlDom.CreateTextNode(postcode);
                xmlPostcode.AppendChild(xmlText);
                xmlAddress.AppendChild(xmlPostcode);
                xmlLng = xmlDom.CreateElement("lng");
                xmlText = xmlDom.CreateTextNode(lng);
                xmlLng.AppendChild(xmlText);
                xmlAddress.AppendChild(xmlLng);
                xmlLat = xmlDom.CreateElement("lat");
                xmlText = xmlDom.CreateTextNode(lat);
                xmlLat.AppendChild(xmlText);
                xmlAddress.AppendChild(xmlLat);
                xmlDistrict = xmlDom.CreateElement("district");
                xmlText = xmlDom.CreateTextNode(district);
                xmlDistrict.AppendChild(xmlText);
                xmlAddress.AppendChild(xmlDistrict);
                xmlTwitch.AppendChild(xmlAddress);
                xmlUser = xmlDom.CreateElement("user");
                attrId = xmlDom.CreateAttribute("id");
                attrId.Value = userId;
                xmlUser.Attributes.Append(attrId);
                xmlUsername = xmlDom.CreateElement("userName");
                xmlText = xmlDom.CreateTextNode(username);
                xmlUsername.AppendChild(xmlText);
                xmlUser.AppendChild(xmlUsername);
                xmlEmail = xmlDom.CreateElement("email");
                xmlText = xmlDom.CreateTextNode(email);
                xmlEmail.AppendChild(xmlText);
                xmlUser.AppendChild(xmlEmail);
                xmlAct_Code = xmlDom.CreateElement("act_code");
                xmlText = xmlDom.CreateTextNode(act_code);
                xmlAct_Code.AppendChild(xmlText);
                xmlUser.AppendChild(xmlAct_Code);
                xmlTwitch.AppendChild(xmlUser);

                //find the twitches images
                SqlDataReader imgTwitch = new Database().ExecuteQuery("SELECT * FROM mp115.Twitch_Image WHERE id_twitch = '" + twitchId + "';");
                xmlImageSet = xmlDom.CreateElement("imageSet");
                while (imgTwitch.Read())
                {
                    xmlImage = xmlDom.CreateElement("image");
			        attrDesc = xmlDom.CreateAttribute("desc");
			        xmlText = xmlDom.CreateTextNode(imgTwitch["description"].ToString());
			        xmlImage.AppendChild(attrDesc);
			        attrFile = xmlDom.CreateAttribute("file");
			        xmlText = xmlDom.CreateTextNode(imgTwitch["filename"].ToString() + ".jpg");
			        xmlImage.AppendChild(attrFile);
			        xmlImageSet.AppendChild(xmlImage);
                }
                xmlTwitch.AppendChild(xmlImageSet);

                //add the twitch to the twitchset
                xmlRoot.AppendChild(xmlTwitch);
            }

            // Tidy up
            dataTwitch.Close();
            // Return the XML documant
            return xmlDom;
        }

        /// <summary>
        /// Create a BWML document contain possible species or locations for search terms
        /// </summary>
        /// <param name="dataTwitch">The query results to transform</param>
        /// <param name="type">Whether to return species or location names</param>
        /// <returns></returns>
        private XmlDocument CreateNameResult(SqlDataReader dataTwitch, string type)
        {
            // Instantiate an XML document
            XmlDocument xmlDom = new XmlDocument();
            xmlDom.AppendChild(xmlDom.CreateElement("", "twitches", ""));
            XmlElement xmlRoot = xmlDom.DocumentElement;
            // Create local variables
            XmlElement xmlTwitch, xmlSpecies, xmlLocation;
            XmlText xmlText;
            string speciesName, locationName;

            while (dataTwitch.Read())
            {
                speciesName = dataTwitch["name"].ToString();
                locationName = dataTwitch["location"].ToString();
                xmlTwitch = xmlDom.CreateElement("twitch");
                if (type == "species")
                {
                    xmlSpecies = xmlDom.CreateElement("species");
                    xmlText = xmlDom.CreateTextNode(speciesName);
                    xmlSpecies.AppendChild(xmlText);
                    xmlTwitch.AppendChild(xmlSpecies);
                }
                if (type == "location")
                {
                    xmlLocation = xmlDom.CreateElement("location");
                    xmlText = xmlDom.CreateTextNode(locationName);
                    xmlLocation.AppendChild(xmlText);
                    xmlTwitch.AppendChild(xmlLocation);
                }
                //add the twitch to the twitchset
                xmlRoot.AppendChild(xmlTwitch);
            }
            // Tidy up
            dataTwitch.Close();
            // Return the XML documant
            return xmlDom;
        }
    }
}
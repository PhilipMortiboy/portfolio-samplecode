using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;

namespace ImageServer
{
    public class EventProcessor
    {
        //api keys
        private static string songkickKey = "hxEceZnZ47OqsYMi";
        private static string skiddleKey = "";
        private static string eventlyKey = "";
        //base urls
        private static string songkickUrl = "http://api.songkick.com/api/3.0/events.xml?apikey=";
        private static string songkickVenueUrl = "http://api.songkick.com/api/3.0/venues/";
        private static string skiddleUrl = "";
        private static string eventlyUrl = "";
        //songkick location ids
        private static string london = "24426";

        /// <summary>
        /// Query the songkick API to find and save events within given date range
        /// </summary>
        public static bool GetSongkickEvents(int limit = 100)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddMonths(1);
            //convert dates into format accepted by songkick API. Day and month must be double digits
            string startDateStr = startDate.Year.ToString() + "-" + startDate.ToString("MM") + "-" + startDate.ToString("dd");
            string endDateStr = startDate.Year.ToString() + "-" + startDate.ToString("MM") + "-" + startDate.ToString("dd");
            //retrive data from songkick
            string queryStr = songkickUrl + songkickKey + "&min_date=" + startDateStr + "&max_date=" + endDateStr + "&location=sk:" + london;
            XmlDocument response = GetXmlResponse(queryStr);
            if (response == null)
                return false;
            else
            {
                int numCompleted = 0;
                int maxNum = limit;
                ImageDbDataContext mDb = new ImageDbDataContext();
                //processes the data returned
                XmlNodeList myEvents = response.GetElementsByTagName("event");
                foreach (XmlNode eventNode in myEvents)
                {
                    Event newEvent = new Event();
                    //find the event name
                    XmlAttributeCollection eventAtt = eventNode.Attributes;
                    foreach (XmlAttribute myAtt in eventAtt)
                    {
                        if (myAtt.Name == "displayName")
                        {
                            newEvent.name = myAtt.Value;
                            break;
                        }
                    }
                    XmlNodeList childNodes = eventNode.ChildNodes;
                    foreach (XmlNode childNode in childNodes)
                    {
                        //find the start datetime string
                        if (childNode.Name == "start")
                        {
                            XmlAttributeCollection childAtt = childNode.Attributes;
                            foreach (XmlAttribute myAtt in childAtt)
                            {
                                if (myAtt.Name == "datetime")
                                {
                                    try
                                    {
                                        newEvent.startDate = DateTime.Parse(myAtt.Value);
                                        //songkick has no data for end time, so just add 6 hours to end time - may need increasing
                                        newEvent.endDate = newEvent.startDate.Value.AddHours(6);
                                    }
                                    catch
                                    {
                                        newEvent.startDate = null;
                                        newEvent.endDate = null;
                                    }
                                    break;
                                }
                            }
                        }
                        //find the venue detaiks
                        if (childNode.Name == "venue")
                        {
                            XmlAttributeCollection childAtt = childNode.Attributes;
                            string lat = "";
                            string lng = "";
                            string id = "";
                            foreach (XmlAttribute myAtt in childAtt)
                            {
                                if (myAtt.Name == "displayName")
                                    newEvent.venueName = myAtt.Value;
                                if (myAtt.Name == "lat")
                                    lat = myAtt.Value;
                                if (myAtt.Name == "lng")
                                    lng = myAtt.Value;
                                if (myAtt.Name == "id")
                                    id = myAtt.Value;
                            }
                            newEvent.latlng = lat + "," + lng;
                            //get the venue's address
                            if (id != "")
                            {
                                string venuequeryStr = songkickVenueUrl + id + ".xml?apikey=" + songkickKey;
                                XmlDocument venueresponse = GetXmlResponse(venuequeryStr);
                                //processes the data returned
                                XmlNodeList myVenue = venueresponse.GetElementsByTagName("venue");
                                foreach (XmlNode venueNode in myVenue)
                                {
                                    //find venue details
                                    XmlAttributeCollection venueAtt = venueNode.Attributes;
                                    foreach (XmlAttribute myAtt in venueAtt)
                                    {
                                        if (myAtt.Name == "zip")
                                            newEvent.postcode = myAtt.Value;
                                        if (myAtt.Name == "street")
                                            newEvent.address = myAtt.Value;
                                        if (myAtt.Name == "description")
                                            newEvent.description = myAtt.Value;
                                    }
                                }
                            }
                        }
                        //find the performance details
                        if (childNode.Name == "performance")
                        {
                            XmlAttributeCollection childAtt = childNode.Attributes;
                            string name = "";
                            bool headliner = false;
                            foreach (XmlAttribute myAtt in childAtt)
                            {
                                if (myAtt.Name == "displayName")
                                    name = myAtt.Value;
                                if (myAtt.Name == "billing")
                                {
                                    if (myAtt.Value == "headline")
                                        headliner = true;
                                }
                            }
                            //only add the headliner as the artist
                            if (headliner)
                                newEvent.artistName = name;
                        }
                    }
                    try
                    {
                        //add new event to database
                        newEvent.source = 1; //1 = songkick
                        newEvent.eventType = AlbumSubject.Music.ToString(); //all songkick events are music
                        //check if event already exists
                        var checkevent = 
                            (from e in mDb.Events
                            where e.name == newEvent.name
                            select e).FirstOrDefault();
                        //if it doesn't, add to the database
                        if (checkevent == null)
                        {
                            mDb.Events.InsertOnSubmit(newEvent);
                            mDb.SubmitChanges();
                        }
                        //check if we have reached the max num of events, if one was set
                        numCompleted++;
                        if (maxNum > 0 && maxNum == numCompleted)
                            return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Query a web service that returns an XML document
        /// </summary>
        /// <param name="requestUrl">The request to be sent</param>
        /// <returns>A XML document object containing the information requested</returns>
        private static XmlDocument GetXmlResponse(string requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                return (xmlDoc);
            }
            catch
            {
                return null;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ObjectModel;

namespace ImageServer.Controllers
{
    [RoutePrefix("v1/Album")]
    public class AlbumController : ApiController //Note: all controllers inherit from the ApiController class, a auto-generated class
    {
        /// <summary>
        /// A test rquest
        /// </summary>
        /// <returns>The number 1</returns>
        [Route("test/1")]
        [HttpGet]
        public int Test()
        {
            return 1;
        }

        /// <summary>
        /// Get all images in a specific album, optionally with rankings tailored to a certain user
        /// </summary>
        /// <param name="id">The id of the album</param>
        /// <param name="userID">The id of the use, optionalr</param>
        /// <returns>An AbumResult object, containing the requested album and all of its images</returns>
        [Route("ByID/{id}/{userID?}")]
        [HttpGet]
        public AlbumResult ByID(int id, int? userID = null)
        {
            return ResultGenerator.GenerateAlbumResult(id, userID);
        }
        
        /// <summary>
        /// Get images of a certain position from a specified album
        /// </summary>
        /// <param name="id">The id of the album</param>
        /// <param name="batchSize">The how many images to retrive</param>
        /// <param name="batchPos">The position to start getting them from</param>
        /// <param name="userId">The id of the user to tailor the results to, optional</param>
        /// <returns>An AlbumResult object, containing the requested album and the requested images</returns>
        [Route("BatchByID/{id}/{batchSize}/{batchPos}/{userID?}")]
        [HttpGet]
        public AlbumResult BatchByID(int id, int batchSize, int batchPos, int? userID = null)
        {
            return ResultGenerator.GenerateAlbumResultImgBatch(id, batchSize, batchPos, userID);
        }

        /// <summary>
        /// Get one of the users favourite albums
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <param name="batchPos">Get a album other than the most recent - optional</param>
        /// <returns>An AlbumResult</returns>
        [Route("GetFave/{userID}/{batchPos?}")]
        [HttpGet]
        public AlbumResult GetFavourite(int userID, int? startPos = null)
        {
            //return ResultGenerator.GenerateAlbumResult(null, userID, AlbumType.Favourite, startPos);
            return ResultGenerator.GenerateFaveAlbumResult(userID);
        }

        /// <summary>
        /// Get the most recent album by image
        /// </summary>
        /// <param name="userID">Tailor the result to the user - optional</param>
        /// <returns></returns>
        [Route("GetLatest/{userID?}/{pos?}")]
        [HttpGet]
        public AlbumResult GetLatest(int? userID = null, int? pos = 0)
        {
            return ResultGenerator.GenerateAlbumResult(null, userID, AlbumType.LatestUserAlbum, pos); 
        }

        [Route("GetLatestAlbum/{userID?}/{pos?}")]
        [HttpGet]
        public AlbumResult GetLatestAlbum(int? userID = null, int? pos = 0)
        {
            return ResultGenerator.GenerateAlbumResult(null, userID, AlbumType.LatestAlbum, pos);
        }

        /// <summary>
        /// Get a album by subject
        /// </summary>
        /// <param name="subject">The subject type</param>
        /// <param name="userID">The id of the user making the request</param>
        /// <returns></returns>
        [Route("GetBySubject/{subject}/{userID?}")]
        [HttpGet]
        public AlbumResult GetBySubject(AlbumSubject subject, int? userID = null)
        {
            return ResultGenerator.GenerateAlbumResult(null, userID, AlbumType.Subject, null, subject);
        }

        /// <summary>
        /// Gets a album recommend for the user
        /// </summary>
        /// <param name="userID">The id of the user to recommend to</param>
        /// <param name="startPos">Get a recommendation other than the first</param>
        /// <returns>A AlbumResult, containing the album recommended</returns>
        [Route("GetRecommended/{userID}/{pos?}")]
        [HttpGet]
        public AlbumResult GetRecommended(int userID, int? pos = null)
        {
            return ResultGenerator.GenerateAlbumResult(null, userID, AlbumType.Recommended);
        }

        /// <summary>
        /// Update an album's event id
        /// </summary>
        /// <param name="albumID">The id of the album to update</param>
        /// <param name="eventID">The new event id</param>
        /// <returns>A Album Result, with the album id if the update was successful</returns>
        [Route("UpdateAlbumEvent/{albumID}/{eventID}")]
        [HttpGet]
        public AlbumResult UpdateAlbumEvent(int albumID, int eventID)
        {
            return DataProcessor.UpdateAlbumEvent(albumID, eventID);
        }
        
        /// <summary>
        /// Get all albums that a user has contributed to
        /// </summary>
        /// <param name="userID">The ID of the user in question</param>
        /// <param name="incFriends">Include albums that the user's friends have contributed to</param>
        /// <param name="eventType">Only get albums of a certain event type, optional</param> //NOTE: this should probs be a enum
        /// <returns>An array of album objects</returns>
        [Route("userAlbums/{userId}/{incFriends}/{eventType?}")]
        [HttpGet]
        public AlbumResult[] UserAlbums(int userID, bool incFriends, int? eventType = null)
        {
            ImageDbDataContext mDb = new ImageDbDataContext();
            //find all images taken by the user
            var imageSet =
                 from n in mDb.Images
                 where n.userID == userID
                 select n;

            //if incFriends == true, find all albums taken by users friends

            List<Album> retAlbums = new List<Album>();
            foreach (Image myImg in imageSet)
            {
                //find the album that this image belongs to
                var albumSet =
                     (from n in mDb.Albums
                     where n.albumID == myImg.albumID
                     select n).FirstOrDefault();

                //only add this album if we haven't already found it
                if (!retAlbums.Contains(albumSet))
                    retAlbums.Add(albumSet);
            }

            List<AlbumResult> albumRes = new List<AlbumResult>();
            foreach (Album myAlbum in retAlbums)
            {
                //find the albums event and images
                var myEvent =
                    (from e in mDb.Events
                    where e.eventID == myAlbum.eventID
                    select e).FirstOrDefault();

                var albumImgs =
                    from n in mDb.Images
                    where n.albumID == myAlbum.albumID
                    select n;

                ImageResult[] myImgs;
                myImgs = ResultGenerator.GenerateImageResultArr(albumImgs.ToArray(), userID);

                AlbumResult myRes = new AlbumResult()
                {
                    Id = myAlbum.albumID.ToString(),
                    ArtistName = myEvent.artistName,
                    Date = myAlbum.startDate.ToString(),
                    EventVenue = myEvent.venueName,
                    Title = myEvent.name,
                    Images = myImgs,
                    //CoverImage = coverImg
                };

                albumRes.Add(myRes);
            }
            return albumRes.ToArray();
        }

        #region Unfinished/Not used methods

        /*
         *         /// <summary>
        /// Get all images in a specified album
        /// </summary>
        /// <param name="id">The id of the album</param>
        /// <returns>An AbumResult object, containing the requested album and all of its images</returns>
        public Objects.AlbumResult Get(int id)
        {
            ImageDbDataContext mDb = new ImageDbDataContext();
            mDb.ObjectTrackingEnabled = true;
            var albumSet =
                from m in mDb.Albums
                where m.albumID == id
                select m;

            var imageSet =
                from n in mDb.Images
                where n.albumID == id
                select n;

            Album myAlbum = albumSet.FirstOrDefault();

            var eventSet =
                 from e in mDb.Events
                 where e.eventID == myAlbum.eventID
                 select e;

            Objects.AlbumResult myAlbumRes = new Objects.AlbumResult() { Album = myAlbum, Images = imageSet.ToArray(), Event = eventSet.FirstOrDefault() };
            return myAlbumRes;

        }
        /// <summary>
        /// Get all albums from a specified location. Must provide a country code, but other paramters may be null
        /// </summary>
        /// <param name="country">The country of the location</param>
        /// <param name="street">The street of the location</param>
        /// <param name="city">The city of the location</param>
        /// <param name="town">The street of the location</param>
        /// <param name="postcode">The postcode of the location</param>
        /// <param name="eventType">Optionally, only get albums of a certain event type</param>
        /// <returns></returns>
        public Objects.Album[] Get(string country, string? street, string? city, string? town, string? postcode, int? eventType)
        {
            //note: any of these paramaters can be null, but at least one must be given
            List<Objects.Album> albums = new List<Objects.Album>();
            albums.Add(new Objects.Album { Id = 23, Name = "Test album 1" });
            albums.Add(new Objects.Album { Id = 23, Name = "Test album 2" });
            return albums.ToArray();
        }
        

        /// <summary>
        /// Get all albums from a specified location by Geolocation
        /// </summary>
        /// <param name="lat">Latitude of the center point</param>
        /// <param name="lng">Longitude of the center point</param>
        /// <param name="radius">Area from in which albums should be found</param>
        /// <param name="eventType">Optionally, only get albums of a certain event type</param>
        /// <returns>A array containing all album objects from the specified area</returns>
        public Objects.Album[] Get(string lat, string lng, double radius, int? eventType)
        {
            List<Objects.Album> albums = new List<Objects.Album>();
            albums.Add(new Objects.Album { Id = 23, Name="Test album 1" });
            albums.Add(new Objects.Album { Id = 23, Name = "Test album 2" });
            return albums.ToArray();
        }

        /// <summary>
        /// Get all albums from a specified event type
        /// </summary>
        /// <param name="eventType">The type of event</param>
        /// <returns>A array containing all album objects from the specified event type</returns>
        public Objects.Album[] Event(int eventType)
        {
            List<Objects.Album> albums = new List<Objects.Album>();
            albums.Add(new Objects.Album { Id = 23, Name = "Test album 1" });
            albums.Add(new Objects.Album { Id = 23, Name = "Test album 2" });
            return albums.ToArray();
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
         * */

        #endregion
    }
}
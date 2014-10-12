using GeoCoding;
using GeoCoding.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;
using Amazon.S3;
using Amazon.S3.Model;
using ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace ImageServer.Controllers
{
    [RoutePrefix("v1/Image")]
    public class ImageController : ApiController
    {
        /// <summary>
        /// Get a specific image
        /// </summary>
        /// <param name="id">The id of the image</param>
        /// <returns>A image object</returns>
        [Route("GetImage/{url}/{type}")]
        [HttpGet]
        public HttpResponseMessage GetImage(string url, string type)
        {
            byte[] bytes = DataProcessor.GetImageBytes(url, type);
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            if (bytes != null)
            {
                httpResponseMessage.Content = new ByteArrayContent(bytes);
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            }
            else
            {
                httpResponseMessage.Content = null;
                httpResponseMessage.StatusCode = HttpStatusCode.NotFound;
            }
            return httpResponseMessage;
        }

        //async loading: http://stackoverflow.com/a/20909515
        /// <summary>
        /// Add a new image to the database and S3
        /// </summary>
        /// <param name="userID">The id of the user who uploaded the image</param>
        /// <param name="cameraDetails">The details of the camera that took it</param>
        /// <param name="lat">Lat coordinate where it was taken</param>
        /// <param name="lng">Lng coordinate where it was taken</param>
        /// <returns>A ImageResult, contain the new images details</returns>
        [Route("NewImage/{userID}/{cameraDetails}/{lat}/{lng}")]
        [HttpPost]
        public async Task<ImageResult> NewImage([FromUri]int userID, [FromUri]string cameraDetails, [FromUri]string lat, [FromUri]string lng)
        {
            //wait for the whole of the request to be read in before moving on
            byte[] rawData = await Request.Content.ReadAsByteArrayAsync();
            return DataProcessor.AddNewImage(new ImageRequest(){ 
                UserID = userID, CameraDetails = cameraDetails, LatLng = Util.DecapsulateCoordinate(lat) + "," + Util.DecapsulateCoordinate(lng), RawData = rawData } );
        }

        /// <summary>
        /// Increment the flag count on a image by 1
        /// </summary>
        /// <param name="urlId">The url_id of the image to increase the count of</param>
        /// <param name="userID">The id of the user adding the flag</param>
        /// <returns>a ImageResult, containing the images details or any errors that occured</returns>
        [Route("FlagImage/{id}/{userID}")]
        [HttpGet]
        public ImageResult FlagImage(string id, int userID)
        {
            return DataProcessor.UpdateImageFlags(id, userID, 1);
        }

        /// <summary>
        /// Remove a flag from a image
        /// </summary>
        /// <param name="id">The id of the image to remove the flag from</param>
        /// <param name="userID">The id of the user removing the flag</param>
        /// <returns>A ImageResult</returns>
        [Route("RemoveFlag/{id}/{userID}")]
        [HttpGet]
        public ImageResult RemoveFlag(string id, int userID)
        {
            return DataProcessor.UpdateImageFlags(id, userID, -1);
        }

        /// <summary>
        /// Update an images user rating
        /// </summary>
        /// <param name="urlId">The url_id of the image to rate</param>
        /// <param name="increase">Whether to increase(true) or decrease(false) the rank</param>
        /// <param name="userID">The id of the user changing the rating</param>
        /// <returns>A Image result with the new rating</returns>
        [Route("UpdateRating/{id}/{userID}/{increase}")]
        [HttpGet]
        public ImageResult UpdateRating(string id, int userID, bool increase)
        {
            return DataProcessor.UpdateRating(id, userID, increase);
        }

        /// <summary>
        /// Add a image to the list of the user's favourites
        /// </summary>
        /// <param name="urlId">The url_id of the image</param>
        /// <param name="userID">The id of the user</param>
        /// <returns>A ImageResult</returns>
        [Route("AddFave/{id}/{userID}")]
        [HttpGet]
        public ImageResult AddFave(string id, int userID)
        {
            return DataProcessor.AddFaveImage(id, userID);
        }

        /// <summary>
        /// Remove a image from the user's favourites
        /// </summary>
        /// <param name="id">The id of the image to remove</param>
        /// <param name="userID">The id of the user</param>
        /// <returns>A ImageResult</returns>
        [Route("RemoveFave/{id}/{userID}")]
        [HttpGet]
        public ImageResult RemoveFave(string id, int userID)
        {
            return DataProcessor.RemoveFaveImage(id, userID);
        }

        [Route("SaveRank/{id}/{rank}")]
        [HttpGet]
        public ImageResult SaveRank(string id, int rank)
        {
            return DataProcessor.UpdateRanking(id, rank);
        }

        #region Unfinished/Not used methods
        /*
         * 
         *
         *         /// <summary>
        /// Add a new image to the database
        /// </summary>
        /// <param name="userID">The id of the user who took the image</param>
        /// <param name="cameraDetails">Details about the device used to take it</param>
        /// <param name="latlng">Where the image was taken</param>
        /// <param name="rawData">The raw data of the image, as a base 64 string</param>
        /// <returns>Server response, 201 if successful, 500 if not</returns>
        /*[Route("NewImage")]
        [HttpPost]
        public ImageResult NewImage([FromBody]byte[] imgData)
        {
            return DataProcessor.AddNewImage(imgData);
        }
         * /// <summary>
        /// Add a new image to the database
        /// </summary>
        /// <param name="userID">The id of the user who took the image</param>
        /// <param name="cameraDetails">Details about the device used to take it</param>
        /// <param name="latlng">Where the image was taken</param>
        /// <param name="rawData">The raw data of the image, as a base 64 string</param>
        /// <returns>Server response, 201 if successful, 500 if not</returns>
        [Route("Post/{userID}/{cameraDetails}/{lat}/{lng}")]
        [HttpPost]
        public ImageResult Post([FromUri]int userID, [FromUri]string cameraDetails, [FromUri]string lat, [FromUri]string lng, [FromBody]string rawData)
        {
            DateTime taken = DateTime.Now;
            try
            {
                
                var geoRequestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false", lat, lng);

                var geoRequest = WebRequest.Create(geoRequestUri);
                var geoResponse = geoRequest.GetResponse();
                var geoXDoc = XDocument.Load(geoResponse.GetResponseStream());

                var geoResult = geoXDoc.Element("GeocodeResponse").Element("result");
                var imgAddress = geoResult.Element("formatted_address");
                var imgPostcode = geoResult.Element("address_components").Descendants("postcode"); //not sure if this is right

                int? albumID = null;
                //Check if there is an event happening at this time and place
                ImageDbDataContext mDb = new ImageDbDataContext();
                var eventSet =
                    from n in mDb.Events
                    where n.address == imgAddress.ToString()
                    select n;
                Event myEvent = eventSet.FirstOrDefault();

                //If no event was found, check if a album exists at this time and place
                Album myAlbum;
                if (myEvent == null)
                {
                    var albumSet =
                        from n in mDb.Albums
                        where n.address == imgAddress.ToString()
                        select n;
                    myAlbum = albumSet.FirstOrDefault();

                    //if no album was found, create a new one
                    if (myAlbum == null)
                    {
                        Album newAlbum = new Album()
                        {
                            startDate = taken,
                            endDate = taken.AddHours(4),
                            address = imgAddress.ToString(),
                            latlng = lat + "," + lng,
                            postcode = imgPostcode.ToString()
                        };
                        mDb.Albums.InsertOnSubmit(newAlbum);
                        mDb.SubmitChanges();
                        //get the id of the album just added - might be a easy and/or more efficent way of doing this
                        Album justAdded = mDb.Albums.Last();
                        albumID = justAdded.albumID;
                    }
                    else
                        albumID = myAlbum.albumID;
                }
                //if an event was found, find the album it relates to
                else
                {
                    var albumSet =
                        from n in mDb.Albums
                        where n.eventID == myEvent.eventID
                        select n;
                    myAlbum = albumSet.FirstOrDefault();

                    if (myAlbum == null)
                    {
                        //if no album was found for this event, create a new one
                        Album newAlbum = new Album()
                        {
                            eventID = myEvent.eventID,
                            startDate = myEvent.startDate,
                            endDate = myEvent.endDate
                        };
                        mDb.Albums.InsertOnSubmit(newAlbum);
                        mDb.SubmitChanges();
                        //get the id of the album just added - might be a easy and/or more efficent way of doing this
                        Album justAdded = mDb.Albums.Last();
                        albumID = justAdded.albumID;
                    }
                    else
                        albumID = myAlbum.albumID;
                }

                ImageResult result = new ImageResult();
                //create new image object
                Image newImg = new Image()
                {
                    userID = userID,
                    taken = taken,
                    cameraDetails = cameraDetails,
                    latlng = lat + "," + lng,
                    albumID = albumID,
                    imgUrl = new Guid().ToString()
                };
                //add the raw image data to the bucket on S3 - need to add keys to app settings
                AmazonS3Client s3Client = new AmazonS3Client();
                PutObjectRequest s3Request = new PutObjectRequest()
                {
                    BucketName = "communalalbumsimages",
                    Key = newImg.imgUrl,
                    ContentBody = rawData
                };
                s3Client.PutObject(s3Request);

                //add the url to raw data on S3 and save
                mDb.Images.InsertOnSubmit(newImg);
                mDb.SubmitChanges();
                return new ImageResult() { Response = HttpStatusCode.Created.ToString() };
            }
            catch
            {
                return new ImageResult()
                {
                    Response = HttpStatusCode.InternalServerError.ToString(),
                    ErrorMsg = Util.GenericError
                };

            }
        }
         * 
        /// <summary>
        /// Get a set of images from a specific album
        /// </summary>
        /// <param name="albumID">ID of the album you want images from</param>
        /// <param name="batchSize">The number of images request</param>
        /// <param name="batchNum">The position of the images within the album</param>
        /// <returns>An array of image objects</returns>
        public Objects.Image[] Get(int albumID, int batchSize, int batchNum)
        {
            List<Objects.Image> images = new List<Objects.Image>();
            images.Add(new Objects.Image { Id = 23, Rank = 0, UserID = 6 });
            return images.ToArray();
        }

        /// <summary>
        /// Add a new image to the database
        /// </summary>
        /// <param name="imgData">The raw data for the image</param>
        /// <param name="userID">The ID of the user uploading the image</param>
        /// <param name="latlng">Coordinates that the image was taken at</param>
        public void Post(byte[] imgData, int userID, string latlng)
        {
            //note: datetime to be taken from server, taking into account user's time zone
            //note: need to resolve lnglat into a address
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
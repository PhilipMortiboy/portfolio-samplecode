using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using ObjectModel;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using Amazon;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace ImageServer
{
    public class DataProcessor
    {
        /// <summary>
        /// Add a new user to the database
        /// </summary>
        /// <param name="userData">The user's details</param>
        /// <returns>A UserResult</returns>
        public static UserResult AddNewUser(UserRequest userData)
        {
            try
            {
                Validator validation = new Validator(userData, true);
                if (validation.Error)
                    return new UserResult() { Response = HttpStatusCode.InternalServerError.ToString(), ErrorMsg = validation.ErrorMsg };
                else
                {
                    User newUser = DbQuery.AddUser(userData); //need to check you can do this
                    return ResultGenerator.GenerateUserResult(newUser);
                }
            }
            catch(Exception e)
            {
                return new UserResult() { Response = HttpStatusCode.InternalServerError.ToString(), ErrorMsg = e.Message };
            }
        }

        /// <summary>
        /// Add a new user to the database
        /// </summary>
        /// <param name="userData">The user's details, as a byte array</param>
        /// <returns>A UserResult</returns>
        public static UserResult AddNewUser(byte[] userData)
        {
            try
            {
                UserRequest req = Util.ByteArrayToObject<UserRequest>(userData);
                return AddNewUser(req);
            }
            catch (Exception e)
            {
                return new UserResult() { Response = HttpStatusCode.InternalServerError.ToString(), ErrorMsg = e.Message };
            }
        }

        /// <summary>
        /// Check a user's login details
        /// </summary>
        /// <param name="userData">A byte array containing the username and password entered</param>
        /// <returns>A UserResult, with errors if the login failed</returns>
        public static UserResult UserLogin(byte[] userData)
        {
            try
            {
                UserRequest req = Util.ByteArrayToObject<UserRequest>(userData);
                Validator validation = new Validator(req, false);
                if (validation.Error)
                    return new UserResult() { Response = HttpStatusCode.InternalServerError.ToString(), ErrorMsg = validation.ErrorMsg };
                else
                {
                    User user = DbQuery.GetSingleUser(req.Username, req.Password);
                    //if no user was found with this username/password combo, return an error
                    if (user == null)
                        return new UserResult() { Error = true, ErrorMsg = "The username and/or password you entered are incorrect. Please try again", Response = HttpStatusCode.NotAcceptable.ToString() };
                    else
                        return ResultGenerator.GenerateUserResult(user);
                }
            }
            catch (Exception e)
            {
                return new UserResult() { Response = HttpStatusCode.InternalServerError.ToString(), ErrorMsg = e.Message };
            }
        }

        /// <summary>
        /// Add a new image to the database and S3
        /// </summary>
        /// <param name="imageData">The image to add</param>
        /// <returns>An ImageResult</returns>
        public static ImageResult AddNewImage(ImageRequest imageData)
        {
            try
            {
                //add the server time to the image's data
                DateTime serverTime = DateTime.Now;
                imageData.Taken = serverTime;

                Validator validation = new Validator(imageData);
                if (validation.Error)
                {
                    return new ImageResult() { Response = HttpStatusCode.InternalServerError.ToString(), ErrorMsg = validation.ErrorMsg };
                }
                else
                {
                    //Geocode the images coordinates
                    string postcode = Util.GetPostcode(imageData.LatLng);
                    //find or create the album for this image, based off its location
                    Album myAlbum;
                    Event[] myEvent = DbQuery.GetEventByLocation(postcode);
                    if (myEvent.Length == 0)
                        //if no event for this location exists, check if a album does
                        myAlbum = DbQuery.GetSingleAlbumByLocation(postcode, imageData.LatLng);
                    else
                    {
                        //if one does, find its album or create one for it
                        myAlbum = DbQuery.GetSingleAlbumByEvent(myEvent.FirstOrDefault());
                        myAlbum.subject = myEvent.FirstOrDefault().eventType;
                    }
                    if (myAlbum == null)
                        //if no album was found, create a new one
                        myAlbum = DbQuery.AddAlbum(postcode, imageData.LatLng);

                    //add the id of the create/found album to the request data
                    imageData.AlbumID = myAlbum.albumID;
                    imageData.Rank = 2; //default value. 
                    //add query to imageraterserver

                    //add the image itself to S3
                    imageData.Url = Guid.NewGuid().ToString(); //assign this image a unique url
                    SaveImageToS3(imageData.RawData, imageData.Url);
                    //add metadata to sql database
                    Image newImg = DbQuery.AddImage(imageData);

                    ImageResult res = ResultGenerator.GenerateImageResult(newImg);
                    if (myEvent.Length > 1)
                        res.EventOptions = ResultGenerator.GenerateEventResultArr(myEvent);
                    return res;
                }
            }
            catch(Exception e)
            {
                return new ImageResult() { Response = HttpStatusCode.InternalServerError.ToString(), ErrorMsg = "source: " + e.Source + ", Stack Trace: " + e.StackTrace + ", Message: " + e.Message + ", Inner Exception" + e.InnerException };
            }
        }

        /// <summary>
        /// Add a new image to the database and S3
        /// </summary>
        /// <param name="imageData">The image to add, as a byte array</param>
        /// <returns>A ImageResult</returns>
        public static ImageResult AddNewImage(byte[] imageData)
        {
            try
            {
                ImageRequest req = Util.ByteArrayToObject<ImageRequest>(imageData);
                return AddNewImage(imageData);
            }
            catch (Exception e)
            {
                return new ImageResult() { Response = HttpStatusCode.InternalServerError.ToString(), ErrorMsg = e.Message };
            }
        }

        /// <summary>
        /// Update a image's flag number
        /// </summary>
        /// <param name="imgID">The id of the image to update</param>
        /// <param name="flag">The amount to increase or decrease the flag count by</param>
        /// <returns>A ImageResult</returns>
        public static ImageResult UpdateImageFlags(string imgID, int userID, int flag)
        {
            try
            {
                Image myImg = DbQuery.GetSingleImage(imgID);
                int? currentFlags = myImg.flags;
                //if flags are null, set them to 0
                if (currentFlags == null)
                    currentFlags = 0;
                //modify the flag by the given value
                currentFlags = currentFlags + flag;
                myImg.flags = currentFlags;
                Image updateImg = DbQuery.UpdateImage(myImg);
                //save a record of this user adding a flag to this image, or if they are decreasing it remove the record
                if(flag > 0)
                {
                    UserImageFlag recordFlag = DbQuery.AddFlagRecord(myImg.imageID, userID);
                }
                else
                    DbQuery.DeleteFlagRecord(myImg.imageID, userID);
                return ResultGenerator.GenerateImageResult(updateImg, userID);
            }
            catch (Exception e)
            {
                return new ImageResult() { Response = HttpStatusCode.InternalServerError.ToString(), ErrorMsg = e.Message };
            }
        }

        /// <summary>
        /// Update's a images rating
        /// </summary>
        /// <param name="imgID">The id of the image</param>
        /// <param name="increase">The number to increase the rating by</param>
        /// <returns>A ImageResult, with the updated rating</returns>
        public static ImageResult UpdateRating(string imgID, int userID, bool increase)
        {
            try
            {
                Image myImg = DbQuery.GetSingleImage(imgID);
                if (myImg.rating == null)
                    myImg.rating = 0;
                //increase or decrease the image rating, but only if its not 0
                if (increase)
                    myImg.rating = myImg.rating++;
                else if(!increase && myImg.rating == 0)
                    myImg.rating = myImg.rating--;
                Image updateImg = DbQuery.UpdateImage(myImg);
                UserImageRating ratingRecord = DbQuery.AddRatingRecord(myImg.imageID, userID, increase);
                return ResultGenerator.GenerateImageResult(updateImg, userID);
            }
            catch (Exception e)
            {
                return new ImageResult() { Response = HttpStatusCode.InternalServerError.ToString(), Error = true, ErrorMsg = e.Message };
            }
        }

        /// <summary>
        /// Add this image to the user's list of favourites
        /// </summary>
        /// <param name="imgUrl">The image to mark as favourite</param>
        /// <param name="userID">The id of the user</param>
        /// <returns>A ImageResult</returns>
        public static ImageResult AddFaveImage(string imgUrl, int userID)
        {
            try
            {
                Image myImg = DbQuery.GetSingleImage(imgUrl);
                FaveImage res = DbQuery.AddFaveImageRecord(myImg, userID);
                return ResultGenerator.GenerateImageResult(myImg, userID);
            }
            catch (Exception e)
            {
                return new ImageResult() { Response = HttpStatusCode.InternalServerError.ToString(), Error = true, ErrorMsg = e.Message };
            }

        }

        /// <summary>
        /// Remove a image from the user's favourites
        /// </summary>
        /// <param name="imgUrl">The id of the image to remove</param>
        /// <param name="userID">The id of the user</param>
        /// <returns>A ImageResult, contain the image id if successful</returns>
        public static ImageResult RemoveFaveImage(string imgUrl, int userID)
        {
            try
            {
                Image myImg = DbQuery.GetSingleImage(imgUrl);
                DbQuery.DeleteFaveImageRecord(myImg, userID);
                return ResultGenerator.GenerateImageResult(myImg);
            }
            catch(Exception e)
            {
                return new ImageResult() { Response = HttpStatusCode.InternalServerError.ToString(), Error = true, ErrorMsg = e.Message };
            }
        }

        /// <summary>
        /// Updates a album's event id
        /// </summary>
        /// <param name="albumID">The id of the album to update</param>
        /// <param name="eventID">The new event id</param>
        /// <returns>A AlbumResult, containing the album's id if the update was successful</returns>
        public static AlbumResult UpdateAlbumEvent(int albumID, int eventID)
        {
            try
            {
                Album myAlbum = DbQuery.GetSingleAlbum(albumID);
                myAlbum.eventID = eventID;
                Album updateAlbum = DbQuery.UpdateAlbum(myAlbum);
                return new AlbumResult()
                {
                    Id = myAlbum.albumID.ToString()
                };
            }
            catch (Exception e)
            {
                return new AlbumResult() { Response = HttpStatusCode.InternalServerError.ToString(), Error = true, ErrorMsg = e.Message };
            }
        }

        /// <summary>
        /// Update a image's ranking
        /// </summary>
        /// <param name="imgID">The id fo the image to update</param>
        /// <param name="ranking">The amount to change the ranking by</param>
        /// <returns>A AlbumResult, containing the album's id if successful</returns>
        public static ImageResult UpdateRanking(string imgID, int ranking)
        {
            try
            {
                Image myImg = DbQuery.GetSingleImage(imgID);
                myImg.baseRank = ranking;
                Image updateImg = DbQuery.UpdateImage(myImg);
                return ResultGenerator.GenerateImageResult(updateImg);
            }
            catch (Exception e)
            {
                return new ImageResult() { Response = HttpStatusCode.InternalServerError.ToString(), Error = true, ErrorMsg = e.Message };
            }
        }

        /// <summary>
        /// Get a images raw data from S3
        /// </summary>
        /// <param name="url">The url of the image to retrive</param>
        /// <returns>A byte array containing the raw data</returns>
        public static byte[] GetImageBytes(string url, string type)
        {
            //get the image's data from S3
	        AmazonS3Client client = new AmazonS3Client(RegionEndpoint.EUWest1);
	        var request = new GetObjectRequest
	        {
                BucketName = "communalalbumsimages",
		        Key = url + "/" + type + ".jpg"
	        };
	        try
	        {
		        var response = client.GetObject(request);
		        var contentLength = (int)response.ContentLength;
		        if (contentLength == 0)
			        return null;
		        byte[] bytes = new byte[response.ContentLength];
		        int bytesRead = 0;
		        //Read in the saved bytes. Have to force it to continue reading with the while loop because
                //it seems to stop before the end of the stream otherwise
		        while (bytesRead < contentLength)
			        bytesRead += response.ResponseStream.Read(bytes, bytesRead, contentLength-bytesRead);
		        response.ResponseStream.Close();
                return bytes;
	        }
	        catch
	        {
		        return null;
	        }
        }

        private static void SaveImageToS3(byte[] rawData, string id)
        {
            //create a new bitmap image from the raw jpeg data
            /*Stream stream = new MemoryStream(rawData);
            JpegBitmapDecoder decoder = new JpegBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapSource bitmapSource = decoder.Frames[0];*/
            //Bitmap baseImage = SourceToBitmap(bitmapSource);
            ImageConverter ic = new ImageConverter();
            System.Drawing.Image img = (System.Drawing.Image)ic.ConvertFrom(rawData);
            Bitmap baseImage = new Bitmap(img);
            for (int i = 0; i < 3; i++)
            {
                string key = "";
                switch (i)
                {
                    case 1:
                        key = id + "/low.jpg";
                        Bitmap standard = CreateLowQualBitmap(baseImage);
                        baseImage = standard;
                        break;
                    case 2:
                        key = id + "/thumbnail.jpg";
                        Bitmap thumbnail = CreateThumbnailBitmap(baseImage);
                        baseImage = thumbnail;
                        break;
                    default:
                        key = id + "/full.jpg";
                        break;
                }
                AmazonS3Client client = new AmazonS3Client(Amazon.RegionEndpoint.EUWest1);
                using (var memoryStream = new MemoryStream())
                {
                    baseImage.Save(memoryStream, ImageFormat.Png);
                    PutObjectRequest titledRequest = new PutObjectRequest
                    {
                        BucketName = "communalalbumsimages",
                        Key = key,
                        InputStream = memoryStream
                    };
                    client.PutObject(titledRequest);
                }
            }
        }

        private static Bitmap CreateLowQualBitmap(Bitmap baseimg)
        {
            var newWidth = (int)(baseimg.Width * 0.5);
            var newHeight = (int)(baseimg.Height * 0.5);
            System.Drawing.Image convImg = baseimg.GetThumbnailImage(newWidth, newHeight, () => false, IntPtr.Zero);
            return new Bitmap(convImg);
        }

        private static Bitmap CreateThumbnailBitmap(Bitmap baseimg)
        {
            var newWidth = (int)(baseimg.Width * 0.3);
            var newHeight = (int)(baseimg.Height * 0.3);
            System.Drawing.Image convImg = baseimg.GetThumbnailImage(newWidth, newHeight, () => false, IntPtr.Zero);
            return new Bitmap(convImg);
        }

        //code from: http://stackoverflow.com/a/21931808
        private static Bitmap SourceToBitmap(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        #region Old Code
        //OLD WAY: by converting to base64
        /*string dataStr = System.Convert.ToBase64String(imageData.RawData, 0, imageData.RawData.Length);
        AmazonS3Client s3Client = new AmazonS3Client(Amazon.RegionEndpoint.EUWest1);
        PutObjectRequest s3Request = new PutObjectRequest()
        {
            BucketName = "communalalbumsimages",
            Key = imageData.Url,
            ContentBody = dataStr
        };
        s3Client.PutObject(s3Request);

        //add metadata to sql database
        Image newImg = DbQuery.AddImage(imageData);

        return new ImageResult()
        {
            Id = newImg.imageID.ToString(),
            Url = newImg.imgUrl,
            UserID = newImg.userID.ToString(),
            //other properties
            Response = HttpStatusCode.Found.ToString()
        };*/
        #endregion
    }
}
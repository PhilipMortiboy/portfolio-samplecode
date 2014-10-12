using ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace ImageServer
{
    public class ResultGenerator
    {
        /// <summary>
        /// Query the database to get a AlbumResult
        /// </summary>
        /// <param name="albumID">The id of the album</param>
        /// <param name="userID">The id of the user</param>
        /// <param name="type">Only get albums of a specific type</param>
        /// <param name="startPos">Get a album other than the first that fufils the search criteria</param>
        /// <returns>An AlbumResult</returns>
        public static AlbumResult GenerateAlbumResult(int? albumID, int? userID = null, AlbumType? type = null, int? startPos = null, AlbumSubject? subject = null)
        {
            try
            {
                Album myAlbum;
                if (type == null)
                    myAlbum = DbQuery.GetSingleAlbum(albumID, startPos);
                else
                    myAlbum = DbQuery.GetSingleAlbumByType(type, startPos, userID, subject);

                //use the album id found on these queries, since albumID may be null, (eg if this method is called as part of a tile request)
                Image[] imageSet = DbQuery.GetImageBatch(myAlbum.albumID); //probably unecessary since the album object has images
                //Image[] imageSet = myAlbum.Images.ToArray(); //see if this works
                Event myEvent = DbQuery.GetSingleEvent(myAlbum.eventID);

                ImageResult[] myImgs;
                if (userID != null)
                    myImgs = TailorImages(imageSet, userID);
                else
                    myImgs = GenerateImageResultArr(imageSet);

                return new AlbumResult()
                {
                    Id = myAlbum.albumID.ToString(),
                    ArtistName = myEvent.artistName,
                    Date = myAlbum.startDate.ToString(),
                    EventVenue = myEvent.venueName,
                    Title = myEvent.name,
                    Images = myImgs,
                    //CoverImage = coverImg
                    Response = HttpStatusCode.Found.ToString()
                };
            }
            catch
            {
                return new AlbumResult() { Response = HttpStatusCode.InternalServerError.ToString(), Error = true, ErrorMsg = Util.GenericError };
            }
        }

        /// <summary>
        /// Get a album with certain images
        /// </summary>
        /// <param name="id">The id of the album</param>
        /// <param name="batchSize">The amount of images to return</param>
        /// <param name="batchPos">Where to start getting the images from</param>
        /// <param name="userID">Tailor the results to a certain user - optional</param>
        /// <returns>An AlbumResult</returns>
        public static AlbumResult GenerateAlbumResultImgBatch(int albumID, int batchSize, int batchPos, int? userID = null)
        {
            try
            {
                Album myAlbum = DbQuery.GetSingleAlbum(albumID);
                Image[] imageSet = DbQuery.GetImageBatch(albumID, batchPos, batchSize);
                Event myEvent = DbQuery.GetSingleEvent(myAlbum.eventID);

                ImageResult[] myImgs;
                if (userID != null)
                    myImgs = TailorImages(imageSet, userID);
                else
                    myImgs = GenerateImageResultArr(imageSet);

                return new ObjectModel.AlbumResult()
                {
                    Id = myAlbum.albumID.ToString(),
                    ArtistName = myEvent.artistName,
                    Date = myAlbum.startDate.ToString(),
                    EventVenue = myEvent.venueName,
                    Title = myEvent.name,
                    Images = myImgs,
                    //CoverImage = coverImg
                    Response = HttpStatusCode.Found.ToString()
                };
            }
            catch
            {
                return new AlbumResult() { Response = HttpStatusCode.Created.ToString(), ErrorMsg = Util.GenericError };
            }
        }

        /// <summary>
        /// Generate a custom album with the user's favourite images
        /// </summary>
        /// <param name="userID">The id of the user</param>
        /// <param name="limit">The maximum number of images in the album</param>
        /// <returns></returns>
        public static AlbumResult GenerateFaveAlbumResult(int userID, int? limit = 50)
        {
            try
            {
                Image[] faveImgs = DbQuery.GetFaveImages(userID, limit);
                ImageResult[] imgRes = GenerateImageResultArr(faveImgs);
                AlbumResult res = new AlbumResult()
                {
                    Title = "My Favourites",
                    Images = imgRes,
                    Response = HttpStatusCode.Found.ToString(),
                };
                return res;
            }
            catch
            {
                return new AlbumResult() { Response = HttpStatusCode.InternalServerError.ToString(), Error = true, ErrorMsg = Util.GenericError };
            }
        }

        /// <summary>
        /// Create a image result from a Image object
        /// </summary>
        /// <param name="myImg">The image to convert</param>
        /// <returns>A ImageResult</returns>
        public static ImageResult GenerateImageResult(Image myImg)
        {
            return GenerateImageResult(myImg, 0);
        }

        /// <summary>
        /// Create a ImageResult object from a Image object
        /// </summary>
        /// <param name="myImg">The Image to convert</param>
        /// <param name="userID">The id of the user to customise the images to</param>
        /// <returns>A ImageResult</returns>
        public static ImageResult GenerateImageResult(Image myImg, int userID)
        {
            try
            {
                if (myImg.assignedRank == null)
                    myImg.assignedRank = 0;
                return new ImageResult()
                    {
                        Id = myImg.imageID.ToString(),
                        Rank = (int)myImg.assignedRank,
                        UserID = myImg.userID.ToString(),
                        Url = myImg.imgUrl,
                        Flags = myImg.flags.ToString(),
                        Rating = myImg.rating.ToString(),
                        UserFave = DbQuery.CheckImageIsFave(myImg.imageID, userID),
                        UserFlagged = DbQuery.CheckImageIsFlagged(myImg.imageID, userID),
                        UserRating = DbQuery.CheckImageUserRating(myImg.imageID, userID),
                        Response = HttpStatusCode.Found.ToString(),
                    };
            }
            catch
            {
                return new ImageResult() { Response = HttpStatusCode.InternalServerError.ToString(), ErrorMsg = Util.GenericError };
            }
        }

        /// <summary>
        /// Create a ImageResult array from a array of Images
        /// </summary>
        /// <param name="myImg">The Image array to convert</param>
        /// <returns>The Image Result</returns>
        public static ImageResult[] GenerateImageResultArr(Image[] myImgs)
        {
            return GenerateImageResultArr(myImgs, 0);
        }

        /// <summary>
        /// Create a ImageResult array from a array of Images
        /// </summary>
        /// <param name="myImg">The Image array to convert</param>
        /// <param name="userID">The id of the user to customise the images to</param>
        /// <returns>The Image Result</returns>
        public static ImageResult[] GenerateImageResultArr(Image[] myImgs, int userID)
        {
            try
            {
                List<ImageResult> myRes = new List<ImageResult>();
                foreach (Image myImg in myImgs)
                    myRes.Add(GenerateImageResult(myImg, userID));
                //sort the image result by rank
                return myRes.OrderByDescending(x => x.Rank).ToArray();
            }
            catch
            {
                return new ImageResult[] { new ImageResult() { Response = HttpStatusCode.InternalServerError.ToString(), ErrorMsg = Util.GenericError } };
            }
        }

        public static EventResult[] GenerateEventResultArr(Event[] myEvents)
        {
            try
            {
                List<EventResult> myRes = new List<EventResult>();
                foreach (Event myEvent in myEvents)
                    myRes.Add(GenerateEventResult(myEvent));
                return myRes.ToArray();
            }
            catch
            {
                return new EventResult[] { new EventResult() { Response = HttpStatusCode.InternalServerError.ToString(), ErrorMsg = Util.GenericError } };
            }
        }

        public static EventResult GenerateEventResult(Event myEvent)
        {
            try
            {
                return new EventResult()
                {
                    Id = myEvent.eventID.ToString(),
                    Date = myEvent.startDate.ToString(),
                    Title = myEvent.name,
                    VenueName = myEvent.venueName,
                    Response = HttpStatusCode.Found.ToString()
                };
            }
            catch
            {
                return new EventResult() { Response = HttpStatusCode.InternalServerError.ToString(), ErrorMsg = Util.GenericError };
            }
        }

        /// <summary>
        /// Tailor the rankings images in a set to a specific user
        /// </summary>
        /// <param name="imageSet">The image set to be ranked</param>
        /// <param name="userID">The user's id</param>
        /// <returns>An array of Image</returns>
        public static ImageResult[] TailorImages(Image[] imageSet, int? userID)
        {
            try
            {
                double aveRating = CalcAverageRating(imageSet);
                foreach (Image myImg in imageSet)
                {
                    myImg.assignedRank = 2; //default rank
                    //if image has a base rank, use this instead of the default
                    if (myImg.baseRank != null)
                        myImg.assignedRank = (int)myImg.baseRank;

                    //if the image belongs to the user, increase rank by 2 from the base
                    if (myImg.userID == userID)
                        myImg.assignedRank = myImg.assignedRank + 2;

                    //if the image's rating is more than 50% better than average, increase ranking
                    if (((double)myImg.rating / aveRating) > 1.5)
                        myImg.assignedRank = myImg.assignedRank + 1;

                    //if the image's rating is more than average, decrease ranking
                    if (((double)myImg.rating / aveRating) < 0.5)
                        myImg.assignedRank = myImg.assignedRank - 1;

                    //if user has increased this image's ranking, increase rank by 2
                    if (DbQuery.CheckImageUserRating((int)myImg.imageID, (int)userID) == true)
                        myImg.assignedRank = myImg.assignedRank + 2;

                    //if user has decreased this image's ranking, decrease by 2
                    if (DbQuery.CheckImageUserRating((int)myImg.imageID, (int)userID) == false)
                        myImg.assignedRank = myImg.assignedRank - 2;

                    //if recieved 3 or 4 flags, decrease rank by 1
                    if (myImg.flags == 3 || myImg.flags == 4)
                        myImg.assignedRank = myImg.assignedRank - 1;

                    //if user has flagged this image, discard
                    if (DbQuery.CheckFlagRecord((int)myImg.imageID, (int)userID))
                        myImg.assignedRank = 0;

                    //if recived more than 4 flags, discard
                    if (myImg.flags > 4)
                        myImg.assignedRank = 0;

                    //if user has marked image as fave, set to highest rank
                    if (DbQuery.CheckImageIsFave((int)myImg.imageID, (int)userID))
                        myImg.assignedRank = 5;
                }
                return GenerateImageResultArr(imageSet, (int)userID);
            }
            catch
            {
                return new ImageResult[] { new ImageResult() { Response = HttpStatusCode.InternalServerError.ToString(), ErrorMsg = Util.GenericError }};
            }
        }

        private static double CalcAverageRating(Image[] imgSet)
        {
            double totalRating = 0;
            double numRes = imgSet.Length;
            foreach (Image img in imgSet)
                totalRating = totalRating + (double)img.rating;
            return totalRating / numRes;
        }

        /// <summary>
        /// Create a new UserResult from a user id
        /// </summary>
        /// <param name="userID">The id of the user</param>
        /// <returns>A UserResult</returns>
        public static UserResult GenerateUserResult(int userID)
        {
            try
            {
                User myUser = DbQuery.GetSingleUserByID(userID);

                return new UserResult()
                {
                    Id = myUser.userID.ToString(),
                    FirstName = myUser.firstName,
                    LastName = myUser.lastName,
                    UserName = myUser.userName,
                    Response = HttpStatusCode.Found.ToString(),
                    ErrorMsg = ""
                };
            }
            catch
            {
                return new UserResult() { Response = HttpStatusCode.InternalServerError.ToString(), Error = true, ErrorMsg = Util.GenericError };
            }
        }

        /// <summary>
        /// Make a new UserResult from a User
        /// </summary>
        /// <param name="myUser">The User to generate the result from</param>
        /// <returns>A UserResult, based off the User</returns>
        public static UserResult GenerateUserResult(User myUser)
        {
            try
            {
                return new UserResult()
                {
                    Id = myUser.userID.ToString(),
                    FirstName = myUser.firstName,
                    LastName = myUser.lastName,
                    UserName = myUser.userName,
                    Response = HttpStatusCode.Found.ToString()
                };
            }
            catch
            {
                return new UserResult() { Response = HttpStatusCode.InternalServerError.ToString(), Error = true, ErrorMsg = Util.GenericError };
            }
        }

        /// <summary>
        /// Make a FullUserResult from a user id
        /// </summary>
        /// <param name="userID">The id of the user</param>
        /// <returns>A FullUserresult</returns>
        public static FullUserResult GenerateFullUserResult(int userID)
        {
            try
            {
                User myUser = DbQuery.GetSingleUserByID(userID);
                Image profilePicImg = DbQuery.GetSingleImage(myUser.profilePicID);

                ImageResult profilePic = GenerateImageResult(profilePicImg, userID);

                return new FullUserResult()
                {
                    Id = myUser.userID.ToString(),
                    FirstName = myUser.firstName,
                    LastName = myUser.lastName,
                    UserName = myUser.userName,
                    ProfilePic = profilePic,
                    Response = HttpStatusCode.Found.ToString()
                };
            }
            catch
            {
                return new FullUserResult() { Response = HttpStatusCode.InternalServerError.ToString(), ErrorMsg = Util.GenericError };
            }
        }
    }
}
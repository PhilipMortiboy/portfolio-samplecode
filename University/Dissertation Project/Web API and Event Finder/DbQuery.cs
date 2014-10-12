using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ObjectModel;

namespace ImageServer
{
    /// <summary>
    /// A collection of Database queries. 
    /// Note that no error checking happens at this level, 
    /// so any calls to DbQuery must be surrounded by try catch statement
    /// </summary>
    public class DbQuery
    {
        //why its a good idea to create new DBContexts each time: http://blogs.msdn.com/b/alexj/archive/2009/05/07/tip-18-how-to-decide-on-a-lifetime-for-your-objectcontext.aspx

        #region Album Queries
        /// <summary>
        /// Get a single Album by id
        /// </summary>
        /// <param name="albumID">The id of the album. If null returns latest album</param>
        /// <param name="startPos">The position to start returning results from. 0 if null</param>
        /// <returns></returns>
        public static Album GetSingleAlbum(int? albumID, int? startPos = null)
        {
            int pos;
            if (startPos == null)
                pos = 0;
            else
                pos = (int)startPos;

            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                Album myAlbum;
                if (albumID == null)
                    myAlbum = GetLatestAlbum(0);
                else
                {
                    myAlbum =
                        (from m in mDb.Albums
                         where m.albumID == albumID
                         select m).Skip(pos).FirstOrDefault();
                }

                return myAlbum;
            }
        }

        /// <summary>
        /// Get a single Album by type
        /// </summary>
        /// <param name="type">The type of album</param>
        /// <param name="startPos">The position to start returning results from. 0 if null</param>
        /// <param name="userID">The id of the user to tailor results to</param>
        /// <param name="subject">The subject type to search on</param>
        /// <returns></returns>
        public static Album GetSingleAlbumByType(AlbumType? type, int? startPos = null, int? userID = null, AlbumSubject? subject = null)
        {
            int pos;
            if (startPos == null)
                pos = 0;
            else
                pos = (int)startPos;

            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                Album myAlbum;
                //defaults to latest album
                switch (type)
                {
                    case AlbumType.Favourite:
                        myAlbum = GetFaveAlbum((int)userID, pos);
                        break;
                    case AlbumType.Recommended:
                        myAlbum = GetRecommended((int)userID, pos);
                        break;
                    case AlbumType.Subject:
                        myAlbum = GetAlbumBySubject((AlbumSubject)subject, pos);
                        break;
                    case AlbumType.LatestAlbum:
                        myAlbum = GetLatestAlbum(pos);
                        break;
                    default: //latest album the user has contributed to
                        myAlbum = GetLatestAlbum(pos, userID);
                        break;
                }

                return myAlbum;
            }
        }

        /// <summary>
        /// Get the most recent album
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="userID">If not null, only search on images upload by the user with this id</param>
        /// <returns></returns>
        private static Album GetLatestAlbum(int pos)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                var latestAlbum =
                            (from a in mDb.Albums
                                select a).OrderByDescending(a => a.albumID).Skip(pos).FirstOrDefault();
                return latestAlbum;
            }
        }

        /// <summary>
        /// Get the album that has most recently had a image added to it
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="userID">If not null, only search on images upload by the user with this id</param>
        /// <returns></returns>
        private static Album GetLatestAlbum(int pos, int? userID = null)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                if (userID == null)
                {
                    var latestAlbum =
                                (from l in mDb.Images
                                 select l).OrderByDescending(l => l.taken).Skip(pos).FirstOrDefault();
                    return GetSingleAlbum(latestAlbum.albumID);
                }
                else
                {
                    var latestAlbum =
                                (from l in mDb.Images
                                 where l.userID == userID
                                 select l).OrderByDescending(l => l.taken).Skip(pos).FirstOrDefault();
                    return GetSingleAlbum(latestAlbum.albumID);
                }
            }
        }

        /// <summary>
        /// Get a album marked as favourite by the user
        /// </summary>
        /// <param name="userID">The id of the user</param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private static Album GetFaveAlbum(int userID, int pos)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                var faveAlbum =
                            (from f in mDb.FaveAlbums
                             where f.userID == userID
                             select f).OrderByDescending(f => f.faveAlbumID).Skip(pos).FirstOrDefault();
                return GetSingleAlbum(faveAlbum.albumID);
            }
        }

        /// <summary>
        /// Get a recommended album for a user
        /// </summary>
        /// <param name="userID">The id of the user</param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private static Album GetRecommended(int userID, int pos)
        {
            //find the most recent 10 albums the user has contributed to
            Album[] userAlbums = GetAlbumsByUser((int)userID, 10);
            List<string> genreList = new List<string>();
            //find the event genere for each of these 
            foreach (Album album in userAlbums)
            {
                Event myEvent = GetSingleEvent(album.eventID);
                genreList.Add(myEvent.genre);
            }
            //get the most freq genre found. Linq query adapted from: http://www.daniweb.com/software-development/csharp/threads/288361/get-the-most-frequent-string-in-array-of-strings#post1240900
            string recommendedGenre = genreList.GroupBy(val => val).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();
            //find a album not posted in by the user of this genre
            bool notFound = true;
            int attempt = 0; int foundNum = 0;
            Album recommendedAlbum = null;
            while (notFound)
            {
                //find the next event of the requested genre
                Event thisEvent = GetSingleEventByGenre(recommendedGenre, attempt);
                //find this album's event
                Album thisAlbum = GetSingleAlbumByEvent(thisEvent);
                //find the images in this album
                Image[] albumImgs = GetImageBatch(thisAlbum.albumID);
                //check if the user has contributed any of these images
                attempt++;
                bool contributed = false;
                foreach (Image thisImg in albumImgs)
                {
                    //if user has contributed to this album, don't recommend it
                    if (thisImg.userID == userID)
                        contributed = true;
                }
                if (!contributed)
                {
                    foundNum++;
                    //if this album is before the start position requested, save it as the recommended one
                    if (foundNum <= pos)
                    {
                        recommendedAlbum = thisAlbum;
                        notFound = false;
                        break;
                    }
                }
            }
            return recommendedAlbum;
        }

        /// <summary>
        /// Get a album by subject
        /// </summary>
        /// <param name="subject">The subject to search on</param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private static Album GetAlbumBySubject(AlbumSubject subject, int pos)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                var album =
                            (from a in mDb.Albums
                             where a.subject == subject.ToString()
                             select a).OrderByDescending(x => x.albumID).Skip(pos).FirstOrDefault();
                return GetSingleAlbum(album.albumID);
            }
        }

        /// <summary>
        /// Get albums a user has contributed to
        /// </summary>
        /// <param name="userID">The id of the user</param>
        /// <param name="batchSize">The maximum amount of results to return</param>
        /// <returns></returns>
        public static Album[] GetAlbumsByUser(int userID, int? batchSize)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                if (batchSize == null)
                    batchSize = 64;
                var imgs =
                    (from i in mDb.Images
                     where i.userID == userID
                     select i).Take((int)batchSize);
                List<Album> userAlbums = new List<Album>();
                foreach (Image myImg in imgs)
                {
                    var album =
                        (from a in mDb.Albums
                         where a.albumID == myImg.albumID
                         select a).FirstOrDefault();
                    userAlbums.Add(album);
                }
                return userAlbums.ToArray();
            }
        }

        /// <summary>
        /// Get a single album by location. If no album can be found for this location, one will be created
        /// </summary>
        /// <param name="address">The locations address</param>
        /// <param name="latlng">The locations geo-location</param>
        /// <returns></returns>
        public static Album GetSingleAlbumByLocation(string address, string latlng)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                try
                {
                    var myAlbum =
                            (from n in mDb.Albums
                             where n.address == address
                             select n).FirstOrDefault();

                    return myAlbum;
                }
                catch
                {
                    //if no album was found, create one
                    return AddAlbum(address, latlng);
                }
            }
        }

        /// <summary>
        /// Get a single album belonging to an event
        /// </summary>
        /// <param name="myEvent">The event to search by</param>
        /// <returns></returns>
        public static Album GetSingleAlbumByEvent(Event myEvent)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                try
                {
                    var myAlbum =
                            (from n in mDb.Albums
                             where n.eventID == myEvent.eventID
                             select n).FirstOrDefault();

                    return myAlbum;
                }
                catch
                {
                    //if no album was found, create one
                    return AddAlbum(myEvent.address, myEvent.latlng);
                }
            }
        }

        /// <summary>
        /// Add a new album at the given location
        /// </summary>
        /// <param name="address">The address of the location</param>
        /// <param name="latlng">The coordinates of the location</param>
        /// <returns></returns>
        public static Album AddAlbum(string address, string latlng)
        {
            Album toAdd = new Album()
            {
                address = address,
                latlng = latlng
            };
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                mDb.Albums.InsertOnSubmit(toAdd);
                mDb.SubmitChanges();
                return toAdd; //return User object, now with id
            }
        }

        /// <summary>
        /// Update an Album
        /// </summary>
        /// <param name="album">The new version of the Album</param>
        /// <returns></returns>
        public static Album UpdateAlbum(Album album)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                var myAlbum =
                    (from a in mDb.Albums
                     where a.albumID == album.albumID
                     select a).FirstOrDefault();
                myAlbum = album;
                mDb.SubmitChanges();
                return myAlbum;
            }
        }

        /// <summary>
        /// Delete a favourite album record
        /// </summary>
        /// <param name="img">The fave album</param>
        /// <param name="userID">The user who marked it as a farouite</param>
        public static void DeleteFaveAlbumRecord(int? albumId, int userID)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                //find the fave img record
                var faveAlbum =
                    (from a in mDb.FaveAlbums
                     where a.albumID == albumId && a.userID == userID
                     select a).FirstOrDefault();
                mDb.FaveAlbums.DeleteOnSubmit(faveAlbum);
                mDb.SubmitChanges();
            }
        }
        #endregion

        #region Event Queries
        /// <summary>
        /// Get a single Event by event id. If null, returns the first event
        /// </summary>
        /// <param name="eventID">The id of the event to search by</param>
        /// <returns></returns>
        public static Event GetSingleEvent(int? eventID)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                Event myEvent;
                if (eventID == null)
                    myEvent = new Event(); //create a empty Event object
                else
                {
                    myEvent =
                         (from e in mDb.Events
                          where e.eventID == eventID
                          select e).FirstOrDefault();
                }

                return myEvent;
            }
        }

        /// <summary>
        /// Get a single Event by location
        /// </summary>
        /// <param name="address">The address to search for</param>
        /// <returns></returns>
        public static Event GetSingleEventByLocation(string address)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                var myEvent =
                        (from n in mDb.Events
                         where n.address == address
                         select n).FirstOrDefault();

                return myEvent;
            }
        }

        public static Event[] GetEventByLocation(string postcode)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                var myEvent =
                        from n in mDb.Events
                        where n.postcode == postcode
                        select n;

                return myEvent.ToArray();
            }
        }

        /// <summary>
        /// Get a single Event by genre
        /// </summary>
        /// <param name="genre">The genre to search for</param>
        /// <param name="pos">The position to start returning results from. 0 if null</param>
        /// <returns></returns>
        public static Event GetSingleEventByGenre(string genre, int? pos)
        {
            if (pos == null)
                pos = 1;
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                var myEvent =
                        (from n in mDb.Events
                         where n.genre == genre
                         select n).Skip((int)pos).FirstOrDefault();
                return myEvent;
            }
        }

        #endregion

        #region Image Queries
        /// <summary>
        /// Get a single Image, by image id. If null, returns the first image
        /// </summary>
        /// <param name="imageID">The id of the image</param>
        /// <returns></returns>
        public static Image GetSingleImage(int? imageID)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                Image myImage;
                if (imageID == null)
                    myImage = (from n in mDb.Images select n).FirstOrDefault();
                else
                {
                    myImage =
                         (from i in mDb.Images
                          where i.imageID == imageID
                          select i).FirstOrDefault();
                }

                return myImage;
            }
        }

        /// <summary>
        /// Get a single Image by UrlId
        /// </summary>
        /// <param name="urlID">The UrlId to search on</param>
        /// <returns></returns>
        public static Image GetSingleImage(string urlID)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                Image myImage;
                if (urlID == null)
                    myImage = (from n in mDb.Images select n).FirstOrDefault();
                else
                {
                    myImage =
                         (from i in mDb.Images
                          where i.imgUrl == urlID
                          select i).FirstOrDefault();
                }

                return myImage;
            }
        }

        /// <summary>
        /// Get a array of images belonging to a given album
        /// </summary>
        /// <param name="albumID">The id of the album</param>
        /// <param name="batchPos">Where to start returning results from. 0 if null</param>
        /// <param name="batchSize">How many results to return. 64 if null</param>
        /// <returns></returns>
        public static Image[] GetImageBatch(int albumID, int? batchPos = null, int? batchSize = null)
        {
            //if no values provided, set to default
            if (batchPos == null)
                batchPos = 1;
            if (batchSize == null)
                batchSize = 64;

            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                var imageSet =
                    (from n in mDb.Images
                     where n.albumID == albumID
                     select n).Skip((int)batchPos).Take((int)batchSize);

                return imageSet.ToArray();
            }
        }

        /// <summary>
        /// Add a new Image
        /// </summary>
        /// <param name="newImg">The Image to add</param>
        /// <returns></returns>
        public static Image AddImage(ImageRequest newImg)
        {
            Image toAdd = ImageFromRequest(newImg);
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                mDb.Images.InsertOnSubmit(toAdd);
                mDb.SubmitChanges();
                return toAdd; //return Image object, now with id
            }
        }

        /// <summary>
        /// Update a Image
        /// </summary>
        /// <param name="img">The new version of the Image</param>
        /// <returns></returns>
        public static Image UpdateImage(Image img)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                var myImg =
                    (from i in mDb.Images
                     where i.imageID == img.imageID
                     select i).FirstOrDefault();
                myImg = img;
                mDb.SubmitChanges();
                return myImg;
            }
        }

        /// <summary>
        /// Add a record of a user adding a flag to a image
        /// </summary>
        /// <param name="imgID">The id of the image that the flag has being added to</param>
        /// <param name="userID">The id of the user adding the flag</param>
        /// <returns></returns>
        public static UserImageFlag AddFlagRecord(int imgID, int userID)
        {
            UserImageFlag flag = new UserImageFlag()
            {
                imageID = imgID,
                userID = userID,
                flag = true
            };
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                mDb.UserImageFlags.InsertOnSubmit(flag);
                mDb.SubmitChanges();
                return flag; //return object, now with new flag
            }
        }

        /// <summary>
        /// Delete a favourite image record
        /// </summary>
        /// <param name="img">The fave image</param>
        /// <param name="userID">The user who marked it as a farouite</param>
        public static void DeleteFlagRecord(int imgID, int userID)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                //find the fave img record
                var flagImg =
                    (from i in mDb.UserImageFlags
                     where i.imageID == imgID && i.userID == userID
                     select i).FirstOrDefault();
                mDb.UserImageFlags.DeleteOnSubmit(flagImg);
                mDb.SubmitChanges();
            }
        }

        public static bool CheckFlagRecord(int imgID, int userID)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                var flag =
                    (from f in mDb.UserImageFlags
                     where f.imageID == imgID && f.userID == userID
                     select f).FirstOrDefault();
                if (flag == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Add a record of a user rating a image
        /// </summary>
        /// <param name="imgID">The id of the image the user has rated</param>
        /// <param name="userID">The id of the user rating the image</param>
        /// <param name="rating">Whether the rank was increaed or decrease. Null = no rating</param>
        /// <returns></returns>
        public static UserImageRating AddRatingRecord(int imgID, int userID, bool? rating)
        {
            UserImageRating newRating = new UserImageRating()
            {
                imageID = imgID,
                userID = userID,
                rating = rating
            };
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                mDb.UserImageRatings.InsertOnSubmit(newRating);
                mDb.SubmitChanges();
                return newRating; //return object, now with the new id
            }
        }

        /// <summary>
        /// Add a image to the list of a user's favourite images. Also adds the album the image belongs to as a fave
        /// </summary>
        /// <param name="img">The id of the image</param>
        /// <param name="userID">The id of the user</param>
        /// <returns></returns>
        public static FaveImage AddFaveImageRecord(Image img, int userID)
        {
            FaveImage fave = new FaveImage()
            {
                imageID = img.imageID,
                userID = userID,
            };
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                mDb.FaveImages.InsertOnSubmit(fave);
                //check if this image's album is already one of the user's faves
                var isFavelbum =
                    (from a in mDb.FaveAlbums
                     where a.albumID == img.imageID && a.userID == userID
                     select a).FirstOrDefault();
                //if it isn't, add it as one
                if (isFavelbum == null)
                {
                    FaveAlbum faveAlbum = new FaveAlbum()
                    {
                        albumID = img.albumID,
                        userID = userID
                    };
                    mDb.FaveAlbums.InsertOnSubmit(faveAlbum);
                }
                mDb.SubmitChanges();
                return fave; //return object, now with id
            }
        }

        /// <summary>
        /// Delete a favourite image record
        /// </summary>
        /// <param name="img">The fave image</param>
        /// <param name="userID">The user who marked it as a farouite</param>
        public static void DeleteFaveImageRecord(Image img, int userID)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                //find the fave img record
                var faveImg =
                    (from i in mDb.FaveImages
                     where i.imageID == img.imageID && i.userID == userID
                     select i).FirstOrDefault();
                mDb.FaveImages.DeleteOnSubmit(faveImg);
                mDb.SubmitChanges();
                //check if this was the last image for its album
                Image[] albumImgs = GetImageBatch((int)img.albumID);
                bool found = false;
                foreach (Image checkImg in albumImgs)
                {
                    var checkFave =
                        (from f in mDb.FaveImages
                         where f.imageID == checkImg.imageID
                         select f);
                    if (checkFave != null)
                    {
                        found = true;
                        break;
                    }
                }
                //if no more fave image records where found for this image, delete the fave album record
                if (!found)
                    DeleteFaveAlbumRecord(img.albumID, userID);
            }
        }

        /// <summary>
        /// Get all of the user's fave images
        /// </summary>
        /// <param name="userID">The id of the user</param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static Image[] GetFaveImages(int userID, int? limit)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                var faveImages =
                            (from f in mDb.FaveImages
                             where f.userID == userID
                             select f).OrderByDescending(f => f.faveImgID).Take((int)limit);
                List<Image> imgs = new List<Image>();
                foreach (FaveImage fave in faveImages)
                {
                    Image myImg = GetSingleImage(fave.imageID);
                    imgs.Add(myImg);
                }

                return imgs.ToArray();
            }
        }

        /// <summary>
        /// Check if a user has marked a image as a favourite
        /// </summary>
        /// <param name="imgID">The id of the image</param>
        /// <param name="userID">The id of the user</param>
        /// <returns></returns>
        public static bool CheckImageIsFave(int imgID, int userID)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                var fave =
                    (from f in mDb.FaveImages
                     where f.imageID == imgID && f.userID == userID
                     select f).FirstOrDefault();
                if (fave == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Check if the user has flagged a image
        /// </summary>
        /// <param name="imgID">The id of the image</param>
        /// <param name="userID">The id of the user</param>
        /// <returns></returns>
        public static bool CheckImageIsFlagged(int imgID, int userID)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                var flagged =
                    (from f in mDb.UserImageFlags
                     where f.imageID == imgID && f.userID == userID
                     select f).FirstOrDefault();
                if (flagged == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Check if the user has rated a image
        /// </summary>
        /// <param name="imgID">The id of the user</param>
        /// <param name="userID">The id of the image</param>
        /// <returns>True = up vote, false = down vote, null = no vote</returns>
        public static bool? CheckImageUserRating(int imgID, int userID)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                var rating =
                    (from r in mDb.UserImageRatings
                     where r.imageID == imgID && r.userID == userID
                     select r).FirstOrDefault();
                if (rating == null)
                    return null;
                else
                    return rating.rating;
            }
        }

        #endregion

        #region User Queries
        public static User GetSingleUserByID(int? userID)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                User myUser;
                if (userID == null)
                    myUser = (from n in mDb.Users select n).FirstOrDefault();
                else
                {
                    myUser =
                        (from n in mDb.Users
                         where n.userID == userID
                         select n).FirstOrDefault();
                }

                return myUser;
            }
        }

        public static User GetSingleUser(string username, string password)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                var user =
                    (from u in mDb.Users
                     where u.userName == username && u.password == password
                     select u).FirstOrDefault();
                return user;
            }
        }

        public static User GetSingleUserByUsername(string username)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                var user =
                    (from u in mDb.Users
                     where u.userName == username
                     select u).FirstOrDefault();
                return user;
            }
        }

        public static User GetSingleUserByEmail(string email)
        {
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                var user =
                    (from u in mDb.Users
                     where u.email == email
                     select u).FirstOrDefault();
                return user;
            }
        }

        public static User AddUser(UserRequest newUser)
        {
            User toAdd = UserFromRequest(newUser);
            //add relevant metadata
            toAdd.active = true;
            toAdd.flags = 0;
            using (ImageDbDataContext mDb = new ImageDbDataContext())
            {
                mDb.Users.InsertOnSubmit(toAdd);
                mDb.SubmitChanges();
                return toAdd; //return User object, now with id
            }
        }
        #endregion

        /// <summary>
        /// Generate a User object from a UserRequest
        /// </summary>
        /// <param name="userData">The UserRequest to convert</param>
        /// <returns></returns>
        private static User UserFromRequest(UserRequest userData)
        {
            return new User()
            {
                email = userData.Email,
                firstName = userData.FirstName,
                lastName = userData.LastName,
                password = userData.Password,
                userName = userData.Username
            };
        }

        /// <summary>
        /// Generate a Image object from a ImageRequest
        /// </summary>
        /// <param name="imgData">The ImageRequest to convert</param>
        /// <returns></returns>
        private static Image ImageFromRequest(ImageRequest imgData)
        {
            return new Image()
            {
                cameraDetails = imgData.CameraDetails,
                flags = imgData.Flag,
                latlng = imgData.LatLng,
                rating = imgData.Rank,
                taken = imgData.Taken,
                imgUrl = imgData.Url,
                imageID = imgData.ID,
                userID = imgData.UserID,
                albumID = imgData.AlbumID,
                baseRank = imgData.Rank,
            };
        }
    }
}
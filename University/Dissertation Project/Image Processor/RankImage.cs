using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageRanker_EmguCV
{
    class RankImage
    {
        private Bitmap myImg_Bitmap;
        private BitmapImage myImg_BitmapImage;
        private Emgu.CV.Image<Bgr, byte> cvImg;
        private Emgu.CV.Image<Bgr, byte> faceImg;
        private Guid imageID;
        private Guid albumID;
        private int rank;
        private int type;
        private int quality;
        private bool isEvent = true;
        private int flags;
        private int rating;

        #region Attribues
        /// <summary> 
        /// get Bitmap data for image
        /// </summary>
        public Bitmap MyImg_Bitmap
        {
            get { return myImg_Bitmap; }
            set { myImg_Bitmap = value; }
        }

        /// <summary> 
        /// get Bitmap data for image
        /// </summary>
        public Emgu.CV.Image<Bgr, byte> CvImg
        {
            get { return cvImg; }
            set { cvImg = value; }
        }

        /// <summary> 
        /// the image with faces outlines
        /// </summary>
        public Emgu.CV.Image<Bgr, byte> FaceImg
        {
            get { return faceImg; }
            set { faceImg = value; }
        }

        /// <summary> 
        /// Image unique ID
        /// </summary>
        public Guid ImageID
        {
            get { return imageID; }
            set { imageID = value; }
        }

        /// <summary> 
        /// Unique ID of the album the image belongs to
        /// </summary>
        public Guid AlbumID
        {
            get { return albumID; }
            set { albumID = value; }
        }

        /// <summary> 
        /// Image base rank, 1 is low, 5 is high
        /// </summary>
        public int Rank
        {
            get { return rank; }
            set { rank = value; }
        }

        /// <summary> 
        /// Image type; 0 - standard, 1 - full, 2 - thumbnail
        /// </summary>
        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        public int Quality
        {
            get { return quality; }
            set { quality = value; }
        }

        /// <summary> 
        /// Is the image part of an event
        /// </summary>
        public bool IsEvent
        {
            get { return isEvent; }
            set { isEvent = value; }
        }

        /// <summary> 
        /// Number of flags assined to the image
        /// </summary>
        public int Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        /// <summary> 
        /// User rating
        /// </summary>
        public int Rating
        {
            get { return rating; }
            set { rating = value; }
        }
        #endregion

        #region Contsrutors
        /// <summary> 
        /// Image object - generates a BitmapImage
        /// </summary>
        /// <param name="myImage"> Raw image data as a BitmapImage
        /// </param>
        public RankImage(BitmapImage myImage)
        {
            myImg_BitmapImage = myImage;
        }

        /// <summary> 
        /// Image object - generates a Bitmap
        /// </summary>
        /// <param name="myImage"> Raw image data as a Bitamp
        /// </param>
        public RankImage(Bitmap myImage)
        {
            myImg_Bitmap = myImage;
        }

        /// <summary> 
        /// Image object - generates a OpenCV image
        /// </summary>
        /// <param name="myImage"> Image file name
        /// </param>
        public RankImage(string filename)
        {
            cvImg = new Image<Bgr, byte>(filename);
            myImg_Bitmap = cvImg.ToBitmap();
            //create a copy of the image to draw face detection onto
            faceImg = new Image<Bgr, byte>(filename);
        }

        #endregion


    }
}

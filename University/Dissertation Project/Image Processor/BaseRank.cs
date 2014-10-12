using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageRanker_EmguCV
{
    class BaseRank
    {
        private RankImage myImg;
        public Bitmap myBitmap;
        private Rectangle[] faces;

        //image stats
        private int numFaces = 0;
        private bool selfie = false;
        private bool farAway = false;
        private bool dark = false;
        private bool light = false;
        private double iqi = 0;
        private Bitmap darkImg;
        private Bitmap lightImg;
        private Bitmap edgeDetectImg;
        private int eventRank;
        private int nonEventRank;

        #region ImageStatAttribues

        public int NumFaces
        {
            get { return numFaces; }
            set { numFaces = value; }
        }

        public double Iqi
        {
            get { return iqi; }
            set { iqi = value; }
        }

        public bool Selfie
        {
            get { return selfie; }
            set { selfie = value; }
        }

        public bool FarAway
        {
            get { return farAway; }
            set { farAway = value; }
        }

        public bool Dark
        {
            get { return dark; }
            set { dark = value; }
        }

        public bool Light
        {
            get { return light; }
            set { light = value; }
        }

        public Bitmap DarkImg
        {
            get { return darkImg; }
            set { darkImg = value; }
        }

        public Bitmap LightImg
        {
            get { return lightImg; }
            set { lightImg = value; }
        }

        public Bitmap EdgeImg
        {
            get { return edgeDetectImg; }
            set { edgeDetectImg = value; }
        }

        public int EventRank
        {
            get { return eventRank; }
            set { eventRank = value; }
        }

        #endregion

        #region Constructors
        /// <summary> 
        /// Assign a new base rank to a image
        /// </summary>
        /// <param name="myImage"> The image to be ranked
        /// </param>
        public BaseRank(RankImage myImage)
        {
            myImg = myImage;
            if (myImg.IsEvent)
                this.EventImage();
            else
                this.NonEventImage();
        }
        #endregion

        /// <summary> 
        /// Start a new ranking process for a event image
        /// </summary>
        private void EventImage()
        {
            double rating = 0;
            int numFaces = FaceDetection();
            //set rank based on face details
            if (numFaces > 0)
                rating = FaceRank();
            else
                rating = 30;
            //modify rank based on light levels
            rating = (rating - LightRank()) + ImageQualityIndex();
            //original version
            //rating = (rating - LightRank()) / 10;
            myImg.Rank = int.Parse(rating.ToString());
        }

        /// <summary>
        /// Return a rank based on the properties of a face
        /// </summary>
        /// <returns></returns>
        private int FaceRank()
        {
            if (IsSelfie() || IsFarAway())
                return 20;
            else
                return 30;
        }

        /// <summary>
        /// The amount to decrease a rank by if image is too dark or light
        /// </summary>
        /// <returns></returns>
        private int LightRank()
        {
            //need to run these to generate light and dark maps for UI
            IsDark();
            IsLight();
            if (dark || light)
                return 10;
            else
                return 0;
        }
        
        #region Flow chart based method
        /*Old flow chart based way
        private void EventImage()
        {
            numFaces = FaceDetection();
            //remove these 4 after testing and replace with commented ones
            selfie = IsSelfie();
            farAway = IsFarAway();
            dark = IsDark();
            light = IsLight();
            blurVal = IsBlurred();
            if (numFaces == 0)
            {
                eventRank = 3;
            }
            if (numFaces == 1 || numFaces == 2)
            {
                //bool selfie = IsSelfie();
                if (selfie)
                {
                    int albumSize = GetAlbumSize();
                    if (albumSize < 180)
                        eventRank = 2;
                    else
                        eventRank = 1;
                }
            }
            if (numFaces > 2)
            {
                //bool farAway = IsFarAway();
                if (farAway)
                    eventRank = 3;
                else
                    eventRank = 2;
            }
            //bool dark = IsDark();
            if (dark)
                eventRank--;
            else
            {
                //bool light = IsLight();
                if (light)
                    eventRank--;
            }
            myImg.Rank = eventRank;
        }*/
        #endregion

        /// <summary> 
        /// Start a new ranking process for a non-event image
        /// </summary>
        private void NonEventImage()
        {
        }

        #region Ranking Functions
        /// <summary> 
        /// Check the image's IQI
        /// </summary>
        private double ImageQualityIndex()
        {
            //canny function documentation: http://www.emgu.com/wiki/files/1.4.0.0/html/ba736ea5-ba99-bcd4-6152-216a82e35066.htm
            //canny edge background info: http://dasl.mem.drexel.edu/alumni/bGreen/www.pages.drexel.edu/_weg22/can_tut.html
            //and http://homepages.inf.ed.ac.uk/rbf/HIPR2/canny.htm
            
            //set a low colour threahold, (to ensure lots of edges are found) and a
            //high linking threshold, to make sure edges are kept invidiual and not linked as one)
            Image<Gray, byte> edgeImg = myImg.CvImg.Canny(100, 150); 
            //count the number of edges found
            int count = edgeImg.CountNonzero().FirstOrDefault();
            //normalise the result by dividing by total num of pixels in image
            double res = (count * 1000 / (edgeImg.Cols * edgeImg.Rows));
            //save the image and result for use in the UI
            edgeDetectImg = edgeImg.ToBitmap();
            iqi = res;
            return res;
        }

        /// <summary> 
        /// Get number of faces
        /// </summary>
        private int FaceDetection()
        {
            //code adapted from this tutorial: http://docs.opencv.org/trunk/doc/py_tutorials/py_objdetect/py_face_detection/py_face_detection.html
            //cascade training files come with opencv install
            /*HaarClassifierCascade face_cascade = HaarClassifierCascade.Load(@"C:\Users\Philip\Documents\Uni\Year 3\Project\Software\ImageRanker\ImageRanker\haarcascade_frontalface_default.xml");
            HaarClassifierCascade eye_cascade = HaarClassifierCascade.Load(@"C:\Users\Philip\Documents\Uni\Year 3\Project\Software\ImageRanker\ImageRanker\haarcascade_eye.xml");

            IplImage origImg = myImg.CvImg;
            IplImage greyImg = new IplImage(new OpenCV.Net.Size(origImg.Width, origImg.Height), origImg.Depth, 1);
            CV.CvtColor(origImg, greyImg, ColorConversion.Bgr2Gray);
            //MemStorage myStorage = new MemStorage();
            using (MemStorage stor = new MemStorage())
            {
                Seq numFaces = eye_cascade.DetectObjects(greyImg, stor);
            }



            */

            //code heavily adapted from: http://www.emgu.com/wiki/index.php/Face_detection, but using newer CasacadeClassifier appraoch
            //construct new cascade calassifier for each feature to be detected
            CascadeClassifier faceCascade = new CascadeClassifier(@"C:\Users\Merwan Rambeau\Desktop\Uni\ImageRanker\ImageRanker_EmguCV\ImageRanker_EmguCV\haarcascade_frontalface_default.xml");
            //CascadeClassifier eyeCascade = new CascadeClassifier(@"\haarcascade_eye.xml");
            //convert image to greyscale
            Image<Gray, byte> grayImg = myImg.CvImg.Convert<Gray, byte>();

            //find all faces in the image using the face cascade classifier
            //faces must be no smaller than 1/12th of the image and no large than the image itself
            faces = faceCascade.DetectMultiScale(
                            grayImg, 1.4, 8, 
                            new Size(myImg.CvImg.Width / 12, myImg.CvImg.Height / 12), 
                            new Size(myImg.CvImg.Width, myImg.CvImg.Height)
                            );

            int facesCount = 0;
            foreach (Rectangle face in faces)
            {
                //check if the face is inside another face to prevent duplicates
                bool duplicate = false;
                foreach (Rectangle otherFace in faces)
                {
                    //check that otherFace is not the current face
                    if (!(face.Size == otherFace.Size))
                    {
                        if (otherFace.Contains(face))
                            duplicate = true;
                    }
                }
                if (!duplicate)
                {
                    //draw a rectangle around the face - for testing purposes only
                    myImg.FaceImg.Draw(face, new Bgr(0, double.MaxValue, 0), 5);
                    facesCount++;
                    /*
                    //check if it has two eyes - probably uneccessary
                    //limit the region of interest to just the rectangle in which the face was found
                    grayImg.ROI = face;
                    //search this region for eyes using the eye cascade classifier
                    var eyes = eyeCascade.DetectMultiScale(
                                  grayImg, 1.1, 2,
                                  new Size(5, 5),
                                  new Size(50, 50)
                                  );

                    //if two eyes were found, it must be a face so add it to the count
                    if (eyes.Count() == 2)
                    {
                        //facesCount++;
                    }
                     * */
                }
            }

            this.numFaces = facesCount;
            return facesCount;
        }

        /// <summary> 
        /// Check if image is a selfie
        /// </summary>
        private bool IsSelfie()
        {
            //if the photo has less than 4 faces, test to see if its a selfie
            if (numFaces < 4)
            {
                //find how much of the image is taken up by faces
                int totalHeight = 0;
                int totalWidth = 0;
                foreach (Rectangle face in faces)
                {
                    totalHeight = face.Height;
                    totalWidth = face.Width;
                }
                double percHeight = (double)totalHeight / myImg.CvImg.Height;
                percHeight = percHeight * 100;
                double percWidth = (double)totalWidth / myImg.CvImg.Width;
                percWidth = percWidth * 100;

                if (percHeight > 50 || percWidth > 50)
                {
                    this.selfie = true;
                    return true;
                }
            }
            return false;
        }

/// <summary> 
/// Check if image is far away
/// </summary>
private bool IsFarAway()
{
    if (faces.Count() > 0)
    {
        //find how many of the faces are far away and how many are close
        int farAwayNum = 0;
        int closeNum = 0;
        foreach (Rectangle face in faces)
        {
            double percHeight = (double)face.Height / myImg.CvImg.Height;
            percHeight = percHeight * 100;
            double percWidth = (double)face.Width / myImg.CvImg.Width;
            percWidth = percWidth * 100;
            if (percHeight < 20 || percWidth < 20)
                farAwayNum++;
            else
                closeNum++;
        }
                
        //if there are more faces far away, then mark image as far away
        if (farAwayNum > closeNum)
        {
            this.farAway = true;
            return true;
        }
    }
    return false;
}

        /// <summary> 
        /// Check if image is too dark - will return a bool
        /// </summary>
        private bool IsDark()
        {
            /*
            //code below is based on answer 1: http://stackoverflow.com/questions/19189482/color-detection-in-opencv
            IplImage inImg = myImg.CvImg;
            //create a output image based on the original
            //IplImage outImg = new IplImage(new OpenCV.Net.Size(myImg.CvImg.Width, myImg.CvImg.Height), myImg.CvImg.Depth, myImg.CvImg.Channels);
            //IplImage outImg;
            //convert to hsv colour space - (why this is a good idea: http://www.aishack.in/2010/01/color-spaces/2/)
            CV.CvtColor(inImg, inImg, ColorConversion.Bgr2Hsv);
            //check if colour is above dark threshold - still to be confirmed what this threshold is
            Scalar lower = new Scalar(0, 0, 50);
            Scalar upper = new Scalar(0, 0, 100);
            CV.InRangeS(inImg, lower, upper, inImg);
            */

            //old way - using emgu cv wrapper.
            /*Emgu.CV.Image<Bgr, Byte> inImg = myImg.CvImg;
            Emgu.CV.Image<Hsv, Byte> outImg = new Image<Hsv, byte>(myImg.CvImg.Width, myImg.CvImg.Height);
            CvInvoke.cvCvtColor(inImg, inImg, COLOR_CONVERSION.CV_BGR2HSV);
            MCvScalar lower = new MCvScalar(0, 0, 50);
            MCvScalar upper = new MCvScalar(0, 0, 100);*/

            //create a copy of the image, with dark areas highlighted in white and the rest black
            Image<Gray, byte> darkMap = myImg.CvImg.InRange(new Bgr(0, 0, 0), new Bgr(60, 60, 60));
            darkImg = darkMap.ToBitmap();

            int darkPixels = 0;
            //code adapted from first answer: http://stackoverflow.com/questions/17004480/get-and-set-pixel-gray-scale-image-using-emgu-cv
            //for each pixel, check if it is black or white
            for (int v = 0; v < darkImg.Height; v++)
            {
                for (int u = 0; u < darkImg.Width; u++)
                {
                    Color pixelColor = darkImg.GetPixel(u, v);
                    //if it is a white pixel, then mark this one as being too dark
                    if (pixelColor.R == 255 && pixelColor.G == 255 && pixelColor.B == 255)
                        darkPixels++;
                }
            }
            //calculate the percentage of the image that is dark
            int totalPixels = darkImg.Height * darkImg.Width;
            double percDark = (double)darkPixels / totalPixels;
            percDark = percDark * 100;

            if (percDark > 70)
            {
                this.dark = true;
                return true;
            }
            else
                return false;
        }

        /// <summary> 
        /// Check if image is too light
        /// </summary>
        private bool IsLight()
        {
            //as above, but using a different threshold to find the light areas
            Image<Gray, byte> lightMap = myImg.CvImg.InRange(new Bgr(220, 220, 220), new Bgr(255, 255, 255));
            lightImg = lightMap.ToBitmap();

            int lightPixels = 0;
            for (int v = 0; v < lightImg.Height; v++)
            {
                for (int u = 0; u < lightImg.Width; u++)
                {
                    Color pixelColor = lightImg.GetPixel(u, v);
                    if (pixelColor.R == 255 && pixelColor.G == 255 && pixelColor.B == 255)
                        lightPixels++;
                }
            }
            int totalPixels = lightImg.Height * lightImg.Width;
            double percDark = (double)lightPixels / totalPixels;
            percDark = percDark * 100;

            if (percDark > 40)
            {
                this.light = true;
                return true;
            }
            else
                return false;
        }
        #endregion

        /// <summary> 
        /// Commit base rank to database - will be changed to a non-static method in final version
        /// </summary>
        public static bool SaveRank(RankImage myImg)
        {
            //note: this will be changed to a update query in the final version
            byte[] imgData = ToByteArray(myImg.MyImg_Bitmap, ImageFormat.Png);
            ImageDbDataContext myDb = new ImageDbDataContext();
            Image newImg = new Image() {
                        albumID = 1, //testing oly
                        userID = 1, //testing only
                        baseRank = myImg.Rank,
                        flags = 0,
                        rating = 0,
                        taken = DateTime.Now,
                        rawData = imgData,
                        cameraDetails = "Taken with the ImageRanker tester"
                    };
            try
            {
                myDb.Images.InsertOnSubmit(newImg);
                myDb.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> 
        /// Get the size of the album that the image belongs to - maybe move this to Image
        /// </summary>
        private int GetAlbumSize()
        {
            Guid albumID = myImg.AlbumID;
            //some query to retrive album size
            return 5; //test value
        }

        /// <summary>
        /// Convert a bitmap image to a byte array - maybe move to until file
        /// </summary>
        /// <param name="myBitmap">Bitmap to be converted</param>
        /// <param name="format">Image format for the resulting raw data</param>
        /// <returns></returns>
        public static byte[] ToByteArray(Bitmap myBitmap, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                myBitmap.Save(ms, format);
                return ms.ToArray();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageRanker_EmguCV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RankImage myImage = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_select_img_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "JPG Files (*.jpg)|*.jpg|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                txt_file.Text = filename;
            }
        }

        private void btn_go_Click(object sender, RoutedEventArgs e)
        {
            lbl_saved.Content = ""; //clear save result
            if (txt_file.Text == null)
                MessageBox.Show("Please select a file before anyalising it");
            else
            {
                //display the image preview
                Bitmap myBitmap = new Bitmap(txt_file.Text);
                BitmapImage myBitmapImg = BitmapToImageSource(myBitmap, System.Drawing.Imaging.ImageFormat.Png);
                img_preview.Source = myBitmapImg;

                //create a new image from the file and rank it
                myImage = new RankImage(txt_file.Text);
                BaseRank myBaseRank = new BaseRank(myImage);

                //display results of ranking
                lbl_numFaces.Content = myBaseRank.NumFaces.ToString();
                Bitmap resBitmap = myImage.FaceImg.ToBitmap();
                BitmapImage resBitampImg = BitmapToImageSource(resBitmap, System.Drawing.Imaging.ImageFormat.Png);
                img_preview.Source = resBitampImg;

                if (myBaseRank.Selfie)
                    lbl_isSelfie.Content = "Yes";
                else
                    lbl_isSelfie.Content = "No";

                if (myBaseRank.FarAway)
                    lbl_farAway.Content = "Yes";
                else
                    lbl_farAway.Content = "No";

                if (myBaseRank.Dark)
                    lbl_tooDark.Content = "Yes";
                else
                    lbl_tooDark.Content = "No";

                if (myBaseRank.Light)
                    lbl_tooLight.Content = "Yes";
                else
                    lbl_tooLight.Content = "No";
                lbl_blurLevel.Content = myBaseRank.Iqi.ToString();

                Bitmap darkBitmap = myBaseRank.DarkImg;
                BitmapImage darkBitmapImg = BitmapToImageSource(darkBitmap, System.Drawing.Imaging.ImageFormat.Png);
                img_dark.Source = darkBitmapImg;

                Bitmap lightBitmap = myBaseRank.LightImg;
                BitmapImage lightBitmapImg = BitmapToImageSource(lightBitmap, System.Drawing.Imaging.ImageFormat.Png);
                img_light.Source = lightBitmapImg;

                Bitmap edgeBitmap = myBaseRank.EdgeImg;
                BitmapImage edgeBitmapImg = BitmapToImageSource(edgeBitmap, System.Drawing.Imaging.ImageFormat.Png);
                img_edge.Source = edgeBitmapImg;

                string eventRankName = "";
                switch (myImage.Rank)
                {
                    case 0:
                        eventRankName = "Discard";
                        break;
                    case 1:
                        eventRankName = "Lowest";
                        break;
                    case 2:
                        eventRankName = "Low";
                        break;
                    case 3:
                        eventRankName = "Medium";
                        break;
                    case 4:
                        eventRankName = "High";
                        break;
                    case 5:
                        eventRankName = "Highest";
                        break;
                }
                eventRankName = myImage.Rank.ToString();
                lbl_eventRank.Content = eventRankName;     
            }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            lbl_saved.Content = "";
            bool success = BaseRank.SaveRank(myImage);
            if (success)
                lbl_saved.Content = "Saved";
            else
                lbl_saved.Content = "Error";
        }

        private BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap, System.Drawing.Imaging.ImageFormat imgFormat)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, imgFormat);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}

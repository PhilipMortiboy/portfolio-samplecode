using System;
using System.Collections.Generic;
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
using DecibelAudioTools;

namespace OggPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TagData myTagData;
        FlacPictureBlock myPictureBlock;
        OggFile myOggFile = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_Import_Click(object sender, RoutedEventArgs e)
        {
            //NOTE - need some error checking for invalid files
            string filename = txt_File.Text;
            myTagData = new TagData();
            if (type_flac.IsChecked == true)
            {
                FlacFile myFile = new FlacFile(filename);
                Tagger.RetriveFileTags(myFile, myTagData);
                UpdateDisplay(myTagData);
                txt_error.Content = "File read successfully";
            }
            else if (type_ogg.IsChecked == true)
            {
                myOggFile = new OggFile(filename);
                Tagger.RetriveFileTags(myOggFile, myTagData);
                UpdateDisplay(myTagData);

                txt_error.Content = "File read successfully";
            }
            else
            {
                txt_error.Content = "MP3 files are not yet supported";
            }
        }

        private void btn_SelectFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (type_flac.IsChecked == true)
            {
                dlg.DefaultExt = ".flac";
                dlg.Filter = "FLAC Files (*.flac)|*.flac";
            }
            else if (type_ogg.IsChecked == true)
            {
                dlg.DefaultExt = ".ogg";
                dlg.Filter = "OGG Files (*.ogg)|*.ogg";
            }
            else
            {
                dlg.DefaultExt = ".mp3";
                dlg.Filter = "MP3 Files (*.mp3)|*.mp3";
            }
            Nullable<bool> result = dlg.ShowDialog();
 
            if (result == true)
            {
                string filename = dlg.FileName;
                txt_File.Text = filename;
            }
        }

        private void btn_write_Click(object sender, RoutedEventArgs e)
        {
            string filename = txt_File.Text;
            if (type_flac.IsChecked == true)
            {
                FlacFile myFile = new FlacFile(filename);
                TagData myTagData = SetTagData();
                Tagger.SetFileTags(myFile, myTagData);
                //myFile.Metadata.Add(myPictureBlock);
                myFile.WriteFile();
                txt_error.Content = "New tags written to file";
            }
            else if (type_ogg.IsChecked == true)
            {
                if (myOggFile != null)
                {
                    TagData myTagData = SetTagData();
                    Tagger.SetFileTags(myOggFile, myTagData, myPictureBlock);
                    /*OggVorbisCommentBlock picCommentBlock = new OggVorbisCommentBlock(myPictureBlock);
                    myFile.Metadata.Add(picCommentBlock);*/
                    myOggFile.WriteFile();
                    txt_error.Content = "New tags written to file";
                }
                else
                {
                    txt_error.Content = "Please read a file before trying to write to it";
                }
            }
            else
            {
                txt_error.Content = "MP3 files are not yet supported";
            }
        }

        private void btn_addArtwork_Click(object sender, RoutedEventArgs e)
        {
            myPictureBlock = new FlacPictureBlock();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".png";
            dlg.Filter = "Image Files(*.PNG;*.JPG;*.GIF)|*.PNG;*.JPG;*.GIF|All files (*.*)|*.*";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                //myPictureBlock.LoadImage(filename);
                //myPictureBlock.CalcData();
                myOggFile.SetImage(filename);

                txt_error.Content = "Cover art added";
            }
        }

        private void btn_removeArtwork_Click(object sender, RoutedEventArgs e)
        {
            txt_error.Content = "This feature is not yet implemented";
        }

        private void btn_readArtwork_Click(object sender, RoutedEventArgs e)
        {
            if (myOggFile != null)
            {
                //FlacPictureBlock picBlock = myOggFile.GetPictureBlock();
                PictureInfo<BitmapImage> myPicInfo = myOggFile.GetImageSourceList();
                txt_error.Content = "Picture info read";
            }
            else
            {
                txt_error.Content = "Please read a file before trying to read its image data";
            }
        }

        private void UpdateDisplay(TagData myTagdata)
        {
            txt_album.Text = myTagdata.AlbumTitle;
            txt_album_barcode.Text = myTagdata.AlbumBarCode;
            txt_artist.Text = myTagdata.Artist;
            txt_cat_num.Text = myTagdata.CatalogueNumber;
            txt_cddb.Text = myTagdata.Cddb;
            txt_cline.Text = myTagdata.CLine;
            txt_composer.Text = myTagdata.Authors;
            txt_disc_count.Text = myTagdata.DiscCount.ToString();
            txt_disc_num.Text = myTagdata.DiscNum.ToString();
            txt_genre.Text = myTagdata.Genre;
            txt_isrc.Text = myTagdata.Isrc;
            txt_location.Text = myTagdata.Location;
            txt_pline.Text = myTagdata.PLine;
            txt_recording_date.Text = myTagdata.RecordingDate;
            txt_title.Text = myTagdata.Title;
            txt_track_count.Text = myTagdata.TrackCount.ToString();
            txt_track_num.Text = myTagdata.TrackNum.ToString();
            txt_venue.Text = myTagdata.Venue;
            txt_year.Text = myTagdata.Year;
        }

        private TagData SetTagData()
        {
            myTagData.AlbumTitle = txt_album.Text;
            myTagData.AlbumBarCode = txt_album_barcode.Text;
            myTagData.Artist = txt_artist.Text;
            myTagData.CatalogueNumber = txt_cat_num.Text;
            myTagData.Cddb = txt_cddb.Text;
            myTagData.CLine = txt_cline.Text;
            myTagData.Authors = txt_composer.Text;
            myTagData.DiscCount = int.Parse(txt_disc_count.Text);
            myTagData.DiscNum = int.Parse(txt_disc_num.Text);
            myTagData.Genre = txt_genre.Text;
            myTagData.Isrc = txt_isrc.Text;
            myTagData.Location = txt_location.Text;
            myTagData.PLine = txt_pline.Text;
            myTagData.RecordingDate = txt_recording_date.Text;
            myTagData.Title = txt_title.Text;
            myTagData.TrackCount = int.Parse(txt_track_count.Text);
            myTagData.TrackNum = int.Parse(txt_track_num.Text);
            myTagData.Venue = txt_venue.Text;
            myTagData.Year = txt_year.Text;

            return myTagData;
        }
    }
}

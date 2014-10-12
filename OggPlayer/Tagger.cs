using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecibelAudioTools;

namespace OggPlayer
{
    class Tagger
    {
        private static string mDefaultLanguageCode = CultureInfo.CurrentCulture.ThreeLetterISOLanguageName;

        public static void RetriveFileTags(OggFile myFile, TagData myTagData)
        {
            int bitRate;
            string bitRateString;
            try
            {
                OggStreamInfoBlock oggStreamInfo = myFile.StreamInfo;
                bitRate = myFile.BitRate / 1000;
                bitRateString = bitRate.ToString() + " kbps";
                myTagData.BitRate = bitRateString;
                myTagData.TrackTimeString = oggStreamInfo.TimeString;
                myTagData.TrackTime = oggStreamInfo.Time;
            }
            catch
            {
                bitRate = -1;
                bitRateString = "Unknown";
                myTagData.BitRate = "Unknown";
                myTagData.TrackTimeString = "??:??:??";
                myTagData.TrackTime = -1;
            }

            myTagData.Artist = myFile.GetTagValues("ARTIST", ", ", false);
            myTagData.AlbumTitle = myFile.GetTagValues("ALBUM", ", ", false);
            myTagData.DiscTitle = myFile.GetTagValues("DISCTITLE", ", ", false);
            myTagData.Title = myFile.GetTagValues("TITLE", ", ", false);
            myTagData.Subtitle = myFile.GetTagValues("SUBTITLE", ", ", false);
            myTagData.Works = myFile.GetTagValues("PIECES", ", ", false);
            myTagData.Year = myFile.GetTagValues("DATE", ", ", false);
            myTagData.PLine = myFile.GetTagValues("P_LINE", ", ", false);
            myTagData.CLine = myFile.GetTagValues("C_LINE", ", ", false);
            myTagData.Venue = myFile.GetTagValues("VENUE", ", ", false);
            myTagData.Location = myFile.GetTagValues("LOCATION", ", ", false);
            myTagData.RecordingDate = myFile.GetTagValues("YEAR", ", ", false);
            myTagData.Band = myFile.GetTagValues("BAND", ", ", false);

            foreach (var myBlock in myFile.Metadata)
            {
                if (myBlock.BlockType == OggBlockType.VorbisComment)
                {
                    OggVorbisCommentBlock myCommentBlock = (OggVorbisCommentBlock)myBlock;
                    string vendor = myCommentBlock.VendorString;
                }
            }

            int trackNum;
            int numTracks;
            string trackString = myFile.GetTagValues("TRACKNUMBER", ", ", false);
            Util.ParseTrackOrDisc(trackString, out trackNum, out numTracks);
            myTagData.TrackNum = trackNum;
            myTagData.TrackCount = numTracks;

            /*Dictionary<string, string> mMediaTypes = DecibelAudioTools.DataReader.MediaTypeByCode;
            myTagData.MediaTypeCode = myFile.GetTagValues("MEDIATYPE", ", ", false);
            if (myTagData.MediaTypeCode == string.Empty)
                myTagData.MediaType = string.Empty;
            else
            {
                if (mMediaTypes.ContainsKey(myTagData.MediaTypeCode))
                    myTagData.MediaType = mMediaTypes[myTagData.MediaTypeCode];
            }*/

            string liveString = myFile.GetTagValues("IS_LIVE", ", ", false).ToUpper();
            if (liveString == "TRUE")
                myTagData.IsLive = true;
            else
                myTagData.IsLive = false;

            string bonusString = myFile.GetTagValues("IS_BONUS", ", ", false).ToUpper();
            if (bonusString == "TRUE")
                myTagData.IsBonus = true;
            else
                myTagData.IsBonus = false;

            myTagData.Cddb = myFile.GetTagValues("CDDB", ", ", false);
            myTagData.Isrc = myFile.GetTagValues("ISRC", ", ", false);
            myTagData.Spars = myFile.GetTagValues("SPARS", ", ", false);
            myTagData.AlbumBarCode = myFile.GetTagValues("BARCODE", ", ", false);
            myTagData.DiscBarCode = myFile.GetTagValues("DISC_BARCODE", ", ", false);
            myTagData.LabelName = myFile.GetTagValues("ORGANIZATION", ", ", false);
            myTagData.CatalogueNumber = myFile.GetTagValues("CATALOGUENUMBER", ", ", false);
            myTagData.DiscCatalogueNumber = myFile.GetTagValues("DISC_CATALOGUENUMBER", ", ", false);
            myTagData.Genre = myFile.GetTagValues("GENRE", ", ", false);
            myTagData.Comment = myFile.GetTagValues("COMMENT", ", ", false);
            myTagData.Notes = myFile.GetTagValues("NOTES", ", ", false);
            myTagData.Authors = myFile.GetTagValues("COMPOSER", ", ", false);
            myTagData.Publishers = myFile.GetTagValues("WORK_PUBLISHER", ", ", false);
            myTagData.OriginalAlbum = myFile.GetTagValues("ORIGINAL_ALBUM", ", ", false);
            myTagData.OriginalLabel = myFile.GetTagValues("ORIGINAL_LABEL", ", ", false);
            myTagData.OriginalCatalogueNumber = myFile.GetTagValues("ORIGINAL_CATALOGUENUMBER", ", ", false);
            myTagData.Matrix = myFile.GetTagValues("MATRIXNUMBER", ", ", false);
            myTagData.Puid = myFile.GetTagValues("MUSICDNS_PUID", ", ", false);

            string involvements = myFile.GetTagValues("INVOLVEMENTS", "/ ", false);
            if (involvements.Length > 0)
            {
                PersonalInvolvementList involvementList = PersonalInvolvementList.Parse(involvements, ", ", "; ", ".");
                myTagData.Participants = involvementList;
            }
            else
                myTagData.Participants = new PersonalInvolvementList();
        }

        public static void RetriveFileTags(FlacFile myFile, TagData myTagData)
        {
            int bitRate;
            string bitRateString;
            try
            {
                FlacStreamInfoBlock flacStreamInfo = myFile.StreamInfo;
                bitRate = Convert.ToInt32(Decimal.Round(Convert.ToDecimal(myFile.BitRate) / 1000));
                bitRateString = bitRate.ToString() + " kbps";
                myTagData.BitRate = bitRateString;
                myTagData.TrackTimeString = flacStreamInfo.TimeString;
                myTagData.TrackTime = flacStreamInfo.Time;
            }
            catch
            {
                bitRate = -1;
                bitRateString = "Unknown";
                myTagData.BitRate = "Unknown";
                myTagData.TrackTimeString = "??:??:??";
                myTagData.TrackTime = -1;
            }

            myTagData.CoverImg = myFile.GetImage();
            myTagData.Artist = myFile.GetTagValues("ARTIST", ", ", false);
            myTagData.AlbumTitle = myFile.GetTagValues("ALBUM", ", ", false);
            myTagData.DiscTitle = myFile.GetTagValues("DISCTITLE", ", ", false);
            myTagData.Title = myFile.GetTagValues("TITLE", ", ", false);
            myTagData.Subtitle = myFile.GetTagValues("SUBTITLE", ", ", false);
            myTagData.Works = myFile.GetTagValues("PIECES", ", ", false);
            myTagData.Year = myFile.GetTagValues("YEAR", ", ", false);
            myTagData.PLine = myFile.GetTagValues("P_LINE", ", ", false);
            myTagData.CLine = myFile.GetTagValues("C_LINE", ", ", false);

            myTagData.Venue = myFile.GetTagValues("VENUE", ", ", false);
            myTagData.Location = myFile.GetTagValues("LOCATION", ", ", false);
            myTagData.RecordingDate = myFile.GetTagValues("DATE", ", ", false);

            myTagData.MixingVenue = myFile.GetTagValues("MIXINGVENUE", ", ", false);
            myTagData.MixingCity = myFile.GetTagValues("MIXINGCITY", ", ", false);
            myTagData.MixingDate = myFile.GetTagValues("MIXINGDATE", ", ", false);

            myTagData.MasteringVenue = myFile.GetTagValues("MASTERINGVENUE", ", ", false);
            myTagData.MasteringCity = myFile.GetTagValues("MASTERINGCITY", ", ", false);
            myTagData.MasteringDate = myFile.GetTagValues("MASTERINGDATE", ", ", false);

            myTagData.RemixVenue = myFile.GetTagValues("REMIXVENUE", ", ", false);
            myTagData.RemixCity = myFile.GetTagValues("REMIXCITY", ", ", false);
            myTagData.RemixDate = myFile.GetTagValues("REMIXDATE", ", ", false);

            myTagData.RemasterVenue = myFile.GetTagValues("REMASTERVENUE", ", ", false);
            myTagData.RemasterCity = myFile.GetTagValues("REMASTERCITY", ", ", false);
            myTagData.RemasterDate = myFile.GetTagValues("REMASTERDATE", ", ", false);

            int trackNum;
            int numTracks;
            myTagData.Band = myFile.GetTagValues("BAND", ", ", false);
            string trackString = myFile.GetTagValues("TRACKNUMBER", ", ", false);
            Util.ParseTrackOrDisc(trackString, out trackNum, out numTracks);
            myTagData.TrackNum = trackNum;
            myTagData.TrackCount = numTracks;

            int discNum;
            int numDiscs;
            string discString = myFile.GetTagValues("DISCNUMBER", ", ", false);
            Util.ParseTrackOrDisc(discString, out discNum, out numDiscs);
            myTagData.DiscNum = discNum;
            myTagData.DiscCount = numDiscs;

            /*Dictionary<string, string> mMediaTypes = DecibelAudioTools.DataReader.MediaTypeByCode;
            myTagData.MediaTypeCode = myFile.GetTagValues("MEDIATYPE", ", ", false);
            if (myTagData.MediaTypeCode == string.Empty)
                myTagData.MediaType = string.Empty;
            else
            {
                if (mMediaTypes.ContainsKey(myTagData.MediaTypeCode))
                    myTagData.MediaType = mMediaTypes[myTagData.MediaTypeCode];
            }*/

            string liveString = myFile.GetTagValues("IS_LIVE", ", ", false).ToUpper();
            if (liveString == "TRUE")
                myTagData.IsLive = true;
            else
                myTagData.IsLive = false;

            string bonusString = myFile.GetTagValues("IS_BONUS", ", ", false).ToUpper();
            if (bonusString == "TRUE")
                myTagData.IsBonus = true;
            else
                myTagData.IsBonus = false;
        }

        public static void RetriveFileTags(MpegAudio mpegInfo, ID3v2 myFile, TagData myTagData)
        {
            try
            {
                string bitRateString = mpegInfo.BitRate.ToString() + " kbps ";
                if (mpegInfo.IsVbr)
                    bitRateString += "(VBR)";
                else
                    bitRateString += "(CBR)";
                string trackTimeString = mpegInfo.Length;
                myTagData.BitRate = bitRateString;
                myTagData.TrackTimeString = trackTimeString;
                myTagData.TrackTime = mpegInfo.ExactLengthInSeconds;
            }
            catch
            {
                myTagData.BitRate = "Error";
                myTagData.TrackTimeString = "Error";
                myTagData.TrackTime = -1;
            }

            //txxx = myFile.GetMappedTagName("TXXX");
            myTagData.Artist = myFile.GetText(myFile.GetMappedTagName("TPE1"));
            myTagData.AlbumTitle = myFile.GetText(myFile.GetMappedTagName("TALB"));
            myTagData.DiscTitle = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Disc Title");
            myTagData.Title = myFile.GetText(myFile.GetMappedTagName("TIT2"));
            myTagData.Works = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Works");
            myTagData.Year = myFile.GetText(myFile.GetMappedTagName("TYER"));
            myTagData.PLine = myFile.GetText(myFile.GetMappedTagName("TXXX"), "P-Line");
            myTagData.CLine = myFile.GetText(myFile.GetMappedTagName("TXXX"), "C-Line");

            myTagData.Venue = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Recording Venue");
            myTagData.Location = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Recording Location");
            myTagData.RecordingDate = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Recording Date");

            myTagData.MixingVenue = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Mixing Venue");
            myTagData.MixingCity = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Mixing City");
            myTagData.MixingDate = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Mixing Date");

            myTagData.MasteringVenue = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Mastering Venue");
            myTagData.MasteringCity = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Mastering City");
            myTagData.MasteringDate = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Mastering Date");

            myTagData.RemixVenue = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Remix Venue");
            myTagData.RemixCity = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Remix City");
            myTagData.RemixDate = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Remix Date");

            myTagData.RemasterVenue = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Remaster Venue");
            myTagData.RemasterCity = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Remaster City");
            myTagData.RemasterDate = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Remaster Date");

            int trackNum;
            int numTracks;
            string trackString = myFile.GetText(myFile.GetMappedTagName("TRCK"));
            Util.ParseTrackOrDisc(trackString, out trackNum, out numTracks);
            myTagData.TrackNum = trackNum;
            myTagData.TrackCount = numTracks;

            int discNum;
            int numDiscs;
            string discString = myFile.GetText(myFile.GetMappedTagName("TPOS"));
            Util.ParseTrackOrDisc(discString, out discNum, out numDiscs);
            myTagData.DiscNum = discNum;
            myTagData.DiscCount = numDiscs;

            Dictionary<string, string> mMediaTypes = DecibelAudioTools.DataReader.MediaTypeByCode;
            myTagData.MediaTypeCode = myFile.GetText(myFile.GetMappedTagName("TMED"));
            if (myTagData.MediaTypeCode == string.Empty)
                myTagData.MediaType = string.Empty;
            else
            {
                if (mMediaTypes.ContainsKey(myTagData.MediaTypeCode))
                    myTagData.MediaType = mMediaTypes[myTagData.MediaTypeCode];
            }
            myTagData.Cddb = myFile.GetText(myFile.GetMappedTagName("TXXX"), "CDDB ID");
            myTagData.Isrc = myFile.GetText(myFile.GetMappedTagName("TSRC"));
            myTagData.Spars = myFile.GetText(myFile.GetMappedTagName("TXXX"), "SPARS");
            myTagData.AlbumBarCode = myFile.GetText(myFile.GetMappedTagName("TXXX"), "UPC Bar Code");
            myTagData.DiscBarCode = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Disc Bar Code");
            myTagData.LabelName = myFile.GetText(myFile.GetMappedTagName("TPUB"));
            myTagData.CatalogueNumber = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Album Catalogue Number");
            myTagData.DiscCatalogueNumber = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Disc Catalogue Number");
            myTagData.Genre = myFile.GetText(myFile.GetMappedTagName("TCON"));
            myTagData.Comment = myFile.GetDefaultCommentText();
            myTagData.Notes = myFile.GetText(myFile.GetMappedTagName("COMM"), "Notes", mDefaultLanguageCode);
            myTagData.Band = myFile.GetText(myFile.GetMappedTagName("TPE2"));
            myTagData.Authors = myFile.GetText(myFile.GetMappedTagName("TCOM"));
            myTagData.Publishers = myFile.GetText(myFile.GetMappedTagName("TXXX"), "WorkPublisher");
            myTagData.OriginalAlbum = myFile.GetText(myFile.GetMappedTagName("TOAL"));
            myTagData.OriginalLabel = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Original Label");
            myTagData.OriginalCatalogueNumber = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Original Catalogue Number");
            myTagData.Matrix = myFile.GetText(myFile.GetMappedTagName("TXXX"), "Matrix Number");
            myTagData.Puid = myFile.GetText(myFile.GetMappedTagName("TXXX"), "MusicDNS Puid");

            discString = myFile.GetText(myFile.GetMappedTagName("TPOS"));
            Util.ParseTrackOrDisc(discString, out discNum, out numDiscs);
            myTagData.DiscNum = discNum;
            myTagData.DiscCount = numDiscs;

            string liveString = myFile.GetText(myFile.GetMappedTagName("TXXX"), "IsLive");
            if (liveString == "True")
                myTagData.IsLive = true;
            else
                myTagData.IsLive = false;

            string bonusString = myFile.GetText(myFile.GetMappedTagName("TXXX"), "IsBonus");
            if (bonusString == "True")
                myTagData.IsBonus = true;
            else
                myTagData.IsBonus = false;

            AudioFileFrame iplsFrame = myFile.Frames[myFile.GetMappedTagName("IPLS")];
            if (iplsFrame != null)
            {
                myTagData.Participants = iplsFrame.InvolvedPeople;
            }
            else
            {
                myTagData.Participants = new PersonalInvolvementList();
            }
        }

        public static void SetFileTags(FlacFile myFile, TagData myTagData)
        {


            myFile.SetComment("COMMENT", myTagData.Comment, false);
            myFile.SetComment("NOTES", myTagData.Notes, false);
            //myFile.SetComment("TRACKNUMBER", myTagData, false); - myTagData.trackString
            //myFile.SetComment("DISCNUMBER", myTagData, false); - myTagData.discSteing
            myFile.SetComment("ARTIST", myTagData.Artist, false);
            myFile.SetComment("ALBUM", myTagData.AlbumTitle, false);
            myFile.SetComment("DISCTITLE", myTagData.DiscTitle, false);
            myFile.SetComment("TITLE", myTagData.Title, false);
            myFile.SetComment("SUBTITLE", myTagData.Subtitle, false);
            myFile.SetComment("PIECES", myTagData.Works, false);
            myFile.SetComment("YEAR", myTagData.Year, false);
            myFile.SetComment("P_LINE", myTagData.PLine, false);
            myFile.SetComment("C_LINE", myTagData.CLine, false);
            myFile.SetComment("VENUE", myTagData.Venue, false);
            myFile.SetComment("LOCATION", myTagData.Location, false);
            myFile.SetComment("DATE", myTagData.RecordingDate, false);
            myFile.SetComment("MIXINGVENUE", myTagData.MasteringVenue, false);
            myFile.SetComment("MIXINGCITY", myTagData.MasteringCity, false);
            myFile.SetComment("MIXINGDATE", myTagData.MixingDate, false);
            myFile.SetComment("REMIXVENUE", myTagData.RemixVenue, false);
            myFile.SetComment("REMIXCITY", myTagData.RemixCity, false);
            myFile.SetComment("REMIXDATE", myTagData.RemixDate, false);
            myFile.SetComment("REMASTERVENUE", myTagData.RemasterVenue, false);
            myFile.SetComment("REMASTERCITY", myTagData.RemasterCity, false);
            myFile.SetComment("REMASTERDATE", myTagData.RemasterDate, false);
            myFile.SetComment("BAND", myTagData.Band, false);
            myFile.SetComment("TRACKNUMBER", myTagData.TrackNum.ToString(), false);
            myFile.SetComment("DISCNUMBER", myTagData.DiscNum.ToString(), false);
            //myFile.SetComment("MEDIATYPE", myTagData, false); myTagData.mediatype
            myFile.SetComment("IS_LIVE", myTagData.IsLive.ToString(), false);
            myFile.SetComment("IS_BONUS", myTagData.IsBonus.ToString(), false);
            myFile.SetComment("CDDB", myTagData.Cddb, false);
            myFile.SetComment("ISRC", myTagData.Isrc, false);
            myFile.SetComment("SPARS", myTagData.Spars, false);
            myFile.SetComment("BARCODE", myTagData.AlbumBarCode, false);
            myFile.SetComment("DISC_BARCODE", myTagData.DiscBarCode, false);
            myFile.SetComment("ORGANIZATION", myTagData.LabelName, false);
            myFile.SetComment("CATALOGUENUMBER", myTagData.CatalogueNumber, false);
            myFile.SetComment("DISC_CATALOGUENUMBER", myTagData.DiscCatalogueNumber, false);
            myFile.SetComment("GENRE", myTagData.Genre, false);
            myFile.SetComment("COMMENT", myTagData.Comment, false);
            myFile.SetComment("NOTES", myTagData.Notes, false);
            myFile.SetComment("WORKPUBLISHER", myTagData.Publishers, false);
            myFile.SetComment("COMPOSER", myTagData.Authors, false);
            myFile.SetComment("ORIGINAL_ALBUM", myTagData.OriginalAlbum, false);
            myFile.SetComment("ORIGINAL_LABEL", myTagData.OriginalLabel, false);
            myFile.SetComment("ORIGINAL_CATALOGUENUMBER", myTagData.OriginalCatalogueNumber, false);
            myFile.SetComment("MATRIXNUMBER", myTagData.Matrix, false);
            myFile.SetComment("MUSICDNS_PUID", myTagData.Puid, false);
        }

        public static void SetFileTags(OggFile myFile, TagData myTagData, FlacPictureBlock myPictureBlock)
        {
            PersonalInvolvementList newList = myTagData.Participants;
            string newListStr = newList.ToString(true, ", ", "; ", ".");
            myFile.SetComment("INVOLVEMENTS", newListStr, false);

            myFile.SetComment("COMMENT", myTagData.Comment, false);
            myFile.SetComment("NOTES", myTagData.Notes, false);
            //myFile.SetComment("TRACKNUMBER", myTagData, false); - myTagData.trackString
            //myFile.SetComment("DISCNUMBER", myTagData, false); - myTagData.discSteing
            myFile.SetComment("ARTIST", myTagData.Artist, false);
            myFile.SetComment("ALBUM", myTagData.AlbumTitle, false);
            myFile.SetComment("DISCTITLE", myTagData.DiscTitle, false);
            myFile.SetComment("TITLE", myTagData.Title, false);
            myFile.SetComment("SUBTITLE", myTagData.Subtitle, false);
            myFile.SetComment("PIECES", myTagData.Works, false);
            myFile.SetComment("YEAR", myTagData.Year, false);
            myFile.SetComment("P_LINE", myTagData.PLine, false);
            myFile.SetComment("C_LINE", myTagData.CLine, false);
            myFile.SetComment("VENUE", myTagData.Venue, false);
            myFile.SetComment("LOCATION", myTagData.Location, false);
            myFile.SetComment("DATE", myTagData.RecordingDate, false);
            myFile.SetComment("MIXINGVENUE", myTagData.MasteringVenue, false);
            myFile.SetComment("MIXINGCITY", myTagData.MasteringCity, false);
            myFile.SetComment("MIXINGDATE", myTagData.MixingDate, false);
            myFile.SetComment("REMIXVENUE", myTagData.RemixVenue, false);
            myFile.SetComment("REMIXCITY", myTagData.RemixCity, false);
            myFile.SetComment("REMIXDATE", myTagData.RemixDate, false);
            myFile.SetComment("REMASTERVENUE", myTagData.RemasterVenue, false);
            myFile.SetComment("REMASTERCITY", myTagData.RemasterCity, false);
            myFile.SetComment("REMASTERDATE", myTagData.RemasterDate, false);
            myFile.SetComment("BAND", myTagData.Band, false);
            //myFile.SetComment("TRACKNUMBER", myTagData.TrackNum.ToString(), false);
            //myFile.SetComment("DISCNUMBER", myTagData.DiscNum.ToString(), false);
            //myFile.SetComment("MEDIATYPE", myTagData, false); myTagData.mediatype
            //myFile.SetComment("IS_LIVE", myTagData.IsLive.ToString(), false);
            //myFile.SetComment("IS_BONUS", myTagData.IsBonus.ToString(), false);
            myFile.SetComment("CDDB", myTagData.Cddb, false);
            myFile.SetComment("ISRC", myTagData.Isrc, false);
            myFile.SetComment("SPARS", myTagData.Spars, false);
            myFile.SetComment("BARCODE", myTagData.AlbumBarCode, false);
            myFile.SetComment("DISC_BARCODE", myTagData.DiscBarCode, false);
            myFile.SetComment("ORGANIZATION", myTagData.LabelName, false);
            myFile.SetComment("CATALOGUENUMBER", myTagData.CatalogueNumber, false);
            myFile.SetComment("DISC_CATALOGUENUMBER", myTagData.DiscCatalogueNumber, false);
            myFile.SetComment("GENRE", myTagData.Genre, false);
            myFile.SetComment("COMMENT", myTagData.Comment, false);
            myFile.SetComment("NOTES", myTagData.Notes, false);
            myFile.SetComment("WORKPUBLISHER", myTagData.Publishers, false);
            myFile.SetComment("COMPOSER", myTagData.Authors, false);
            myFile.SetComment("ORIGINAL_ALBUM", myTagData.OriginalAlbum, false);
            myFile.SetComment("ORIGINAL_LABEL", myTagData.OriginalLabel, false);
            myFile.SetComment("ORIGINAL_CATALOGUENUMBER", myTagData.OriginalCatalogueNumber, false);
            myFile.SetComment("MATRIXNUMBER", myTagData.Matrix, false);
            myFile.SetComment("MUSICDNS_PUID", myTagData.Puid, false);

            /*if (myPictureBlock != null)
            {
                if (myPictureBlock.RawData != null)
                {
                    string myPicture = System.Convert.ToBase64String(myPictureBlock.RawData);
                    myFile.SetComment("METADATA_BLOCK_PICTURE", myPicture, false);
                }
            }*/
        }
    }
}

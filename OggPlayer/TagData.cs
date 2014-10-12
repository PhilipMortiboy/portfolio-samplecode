using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecibelAudioTools;

namespace OggPlayer
{
    class TagData
    {
        public string BitRate;
        public string TrackTimeString;
        public decimal TrackTime;
        public string Artist;
        public string AlbumTitle;
        public string DiscTitle;
        public string Title;
        public string Subtitle;
        public string Works;
        public string Year;
        public string PLine;
        public string CLine;
        public string Venue;
        public string Location;
        public string RecordingDate;
        public string Band;
        public int TrackNum;
        public int TrackCount;
        public string MediaTypeCode;
        public string MediaType;
        public bool IsLive;
        public bool IsBonus;
        public string Cddb;
        public string Isrc;
        public string Spars;
        public string AlbumBarCode;
        public string DiscBarCode;
        public string LabelName;
        public string CatalogueNumber;
        public string DiscCatalogueNumber;
        public string Genre;
        public string Comment;
        public string Notes;
        public string Authors;
        public string Publishers;
        public string OriginalAlbum;
        public string OriginalLabel;
        public string OriginalCatalogueNumber;
        public string Matrix;
        public string Puid;
        public PersonalInvolvementList Participants;
        public string MixingVenue;
        public string MixingCity;
        public string MixingDate;
        public string MasteringVenue;
        public string MasteringCity;
        public string MasteringDate;
        public string RemixVenue;
        public string RemixCity;
        public string RemixDate;
        public string RemasterVenue;
        public string RemasterCity;
        public string RemasterDate;
        public int DiscNum;
        public int DiscCount;
        public System.Drawing.Image CoverImg;

        public string CreateTagString(int filetype)
        {
            string tagString = null;
            switch(filetype)
            {
                case 1:
                    tagString = (
                        "Bit rate: " + BitRate +
                        "\nTrackTime: " + TrackTime +
                        "\nArtist: " + Artist +
                        "\nAlbumTitle: " + AlbumTitle +
                        "\nDiscTitle: " + DiscTitle +
                        "\nTitle: " + Title +
                        "\nSubtitle: " + Subtitle +
                        "\nWorks: " + Works +
                        "\nYear: " + Year +
                        "\nPLine: " + PLine +
                        "\nCLine: " + CLine +
                        "\nVenue: " + Venue +
                        "\nLocation: " + Location +
                        "\nRecordingDate: " + RecordingDate +
                        "\nBand: " + Band +
                        "\nTrackNum: " + TrackNum +
                        "\nTrackCount: " + TrackCount +
                        "\nMediaTypeCode: " + MediaTypeCode +
                        "\nMediaType: " + MediaType +
                        "\nIsLive: " + IsLive +
                        "\nIsBonus: " + IsBonus +
                        "\nCddb: " + Cddb +
                        "\nIsrc: " + Isrc +
                        "\nSpars: " + Spars +
                        "\nAlbumBarCode: " + AlbumBarCode +
                        "\nDiscBarCode: " + DiscBarCode +
                        "\nLabelName: " + LabelName +
                        "\nCatalogueNumber: " + CatalogueNumber +
                        "\nDiscCatalogueNumber: " + DiscCatalogueNumber +
                        "\nGenre: " + Genre +
                        "\nComment: " + Comment +
                        "\nNotes: " + Notes +
                        "\nAuthors: " + Authors +
                        "\nPublishers: " + Publishers +
                        "\nOriginalAlbum: " + OriginalAlbum +
                        "\nOriginalLabel: " + OriginalLabel +
                        "\nOriginalCatalogueNumber: " + OriginalCatalogueNumber +
                        "\nMatrix: " + Matrix +
                        "\nPuid: " + Puid
                        );
                    break;
                case 2:
                    tagString = (
                        "Bit rate: " + BitRate +
                        "\nTrackTime: " + TrackTime +
                        "\nArtist: " + Artist +
                        "\nAlbumTitle: " + AlbumTitle +
                        "\nDiscTitle: " + DiscTitle +
                        "\nTitle: " + Title +
                        "\nSubtitle: " + Subtitle +
                        "\nWorks: " + Works +
                        "\nYear: " + Year +
                        "\nPLine: " + PLine +
                        "\nCLine: " + CLine +
                        "\nVenue: " + Venue +
                        "\nLocation: " + Location +
                        "\nRecordingDate: " + RecordingDate +
                        "\nBand: " + Band +
                        "\nTrackNum: " + TrackNum +
                        "\nTrackCount: " + TrackCount +
                        "\nMediaTypeCode: " + MediaTypeCode +
                        "\nMediaType: " + MediaType +
                        "\nIsLive: " + IsLive +
                        "\nIsBonus: " + IsBonus +
                        "\nCddb: " + Cddb +
                        "\nIsrc: " + Isrc +
                        "\nSpars: " + Spars +
                        "\nAlbumBarCode: " + AlbumBarCode +
                        "\nDiscBarCode: " + DiscBarCode +
                        "\nLabelName: " + LabelName +
                        "\nCatalogueNumber: " + CatalogueNumber +
                        "\nDiscCatalogueNumber: " + DiscCatalogueNumber +
                        "\nGenre: " + Genre +
                        "\nComment: " + Comment +
                        "\nNotes: " + Notes +
                        "\nAuthors: " + Authors +
                        "\nPublishers: " + Publishers +
                        "\nOriginalAlbum: " + OriginalAlbum +
                        "\nOriginalLabel: " + OriginalLabel +
                        "\nOriginalCatalogueNumber: " + OriginalCatalogueNumber +
                        "\nMatrix: " + Matrix +
                        "\nPuid: " + Puid
                        );
                    break;
                case 3:
                    tagString = "mp3 files not yet supported";
                    break;
            }

            return tagString;
        }
    }
}

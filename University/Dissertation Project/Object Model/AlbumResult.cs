using System;

namespace ObjectModel
{
    public class AlbumResult : ObjectResult
    {
        public ImageResult[] Images{ get; set; }
        public ImageResult CoverImg { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public string ArtistName { get; set; }
        public string EventVenue { get; set; }
        public bool UserFave { get; set; }
        public EventResult[] EventOptions { get; set; }
    }
}
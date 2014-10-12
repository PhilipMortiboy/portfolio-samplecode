using System;

namespace ObjectModel
{
    public class ImageResult : ObjectResult
    {
        public string UserID { get; set; }
        public int Rank { get; set; }
        public byte[] rawData { get; set; }
        public string Url { get; set; }
        public string Rating { get; set; }
        public string Flags { get; set; }
        public bool? UserRating { get; set; }
        public bool UserFlagged { get; set; }
        public bool UserFave { get; set; }
        public EventResult[] EventOptions { get; set; }
    }
}
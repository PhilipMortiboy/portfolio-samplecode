using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectModel
{
    [Serializable]
    public class ImageRequest : ObjectRequest
    {
        public int UserID { get; set; }
        public int Rank { get; set; }
        public int Flag { get; set; }
        public string LatLng { get; set; }
        public DateTime Taken { get; set; }
        public string CameraDetails { get; set; }
        public byte[] RawData { get; set; }
        public string Url { get; set; }
        public int AlbumID { get; set; }
    }
}

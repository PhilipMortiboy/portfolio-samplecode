using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectModel
{
    public class EventResult : ObjectResult
    {
        public string Title { get; set; }
        public string VenueName { get; set; }
        public string Date { get; set; }
    }
}

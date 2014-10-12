using System;

namespace ObjectModel
{
    public class ObjectResult
    {
        public string Id { get; set; }
        public string Response { get; set; }
        public string ErrorMsg { get; set; }
        public bool Error { get; set; }
    }
}

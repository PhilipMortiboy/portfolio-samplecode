using System;

namespace ObjectModel
{
    public class UserResult : ObjectResult
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TimeZone { get; set; }
    }
}
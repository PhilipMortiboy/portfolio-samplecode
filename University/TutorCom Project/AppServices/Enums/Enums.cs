using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Enums
{
    public enum Order
    {
        ByDateAsc,
        ByDateDesc,
        Alphabetical,
        ItemType
    }

    public enum UserType
    {
        Student,
        Tutor
    }

    public enum MessageBox
    {
        Inbox,
        Sentbox
    }

    public enum ItemType
    {
        Message,
        Blog,
        Meeting,
        BlogComment,
        FileUpload,
        FileComment,
        MeetingAttended,
        MeetingRequest,
        MeetingNotAttended,
        MeetingRejected,
        MeetingAccepted,
    }

    public enum Days
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday
    }

    public enum MeetingStatus
    {
        Wating,
        Confirmed,
        Rejected,
        Past
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppServices.Enums;

namespace AppServices.Results
{
    public class DashboardResult : Dashboard
    {
        private bool error = false;
        private string errorMsg = "";
        private ItemType itemType;
        private string url;

        #region Attributes
        public bool Error
        {
            get { return error; }
        }
        public string ErrorMsg
        {
            get { return errorMsg; }
        }
        public ItemType ItemType
        {
            get { return itemType; }
        }
        public string Url
        {
            get { return url; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public DashboardResult()
        {
        }

        /// <summary>
        /// Create a new dashboard result from a dashboard
        /// </summary>
        /// <param name="b">The blog to convert</param>
        public DashboardResult(Dashboard d)
        {
            dId = d.dId;
            dUId = d.dUId;
            dItemID = d.dItemID;
            dTimestamp = d.dTimestamp;
            dNotification = d.dNotification;
            dViewed = d.dViewed;
            itemType = (ItemType)d.dItemType; //need to check this works
            url = GenerateUrl((ItemType)d.dItemType, d.dItemID);
        }
        /// <summary>
        /// Create a error blog result
        /// </summary>
        /// <param name="msg">The error message</param>
        public DashboardResult(string msg)
        {
            SetError(msg);
        }
        #endregion

        /// <summary>
        /// Set an error notification for this blog result - probably don't need this
        /// </summary>
        /// <param name="msg">The error message to send</param>
        public void SetError(string msg)
        {
            error = true;
            errorMsg = msg;
        }

        private string GenerateUrl(ItemType type, int? id)
        {
            string myUrl = "";
            switch (type)
            {
                case ItemType.Blog:
                    myUrl = myUrl + "ViewBlog.aspx?id=" + id;
                    break;
                case ItemType.BlogComment:
                    myUrl = myUrl + "ViewBlog.aspx?id=" + id;
                    break;
                case ItemType.FileComment:
                    myUrl = myUrl + "ViewFile.aspx?id=" + id;
                    break;
                case ItemType.FileUpload:
                    myUrl = myUrl + "ViewFiles.aspx";
                    break;
                case ItemType.Meeting:
                    myUrl = myUrl + "MeetingsLog.aspx";
                    break;
                case ItemType.Message:
                    myUrl = myUrl + "MessageInbox.aspx";
                    break;
                case ItemType.MeetingRequest:
                    myUrl = myUrl + "MeetingsLog.aspx";
                    break;
                case ItemType.MeetingAccepted:
                    myUrl = myUrl + "MeetingsLog.aspx";
                    break;
                case ItemType.MeetingRejected:
                    myUrl = myUrl + "MeetingsLog.aspx";
                    break;
                case ItemType.MeetingAttended:
                    myUrl = myUrl + "MeetingMinutes.aspx?id=" + id;
                    break;
                case ItemType.MeetingNotAttended:
                    myUrl = myUrl + "MeetingMinutes.aspx?id=" + id;
                    break;
                default:
                    myUrl = myUrl + "Error";
                    break;
            }
            return myUrl;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Results
{
    public class MessageResultSet
    {
        List<List<MessageResult>> pages;
        bool baseError = false;
        string baseErrorMsg = "";

        #region Attributes
        public List<List<MessageResult>> Pages
        {
            get { return pages; }
            set { pages = value; }
        }
        public bool BaseError
        {
            get { return baseError; }
            set { baseError = value; }
        }
        public string BaseErrorMsg
        {
            get { return baseErrorMsg; }
            set { baseErrorMsg = value; }
        }
        #endregion

        #region Constructors
        public MessageResultSet(List<MessageResult> myResults)
        {
            pages = Util.GenerateResultSet(myResults, 10);
            if (pages.Count == 0)
                SetError("No messages were found");  
        }

        public MessageResultSet(List<MessageResult> myResults, int perPage)
        {
            pages = Util.GenerateResultSet(myResults, perPage);
            if (pages.Count == 0)
                SetError("No messages were found");  
        }

        public MessageResultSet(string msg)
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
            baseError = true;
            baseErrorMsg = msg;
        }
    }

    //A set of BlogResult
    public class BlogResultSet
    {
        List<List<BlogResult>> pages;
        bool baseError = false;
        string baseErrorMsg = "";

        #region Attributes
        public List<List<BlogResult>> Pages
        {
            get { return pages; }
            set { pages = value; }
        }
        public bool BaseError
        {
            get { return baseError; }
            set { baseError = value; }
        }
        public string BaseErrorMsg
        {
            get { return baseErrorMsg; }
            set { baseErrorMsg = value; }
        }
        #endregion

        #region Constructors
        public BlogResultSet(List<BlogResult> myResults)
        {
            pages = Util.GenerateResultSet(myResults, 10);
            if(pages.Count == 0)
              SetError("No blogs were found");  
        }

        public BlogResultSet(List<BlogResult> myResults, int perPage)
        {
            pages = Util.GenerateResultSet(myResults, perPage);
            if (pages.Count == 0)
                SetError("No blogs were found"); 
        }
        
        public BlogResultSet(string msg)
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
            baseError = true;
            baseErrorMsg = msg;
        }
    }

    /// <summary>
    /// A set of FileResults
    /// </summary>
    public class FileResultSet
    {
        List<List<FileResult>> pages;
        bool baseError = false;
        string baseErrorMsg = "";

        #region Attributes
        public List<List<FileResult>> Pages
        {
            get { return pages; }
            set { pages = value; }
        }
        public bool BaseError
        {
            get { return baseError; }
            set { baseError = value; }
        }
        public string BaseErrorMsg
        {
            get { return baseErrorMsg; }
            set { baseErrorMsg = value; }
        }
        #endregion

        #region Constructors
        public FileResultSet(List<FileResult> myResults)
        {
            pages = Util.GenerateResultSet(myResults, 10);
            if (pages.Count == 0)
                SetError("No blogs were found");
        }

        public FileResultSet(List<FileResult> myResults, int perPage)
        {
            pages = Util.GenerateResultSet(myResults, perPage);
            if (pages.Count == 0)
                SetError("No blogs were found");
        }

        public FileResultSet(string msg)
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
            baseError = true;
            baseErrorMsg = msg;
        }
    }

    /// <summary>
    /// A set of ModuleResults
    /// </summary>
    public class MeetingResultSet
    {
        List<List<MeetingResult>> pages;
        bool baseError = false;
        string baseErrorMsg = "";

        #region Attributes
        public List<List<MeetingResult>> Pages
        {
            get { return pages; }
            set { pages = value; }
        }
        public bool BaseError
        {
            get { return baseError; }
            set { baseError = value; }
        }
        public string BaseErrorMsg
        {
            get { return baseErrorMsg; }
            set { baseErrorMsg = value; }
        }
        #endregion

        #region Constructors
        public MeetingResultSet(List<MeetingResult> myResults)
        {
            pages = Util.GenerateResultSet(myResults, 10);
            if (pages.Count == 0)
                SetError("No blogs were found");
        }

        public MeetingResultSet(List<MeetingResult> myResults, int perPage)
        {
            pages = Util.GenerateResultSet(myResults, perPage);
            if (pages.Count == 0)
                SetError("No blogs were found");
        }

        public MeetingResultSet(string msg)
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
            baseError = true;
            baseErrorMsg = msg;
        }
    }

    /// <summary>
    /// A set of MeetingSlotResults
    /// </summary>
    public class MeetingSlotResultSet
    {
        List<List<MeetingSlotResult>> pages;
        bool baseError = false;
        string baseErrorMsg = "";

        #region Attributes
        public List<List<MeetingSlotResult>> Pages
        {
            get { return pages; }
            set { pages = value; }
        }
        public bool BaseError
        {
            get { return baseError; }
            set { baseError = value; }
        }
        public string BaseErrorMsg
        {
            get { return baseErrorMsg; }
            set { baseErrorMsg = value; }
        }
        #endregion

        #region Constructors
        public MeetingSlotResultSet(List<MeetingSlotResult> myResults)
        {
            pages = Util.GenerateResultSet(myResults, 10);
            if (pages.Count == 0)
                SetError("No blogs were found");
        }

        public MeetingSlotResultSet(List<MeetingSlotResult> myResults, int perPage)
        {
            pages = Util.GenerateResultSet(myResults, perPage);
            if (pages.Count == 0)
                SetError("No blogs were found");
        }

        public MeetingSlotResultSet(string msg)
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
            baseError = true;
            baseErrorMsg = msg;
        }
    }

    /// <summary>
    /// A set of ModuleResults
    /// </summary>
    public class ModuleResultSet
    {
        List<List<ModuleResult>> pages;
        bool baseError = false;
        string baseErrorMsg = "";

        #region Attributes
        public List<List<ModuleResult>> Pages
        {
            get { return pages; }
            set { pages = value; }
        }
        public bool BaseError
        {
            get { return baseError; }
            set { baseError = value; }
        }
        public string BaseErrorMsg
        {
            get { return baseErrorMsg; }
            set { baseErrorMsg = value; }
        }
        #endregion

        #region Constructors
        public ModuleResultSet(List<ModuleResult> myResults)
        {
            pages = Util.GenerateResultSet(myResults, 10);
            if (pages.Count == 0)
                SetError("No blogs were found");
        }

        public ModuleResultSet(List<ModuleResult> myResults, int perPage)
        {
            pages = Util.GenerateResultSet(myResults, perPage);
            if (pages.Count == 0)
                SetError("No blogs were found");
        }

        public ModuleResultSet(string msg)
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
            baseError = true;
            baseErrorMsg = msg;
        }
    }

    /// <summary>
    /// A set of User Results. Can contain either modules or just users
    /// </summary>
    public class UserResultSet
    {
        List<List<UserResult>> pages;
        List<UserResult> users;
        List<ModuleResult> modules;
        bool baseError = false;
        string baseErrorMsg = "";

        #region Attributes
        public List<List<UserResult>> Pages
        {
            get { return pages; }
            set { pages = value; }
        }
        public List<UserResult> Users
        {
            get { return users; }
            set { users = value; }
        }
        public List<ModuleResult> Modules
        {
            get { return modules; }
            set { modules = value; }
        }
        public bool BaseError
        {
            get { return baseError; }
            set { baseError = value; }
        }
        public string BaseErrorMsg
        {
            get { return baseErrorMsg; }
            set { baseErrorMsg = value; }
        }
        #endregion

        #region Constructors
        public UserResultSet(List<UserResult> myResults, int perPage)
        {
            pages = Util.GenerateResultSet(myResults, perPage);
            if (pages.Count == 0)
                SetError("No users were found");
        }
        public UserResultSet(List<UserResult> myResults)
        {
            users = myResults;
            if(users.Count == 0)
              SetError("No users were found");  
        }

        public UserResultSet(List<ModuleResult> myResults)
        {
            modules = myResults;
            if (users.Count == 0)
                SetError("No modules were found"); 
        }

        public UserResultSet(string msg)
        {
            SetError(msg);
        }
        #endregion

        /// <summary>
        /// Set an error notification dashboard result
        /// </summary>
        /// <param name="msg">The error message to send</param>
        public void SetError(string msg)
        {
            baseError = true;
            baseErrorMsg = msg;
        }
    }

    public class DashboardResultSet
    {
        List<List<DashboardResult>> notifications;
        bool baseError = false;
        string baseErrorMsg = "";

        #region Attributes
        public List<List<DashboardResult>> Notifications
        {
            get { return notifications; }
            set { notifications = value; }
        }
        public bool BaseError
        {
            get { return baseError; }
            set { baseError = value; }
        }
        public string BaseErrorMsg
        {
            get { return baseErrorMsg; }
            set { baseErrorMsg = value; }
        }
        #endregion

        #region Constructors
        public DashboardResultSet(List<DashboardResult> myResults)
        {
            notifications = Util.GenerateResultSet(myResults, 10);
            if(notifications.Count == 0)
              SetError("No notifications were found");  
        }

        public DashboardResultSet(List<DashboardResult> myResults, int perPage)
        {
            notifications = Util.GenerateResultSet(myResults, perPage);
            if (notifications.Count == 0)
                SetError("No notifications were found"); 
        }

        public DashboardResultSet(string msg)
        {
            SetError(msg);
        }
        #endregion

        /// <summary>
        /// Set an error notification dashboard result
        /// </summary>
        /// <param name="msg">The error message to send</param>
        public void SetError(string msg)
        {
            baseError = true;
            baseErrorMsg = msg;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Results
{
    public class MeetingResult : Meeting
    {
        private bool error = false;
        private string errorMsg = "";
        private int msDay;
        private int msSlot;

        #region Attributes
        public bool Error
        {
            get { return error; }
        }
        public string ErrorMsg
        {
            get { return errorMsg; }
        }
        public int MsDay
        {
            get { return msDay; }
            set { msSlot = value; }
        }
        public int MsSlot
        {
            get { return msSlot; }
            set { msSlot = value; }
        }
        //formatted data to display on site
        public string msDayStr { get; set; }
        public string msTimeStr { get; set; }
        public string msTutorNameStr { get; set; }
        public string msRoom { get; set; }
        public string msStudentNameStr { get; set; }
        public string msStudentCommentsStr { get; set; }
        public string msTutorCommentsStr { get; set; }
        public string msAttendedStr { get; set; }
        #endregion

         #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public MeetingResult()
        {
        }

        /// <summary>
        /// Create a new Meeting result from a blog
        /// </summary>
        /// <param name="b">The blog to convert</param>
        public MeetingResult(Meeting m)
        {
            mAttended = m.mAttended;
            mId = m.mId;
            mMsId = m.mMsId;
            mPreviousRequestID = m.mPreviousRequestID;
            mRequestStatus = m.mRequestStatus;
            mSId = m.mSId;
            mStudentMinutes = m.mStudentMinutes;
            mTutorMinutes = m.mTutorMinutes;
            mWeek = m.mWeek;
            if(m.mStudentMinutes != null)
                msStudentCommentsStr = Util.ConvertToString(m.mStudentMinutes);
            if(m.mTutorMinutes != null)
                msTutorCommentsStr = Util.ConvertToString(m.mTutorMinutes);
        }

        /// <summary>
        /// Create a error blog result
        /// </summary>
        /// <param name="msg">The error message</param>
        public MeetingResult(string msg)
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
    }
}

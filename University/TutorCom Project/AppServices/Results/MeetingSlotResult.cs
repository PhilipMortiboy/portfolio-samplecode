using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Results
{
    public class MeetingSlotResult : MeetingSlot
    {
        private bool error = false;
        private string errorMsg = "";

        #region Attributes
        public bool Error
        {
            get { return error; }
        }
        public string ErrorMsg
        {
            get { return errorMsg; }
        }
        //formatted data to display on site
        public string mDayStr { get; set; }
        public string mTimeStr { get; set; }
        public string mTutorNameStr { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public MeetingSlotResult()
        {
        }

        /// <summary>
        /// Create a new Meeting result from a blog
        /// </summary>
        /// <param name="b">The blog to convert</param>
        public MeetingSlotResult(MeetingSlot ms)
        {
            msDay = ms.msDay;
            msId = ms.msId;
            msSlot = ms.msSlot;
            msTId = ms.msTId;
        }

        /// <summary>
        /// Create a error blog result
        /// </summary>
        /// <param name="msg">The error message</param>
        public MeetingSlotResult(string msg)
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

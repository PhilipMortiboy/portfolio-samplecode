using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Results
{
    public class MessageResult : Message
    {
        private bool error = false;
        private string errorMsg = "";
        private string from;
        private string to;
        private int toID;
        private int fromID;
        private List<string> groupTo = null;

        #region Attributes
        public bool Error
        {
            get { return error; }
        }
        public string ErrorMsg
        {
            get { return errorMsg; }
        }
        public string From
        {
            get { return from; }
            set { from = value; }
        }
        public string To
        {
            get { return to; }
            set { to = value; }
        }
        public int FromID
        {
            get { return fromID; }
            set { fromID = value; }
        }
        public int ToID
        {
            get { return toID; }
            set { toID = value; }
        }
        public string[] GroupTo
        {
            get { return groupTo.ToArray(); }
            set { groupTo = value.ToList(); }
        }
        public string mContentStr;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public MessageResult()
        {
        }

        /// <summary>
        /// Create a new message result from a message
        /// </summary>
        /// <param name="m">The message to convert</param>
        public MessageResult(Message m)
        {
            mContent = m.mContent;
            mId = m.mId;
            mSubject = m.mSubject;
            mTimestamp = m.mTimestamp;
            mContentStr = Util.ConvertToString(m.mContent);
        }
        /// <summary>
        /// Create a error blog result
        /// </summary>
        /// <param name="msg">The error message</param>
        public MessageResult(string msg)
        {
            SetError(msg);
        }
        #endregion

        /// <summary>
        /// Set an error notification for this blog result
        /// </summary>
        /// <param name="msg">The error message to send</param>
        public void SetError(string msg)
        {
            error = true;
            errorMsg = msg;
        }

        /// <summary>
        /// Add a name to the GroupTo list
        /// </summary>
        /// <param name="name">The name to add</param>
        public void AddTo(string name)
        {
            groupTo.Add(name);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Results
{
    public class BlogResult : Blog
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
        public string bContentStr { get; set; }
        public string bPostedStr { get; set; }
        public string bLastEditStr { get; set; }
        public string bStudentNameStr { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public BlogResult()
        {
        }

        /// <summary>
        /// Create a new blog result from a blog
        /// </summary>
        /// <param name="b">The blog to convert</param>
        public BlogResult(Blog b)
        {
            bContent = b.bContent;
            bId = b.bId;
            bLastEdited = b.bLastEdited;
            bPosted = b.bPosted;
            bSId = b.bSId;
            bSubject = b.bSubject;
            bContentStr = Util.ConvertToString(b.bContent);
            bPostedStr = Util.FormatDate(b.bPosted);
            if(b.bLastEdited != null)
                bLastEditStr = Util.FormatDate((DateTime)b.bLastEdited);
        }

        /// <summary>
        /// Create a error blog result
        /// </summary>
        /// <param name="msg">The error message</param>
        public BlogResult(string msg)
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

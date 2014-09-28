using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Results
{
    public class FileResult : FileUpload
    {
        private bool error = false;
        private string errorMsg = "";
        private UploadComment[] comments;

        #region Attributes
        public bool Error
        {
            get { return error; }
        }
        public string ErrorMsg
        {
            get { return errorMsg; }
        }
        public UploadComment[] Comments
        {
            get { return comments; }
            set { comments = value; }
        }
        public string UploaderNameStr { get; set; }
        public string fuTimestampStr { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public FileResult()
        {
        }

        /// <summary>
        /// Create a new blog result from a blog
        /// </summary>
        /// <param name="b">The blog to convert</param>
        public FileResult(FileUpload f)
        {
            fuCanSystemRead = f.fuCanSystemRead;
            fuExtention = f.fuExtention;
            fuFileName = f.fuFileName;
            fuId = f.fuId;
            fuMsId = f.fuMsId;
            fuPath = f.fuPath;
            fuSize = f.fuSize;
            fuTimestamp = f.fuTimestamp;
            if (fuTimestamp != null)
                fuTimestampStr = Util.FormatDate(fuTimestamp);
        }

        /// <summary>
        /// Create a error blog result
        /// </summary>
        /// <param name="msg">The error message</param>
        public FileResult(string msg)
        {
            SetError(msg);
        }
        #endregion

        /// <summary>
        /// Set an error notification for this file result
        /// </summary>
        /// <param name="msg">The error message to send</param>
        public void SetError(string msg)
        {
            error = true;
            errorMsg = msg;
        }

    }
}

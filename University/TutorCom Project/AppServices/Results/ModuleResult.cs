using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Results
{
    public class ModuleResult : Module
    {
        List<UserResult> users;
        bool baseError = false;
        string baseErrorMsg = "";

        #region Attributes
        public List<UserResult> Users
        {
            get { return users; }
            set { users = value; }
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
        public ModuleResult(List<UserResult> myResults, Module myModule)
        {
            users = myResults;
            if (users.Count == 0)
                SetError("No students were found in this module");
            //add module details
            mCId = myModule.mCId;
            mCredits = myModule.mCredits;
            mID = myModule.mID;
            mLevel = myModule.mLevel;
            mName = myModule.mName;
        }

        public ModuleResult(Module myModule)
        {
            mCId = myModule.mCId;
            mCredits = myModule.mCredits;
            mID = myModule.mID;
            mLevel = myModule.mLevel;
            mName = myModule.mName;
        }

        public ModuleResult(string msg)
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

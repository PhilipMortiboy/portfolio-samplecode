using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppServices.Enums;

namespace AppServices.Results
{
    public class UserResult
    {
        private bool error = false;
        private string errorMsg = "";
        private string userName = "";
        
        private string email = "";
        private string room = "";
        private int userId;
        private UserType userType;
        private int userTypeId;
        private bool isAdmin = false;

        #region Attributes
        public bool Error
        {
            get { return error; }
        }
        public string ErrorMsg
        {
            get { return errorMsg; }
        }
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public string Room
        {
            get { return room; }
            set { room = value; }
        }
        /// <summary>
        /// The user's userID
        /// </summary>
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        /// <summary>
        /// Is the user a student or a tutor
        /// </summary>
        public UserType UserType
        {
            get { return userType; }
            set { userType = value; }
        }
        /// <summary>
        /// The user's student or tutor id
        /// </summary>
        public int UserTypeId
        {
            get { return userTypeId; }
            set { userTypeId = value; }
        }
        public bool IsAdmin
        {
            get { return isAdmin; }
            set { isAdmin = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public UserResult()
        {
        }

        /// <summary>
        /// Make a new user based off a student
        /// </summary>
        /// <param name="s"></param>
        public UserResult(Student s)
        {
            userTypeId = s.sId;
            userName = Util.MakeName(s.sForename, s.sSurname);
            userType = UserType.Student;
            //get the user id
            workDbDataContext mDb = new workDbDataContext();
            var user =
                (from u in mDb.Users
                where u.uSId == s.sId
                select u).FirstOrDefault();
            userId = user.uId;
            //update the last login
            user.uLastLogin = DateTime.Now;
            mDb.SubmitChanges();
        }

        /// <summary>
        /// Make a new user based off a tutor
        /// </summary>
        /// <param name="t"></param>
        public UserResult(Tutor t)
        {
            userTypeId = t.tId;
            userName = Util.MakeName(t.tForename, t.tSurname);
            userType = UserType.Tutor;
            if (t.tAdmin == 1)
                isAdmin = true;
            //get the user id
            workDbDataContext mDb = new workDbDataContext();
            var user =
                (from u in mDb.Users
                 where u.uTId == t.tId
                 select u).FirstOrDefault();
            userId = user.uId;
            //update the last login
            user.uLastLogin = DateTime.Now;
            mDb.SubmitChanges();
        }

        /// <summary>
        /// Make a error user result
        /// </summary>
        /// <param name="errorMsg">The error message to display</param>
        public UserResult(string errorMsg)
        {
            SetError(errorMsg);
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

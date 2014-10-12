using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ObjectModel;

namespace ImageServer
{
    public class Validator
    {
        private bool error = false;
        private string errorMsg = "";
        private static char[] badCharacters = {'<', '>', '!', '%', '*'};

        #region Properties
        public bool Error
        {
            get { return error; }
            set { error = value; }
        }
        public string ErrorMsg
        {
            get { return errorMsg; }
            set { errorMsg = value; }
        }
        #endregion

        /// <summary>
        /// Validate user data
        /// </summary>
        /// <param name="userData">The request to validate</param>
        public Validator(UserRequest userData, bool register)
        {
            //this type of stuff should (also) happen client side
            if (userData.Username.Length > 50 || ContainsBadChars(userData.Username))
                SetError(errorMsg + "Your username is too long or uses characters that are not allowed;");
            //only perform the following checks on registration data
            if (register)
            {
                if (userData.Email.Length > 150 || ContainsBadChars(userData.Email))
                    SetError(errorMsg + "Your email address is too long or uses characters that are not allowed;");
                if (userData.FirstName.Length > 50 || ContainsBadChars(userData.FirstName))
                    SetError(errorMsg + "Your first name is too long or uses characters that are not allowed;");
                if (userData.LastName.Length > 50 || ContainsBadChars(userData.LastName))
                    SetError(errorMsg + "is too long or uses characters that are not allowed;");
                //only perform db checks if user data is ok
                if (register || error == false)
                {
                    //check if a user with this username already exists
                    User myUser = DbQuery.GetSingleUserByUsername(userData.Username);
                    if (myUser != null)
                        SetError(errorMsg + "A user with this username already exists;");
                    myUser = DbQuery.GetSingleUserByEmail(userData.Email);
                    if (myUser != null)
                        SetError(errorMsg + "A user with this email address already exists;");
                }
            }
        }

        /// <summary>
        /// Validate image data
        /// </summary>
        /// <param name="imgData">The Image object to validate</param>
        public Validator(ImageRequest imgData)
        {
        }

        private void SetError(string msg)
        {
            error = true;
            errorMsg = msg;
        }

        //based on: http://stackoverflow.com/a/1390774
        private bool ContainsBadChars(string text)
        {
            return text.IndexOfAny(badCharacters) >= 0;
        }
    }
}
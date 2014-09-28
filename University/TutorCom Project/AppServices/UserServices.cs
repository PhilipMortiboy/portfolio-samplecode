using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AppServices.Results;
using AppServices.Enums;

namespace AppServices
{
    public class UserServices
    {
        /// <summary>
        /// Check if a login user's login details 
        /// </summary>
        /// <param name="username">The username entered</param>
        /// <param name="password">The password entered</param>
        /// <returns>A UserResult containing their details if the login was valid, or a error if it wasn't</returns>
        public static UserResult CheckLogin(string username, string password)
        {
            try
            {
                // Hash password - code from: http://stackoverflow.com/questions/785016/best-practices-for-encrypting-and-decrypting-passwords-c-net
                var hash = new SHA256Managed();
                var plainTextBytes = Encoding.UTF8.GetBytes(username + password);
                var hashBytes = hash.ComputeHash(plainTextBytes);
                var hashedPwd = Convert.ToBase64String(hashBytes);

                // First, check if this is a student loggin in
                using (var mDb = new workDbDataContext())
                {
                    var student =
                        from s in mDb.Students
                        where s.sEmail == Util.TrimEmail(username) //&& s.sPassword == hashedPwd
                        select s;

                    // If a result was found, this must be a student
                    if (student.Count() > 0)
                        return new UserResult(student.FirstOrDefault());
                    else
                    {
                        // Otherwise, look for a tutor
                        var tutor =
                            from t in mDb.Tutors
                            where t.tEmail == Util.TrimEmail(username)
                            select t;

                        if (tutor.Count() > 0)
                        {
                            var myRes = new UserResult(tutor.FirstOrDefault());
                            if (tutor.FirstOrDefault().tAdmin == 1)
                                myRes.IsAdmin = true;
                            return myRes;
                        }
                        // If no result was found, then this password/username combo must be wrong
                        else
                            return new UserResult("The username or password entered is not valid. Please try again");
                    }
                }
            }
            catch // Catch any unexpected errors
            {
                return new UserResult(Util.GenericError);
            }
        }

        /// <summary>
        /// Find a user's details
        /// </summary>
        /// <param name="userID">The id of the user</param>
        /// <returns>A UserResult object</returns>
        public static UserResult GetUser(int userID)
        {
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    var userSet =
                        (from u in mDb.Users
                         where u.uId == userID
                         select u).FirstOrDefault();

                    // If user is a student
                    if (userSet.uSId != null)
                    {
                        var stuSet =
                            (from s in mDb.Students
                             where s.sId == userSet.uSId
                             select s).FirstOrDefault();

                        return new UserResult(stuSet);
                    }
                    else
                    {
                        var tutSet =
                            (from t in mDb.Tutors
                             where t.tId == userSet.uTId
                             select t).FirstOrDefault();

                        return new UserResult(tutSet);
                    }
                }
            }
            catch (Exception e)
            {
                return new UserResult(Util.GenericError);
            }
        }

        /// <summary>
        /// Find a student's tutor
        /// </summary>
        /// <param name="studentID">The id of the student</param>
        /// <returns>A UserResult</returns>
        public static UserResult GetTutor(int studentID)
        {
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    var tutorSet =
                        (from s in mDb.Students
                         join t in mDb.Tutors on s.sTId equals t.tId
                         where s.sId == studentID
                         select t).FirstOrDefault();

                    if (tutorSet != null)
                        return new UserResult(tutorSet);
                    else
                        return new UserResult("You have not been assigned a tutor yet");
                }
            }
            catch (Exception e)
            {
                return new UserResult(Util.GenericError);
            }
        }

        /// <summary>
        /// Get a user result from a user type id
        /// </summary>
        /// <param name="studentID">The type id of the user</param>
        /// <returns>A UserResult</returns>
        public static UserResult GetUserResult(int id, UserType type)
        {
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    if (type == UserType.Student)
                    {
                        var studentSet =
                            (from s in mDb.Students
                             where s.sId == id
                             select s).FirstOrDefault();
                        return new UserResult(studentSet);
                    }
                    else
                    {
                        var tutorSet =
                            (from t in mDb.Tutors
                             where t.tId == id
                             select t).FirstOrDefault();
                        return new UserResult(tutorSet);
                    }
                }
            }
            catch (Exception e)
            {
                return new UserResult(Util.GenericError);
            }
        }

        /// <summary>
        /// Get all students belonging to a tutor
        /// </summary>
        /// <param name="tutorID">The id of the student</param>
        /// <returns>A set of UserResults, contain the tutor's students sorted by module</returns>
        public static UserResultSet GetStudents(int tutorID)
        {
            var myUsers = new List<UserResult>();
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    var studentSet =
                        (from s in mDb.Students
                         join t in mDb.Tutors on s.sTId equals t.tId
                         where t.tId == tutorID
                         select s).OrderBy(x => x.sSurname);

                    if (studentSet != null)
                    {
                        foreach (Student stu in studentSet)
                            myUsers.Add(new UserResult(stu));
                        return new UserResultSet(myUsers);
                    }
                    else
                        return new UserResultSet("You have no students");
                }
            }
            catch (Exception e)
            {
                return new UserResultSet(Util.GenericError);
            }
        }

        /// <summary>
        /// Get all students taught by a teacher, seperated by module
        /// </summary>
        /// <param name="tutorID">The id of the tutor</param>
        /// <returns>A UserResult, contain all modules the tutor teachers on, which contain all the students on the module</returns>
        public static UserResultSet GetStudentsByModule(int tutorID)
        {
            try
            {
                // Note: queries broken down a bit to avoid too many joins - helps with debugging
                using (var mDb = new workDbDataContext())
                {
                    List<ModuleResult> modules = new List<ModuleResult>();
                    // First, get all the modules the tutor teaches on
                    var moduleSet =
                        from m in mDb.Modules
                        join mt in mDb.ModuleTutors on m.mID equals mt.mtTId
                        where mt.mtTId == tutorID
                        select m;
                    foreach (var module in moduleSet)
                    {
                        // For every module, find the students taking it
                        var stuSet =
                            (from s in mDb.Students
                             join ms in mDb.ModuleStudents on s.sId equals ms.msSId
                             join m in mDb.Modules on ms.msId equals m.mID
                             where m.mID == module.mID
                             select s).OrderBy(x => x.sSurname);
                        // Add these students to a list of UserResult
                        var users = new List<UserResult>();
                        foreach (Student student in stuSet)
                            users.Add(new UserResult(student));
                        // Add the user and module details to a new module result set
                        modules.Add(new ModuleResult(users, module));
                    }
                    return new UserResultSet(modules);
                }
            }
            catch (Exception e)
            {
                return new UserResultSet(Util.GenericError);
            }
        }

        /// <summary>
        /// Get all modules taught by a tutor
        /// </summary>
        /// <param name="myUser">The tutor to get the modules for</param>
        /// <returns></returns>
        public static ModuleResultSet GetUserModules(UserResult myUser)
        {
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    if (myUser.UserType == UserType.Student)
                    {
                        var moduleSet =
                            (from ms in mDb.ModuleStudents
                            join mt in mDb.ModuleTutors on ms.msMtId equals mt.mtTId
                            join m in mDb.Modules on mt.mtMId equals m.mID
                            where ms.msSId == myUser.UserTypeId
                            select m).OrderBy(x => x.mName);
                        var modules = new List<ModuleResult>();
                        foreach (Module myModule in moduleSet)
                            modules.Add(new ModuleResult(myModule));
                        return new ModuleResultSet(modules);
                    }
                    else
                        return new ModuleResultSet("Only students can view the list of modules they are enrolled on");
                }
            }
            catch(Exception e)
            {
                return new ModuleResultSet(Util.GenericError);// + "(Message: " + e.Message + ", Source: " + e.Source + ", Stack Trace: " + e.StackTrace + ")");
            }
        }
    }
}
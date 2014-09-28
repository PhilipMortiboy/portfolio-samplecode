using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppServices.Results;
using AppServices.Enums;

namespace AppServices
{
    public class AdminServices
    {
        /// <summary>
        /// Get all user's who haven't been assigned a tutor
        /// </summary>
        /// <returns>A UserResultSet containing all students without tutors</returns>
        public static UserResultSet GetUsersWithoutTutors()
        {
            try
            {
                using (workDbDataContext mDb = new workDbDataContext())
                {
                    var students =
                        (from s in mDb.Students
                        where (object.Equals(s.sTId, null))
                        select s).OrderBy(x => x.sSurname);
                    if (students.Count() == 0)
                        return new UserResultSet("There are currently no students without tutors");
                    var myUsers = new List<UserResult>();
                    foreach (Student stu in students)
                        myUsers.Add(new UserResult(stu));
                    return new UserResultSet(myUsers);
                }
            }
            catch (Exception e)
            {
                return new UserResultSet(Util.GenericError);
            }
        }

        /// <summary>
        /// Assign a tutor to a group of students
        /// </summary>
        /// <param name="studentIDs">The id(s) of the students</param>
        /// <param name="tutorID">The id of the tutor to assign</param>
        /// <returns>A UserResultSet with the updated students</returns>
        public static UserResultSet AssignTutor(int[] studentIDs, int tutorID)
        {
            try
            {
                using (workDbDataContext mDb = new workDbDataContext())
                {
                    List<UserResult> myUsers = new List<UserResult>();
                    foreach (int stuID in studentIDs)
                    {
                        // Get the students details from the database
                        var stu =
                            (from s in mDb.Students
                             where s.sId == stuID
                             select s).FirstOrDefault();
                        // Assign the tutor to the student
                        stu.sTId = tutorID;
                        mDb.SubmitChanges();
                        // Send a notifcaiton email to the student
                        var tut = 
                            (from t in mDb.Tutors
                             where t.tId == tutorID
                             select t).FirstOrDefault();
                        EmailServices.SendTutAllocEmail(stu, tut.tForename + " " + tut.tSurname);
                        myUsers.Add(new UserResult(stu));
                    }
                    return new UserResultSet(myUsers);
                }
            }
            catch (Exception e)
            {
                if (e.Message == "Failure sending mail")
                    return new UserResultSet("Allocation has been saved, however the confirmation email failed to send");
                else
                    return new UserResultSet(Util.GenericError);
            }
        }

        /// <summary>
        /// Get all the tutors in the system
        /// </summary>
        /// <returns>a UserResultSet containing all the tutors</returns>
        public static UserResultSet GetAllTutors()
        {
            try
            {
                using (workDbDataContext mDb = new workDbDataContext())
                {
                    var tutorSet =
                        (from t in mDb.Tutors
                        select t).OrderBy(x => x.tSurname);
                    var res = new List<UserResult>();
                    foreach (Tutor tutor in tutorSet)
                        res.Add(new UserResult(tutor));
                    return new UserResultSet(res);
                }
            }
            catch (Exception e)
            {
                return new UserResultSet(Util.GenericError);
            }
        }

        /// <summary>
        /// Get the number of messages in the last 7 days
        /// </summary>
        /// <returns></returns>
        public static int MessagesInLast7Days()
        {
            try
            {
                using (workDbDataContext mDb = new workDbDataContext())
                {
                    var now = DateTime.Now;
                    var period = now.AddDays(-7); //move back 7 days
                    var messageSet =
                        from m in mDb.Messages
                        where m.mTimestamp > period
                        select m;
                    return messageSet.Count();
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Get the average number of messages recieved by each tutor
        /// </summary>
        /// <returns></returns>
        public static float AveMessages(UserType sender)
        {
            try
            {
                using (workDbDataContext mDb = new workDbDataContext())
                {
                    UserResultSet tutors = GetAllTutors();
                    var total = 0;
                    foreach (UserResult tutor in tutors.Users)
                    {
                        var messageSet =
                            from m in mDb.Participants
                            where m.pTId == tutor.UserTypeId && m.pSender == (int)sender
                            select m;
                        total = total + messageSet.Count();
                    }
                    var ave = (total / (float)tutors.Users.Count);
                    return ave;
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Get all students who have not logged in for a certain amount of days
        /// </summary>
        /// <param name="days">The number of days. Should be a negative number</param>
        /// <returns></returns>
        public static UserResultSet InactiveStudents(double days)
        {
            try
            {
                using (workDbDataContext mDb = new workDbDataContext())
                {
                    DateTime now = DateTime.Now;
                    DateTime period = now.AddDays(days); //move back x days
                    // Get the students
                    var studentSet =
                        from s in mDb.Users
                        where (!object.Equals(s.uSId, null))
                        select s;
                    var userRes = new List<UserResult>();
                    foreach (User user in studentSet)
                    {
                        UserResult myUser = UserServices.GetUser(user.uId);
                        userRes.Add(myUser);
                    }
                    var userResOrder = userRes.OrderBy(x => x.UserName).ToList();
                    return new UserResultSet(userResOrder, 10);
                }
            }
            catch (Exception e)
            {
                return new UserResultSet(Util.GenericError);
            } 
        }
    }
}

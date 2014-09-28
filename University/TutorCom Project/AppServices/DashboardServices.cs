using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppServices.Results;
using AppServices.Enums;

namespace AppServices
{
    public class DashboardServices
    {
        private const string dashboardError = "No notifications could be found";

        /// <summary>
        /// Get all notifications that have not been viewed
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>A DashboardResultSet, containing a paginated list of all the requested notifications</returns>
        [Obsolete("Use GetNotifications")]
        public static DashboardResultSet GetNotViewed(int userId)
        {
            return GetNotViewed(userId, 50, Order.ByDateDesc);
        }

        /// <summary>
        /// Get all notifications that have not been viewed
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <param name="limit">The maximum amount of notifications to return</param>
        /// <returns>A DashboardResultSet, containing a paginated list of all the requested notifications</returns>
        [Obsolete("Use GetNotifications")]
        public static DashboardResultSet GetNotViewed(int userId, int limit)
        {
            return GetNotViewed(userId, limit, Order.ByDateDesc);
        }

        /// <summary>
        /// Get all notifications that have not been viewed
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <param name="limit">The maximum amount of notifications to return</param>
        /// <param name="order">The order in which to return the notifications; ByDateDesc, ByDateAsc, Alphabetical, ItemType </param>
        /// <returns>A DashboardResultSet, containing a paginated list of all the requested notifications</returns>
        [Obsolete("Use GetNotifications")]
        public static DashboardResultSet GetNotViewed(int userId, int limit, Order order)
        {
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    var dashboardItems = new List<DashboardResult>();
                    if (order == Order.ByDateAsc)
                    {

                        var dashboardSet =
                            (from d in mDb.Dashboards
                             where d.dUId == userId && d.dViewed == 0 // Assuming 0 is false
                             select d).Take(limit).OrderBy(x => x.dTimestamp).ToArray();

                        foreach (var d in dashboardSet)
                            dashboardItems.Add(new DashboardResult(d));
                    }
                    else if (order == Order.ByDateDesc)
                    {
                        var dashboardSet =
                            (from d in mDb.Dashboards
                             where d.dUId == userId && d.dViewed == 0 // Assuming 0 is false
                             select d).Take(limit).OrderByDescending(x => x.dTimestamp).ToArray();

                        foreach (var d in dashboardSet)
                            dashboardItems.Add(new DashboardResult(d));
                    }
                    else if (order == Order.ItemType)
                    {
                        var dashboardSet =
                            (from d in mDb.Dashboards
                             where d.dUId == userId && d.dViewed == 0 // Assuming 0 is false
                             select d).Take(limit).OrderByDescending(x => x.dItemType).ToArray();

                        foreach (var d in dashboardSet)
                            dashboardItems.Add(new DashboardResult(d));
                    }
                    else
                    {
                        var dashboardSet =
                            (from d in mDb.Dashboards
                             where d.dUId == userId && d.dViewed == 0 // Assuming 0 is false
                             select d).Take(limit).OrderByDescending(x => x.dNotification).ToArray();

                        foreach (Dashboard d in dashboardSet)
                            dashboardItems.Add(new DashboardResult(d));
                    }

                    if (dashboardItems.Count == 0)
                        return new DashboardResultSet(dashboardError);

                    return new DashboardResultSet(dashboardItems);
                }
            }
            catch (Exception e)
            {
                return new DashboardResultSet(Util.GenericError);
            }
        }

        /// <summary>
        /// Get all notifications of a certain type
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <param name="itemType">The type of notifications</param>
        /// <returns>A DashboardResultSet</returns>
        public static DashboardResultSet GetByType(int userId, ItemType itemType)
        {
            return GetByType(userId, itemType, 50);
        }

        /// <summary>
        /// Get all notifications of a certain type
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <param name="itemType">The type of notifications</param>
        /// <param name="limit">The maximum number of notifications to return</param>
        /// <returns>A DashboardResultSet</returns>
        public static DashboardResultSet GetByType(int userId, ItemType itemType, int limit)
        {
            List<DashboardResult> dashboardItems = new List<DashboardResult>();
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    var dashboardSet =
                        (from d in mDb.Dashboards
                         where d.dUId == userId && d.dViewed == 0 // Assuming 0 is false
                         select d).Take(limit).ToArray();

                    foreach (var d in dashboardSet)
                        dashboardItems.Add(new DashboardResult(d));

                    if (dashboardItems.Count == 0)
                        return new DashboardResultSet(dashboardError);

                    return new DashboardResultSet(dashboardItems);
                }
            }
            catch (Exception e)
            {
                return new DashboardResultSet(Util.GenericError);
            }
        }

        /// <summary>
        /// Get all notifications for a user
        /// </summary>
        /// <param name="user">The user to get the notifications for</param>
        /// <returns></returns>
        public static DashboardResultSet GetNotifications(UserResult user)
        {
            try
            {
                using (workDbDataContext mDb = new workDbDataContext())
                {
                    var dashRes = new List<DashboardResult>();
                    // If user is a student, just get thier notifications
                    if (user.UserType == UserType.Student)
                    {
                        var dashSet =
                            (from d in mDb.Dashboards
                             where d.dUId == user.UserId
                             select d).OrderByDescending(x => x.dTimestamp).ToList();
                        foreach (Dashboard dash in dashSet)
                            dashRes.Add(new DashboardResult(dash));
                    }
                    else
                    {
                        // Get the tutor's students
                        var myStudents = UserServices.GetStudents(user.UserTypeId);
                        foreach (var student in myStudents.Users)
                        {
                            var dashSet =
                                (from d in mDb.Dashboards
                                 where d.dUId == student.UserId
                                 select d).OrderByDescending(x => x.dTimestamp).ToList();
                            foreach (var dash in dashSet)
                                dashRes.Add(new DashboardResult(dash));
                        }
                    }
                    return new DashboardResultSet(dashRes);
                }
            }
            catch (Exception e)
            {
                return new DashboardResultSet(Util.GenericError);
            }
        }
        /// <summary>
        /// Add a new dashboard notification
        /// </summary>
        /// <param name="itemId">The id of the item</param>
        /// <param name="iType">The item's type</param>
        /// <param name="userId">The id of the user to send to notification to</param>
        /// <param name="uType">The user's type</param>
        public static void AddNotification(int itemId, ItemType iType, int userId, UserType uType, int relatedUser = 0, UserType? rType = null)
        {
            //get the user's name to add to the notification string
            string name;
            string relatedName = "";
            using (var mDb = new workDbDataContext())
            {
                var userSet =
                        (from u in mDb.Users
                         where u.uId == userId
                         select u).FirstOrDefault();
                var user = UserServices.GetUser(userSet.uId);
                name = user.UserName;
                //if this item relates to another user, get thier details - probably not needed
                if (relatedUser != 0)
                {
                    var relSet =
                        (from u in mDb.Users
                         where u.uId == relatedUser
                         select u).FirstOrDefault();
                    var relUser = UserServices.GetUser(relSet.uId);
                    relatedName = relUser.UserName;
                }
                string notification;
                switch (iType)
                {
                    case (ItemType.Message):
                        notification = "You have recieved a new message from " + relatedName;
                        break;
                    case (ItemType.MeetingRequest):
                        notification = "You have requested a meeting with " + relatedName;
                        break;
                    case (ItemType.MeetingAccepted):
                        notification = relatedName + " has accepted your meeting request";
                        break;
                    case (ItemType.MeetingRejected):
                        notification = relatedName + " has rejected your meeting request";
                        break;
                    case (ItemType.MeetingAttended):
                        notification = relatedName + " has marked your meeting as attended";
                        break;
                    case (ItemType.MeetingNotAttended):
                        notification = "You have not attended your metting with " + relatedName;
                        break;
                    default:
                        notification = "You have added a new " + iType.ToString();
                        break;
                }
                // Add the blog post as a notification for the dashboard
                var newDashboard = new Dashboard()
                {
                    dItemID = itemId,
                    dItemType = (int)iType,
                    dNotification = notification,
                    dUId = userId,
                    dViewed = 0
                };
                mDb.Dashboards.InsertOnSubmit(newDashboard);
                mDb.SubmitChanges();
                return;
            }
        }
    }
}
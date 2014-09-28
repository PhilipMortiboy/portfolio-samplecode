using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppServices.Results;
using AppServices.Enums;

namespace AppServices
{
    public class MeetingServices
    {
        /// <summary>
        /// Get a meeting by id
        /// </summary>
        /// <param name="id">The id of the meeting</param>
        /// <returns>A MeetingResult</returns>
        public static MeetingResult GetMeetingByID(int id)
        {
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    var meeting =
                        (from m in mDb.Meetings
                         where m.mId == id
                         select m).FirstOrDefault();
                    return GenerateMeetingResult(meeting);
                }
            }
            catch (Exception e)
            {
                return new MeetingResult(Util.GenericError);
            }
        }

        /// <summary>
        /// Book meeting slots
        /// </summary>
        /// <param name="myUser">The student booking the slot</param>
        /// <param name="weekStartStr">The week they are booking the slot for. Must be parsable to a DateTime value</param>
        /// <param name="slotArr">The id of the slots selected </param>
        /// <returns>A MeetingResultSet, with confirmation of the booked meetings</returns>
        public static MeetingResultSet BookMeeting(UserResult myUser, string weekStartStr, int[] slotArr)
        {
            if (myUser.UserType != UserType.Student)
                return new MeetingResultSet("Only students can book meeting slots");
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    // First, check that these slots haven't been booked already
                    foreach (int slot in slotArr)
                    {
                        var checkMeet =
                            from m in mDb.Meetings
                            where m.mMsId == slot
                            select m;
                        if (checkMeet.Count() > 0)
                            return new MeetingResultSet("One or more of the slots selected have already been booked");
                    }
                    // Book the meeting slot
                    // If all slots are ok, add new meetings
                    var myMeetings = new List<MeetingResult>();
                    foreach (int slot in slotArr)
                    {
                        var slotID = GetSlotID(myUser, slot);
                        var myMeeting = new Meeting()
                        {
                            mAttended = null,
                            mMsId = slotID,
                            mPreviousRequestID = 0,
                            mSId = myUser.UserTypeId,
                            mRequestStatus = null,
                            mStudentMinutes = null,
                            mTutorMinutes = null,
                            mWeek = DateTime.Parse(weekStartStr),
                        };
                        mDb.Meetings.InsertOnSubmit(myMeeting);
                        mDb.SubmitChanges();
                        // Generate a dashboard item
                        var myTutor = UserServices.GetTutor(myUser.UserTypeId);
                        DashboardServices.AddNotification(myMeeting.mId, ItemType.MeetingRequest, myUser.UserId, myUser.UserType, myTutor.UserId, myTutor.UserType);
                        myMeetings.Add(GenerateMeetingResult(myMeeting));
                    }
                    return new MeetingResultSet(myMeetings);
                }
            }
            catch (Exception e)
            {
                return new MeetingResultSet(Util.GenericError);
            }
        }

        /// <summary>
        /// Add new meeting slots
        /// </summary>
        /// <param name="myTutor">The tutor adding the slots</param>
        /// <param name="slots">The meeting slots to add</param>
        /// <returns></returns>
        public static MeetingSlotResultSet AddMeetingSlots(UserResult myTutor, int[] slots)
        {
            if (myTutor.UserType != UserType.Tutor)
                return new MeetingSlotResultSet("Only tutors can make meeting slots avalible");
            else
            {
                try
                {
                    using (var mDb = new workDbDataContext())
                    {
                        // Remove all slots made by this tutor
                        var existingSlots =
                            from s in mDb.MeetingSlots
                            where s.msTId == myTutor.UserTypeId
                            select s;
                        foreach (var existingSlot in existingSlots)
                            mDb.MeetingSlots.DeleteOnSubmit(existingSlot);
                        mDb.SubmitChanges();
                        // Add in the new selection
                        var myMeetingSlots = new List<MeetingSlotResult>();
                        foreach (int slot in slots)
                        {
                            var day = GetDay(slot);
                            var mySlot = new MeetingSlot()
                            {
                                msDay = (int)day,
                                msSlot = slot,
                                msTId = myTutor.UserTypeId,
                            };
                            mDb.MeetingSlots.InsertOnSubmit(mySlot);
                            mDb.SubmitChanges();
                            myMeetingSlots.Add(new MeetingSlotResult(mySlot));
                        }
                        return new MeetingSlotResultSet(myMeetingSlots);
                    }
                }
                catch (Exception e)
                {
                    return new MeetingSlotResultSet(Util.GenericError);
                }
            }
        }

        /// <summary>
        /// Get an array contain all the positons of all avalible meeting slots
        /// </summary>
        /// <param name="user">The user view the calender</param>
        /// <returns></returns>
        public static int[] GetMeetingSlotPos(UserResult user)
        {
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    var slotPos = new List<int>();
                    // Work out which tutor's user slots need selecting
                    int tutorID;
                    if (user.UserType == UserType.Student)
                    {
                        var myStu =
                            (from s in mDb.Students
                             where s.sId == user.UserTypeId
                             select s).FirstOrDefault();
                        if (myStu.sTId == null)
                            return null;
                        else
                            tutorID = (int)myStu.sTId;
                    }
                    else
                        tutorID = user.UserTypeId;
                    // Get the meeting slots
                    var slots =
                        from m in mDb.MeetingSlots
                        where m.msTId == tutorID
                        select m;
                    foreach (var slot in slots)
                        slotPos.Add(slot.msSlot);
                    return slotPos.ToArray();
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Get the poisitons of all the already booked slots
        /// </summary>
        /// <param name="user">The user viewing the slots</param>
        /// <returns></returns>
        public static int[] GetBookedSlotPos(UserResult user, string weekStartStr)
        {
            try
            {
                using (workDbDataContext mDb = new workDbDataContext())
                {
                    var slotPos = new List<int>();
                    // Work out which tutor's user slots need selecting
                    int tutorID;
                    if (user.UserType == UserType.Student)
                    {
                        var myStu =
                            (from s in mDb.Students
                             where s.sId == user.UserTypeId
                             select s).FirstOrDefault();
                        if (myStu.sTId == null)
                            return null;
                        else
                            tutorID = (int)myStu.sTId;
                    }
                    else
                        tutorID = user.UserTypeId;
                    // Get the meeting slots
                    var slots =
                        from m in mDb.MeetingSlots
                        where m.msTId == tutorID
                        select m;
                    // Check if the meeting slot for this week has been booked
                    foreach (var slot in slots)
                    {
                        var week = DateTime.Parse(weekStartStr);
                        var meeting =
                            from m in mDb.Meetings
                            where m.mMsId == slot.msId && m.mRequestStatus != 0 && m.mWeek == week
                            select m;
                        // If a meeting for this slot was found, ad its pos to the list of booked slots
                        if (meeting.Count() > 0)
                            slotPos.Add(slot.msSlot);
                    }
                    return slotPos.ToArray();
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Get the meeting slots avalible to a user
        /// </summary>
        /// <param name="user">The user to get meeting slots for</param>
        /// <returns></returns>
        public static MeetingSlotResultSet GetMeetingSlots(UserResult user)
        {
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    // Work out which tutor's user slots need selecting
                    int tutorID;
                    if (user.UserType == UserType.Student)
                    {
                        var myStu =
                            (from s in mDb.Students
                             where s.sId == user.UserTypeId
                             select s).FirstOrDefault();
                        if (myStu.sTId == null)
                            return new MeetingSlotResultSet("You do not have a tutor assigned. Please contact the CMS office to get a tutor");
                        else
                            tutorID = (int)myStu.sTId;
                    }
                    else
                        tutorID = user.UserTypeId;
                    // Get the meeting slots
                    var slots =
                        from m in mDb.MeetingSlots
                        where m.msTId == tutorID
                        select m;
                    var mySlots = new List<MeetingSlotResult>();
                    foreach (MeetingSlot slot in slots)
                        mySlots.Add(new MeetingSlotResult(slot));

                    return new MeetingSlotResultSet(mySlots);
                }
            }
            catch (Exception e)
            {
                return new MeetingSlotResultSet(Util.GenericError);// + "(Message: " + e.Message + ", Source: " + e.Source + ", Stack Trace: " + e.StackTrace + ")");
            }
        }

        /// <summary>
        /// Confirm a meeting
        /// </summary>
        /// <param name="meetingID">The id of the meeting</param>
        /// <param name="res">Whether to accept the meeting or not</param>
        /// <returns></returns>
        public static MeetingResult ConfirmMeeting(int meetingID, bool res, string reason = "")
        {
            var status = 0;
            if (res)
                status = 1;
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    // Get the meeting
                    var meeting =
                        (from m in mDb.Meetings
                         where m.mId == meetingID
                         select m).FirstOrDefault();
                    // Edit the meeting's request status
                    meeting.mRequestStatus = status;
                    meeting.mTutorMinutes = Util.ConvertToBinary(reason);
                    // Save the changes
                    mDb.SubmitChanges();
                    // Generate a dashboard item
                    var myTutor = UserServices.GetTutor(meeting.mSId);
                    DashboardServices.AddNotification(meeting.mId, ItemType.MeetingAccepted, meeting.mSId, UserType.Student, myTutor.UserId, myTutor.UserType);
                    return new MeetingResult(meeting);
                }
            }
            catch (Exception e)
            {
                return new MeetingResult(Util.GenericError);
            }
        }

        /// <summary>
        /// Confirm whether a student has attended the meeting or not
        /// </summary>
        /// <param name="meetingID">The id of the meeting</param>
        /// <param name="res">Whether the student attendend or not</param>
        /// <returns></returns>
        public static MeetingResult ConfirmAttendance(int meetingID, MeetingStatus res, string reason = "")
        {
            int status = 0;
            if (res == MeetingStatus.Confirmed)
                status = 1;
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    // Get the meeting
                    var meeting =
                        (from m in mDb.Meetings
                         where m.mId == meetingID
                         select m).FirstOrDefault();
                    // Edit the meeting's request status
                    meeting.mAttended = status;
                    meeting.mTutorMinutes = Util.ConvertToBinary(reason);
                    // Save the changes
                    mDb.SubmitChanges();
                    // Generate a dashboard item
                    var myTutor = UserServices.GetTutor(meeting.mSId);
                    if (res == MeetingStatus.Confirmed)
                        DashboardServices.AddNotification(meeting.mId, ItemType.MeetingAttended, meeting.mSId, UserType.Student, myTutor.UserId, myTutor.UserType);
                    else
                        DashboardServices.AddNotification(meeting.mId, ItemType.MeetingNotAttended, meeting.mSId, UserType.Student, myTutor.UserId, myTutor.UserType);
                    return new MeetingResult(meeting);
                }
            }
            catch (Exception e)
            {
                return new MeetingResult(Util.GenericError);
            }
        }

        /// <summary>
        /// Add a comment/minute to a meeting
        /// </summary>
        /// <param name="meetingId">The id of the meeting</param>
        /// <param name="comment">The comment to add</param>
        /// <param name="type">The type of the user adding the comment</param>
        /// <returns></returns>
        public static MeetingResult AddComment(int meetingId, string comment, UserType type)
        {
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    // Get the meeting
                    var meeting =
                        (from m in mDb.Meetings
                         where m.mId == meetingId
                         select m).FirstOrDefault();
                    if (type == UserType.Student)
                        meeting.mStudentMinutes = Util.ConvertToBinary(comment);
                    else
                        meeting.mTutorMinutes = Util.ConvertToBinary(comment);
                    // Save the changes
                    mDb.SubmitChanges();
                    return new MeetingResult(meeting);
                }
            }
            catch (Exception e)
            {
                return new MeetingResult(Util.GenericError);
            }
        }

        /// <summary>
        /// Get a user's message by status
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="status">The status requested</param>
        /// <returns></returns>
        public static MeetingResultSet GetMeetingsByStatus(UserResult user, MeetingStatus status)
        {
            try
            {
                int? statusInt = null;
                if (status == MeetingStatus.Confirmed)
                    statusInt = 1;
                if (status == MeetingStatus.Rejected)
                    statusInt = 0;
                using (var mDb = new workDbDataContext())
                {
                    var myMeetings = new List<Meeting>();
                    if (user.UserType == UserType.Student)
                    {
                        if (status == MeetingStatus.Past)
                        {
                            myMeetings =
                               (from m in mDb.Meetings
                                where m.mSId == user.UserTypeId && (!object.Equals(m.mAttended, null)) // Only get past meeting if status is past
                                select m).ToList();
                        }
                        else
                        {
                            myMeetings =
                                (from m in mDb.Meetings
                                 where m.mSId == user.UserTypeId && (object.Equals(m.mRequestStatus, statusInt)) && (object.Equals(m.mAttended, null)) //only get past meeting if status is past
                                 select m).ToList();
                        }
                    }
                    else
                    {
                        var mySlots =
                            from s in mDb.MeetingSlots
                            where s.msTId == user.UserTypeId
                            select s;
                        foreach (var slot in mySlots)
                        {
                            if (status == MeetingStatus.Past)
                            {
                                var meeting =
                                    (from m in mDb.Meetings
                                     where m.mMsId == slot.msId && (!object.Equals(m.mAttended, null)) //get events that have already happend
                                     select m).FirstOrDefault();
                                myMeetings.Add(meeting);
                            }
                            else
                            {
                                var meeting =
                                    (from m in mDb.Meetings
                                     where m.mMsId == slot.msId && (object.Equals(m.mRequestStatus, statusInt)) && (object.Equals(m.mAttended, null)) //&& m.mAttended == null
                                     select m).FirstOrDefault();
                                myMeetings.Add(meeting);
                            }
                        }
                    }
                    var myRes = new List<MeetingResult>();
                    // If no results were found, return a empty list
                    if (myMeetings.Count == 0)
                        return new MeetingResultSet(new List<MeetingResult>());
                    // Otherwise, add each meeting to the set and return
                    else
                    {
                        foreach (var meeting in myMeetings)
                        {
                            // Only add found meetings
                            if (meeting != null)
                                myRes.Add(GenerateMeetingResult(meeting));
                        }
                        return new MeetingResultSet(myRes);
                    }
                }
            }
            catch (Exception e)
            {
                return new MeetingResultSet(Util.GenericError);
            }
        }

        /// <summary>
        /// Gets a week start as a string
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static string GetWeek(double pos)
        {
            var week = DateTime.Now;
            int subtract;
            // Move the day back to the start of the week
            switch (week.DayOfWeek)
            {
                case DayOfWeek.Tuesday:
                    subtract = 24 * 1;
                    break;
                case DayOfWeek.Wednesday:
                    subtract = 24 * 2;
                    break;
                case DayOfWeek.Thursday:
                    subtract = 24 * 3;
                    break;
                case DayOfWeek.Friday:
                    subtract = 24 * 4;
                    break;
                case DayOfWeek.Saturday:
                    subtract = 24 * 5;
                    break;
                case DayOfWeek.Sunday:
                    subtract = 24 * 6;
                    break;
                default:
                    subtract = 24 * 0;
                    break;
            }
            var startWeek = week.Subtract(new TimeSpan(subtract, 0, 0));
            startWeek.AddDays(7 * pos);
            return startWeek.ToShortDateString();
        }

        /// <summary>
        /// Get the time label for a row in the display table
        /// </summary>
        /// <param name="row">The id of the row</param>
        /// <returns></returns>
        public static string GetTime(int slot)
        {
            // Modify slot number to a row number
            var thisDay = GetDay(slot);
            var dayNum = (int)thisDay;
            var row = slot - (dayNum * 32);
            var hour = (float)row / 4;
            hour = hour + 9;
            var timeStr = hour.ToString();
            var timeStrList = timeStr.Split('.').ToList();
            if (timeStrList.Count < 2)
                timeStrList.Add("0");
            var timeStrArr = timeStrList.ToArray();
            string timeStart;
            string timeEnd;
            switch (timeStrArr[1])
            {
                case "25":
                    timeStart = timeStrArr[0] + ".15";
                    timeEnd = timeStrArr[0] + ".30";
                    break;
                case "5":
                    timeStart = timeStrArr[0] + ".30";
                    timeEnd = timeStrArr[0] + ".45";
                    break;
                case "75":
                    timeStart = timeStrArr[0] + ".45";
                    int newHour = int.Parse(timeStrArr[0]) + 1;
                    timeEnd = newHour.ToString() + ".00";
                    break;
                default:
                    timeStart = timeStrArr[0] + ".00";
                    timeEnd = timeStrArr[0] + ".15";
                    break;
            }
            return timeStart + " - " + timeEnd;
        }

        private static int GetSlotID(UserResult myUser, int slot)
        {
            using (var mDb = new workDbDataContext())
            {
                var student =
                    (from s in mDb.Students
                     where s.sId == myUser.UserTypeId
                     select s).FirstOrDefault();
                if (student.sTId == null)
                    return 0;
                else
                {
                    var slotSet =
                        (from l in mDb.MeetingSlots
                         where l.msTId == student.sTId && l.msSlot == slot
                         select l).FirstOrDefault();
                    return slotSet.msId;
                }

            }
        }

        private static MeetingResult GenerateMeetingResult(Meeting meeting)
        {
            using (var mDb = new workDbDataContext())
            {
                var res = new MeetingResult(meeting);
                // Get the details about the slot selected
                var mySlot =
                    (from s in mDb.MeetingSlots
                     where s.msId == meeting.mMsId
                     select s).FirstOrDefault();
                res.MsDay = mySlot.msDay;
                res.msDayStr = GetDay(mySlot.msSlot).ToString();
                res.msTimeStr = GetTime(mySlot.msSlot);
                // Get the details of the tutor the slot was booked with
                var myTutor =
                    (from t in mDb.Tutors
                     where t.tId == mySlot.msTId
                     select t).FirstOrDefault();
                res.msTutorNameStr = Util.MakeName(myTutor.tForename, myTutor.tSurname);
                res.msRoom = myTutor.tRoom;
                // Get the details of the tutor the slot was booked with
                var myStudent =
                    (from s in mDb.Students
                     where s.sId == meeting.mSId
                     select s).FirstOrDefault();
                res.msStudentNameStr = Util.MakeName(myStudent.sForename, myStudent.sSurname);
                //convert the attended res to a string
                if (meeting.mAttended == 1)
                    res.msAttendedStr = "Yes";
                else if (meeting.mAttended == 0)
                    res.msAttendedStr = "No";
                else
                    res.msAttendedStr = "N/A";
                return res;
            }
        }

        private static Days GetDay(int slot)
        {
            // 32 slots per day. Remember slot number starts at 0
            Days day;
            if (slot < 32)
                day = Days.Monday;
            else if (slot > 31 && slot < 64)
                day = Days.Tuesday;
            else if (slot > 63 && slot < 96)
                day = Days.Wednesday;
            else if (slot > 95 && slot < 128)
                day = Days.Thursday;
            else
                day = Days.Friday;
            return day;
        }
    }
}

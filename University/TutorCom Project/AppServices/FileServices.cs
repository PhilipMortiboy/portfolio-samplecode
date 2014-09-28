using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AppServices.Results;
using AppServices.Enums;

namespace AppServices
{
    public class FileServices
    {
        /// <summary>
        /// Get a file, optionally with comments, by ID
        /// </summary>
        /// <param name="fileID">The id of the file</param>
        /// <param name="withComments">Whether to get the file comments</param>
        /// <returns></returns>
        public static FileResult GetFile(int fileID, bool withComments = true)
        {
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    // Get the file itself
                    var fileRes =
                        (from f in mDb.FileUploads
                            where f.fuId == fileID
                            select f).FirstOrDefault();
                    var myFile = new FileResult(fileRes);
                    if (withComments)
                        myFile.Comments = GetFileComments(fileID);
                    return myFile;
                }
            }
            catch
            {
                return new FileResult(Util.GenericError);
            }
        }

        /// <summary>
        /// Get all files uploaded by a user
        /// </summary>
        /// <param name="userID">The user who uploaded the files</param>
        /// <returns></returns>
        public static FileResultSet GetUserFiles(UserResult myUser, int moduleID = 0)
        {
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    var results = new List<FileResult>();
                    if (myUser.UserType == UserType.Student)
                    {
                        var fileRes =
                            (from f in mDb.FileUploads
                             where f.fuMsId == myUser.UserTypeId
                             select f).OrderByDescending(x => x.fuTimestamp).ToList();
                        foreach (var file in fileRes)
                            results.Add(new FileResult(file));
                        if (results.Count() == 0)
                            return new FileResultSet("You haven't uploaded any documents yet");
                    }
                    else
                    {
                        var myStudents = UserServices.GetStudents(myUser.UserTypeId);
                        foreach (var student in myStudents.Users)
                        {
                            // Get all files uploaded by this student
                            var fileSet =
                                (from f in mDb.FileUploads
                                 where f.fuMsId == student.UserTypeId
                                 select f).ToList();
                            // For each file, convert it to a file result and add the student's details
                            foreach (var file in fileSet)
                            {
                                var res = new FileResult(file);
                                res.UploaderNameStr = student.UserName;
                                results.Add(res);
                            }
                        }
                    }
                    return new FileResultSet(results);
                }
            }
            catch (Exception e)
            {
                return new FileResultSet(Util.GenericError);
            }
        }

        /// <summary>
        /// Add a file's metadata to the database
        /// </summary>
        /// <param name="file">The uploaded file</param>
        /// <param name="myUser">The user who uploaded it</param>
        /// <returns></returns>
        public static FileResult AddFileMetaData(HttpPostedFile file, UserResult myUser, int moduleID = 0)
        {
            try
            {
                using (workDbDataContext mDb = new workDbDataContext())
                {
                    // Check if a file with this name already exists
                    var fileSet =
                        from f in mDb.FileUploads
                        where f.fuFileName == file.FileName
                        select f;
                    if (fileSet.Count() > 0)
                        return new FileResult("A file with this name already exists");

                    var ext = file.FileName.Split('.');
                    var myFile = new FileUpload()
                    {
                        fuCanSystemRead = 0,
                        fuExtention = ext[1],
                        fuFileName = file.FileName,
                        fuPath = "Uploads/" + file.FileName,
                        fuSize = file.ContentLength,
                        fuTimestamp = DateTime.Now,
                        fuMsId = myUser.UserTypeId, // Using MsId to store the studentID
                    };
                    mDb.FileUploads.InsertOnSubmit(myFile);
                    mDb.SubmitChanges();
                    DashboardServices.AddNotification(myFile.fuId, ItemType.FileUpload, myUser.UserId, myUser.UserType);
                    return new FileResult(myFile);
                }
            }
            catch (Exception e)
            {
                return new FileResult(Util.GenericError);
            }
        }

        /// <summary>
        /// Get all the comments on a file
        /// </summary>
        /// <param name="fileID">The id of the file</param>
        /// <returns>An array of comment objects</returns>
        private static UploadComment[] GetFileComments(int fileID)
        {
            using (var mDb = new workDbDataContext())
            {
                var commentRes =
                    from c in mDb.UploadComments
                    where c.ucFuId == fileID
                    select c;
                return commentRes.ToArray();
            }
        }
    }
}

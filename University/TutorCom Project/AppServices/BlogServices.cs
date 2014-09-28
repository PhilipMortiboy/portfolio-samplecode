using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppServices.Results;
using AppServices.Enums;

namespace AppServices
{
    public class BlogServices
    {
        /// <summary>
        /// Returns the blog post with the id specified
        /// </summary>
        /// <param name="blogId">The id of the blog post you wish to retrive</param>
        /// <returns>A Blog object, containing all the blogs details</returns>
        public static BlogResult GetBlog(int blogId)
        {
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    var blogSet =
                        from m in mDb.Blogs
                        where m.bId == blogId
                        select m;

                    return new BlogResult(blogSet.FirstOrDefault());
                }
            }
            catch (Exception e)
            {
                return new BlogResult(Util.GenericError);
            }
        }

        /// <summary>
        /// Gets all blog posts by a certain student
        /// </summary>
        /// <param name="studentId">The id of the student</param>
        /// <returns>A array of Blog objects</returns>
        public static BlogResultSet GetStudentBlog(UserResult student)
        {
            BlogResultSet myBlogs = GetStudentBlog(student, 10, Order.ByDateDesc);
            return myBlogs;
        }

        /// <summary>
        /// Get a limited number of blog posts by a certain student
        /// </summary>
        /// <param name="studentId">The id of the student</param>
        /// <param name="limit">The maximum number of blog posts to return</param>
        /// <returns></returns>
        public static BlogResultSet GetStudentBlog(UserResult student, int limit)
        {
            BlogResultSet myBlogs = GetStudentBlog(student, limit, Order.ByDateDesc);
            return myBlogs;
        }

        /// <summary>
        /// Get a limited number of blog posts by a certain student, order in a specified way
        /// </summary>
        /// <param name="studentId">The id of the student</param>
        /// <param name="limit">The maximum number of blog posts to return</param>
        /// <param name="order">0 = most recent first, 1 = oldest first, 2 = alphabetical </param>
        /// <returns></returns>
        public static BlogResultSet GetStudentBlog(UserResult student, int limit, Order order)
        {
            var myBlogs = new List<BlogResult>();
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    if (order == Order.ByDateAsc)
                    {
                        var blogSet = (from m in mDb.Blogs
                                       where m.bSId == student.UserTypeId
                                       select m).Take(limit).OrderBy(x => x.bLastEdited).ToArray();

                        foreach (var b in blogSet)
                        {
                            var myBlog = new BlogResult(b);
                            myBlog.bStudentNameStr = student.UserName;
                            myBlogs.Add(myBlog);
                        }
                    }
                    else if (order == Order.ByDateDesc)
                    {
                        var blogSet = (from m in mDb.Blogs
                                       where m.bSId == student.UserTypeId
                                       select m).Take(limit).OrderByDescending(x => x.bLastEdited).ToArray();

                        foreach (var b in blogSet)
                        {
                            BlogResult myBlog = new BlogResult(b);
                            myBlog.bStudentNameStr = student.UserName;
                            myBlogs.Add(myBlog);
                        }
                    }
                    else
                    {
                        var blogSet = (from m in mDb.Blogs
                                       where m.bSId == student.UserTypeId
                                       select m).Take(limit).OrderByDescending(x => x.bSubject).ToArray();

                        foreach (var b in blogSet)
                        {
                            var myBlog = new BlogResult(b);
                            myBlog.bStudentNameStr = student.UserName;
                            myBlogs.Add(myBlog);
                        }
                    }
                    if (myBlogs.Count() == 0)
                        return new BlogResultSet("This user hasn't posted any blogs yet");

                    return new BlogResultSet(myBlogs);
                }
            }
            catch (Exception e)
            {
                return new BlogResultSet(Util.GenericError);
            }
        }

        public static BlogResultSet GetTutorBlogs(int tutorID)
        {
            try
            {
                using (workDbDataContext mDb = new workDbDataContext())
                {
                    // Find the tutor's students
                    var myStudents = UserServices.GetStudents(tutorID);
                    var myBlogs = new List<BlogResult>();
                    foreach (var student in myStudents.Users)
                    {
                        var blogSet =
                            from b in mDb.Blogs
                            where b.bSId == student.UserTypeId
                            select b;
                        foreach (var b in blogSet)
                        {
                            var myBlog = new BlogResult(b);
                            myBlog.bStudentNameStr = student.UserName;
                            myBlogs.Add(myBlog);
                        }
                    }
                    // Order the blogs found by date posted
                    var orderedBlogs = myBlogs.OrderByDescending(x => x.bPosted).ToList();
                    return new BlogResultSet(orderedBlogs);
                }
            }
            catch (Exception e)
            {
                return new BlogResultSet(Util.GenericError);
            }
        }

        /// <summary>
        /// Add a new blog post
        /// </summary>
        /// <param name="studentId">The id of the student who made the post</param>
        /// <param name="title">The title</param>
        /// <param name="subject">The subject</param>
        /// <param name="content">The content</param>
        /// <returns>The blog just posted</returns>
        public static BlogResult PostBlog(int studentId, string subject, string content)
        {
            BlogResult myBlog;
            if (subject != "" && content != "")
            {
                // Validate the input
                string errors = "";
                if (subject.ToString().Length > 255)
                    errors = errors + "The subject for your blog must be no more than 255 characters long. Yours is " + subject.ToString().Length + "\n";
                if (errors != "")
                    myBlog = new BlogResult(errors);
                else
                {
                    try
                    {
                        // If all ok, attempt to create a new blog and add it to the database
                        using (var mDb = new workDbDataContext())
                        {
                            var newBlog = new Blog()
                            {
                                bSId = studentId,
                                bSubject = subject.ToString(),
                                bContent = Util.ConvertToBinary(content.ToString()),
                                bLastEdited = DateTime.Now,
                                bPosted = DateTime.Now,
                                bDeleted = "0"
                            };
                            mDb.Blogs.InsertOnSubmit(newBlog);
                            mDb.SubmitChanges();
                            // Add this activity to the dashboard
                            DashboardServices.AddNotification(newBlog.bId, ItemType.Blog, studentId, UserType.Student);
                            // Return the blog just submitted
                            myBlog = new BlogResult(newBlog);
                        }
                    }
                    catch (Exception e)
                    {
                        myBlog = new BlogResult(Util.GenericError);
                    }
                }
            }
            else
            {
                myBlog = new BlogResult("A blog needs a subject and some content");
            }
            return myBlog;
        }

        /// <summary>
        /// Edit a existing blog post
        /// </summary>
        /// <param name="blogId">The id of the post to be edited</param>
        /// <param name="newTitle">The new title</param>
        /// <param name="newSubject">The new subject</param>
        /// <param name="newContent">The new content</param>
        /// <returns></returns>
        public static BlogResult EditBlog(int blogId, string subject, string content)
        {
            var myBlog = new BlogResult();
            if (subject != "" && content != "")
            {
                // Validate the input
                string errors = "";
                if (subject.ToString().Length > 255)
                    errors = errors + "The subject for your blog must be no more than 255 characters long. Yours is " + subject.ToString().Length + "\n";
                if (errors != "")
                    myBlog = new BlogResult(errors);
                else
                {
                    try
                    {
                        // If all ok, attempt to create a new blog and add it to the database
                        using (var mDb = new workDbDataContext())
                        {
                            var blog = mDb.Blogs.Single(x => x.bId == blogId);
                            if (blog == null)
                                myBlog = new BlogResult("The blog you are trying to edit no longer exists");
                            else
                            {
                                // If found, update the blog
                                blog.bSubject = subject;
                                blog.bContent = Util.ConvertToBinary(content);
                                blog.bLastEdited = DateTime.Now;
                                mDb.SubmitChanges();
                                // Return the blog
                                myBlog = new BlogResult(blog);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        myBlog = new BlogResult(Util.GenericError);
                    }
                }
            }
            else
            {
                myBlog = new BlogResult("A blog needs a subject and some content");
            }
            return myBlog;
        }

        /// <summary>
        /// Delete a single blog
        /// </summary>
        /// <param name="blogId">The id of the blog to be deleted</param>
        /// <returns>A BlogResult containing the details of the blog just deleted</returns>
        public static BlogResult DeleteBlog(int blogId)
        {
            try
            {
                using (var mDb = new workDbDataContext())
                {
                    var myBlog =
                        (from b in mDb.Blogs
                         where b.bId == blogId
                         select b).FirstOrDefault();
                    var blogCopy = myBlog;
                    mDb.Blogs.DeleteOnSubmit(myBlog);
                    mDb.SubmitChanges();
                    return new BlogResult(myBlog);
                }
            }
            catch (Exception e)
            {
                return new BlogResult(Util.GenericError);
            }
        }
    }
}
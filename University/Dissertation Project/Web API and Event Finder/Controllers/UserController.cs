using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ObjectModel;
using System.Threading.Tasks;

namespace ImageServer.Controllers
{
    [RoutePrefix("v1/User")]
    public class UserController : ApiController //Note: all controllers inherit from the ApiController class, a auto-generated class
    {
        /// <summary>
        /// Get a specific user
        /// </summary>
        /// <param name="id">ID of the user</param>
        /// <returns>A user object</returns>
        [Route("ByID/{id}")]
        [HttpGet]
        public UserResult ByID(int id)
        {
            return ResultGenerator.GenerateUserResult(id);
        }

        /// <summary>
        /// Get a specific user
        /// </summary>
        /// <param name="id">ID of the user</param>
        /// <returns>A full set of the user's details</returns>
        [Route("FullByID/{id}")]
        [HttpGet]
        public FullUserResult FullByID(int id)
        {
            return ResultGenerator.GenerateFullUserResult(id);
        }

        /// <summary>
        /// Check a user's login creditials
        /// </summary>
        /// <param name="username">The user's username</param>
        /// <param name="password">the user's encrypted password</param>
        /// <returns>A UserResult</returns>
        [Route("Login")]
        [HttpPost]
        public async Task<UserResult> Login()
        {
            byte[] userData = await Request.Content.ReadAsByteArrayAsync();
            return DataProcessor.UserLogin(userData);
        }

        [Route("Register")]
        [HttpPost]
        public async Task<UserResult> Register()
        {
            byte[] userData = await Request.Content.ReadAsByteArrayAsync();
            return DataProcessor.AddNewUser(userData);
        }

        #region Unfinished/Not used methods
        /*
        [Route("Register")]
        [HttpPost]
        public UserResult Register([FromBody]UserRequest userData)
        {
            return DataProcessor.AddNewUser(userData);
        }
        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }*/
        #endregion
    }
}
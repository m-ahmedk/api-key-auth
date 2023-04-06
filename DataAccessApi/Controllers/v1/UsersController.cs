using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Cors;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using Database.Models;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.OpenApi.Models;

namespace DataAccessApi.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [Auth]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private IConfiguration Configuration;
        BraincropContext _context = new BraincropContext();

        public UsersController(BraincropContext context, ILogger<UsersController> logger, IConfiguration _configuration)
        {
            _logger = logger;
            _context = context;
            Configuration = _configuration;
        }

        /// <summary>
        /// Fetch Users
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     GET api/v1/Users/GetUsers
        /// </remarks>
        /// <response code="200">Users fetched</response>
        /// <response code="500">An error occurred while fetching users..</response>
        [HttpGet(Name = "GetUsers")]
        public async Task<IActionResult> GetUsers([FromQuery] int pageNo = 1, int pageSize = 10)
        {
            List<AppUser> Users = new List<AppUser>();

            try
            {
                int skip = (pageNo - 1) * pageSize;

                Users = await _context.AppUsers.Skip(skip).Take(pageSize).ToListAsync();
                return Ok(Users);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, Users);
            }
        }

        /// <summary>
        /// Get User by Id
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     GET api/v1/Users/GetUser/{id}
        /// </remarks>
        /// <response code="200">User fetched</response>
        /// <response code="500">An error occurred while fetching the user..</response>
        [HttpGet(Name = "GetUser/{id}")]
        [ResponseCache(Duration = 10800, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> GetUser(int? id)
        {
            try
            {
                var User = await _context.AppUsers.FindAsync(id);

                if (User == null)
                {
                    return NotFound();
                }

                return Ok(User);
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Creates a User.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/v1/Users/CreateUser
        ///     {        
        ///       "FirstName": "Muhammad Ahmed Villa",
        ///       "LastName": "Khan",
        ///       "Email": "m.ahmedk287@gmail.com"
        ///       "BirthDate": "",
        ///       "Phone": "string",
        ///       "Zipcode": "string"
        ///     }
        /// </remarks>
        /// <param name="user"></param>     
        /// <response code="201">User created</response>
        /// <response code="500">An error occurred while creating a user..</response>
        [HttpPost(Name = "CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] AppUserDTO user)
        {
            try
            {
                // Map User DTO to an AppUser entity
                var appUser = new AppUser
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    BirthDate = user.BirthDate,
                    Email = user.Email,
                    Phone = user.Phone,
                    Zipcode = user.Zipcode,
                    CreatedAt = DateTime.Now
                };

                _context.AppUsers.Add(appUser);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { id = appUser.UserId }, appUser);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     GET api/v1/Users/UpdateUser/{id}
        /// </remarks>
        /// <response code="200">User updated</response>
        /// <response code="500">An error occurred while updating the user..</response>
        [HttpPut(Name = "UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(int? id, [FromBody] AppUserDTO user)
        {
            try
            {
                AppUser? appUser = 
                    await _context.AppUsers.Where(u => u.UserId == id).FirstOrDefaultAsync();


                if (appUser == null)
                {
                    return BadRequest();
                }

                appUser = AssignUser(appUser, user);

                _context.Entry(appUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     GET api/v1/Users/DeleteUser/{id}
        /// </remarks>
        /// <response code="200">User deleted</response>
        /// <response code="500">An error occurred while deleting the user..</response>
        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var User = await _context.AppUsers.FindAsync(id);
            if (User == null)
            {
                return NotFound();
            }

            _context.AppUsers.Remove(User);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int? id)
        {
            return _context.AppUsers.Any(e => e.UserId == id);
        }

        private AppUser AssignUser(AppUser appUser, AppUserDTO user)
        {
            if (!(string.IsNullOrEmpty(user.Email)))
            {
                appUser.Email = user.Email;
            }

            if (!(string.IsNullOrEmpty(user.FirstName)))
            {
                appUser.FirstName = user.FirstName;
            }

            if (!(string.IsNullOrEmpty(user.LastName)))
            {
                appUser.LastName = user.LastName;
            }

            if (!(string.IsNullOrEmpty(user.Zipcode)))
            {
                appUser.Zipcode = user.Zipcode;
            }

            if (user.BirthDate!= null)
            {
                appUser.BirthDate = user.BirthDate;
            }

            return appUser;
        }
    }
}

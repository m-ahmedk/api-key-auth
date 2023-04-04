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

        [HttpGet(Name = "GetUsers")]
        [ResponseCache(Duration = 10800, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> GetUsers()
        {
            List<AppUser> Users = new List<AppUser>();

            try
            {
                Users = await _context.AppUsers.ToListAsync();
                return Ok(Users);
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, Users);
            }
        }

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
       
        [HttpPost(Name = "CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] AppUser user)
        {
            try
            {
                _context.AppUsers.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut(Name = "UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(int? id, [FromBody] AppUser user)
        {
            try
            {
                if (id != user.UserId)
                {
                    return BadRequest();
                }

                _context.Entry(user).State = EntityState.Modified;
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
    }
}

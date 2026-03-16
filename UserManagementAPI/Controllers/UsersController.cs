using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // In-memory data store
        private static readonly List<User> _users = new List<User>
        {
            new User { Id = 1, Name = "John Doe", Email = "john@example.com", Role = "Admin", CreatedAt = DateTime.UtcNow },
            new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Role = "User", CreatedAt = DateTime.UtcNow }
        };

        // GET: api/users
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            try
            {
                // Optimization: Use ToList() to prevent potential deferred execution issues
                // and return directly.
                return Ok(_users.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while retrieving users.", details = ex.Message });
            }
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            try
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound(new { error = $"User with ID {id} not found." });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while retrieving the user.", details = ex.Message });
            }
        }

        // POST: api/users
        [HttpPost]
        public ActionResult<User> CreateUser([FromBody] User newUser)
        {
            try
            {
                if (newUser == null)
                {
                    return BadRequest(new { error = "User data is missing from the request body." });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Simple ID generation
                newUser.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
                newUser.CreatedAt = DateTime.UtcNow;

                _users.Add(newUser);

                return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while creating the user.", details = ex.Message });
            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            try
            {
                if (updatedUser == null)
                {
                    return BadRequest(new { error = "User data is missing from the request body." });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUser = _users.FirstOrDefault(u => u.Id == id);
                if (existingUser == null)
                {
                    return NotFound(new { error = $"User with ID {id} not found." });
                }

                // Update properties
                existingUser.Name = updatedUser.Name;
                existingUser.Email = updatedUser.Email;
                existingUser.Role = updatedUser.Role;
                // Note: Id and CreatedAt are preserved

                return Ok(existingUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while updating the user.", details = ex.Message });
            }
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound(new { error = $"User with ID {id} not found." });
                }

                _users.Remove(user);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while deleting the user.", details = ex.Message });
            }
        }
    }
}

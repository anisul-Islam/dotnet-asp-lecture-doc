using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_db_api.Models;
using ecommerce_db_api.Services;
using ecommerce_db_api.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_db_api.Controllers
{
    [ApiController, Route("/api/users")]

    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }


        // POST => /api/users => Create an User
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto newUser)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine($"Inside the ModelState");

                // Log the errors or handle them as needed
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                Console.WriteLine("Validation errors:");
                errors.ForEach(error => Console.WriteLine(error));

                // Return a custom response with validation errors
                return BadRequest(new { Message = "Validation failed", Errors = errors });
            }

            try
            {
                var user = await _userService.CreateUserServiceAsync(newUser);
                return ApiResponse.Created(user, "User is created");
                // return Created("/api/users/${user.UserId}", user);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, "Server error: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, "Server error: " + ex.Message);
            }
        }


        // ReadUsers
        // ReadUser
        // DeleteUser
        // UpdateUser
        // LoginUser
        // LogoutUser
        // banUser
        // unBanUser
        // UpdateUserRole
        // UpdateUserRole
    }
}
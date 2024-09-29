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
    [ApiController, Route("/api/categories")]

    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        // POST => /api/categories => Create a category
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto newCategory)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse.BadRequest("Invalid category Data");
            }
            try
            {
                var category = await _categoryService.CreateCategoryServiceAsync(newCategory);
                return ApiResponse.Created(category, "Category is created");
            }
            catch (ApplicationException ex)
            {
                return ApiResponse.ServerError("Server error: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                return ApiResponse.ServerError("Server error: " + ex.Message);
            }
        }

        // // GET => /api/users => RETURN all the Users
        // [HttpGet]
        // public async Task<IActionResult> GetUsers()
        // {
        //     try
        //     {
        //         var users = await _userService.GetUsersServiceAsync();
        //         return ApiResponse.Success(users, "Users are returned succesfully");
        //     }
        //     catch (ApplicationException ex)
        //     {
        //         return ApiResponse.ServerError("Server error: " + ex.Message);
        //     }
        //     catch (System.Exception ex)
        //     {
        //         return ApiResponse.ServerError("Server error: " + ex.Message);
        //     }
        // }

        // // // GET => /api/users/{userId} => return a single User
        // [HttpGet("{userId}")]
        // public async Task<IActionResult> GetUser(Guid userId)
        // {
        //     try
        //     {
        //         var user = await _userService.GetUserServiceByIdAsync(userId);
        //         if (user == null)
        //         {
        //             return ApiResponse.NotFound($"User with this id {userId} does not exist");
        //         }
        //         return ApiResponse.Success(user, "User is returned succesfully");
        //     }
        //     catch (ApplicationException ex)
        //     {
        //         return ApiResponse.ServerError("Server error: " + ex.Message);
        //     }
        //     catch (System.Exception ex)
        //     {
        //         return ApiResponse.ServerError("Server error: " + ex.Message);
        //     }
        // }

        // // DELETE => /api/users/{userId} => delete a single User
        // [HttpDelete("{userId}")]
        // public async Task<IActionResult> DeleteUser(Guid userId)
        // {
        //     try
        //     {
        //         var result = await _userService.DeleteUserServiceByIdAsync(userId);
        //         if (result == false)
        //         {
        //             return ApiResponse.NotFound($"User with this id {userId} does not exist");
        //         }
        //         return ApiResponse.Success(result, "User is deleted succesfully");
        //     }
        //     catch (ApplicationException ex)
        //     {
        //         return ApiResponse.ServerError("Server error: " + ex.Message);
        //     }
        //     catch (System.Exception ex)
        //     {
        //         return ApiResponse.ServerError("Server error: " + ex.Message);
        //     }
        // }

        // // PUT => /api/users/{userId} => Update an User
        // [HttpPut("{userId}")]
        // public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUser, Guid userId)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return ApiResponse.BadRequest("Invalid User Data");
        //     }
        //     try
        //     {
        //         var user = await _userService.UpdateUserServiceAsync(updateUser, userId);
        //         return ApiResponse.Success(user, "User is updated");
        //     }
        //     catch (ApplicationException ex)
        //     {
        //         return ApiResponse.ServerError("Server error: " + ex.Message);
        //     }
        //     catch (System.Exception ex)
        //     {
        //         return ApiResponse.ServerError("Server error: " + ex.Message);
        //     }
        // }




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

// Create => context.TableName.AddAsync(user), context.TableName.SaveChangesAsync()
// Read all the data =>  context.TableName.ToListAsync()
// Read a single data =>  context.TableName.FindAsync()
// Update =>  context.TableName.Remove(user), context.TableName.SaveChangesAsync()
// Delete =>  context.TableName.UpdateAsync(), context.TableName.SaveChangesAsync()

// AutoMapper => Map automatically (Entity <=> Dtos)
// Step 1: install extension
// Step 2: add the service
// Step 3: Create automapper class 
// Step 4: Create the service
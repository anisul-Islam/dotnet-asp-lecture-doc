using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_db_api.Models;
using ecommerce_db_api.Models.products;
using ecommerce_db_api.Services;
using ecommerce_db_api.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_db_api.Controllers
{
    [ApiController, Route("/api/products")]

    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        public ProductController(ProductService productService)
        {
            _productService = productService;
        }


        // POST => /api/users => Create an User
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProdutDto newProduct)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse.BadRequest("Invalid product Data");
            }
            try
            {
                var product = await _productService.CreateProductServiceAsync(newProduct);
                return ApiResponse.Created(product, "Product is created");
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

        // GET => /api/products => RETURN all the product
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                Console.WriteLine($"----test1---------");
                
                var products = await _productService.GetProductsServiceAsync();
                Console.WriteLine($"----test2---------");

                return ApiResponse.Success(products, "Products are returned succesfully");
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

        // // GET => /api/users/{userId} => return a single User
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
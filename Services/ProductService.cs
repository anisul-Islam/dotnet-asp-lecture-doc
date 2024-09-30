using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ecommerce_db_api.EFCore;
using ecommerce_db_api.Models;
using ecommerce_db_api.Models.categories;
using ecommerce_db_api.Models.products;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ecommerce_db_api.Services
{
    public class ProductService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public ProductService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<ProductDto> CreateProductServiceAsync(CreateProdutDto newProduct)
        {
            try
            {
                var product = _mapper.Map<Product>(newProduct);
                await _appDbContext.Products.AddAsync(product);
                await _appDbContext.SaveChangesAsync();
                // product entity here => Product Dto 
                var productData = _mapper.Map<ProductDto>(product);
                return productData;
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions (like unique constraint violations)
                Console.WriteLine($"Database Update Error: {dbEx.Message}");
                throw new ApplicationException("An error occurred while saving to the database. Please check the data and try again.");
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred. Please try again later.");
            }
        }

        public async Task<List<ProductDto>> GetProductsServiceAsync()
        {
            try
            {
                Console.WriteLine($"----------test2---------");

                var products = await _appDbContext.Products.Include(p => p.Category).ToListAsync();
                Console.WriteLine($"----------test3---------");

                var productsData = _mapper.Map<List<ProductDto>>(products);

                Console.WriteLine($"----------test4---------");

                return productsData;
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions (like unique constraint violations)
                Console.WriteLine($"Database Update Error: {dbEx.Message}");
                throw new ApplicationException("An error occurred while saving to the database. Please check the data and try again.");
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred. Please try again later.");
            }
        }

        // // public async Task<UserDto?> GetUserServiceByIdAsync(Guid userId)
        // {
        //     try
        //     {
        //         var user = await _appDbContext.Users.FindAsync(userId);
        //         if (user == null)
        //         {
        //             return null;
        //         }
        //         // User Model to UserDto
        //         // User Model to UserDto
        //         var userData = _mapper.Map<UserDto>(user);
        //         return userData;
        //     }
        //     catch (DbUpdateException dbEx)
        //     {
        //         // Handle database update exceptions (like unique constraint violations)
        //         Console.WriteLine($"Database Update Error: {dbEx.Message}");
        //         throw new ApplicationException("An error occurred while saving to the database. Please check the data and try again.");
        //     }
        //     catch (Exception ex)
        //     {
        //         // Handle any other unexpected exceptions
        //         Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        //         throw new ApplicationException("An unexpected error occurred. Please try again later.");
        //     }
        // }
        // public async Task<bool> DeleteUserServiceByIdAsync(Guid userId)
        // {
        //     try
        //     {
        //         var user = await _appDbContext.Users.FindAsync(userId);
        //         if (user == null)
        //         {
        //             return false;
        //         }
        //         _appDbContext.Users.Remove(user);
        //         await _appDbContext.SaveChangesAsync();
        //         return true;
        //     }
        //     catch (DbUpdateException dbEx)
        //     {
        //         // Handle database update exceptions (like unique constraint violations)
        //         Console.WriteLine($"Database Update Error: {dbEx.Message}");
        //         throw new ApplicationException("An error occurred while saving to the database. Please check the data and try again.");
        //     }
        //     catch (Exception ex)
        //     {
        //         // Handle any other unexpected exceptions
        //         Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        //         throw new ApplicationException("An unexpected error occurred. Please try again later.");
        //     }
        // }

        // public async Task<UserDto?> UpdateUserServiceAsync(UpdateUserDto updateUser, Guid userId)
        // {
        //     try
        //     {
        //         var user = await _appDbContext.Users.FindAsync(userId);
        //         if (user == null)
        //         {
        //             return null;
        //         }

        //         user.UserName = updateUser.UserName ?? user.UserName;

        //         if (updateUser.Password != null)
        //         {
        //             user.Password = BCrypt.Net.BCrypt.HashPassword(updateUser.Password) ?? user.Password;
        //         }

        //         user.Address = updateUser.Address ?? user.Address;
        //         user.Image = updateUser.Image ?? user.Image;

        //         _appDbContext.Update(user);
        //         await _appDbContext.SaveChangesAsync();

        //         // User Model to UserDto
        //         var usersData = _mapper.Map<UserDto>(user);
        //         return usersData;
        //     }
        //     catch (DbUpdateException dbEx)
        //     {
        //         // Handle database update exceptions (like unique constraint violations)
        //         Console.WriteLine($"Database Update Error: {dbEx.Message}");
        //         throw new ApplicationException("An error occurred while saving to the database. Please check the data and try again.");
        //     }
        //     catch (Exception ex)
        //     {
        //         // Handle any other unexpected exceptions
        //         Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        //         throw new ApplicationException("An unexpected error occurred. Please try again later.");
        //     }
        // }


    }
}
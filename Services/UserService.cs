using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_db_api.EFCore;
using ecommerce_db_api.Models;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_db_api.Services
{
    public class UserService
    {
        private readonly AppDbContext _appDbContext;

        public UserService(AppDbContext appDbContext){
            _appDbContext = appDbContext;
        }

        public async Task<User> CreateUserServiceAsync(CreateUserDto newUser){
           try
           {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUser.Password);

                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    UserName = newUser.UserName,
                    Email = newUser.Email,
                    Password = hashedPassword,
                    Address = newUser.Address ?? string.Empty,
                    Image = newUser.Image ?? string.Empty,
                };

                await _appDbContext.AddAsync(user);
                await _appDbContext.SaveChangesAsync();
                return user;
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions (like unique constraint violations)
                Console.WriteLine($"Database Update Error: {dbEx.Message}");
                throw new ApplicationException("An error occurred while saving to the database. Please check the data and try again.");
            }
            catch (ValidationException valEx)
            {
                // Handle validation exceptions
                Console.WriteLine($"Validation Error: {valEx.Message}");
                throw new ApplicationException("Validation failed for the provided data.");
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred. Please try again later.");
            }
        }
    }
}
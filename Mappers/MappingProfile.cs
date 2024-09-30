using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ecommerce_db_api.EFCore;
using ecommerce_db_api.Models;
using ecommerce_db_api.Models.categories;
using ecommerce_db_api.Models.products;

namespace ecommerce_db_api.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>();

            CreateMap<CreateCategoryDto, Category>();
            CreateMap<Category, CategoryDto>();

            CreateMap<CreateProdutDto, Product>();
            CreateMap<Product, ProductDto>();
        }
    }
}
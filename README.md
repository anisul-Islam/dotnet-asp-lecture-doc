##### P23. Product API => Sorting by Name, Price, CreatedAt

```csharp
// Model
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp_ecommerce_web_api.Models
{
    public class PagedResult<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public string? SearchQuery { get; set; } = null;
        public string? SortBy { get; set; } = null;
        public string? SortOrder { get; set; } = "asc";
        public IEnumerable<T> Items { get; set; }
    }

}

// Services
public interface IProductService
{
  PagedResult<ProductDto> GetAllProducts(int pageNumber, int pageSize, string? SearchQuery, string? sortBy, string? sortOrder);
  ProductDto GetProductById(Guid id);
  Product CreateProduct(CreateProductDto newProduct);
  bool ProductExistsByName(string name);
  void DeleteProduct(Guid id);
  void UpdateProduct(Guid id, UpdateProductDto updateProduct);
}

public PagedResult<ProductDto> GetAllProducts(int pageNumber, int pageSize, string? searchQuery = null, string? sortBy = null, string? sortOrder = "asc")
  {
    var filteredProducts = _products.AsQueryable();

    // Apply search filter if a searchQuery is provided
    if (!string.IsNullOrEmpty(searchQuery))
    {
      filteredProducts = filteredProducts.Where(p => p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
    }

    // Apply sorting based on the sortBy and sortOrder parameters
    filteredProducts = sortBy?.ToLower() switch
    {
      "name" => sortOrder == "desc" ? filteredProducts.OrderByDescending(p => p.Name) : filteredProducts.OrderBy(p => p.Name),
      "price" => sortOrder == "desc" ? filteredProducts.OrderByDescending(p => p.Price) : filteredProducts.OrderBy(p => p.Price),
      "date" => sortOrder == "desc" ? filteredProducts.OrderByDescending(p => p.CreatedAt) : filteredProducts.OrderBy(p => p.CreatedAt),
      _ => filteredProducts.OrderBy(p => p.Name) // Default sorting by Name
    };

    var totalProducts = filteredProducts.Count();
    var paginatedProducts = filteredProducts.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(p => new ProductDto
    {
      Id = p.Id,
      Name = p.Name,
      Description = p.Description,
      Price = p.Price,
      CreatedAt = p.CreatedAt
    }).ToList();


    return new PagedResult<ProductDto>
    {
      PageNumber = pageNumber,
      PageSize = pageSize,
      TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize),
      TotalItems = totalProducts,
      Items = paginatedProducts
    };

  }


// Controller
 // GET: /api/products
  [HttpGet]
  public IActionResult GetAllProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 3, [FromQuery] string? searchQuery = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = "asc")
  {
    if (pageNumber < 1 || pageSize < 1)
    {
      return BadRequest("Page number and page size must be greater than 0.");
    }

    var paginatedResult = _productService.GetAllProducts(pageNumber, pageSize, searchQuery, sortBy, sortOrder);
    return Ok(paginatedResult);
  }
```

```http
Example Requests:
Sort by name ascending (default):
GET /api/products?sortBy=name&sortOrder=asc

Sort by name descending:
GET /api/products?sortBy=name&sortOrder=desc

Sort by price ascending:
GET /api/products?sortBy=price&sortOrder=asc

Sort by created date descending:
GET /api/products?sortBy=date&sortOrder=desc
```

##### P24. Concept => Handle the parameters more cleaner way

Handling method parameters in a cleaner and more maintainable way can be achieved by encapsulating them into a dedicated class or object. This approach simplifies method signatures, improves readability, and makes it easier to manage and extend parameters.

##### P25. Product API => Handle the parameters more cleaner way

```csharp
// Create the following class in a model or DTOs folder (best thing to do) or Requests or Filter or Queries Folder
public class ProductQueryParameters
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchQuery { get; set; } = null;
    public string? SortBy { get; set; } = null;
    public string? SortOrder { get; set; } = "asc";
}

// Change is controller
  // GET: /api/products
  [HttpGet]
  public IActionResult GetAllProducts([FromQuery] ProductQueryParameters productQueryParameters)
  {
    if (productQueryParameters.PageNumber < 1 || productQueryParameters.PageSize < 1)
    {
      return BadRequest("Page number and page size must be greater than 0.");
    }

    var paginatedResult = _productService.GetAllProducts(productQueryParameters);
    return Ok(paginatedResult);
  }

// Change is service
public interface IProductService
{
  PagedResult<ProductDto> GetAllProducts(ProductQueryParameters productQueryParameters);
  ProductDto GetProductById(Guid id);
  Product CreateProduct(CreateProductDto newProduct);
  bool ProductExistsByName(string name);
  void DeleteProduct(Guid id);
  void UpdateProduct(Guid id, UpdateProductDto updateProduct);
}

public PagedResult<ProductDto> GetAllProducts(ProductQueryParameters productQueryParameters)
  {
    var filteredProducts = _products.AsQueryable();

    // Apply search filter if a searchQuery is provided
    if (!string.IsNullOrEmpty(productQueryParameters.SearchQuery))
    {
      filteredProducts = filteredProducts.Where(p => p.Name.Contains(productQueryParameters.SearchQuery, StringComparison.OrdinalIgnoreCase));
    }

    // Apply sorting based on the sortBy and sortOrder parameters
    filteredProducts = productQueryParameters.SortBy?.ToLower() switch
    {
      "name" => productQueryParameters.SortOrder == "desc" ? filteredProducts.OrderByDescending(p => p.Name) : filteredProducts.OrderBy(p => p.Name),
      "price" => productQueryParameters.SortOrder == "desc" ? filteredProducts.OrderByDescending(p => p.Price) : filteredProducts.OrderBy(p => p.Price),
      "date" => productQueryParameters.SortOrder == "desc" ? filteredProducts.OrderByDescending(p => p.CreatedAt) : filteredProducts.OrderBy(p => p.CreatedAt),
      _ => filteredProducts.OrderBy(p => p.Name) // Default sorting by Name
    };

    var totalProducts = filteredProducts.Count();
    var paginatedProducts = filteredProducts.Skip((productQueryParameters.PageNumber - 1) * productQueryParameters.PageSize).Take(productQueryParameters.PageSize).Select(p => new ProductDto
    {
      Id = p.Id,
      Name = p.Name,
      Description = p.Description,
      Price = p.Price,
      CreatedAt = p.CreatedAt
    }).ToList();


    return new PagedResult<ProductDto>
    {
      PageNumber = productQueryParameters.PageNumber,
      PageSize = productQueryParameters.PageSize,
      TotalPages = (int)Math.Ceiling(totalProducts / (double)productQueryParameters.PageSize),
      TotalItems = totalProducts,
      Items = paginatedProducts
    };

  }

// 4. Update the PagedResult Class. You can remove redundant parameters from PagedResult, as they are already encapsulated in ProductQueryParameters.

public class PagedResult<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public IEnumerable<T> Items { get; set; }
}

```

##### P26. Concept => Validation with Data Annotations Validators

To add comprehensive data annotations to your DTOs in ASP.NET Core, we can use attributes that are part of the System.ComponentModel.DataAnnotations namespace. These annotations are primarily used for validation, which makes your code more robust by ensuring that incoming data follows certain rules.

- [visit here](https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/models-data/validation-with-the-data-annotation-validators-cs)
- what happen if you do not pass user name and it creates the user? it should be a bad request 400
- add the data annotation to the DTOs
<!-- - add the nuget package (MinimalApis.Extensions) from the package manager (add the vsextension) -->

##### P27. Product API => Add Input Validation to the DTOs

```csharp

using System.ComponentModel.DataAnnotations;

public class CreateProductDto
{
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Product price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Product price must be greater than zero.")]
    public decimal Price { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string? Description { get; set; }
}

using System.ComponentModel.DataAnnotations;

public class UpdateProductDto
{
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 characters.")]
    public string? Name { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Product price must be greater than zero.")]
    public decimal? Price { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string? Description { get; set; }
}

using System.ComponentModel.DataAnnotations;

public class ProductDto
{
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Product price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Product price must be greater than zero.")]
    public decimal Price { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string? Description { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
}


public class PagedResult<T>
{
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1.")]
    public int PageNumber { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Page size must be at least 1.")]
    public int PageSize { get; set; }

    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public IEnumerable<T> Items { get; set; }
}

```

- [Required]: Ensures that the Name and Price fields are not null or empty.
- [StringLength]: Enforces the minimum and maximum length of the Name and Description.
- [Range]: Ensures that Price is greater than zero.

- more detailed example

```csharp
using System.ComponentModel.DataAnnotations;

public class User
{
  public Guid UserId { get; set; }

  [Required(ErrorMessage = "User Name is Required")]
  [MaxLength(32, ErrorMessage = "User Name must be less than 32 characters")]
  [MinLength(2, ErrorMessage = "User Name must be at least 2 characters")]
  public required string Name { get; set; }

  [EmailAddress(ErrorMessage = "User Email is not a valid email")]

  public required string Email { get; set; }

  [MinLength(6, ErrorMessage = "User Password must be at least 2 characters")]
  public required string Password { get; set; }
  public string Address { get; set; } = string.Empty;
  public string Image { get; set; } = string.Empty;
  public bool IsAdmin { get; set; }
  public bool IsBanned { get; set; }
  public DateTime CreatedAt { get; set; }
}
```

##### P28. Concept => Exception Handling

```csharp
// Controllers/UserController.cs
   public IActionResult GetAllUsers()
        {
            try
            {
                var users = _userService.GetAllUsersService();
                if (users.ToList().Count < 1)
                {
                    return NotFound();
                }
                return Ok(new { message = "Users retrieved successfully", data = users });
            }
            catch (Exception ex)
            {

                // Log the exception for debugging and monitoring purposes
                Console.WriteLine($"An error occurred while retrieving users: {ex.Message}");

                // Return a server error response with a meaningful error message
                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }
```

##### P29. Product API => Add Exception Handling

```csharp
// Service.cs
public PagedResult<ProductDto> GetAllProducts(ProductQueryParameters productQueryParameters)
{
    try
    {
        var filteredProducts = _products.AsQueryable();

        // Apply search filter if a searchQuery is provided
        if (!string.IsNullOrEmpty(productQueryParameters.SearchQuery))
        {
            filteredProducts = filteredProducts.Where(p => p.Name.Contains(productQueryParameters.SearchQuery, StringComparison.OrdinalIgnoreCase));
        }

        // Apply sorting based on the sortBy and sortOrder parameters
        filteredProducts = productQueryParameters.SortBy?.ToLower() switch
        {
            "name" => productQueryParameters.SortOrder == "desc" ? filteredProducts.OrderByDescending(p => p.Name) : filteredProducts.OrderBy(p => p.Name),
            "price" => productQueryParameters.SortOrder == "desc" ? filteredProducts.OrderByDescending(p => p.Price) : filteredProducts.OrderBy(p => p.Price),
            "date" => productQueryParameters.SortOrder == "desc" ? filteredProducts.OrderByDescending(p => p.CreatedAt) : filteredProducts.OrderBy(p => p.CreatedAt),
            _ => filteredProducts.OrderBy(p => p.Name) // Default sorting by Name
        };

        var totalProducts = filteredProducts.Count(); // Synchronous Count for in-memory collection
        var paginatedProducts = filteredProducts
            .Skip((productQueryParameters.PageNumber - 1) * productQueryParameters.PageSize)
            .Take(productQueryParameters.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CreatedAt = p.CreatedAt
            })
            .ToList(); // Synchronous ToList

        return new PagedResult<ProductDto>
        {
            PageNumber = productQueryParameters.PageNumber,
            PageSize = productQueryParameters.PageSize,
            TotalPages = (int)Math.Ceiling(totalProducts / (double)productQueryParameters.PageSize),
            TotalItems = totalProducts,
            Items = paginatedProducts
        };
    }
    catch (Exception ex)
    {
        // Log the exception (optional)
        // _logger.LogError(ex, "Error occurred while getting products");

        // Re-throw the exception to be handled at a higher level or return a custom result
        throw new ApplicationException("An error occurred while retrieving products.", ex);
    }
}

//Controller.cs
[HttpGet]
public IActionResult GetAllProducts([FromQuery] ProductQueryParameters productQueryParameters)
{
    try
    {
        if (productQueryParameters.PageNumber < 1 || productQueryParameters.PageSize < 1)
        {
            return BadRequest("Page number and page size must be greater than 0.");
        }

        var paginatedResult = _productService.GetAllProducts(productQueryParameters);
        return Ok(paginatedResult);
    }
    catch (ApplicationException ex)
    {
        // Return a custom error response
        return StatusCode(500, new { Message = ex.Message });
    }
    catch (Exception ex)
    {
        // Log the error (optional)
        // _logger.LogError(ex, "An unexpected error occurred");

        // Return a generic error message
        return StatusCode(500, new { Message = "An unexpected error occurred. Please try again later." });
    }
}

```

##### P30 Concept => Centralized Error Response

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public ApiResponse(bool success, string message, T? data = default, List<string>? errors = null)
    {
        Success = success;
        Message = message;
        Data = data;
        Errors = errors;
    }

    public static ApiResponse<T> SuccessResponse(string message, T? data = default)
    {
        return new ApiResponse<T>(true, message, data);
    }

    public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>(false, message, default, errors);
    }
}

// Controller
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: /api/products
    [HttpGet]
    public IActionResult GetAllProducts([FromQuery] ProductQueryParameters productQueryParameters)
    {
        try
        {
            var paginatedResult = _productService.GetAllProducts(productQueryParameters);
            return Ok(ApiResponse<PagedResult<ProductDto>>.SuccessResponse("Products fetched successfully", paginatedResult));
        }
        catch (Exception ex)
        {
            // Log the error (optional)
            // _logger.LogError(ex, "An unexpected error occurred");

            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while fetching products.", new List<string> { ex.Message }));
        }
    }

    // POST: /api/products
    [HttpPost]
    public IActionResult CreateProduct([FromBody] CreateProductDto newProduct)
    {
        if (string.IsNullOrEmpty(newProduct.Name))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Product name is required"));
        }

        if (newProduct.Price <= 0)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Product price must be greater than zero"));
        }

        try
        {
            var createdProduct = _productService.CreateProduct(newProduct);
            return CreatedAtAction(nameof(GetAllProducts), new { id = createdProduct.Id }, ApiResponse<ProductDto>.SuccessResponse("Product created successfully", createdProduct));
        }
        catch (Exception ex)
        {
            // Log the error (optional)
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while creating the product.", new List<string> { ex.Message }));
        }
    }

    // PUT: /api/products/{id}
    [HttpPut("{id:guid}")]
    public IActionResult UpdateProduct(Guid id, [FromBody] UpdateProductDto updatedProduct)
    {
        try
        {
            var product = _productService.UpdateProduct(id, updatedProduct);
            return Ok(ApiResponse<ProductDto>.SuccessResponse("Product updated successfully", product));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Product not found", new List<string> { ex.Message }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while updating the product.", new List<string> { ex.Message }));
        }
    }

    // DELETE: /api/products/{id}
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteProduct(Guid id)
    {
        try
        {
            _productService.DeleteProduct(id);
            return Ok(ApiResponse<object>.SuccessResponse("Product deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Product not found", new List<string> { ex.Message }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while deleting the product.", new List<string> { ex.Message }));
        }
    }
}

// Services
public class ProductService : IProductService
{
    private readonly List<Product> _products;

    public ProductService()
    {
        _products = new List<Product>(); // Example: In-memory product list
    }

    public ProductDto CreateProduct(CreateProductDto newProduct)
    {
        // Simulate checking for a product with the same name
        var existingProduct = _products.FirstOrDefault(p => p.Name.Equals(newProduct.Name, StringComparison.OrdinalIgnoreCase));

        if (existingProduct != null)
        {
            // Throwing a custom exception if the product name already exists
            throw new InvalidOperationException($"A product with the name '{newProduct.Name}' already exists.");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = newProduct.Name,
            Price = newProduct.Price,
            Description = newProduct.Description,
            CreatedAt = DateTime.Now
        };

        _products.Add(product);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            CreatedAt = product.CreatedAt
        };
    }

    // Other CRUD methods...
}
```

##### P31. Concept => Asynchronous

Asycnhornous improves scalability by freeing up threads for other requests while waiting for IO-bound operations to complete.

Converting a synchronous method to an asynchronous one in C# typically involves utilizing the `async` and `await` keywords, as well as leveraging asynchronous APIs like `Task`, `Task<T>`, and others.

Here’s a step-by-step guide on how to convert your synchronous service methods to asynchronous ones:

###### Steps to Convert Sync to Async

1. **Add the `async` Keyword**:

   - The method signature should include the `async` keyword.

2. **Use `Task` or `Task<T>` as Return Type**:

   - For methods that return `void` synchronously, use `Task`.
   - For methods that return a specific type (like a list of products), use `Task<T>`, where `T` is the return type.

3. **Use `await` with Asynchronous Methods**:

   - Replace blocking calls (e.g., `.ToList()`, `.Find()`) with their asynchronous counterparts (e.g., `ToListAsync()`, `FirstOrDefaultAsync()`).

4. **Leverage Asynchronous Libraries**:
   - For database operations (e.g., Entity Framework), use asynchronous methods like `ToListAsync()`, `FirstOrDefaultAsync()`, etc.

###### Example: Converting `GetAllProducts` to Async

Here’s how you can convert the synchronous `GetAllProducts` method to an asynchronous one:

###### **Synchronous Method**

```csharp
public PagedResult<ProductDto> GetAllProducts(ProductQueryParameters queryParameters)
{
    var filteredProducts = _products.AsQueryable();

    // Apply search filter
    if (!string.IsNullOrEmpty(queryParameters.SearchQuery))
    {
        filteredProducts = filteredProducts.Where(p => p.Name.Contains(queryParameters.SearchQuery, StringComparison.OrdinalIgnoreCase));
    }

    // Apply sorting
    filteredProducts = queryParameters.SortBy?.ToLower() switch
    {
        "name" => queryParameters.SortOrder == "desc" ? filteredProducts.OrderByDescending(p => p.Name) : filteredProducts.OrderBy(p => p.Name),
        "price" => queryParameters.SortOrder == "desc" ? filteredProducts.OrderByDescending(p => p.Price) : filteredProducts.OrderBy(p => p.Price),
        "date" => queryParameters.SortOrder == "desc" ? filteredProducts.OrderByDescending(p => p.CreatedAt) : filteredProducts.OrderBy(p => p.CreatedAt),
        _ => filteredProducts.OrderBy(p => p.Name)
    };

    var totalProducts = filteredProducts.Count();
    var paginatedProducts = filteredProducts.Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
        .Take(queryParameters.PageSize)
        .Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            CreatedAt = p.CreatedAt
        })
        .ToList();

    return new PagedResult<ProductDto>
    {
        PageNumber = queryParameters.PageNumber,
        PageSize = queryParameters.PageSize,
        TotalPages = (int)Math.Ceiling(totalProducts / (double)queryParameters.PageSize),
        TotalItems = totalProducts,
        Items = paginatedProducts
    };
}
```

###### **Converted Asynchronous Method**

To convert this method, let’s assume you are using Entity Framework (which has async methods like `ToListAsync()`, `CountAsync()`). Here's the async version of this method:

```csharp
public async Task<PagedResult<ProductDto>> GetAllProductsAsync(ProductQueryParameters queryParameters)
{
    var filteredProducts = _products.AsQueryable();

    // Apply search filter
    if (!string.IsNullOrEmpty(queryParameters.SearchQuery))
    {
        filteredProducts = filteredProducts.Where(p => p.Name.Contains(queryParameters.SearchQuery, StringComparison.OrdinalIgnoreCase));
    }

    // Apply sorting
    filteredProducts = queryParameters.SortBy?.ToLower() switch
    {
        "name" => queryParameters.SortOrder == "desc" ? filteredProducts.OrderByDescending(p => p.Name) : filteredProducts.OrderBy(p => p.Name),
        "price" => queryParameters.SortOrder == "desc" ? filteredProducts.OrderByDescending(p => p.Price) : filteredProducts.OrderBy(p => p.Price),
        "date" => queryParameters.SortOrder == "desc" ? filteredProducts.OrderByDescending(p => p.CreatedAt) : filteredProducts.OrderBy(p => p.CreatedAt),
        _ => filteredProducts.OrderBy(p => p.Name)
    };

    var totalProducts = await filteredProducts.CountAsync(); // Asynchronous count
    var paginatedProducts = await filteredProducts.Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
        .Take(queryParameters.PageSize)
        .Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            CreatedAt = p.CreatedAt
        })
        .ToListAsync(); // Asynchronous ToList

    return new PagedResult<ProductDto>
    {
        PageNumber = queryParameters.PageNumber,
        PageSize = queryParameters.PageSize,
        TotalPages = (int)Math.Ceiling(totalProducts / (double)queryParameters.PageSize),
        TotalItems = totalProducts,
        Items = paginatedProducts
    };
}
```

- Key Points

1. **Async Task**: Use `Task<T>` for methods returning data and `Task` for void methods.
2. **Await Asynchronous Methods**: Ensure that any method that involves IO operations (e.g., database access) uses its asynchronous counterpart.
3. **No Synchronous Blocking**: Avoid synchronous methods like `.ToList()` or `.Count()` when performing IO-bound tasks like database queries.

This conversion improves scalability by freeing up threads for other requests while waiting for IO-bound operations to complete.

##### P32. Product API => Asynchronous

### 4. Complete Ecommerce REST API


### 5 User API

#### 5.1 Create the User Entity

- What is an Entity?

In the context of databases and Entity Framework Core (EF Core), an entity refers to a class that represents a table in a database. Each instance of the entity class corresponds to a row in the table, and the properties of the entity represent the columns of the table.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class User
{
      public Guid UserId { get; set; }
      public string UserName { get; set; } = string.Empty;
      public string Email { get; set; } = string.Empty;
      public string Password { get; set; } = string.Empty;
      public string? Address { get; set; }
      public string? Image { get; set; }
      public bool IsAdmin { get; set; } = false;
      public bool IsBanned { get; set; } = false;
      public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

#### 5.2 Create the User Table in Database

- Modify the DbContext

  ```csharp
  // EFCore/AppDbContext.cs
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) {}
    }

    public DbSet<User> Users { get; set; } // DbSet will convert LINQ TO SQL Queries
  ```

- run the migration script to change the schema

  ```shell
  dotnet ef migrations add UserTable
  dotnet ef database update
  ```

#### 5.3 Setup the Service and Controller for User (MVC Pattern)

- Create User Service

  ```csharp
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ecommerce_api.EFCore;


   public class UserService
    {
        private readonly AppDbContext _appDbContext;

        public UserService(AppDbContext appDbContext){
            _appDbContext = appDbContext;
        }
    }
  ```

- Create User Controller

  ```csharp
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ecommerce_api.Services;
    using Microsoft.AspNetCore.Mvc;


    [ApiController, Route("/api/users")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }
    }
  ```

- Register Services and controllers in Startup file

  ```csharp
  // Program.cs
  builder.Services.AddScoped<UserService>();
  // Program.cs
  builder.Services.AddControllers();
  app.MapControllers();
  ```

#### 5.4 POST => /api/users => Create an user

- Few methods for CRUD

  - Find all data => context.TableName.ToListAsync()
  - Find data => context.TableName.FindAsync(identidier);
  - Save data => context.TableName.AddAsync(newData); context.TableName.SaveChangesAsync();
  - Remove data => context.TableName.Remove(dataToBeDeleted); context.TableName.SaveChangesAsync();
  - Update data => context.TableName.Update(data); context.TableName.SaveChangesAsync();

- workflow of MVC

![alt text](image-17.png)

```csharp
// Models/Dtos/users/CreateUserDto.cs => What data you need to create an user?
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CreateUserDto
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Image { get; set; } = string.Empty;
}

// Create a Controller to handle request for creating an user
// POST => api/users => Create an User
[HttpPost]
public async Task<IActionResult> CreateUser([FromBody] CreateUserDto newUser)
{
    var user = await _userService.CreateUserAsync(newUser);
    var response = new { Message = "An user created successfully", User = user };
    return Created($"/api/users/{user.UserId}", response);
}

// Create a Service to store the user in DB
//  Services/UserService.cs
public async Task<User> CreateUserAsync(CreateUserDto newUserDto)
{
    // Hash the password using a library like BCrypt.Net
    // install => dotnet add package BCrypt.Net-Next
    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUserDto.Password);

    var user = new User
    {
        UserId = Guid.NewGuid(),
        UserName = newUserDto.UserName,
        Email = newUserDto.Email,
        Password = hashedPassword, // Store hashed password
        Address = newUserDto.Address,
        Image = newUserDto.Image
    };

    // - AddAsync: Marks the entity as to be inserted in the context.
    // - SaveChangesAsync: Actually commits the changes (including the new user) to the database.
    await _appDbContext.Users.AddAsync(user);
    await _appDbContext.SaveChangesAsync();
    return user;
}

```

#### 5.6 How to Respond with the desried data

```csharp
// Create the Models/UserDto.cs => Create a dto/template for what you want to return
// DTOs/users/CreateUser.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace review_ecommerce_api.Models
{
    public class UserDto
    {
        public Guid UserId { get; set; }

        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Image { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBanned { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

// Update the UserService.cs
  public async Task<UserDto> CreateUserAsync(CreateUserDto newUserDto)
{

    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUserDto.Password);

    var user = new User
    {
        UserName = newUserDto.UserName,
        Email = newUserDto.Email,
        Password = hashedPassword, // Store hashed password
        Address = newUserDto.Address,
        Image = newUserDto.Image
    };

    await _appDbContext.Users.AddAsync(user);
    await _appDbContext.SaveChangesAsync();

    var userResponse = new UserDto
    {
        UserId = user.UserId,
        UserName = user.UserName,
        Email = user.Email,
        Address = user.Address,
        Image = user.Image,
        IsAdmin = user.IsAdmin,
        IsBanned = user.IsBanned,
    };
    return userResponse;
}

```

#### 5.7 Concept: Data Validation With Data Annotation vs Fluent API

Data Annotations and Fluent API are both used in Entity Framework Core (EF Core) to configure entity classes and their properties, but they work differently in terms of how they handle validation, configuration, and exception handling.

##### 1. **Data Annotations:**

- **Purpose:** Data Annotations are declarative, attribute-based configurations applied directly to your model classes and properties. They provide a simple way to apply rules and validations.
- **How it works:** Data Annotations are placed directly on the properties of the entity class. They handle validation before the database interaction, and if the data doesn't meet the criteria, an exception will be raised when saving the changes to the database.
- **Example:**

  ```csharp
  public class User

  {
     [Required]
     [EmailAddress]
     [StringLength(100)]
     [Column(TypeName = "varchar(100)")]
     public string Email { get; set; }
  }
  ```

- **Exception Handling in Data Annotations:**

  - **Where:** Validation is done before changes are sent to the database (client-side). If an entity violates a rule, `ValidationException` will be raised before interacting with the database, and no database query will be executed.
  - **Exception Example:**

    ```csharp
     var user = new User { Email = "not-an-email"};
     _context.Users.Add(user);
     try
     {
         _context.SaveChanges();
     }
     catch (ValidationException ex)
     {
         Console.WriteLine(ex.Message); // Handles Data Annotation validation failure
     }
    ```

- **Advantages:**

  - Easier to use, especially for simple validations like `Required`, `StringLength`, `Range`, etc.
  - Validation occurs at the model level, simplifying form validations.

- **Disadvantages:**
  - Limited flexibility for more complex configurations.
  - Harder to reuse across different contexts and databases.

##### 2. **Fluent API:**

- **Purpose:** Fluent API is a more flexible and comprehensive way to configure EF Core models. It's ideal for more complex configurations that cannot be handled by Data Annotations.
- **How it works:** Fluent API is defined in the `OnModelCreating` method of the `DbContext` class, providing a more granular control over how entities are mapped to the database schema.
- **Example:**

  ```csharp
     protected override void OnModelCreating(ModelBuilder modelBuilder)

     {
         modelBuilder.Entity<User>(entity =>
         {
             // Configuring the Email property using Fluent API
             entity.Property(e => e.Email)
                 .IsRequired() // Equivalent to [Required]
                 .HasMaxLength(100) // Equivalent to [StringLength(100)]
                 .HasColumnType("varchar(100)") // Equivalent to [Column(TypeName = "varchar(100)")]
                 .HasAnnotation("EmailAddress", true); // Email validation, this can be custom and part of validation logic.
         });
     }
  ```

- **Exception Handling in Fluent API:**

  - **Where:** Fluent API configurations are enforced when EF Core interacts with the database (server-side). Unlike Data Annotations, these configurations apply at the database level, and exceptions such as `DbUpdateException` will be thrown during `SaveChanges()` when the rules are violated.
  - **Exception Example:**

    ```csharp
    try
    {
        _context.SaveChanges();
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine(ex.Message); // Handles Fluent API rule violations (like duplicate key, etc.)
    }
    ```

- **Advantages:**

  - More flexible and powerful for handling complex relationships, unique constraints, table splitting, and other advanced configurations.
  - Fluent API configurations are centralized in one place (inside `OnModelCreating`), making it easier to manage changes across the entire application.

- **Disadvantages:**
  - More verbose than Data Annotations for simple cases.
  - May require more understanding of EF Core internals to handle complex scenarios.

##### **Key Differences:**

| **Aspect**                 | **Data Annotations**                                                   | **Fluent API**                                                    |
| -------------------------- | ---------------------------------------------------------------------- | ----------------------------------------------------------------- |
| **Configuration Location** | Applied directly to the model properties using attributes.             | Configured centrally inside `OnModelCreating` in `DbContext`.     |
| **Complexity**             | Simple to use for basic configurations and validations.                | Suitable for complex configurations and relationships.            |
| **Scope of Application**   | Works at the **model** level and applies rules at **compile time**.    | Works at the **database** level and applies rules at **runtime**. |
| **Exception Handling**     | Throws `ValidationException` before interacting with the database.     | Throws `DbUpdateException` when saving to the database.           |
| **Flexibility**            | Limited in terms of complex configurations (e.g., unique constraints). | Full flexibility to define more advanced configurations.          |
| **Validation Location**    | Mostly client-side, before interaction with the database.              | Mostly server-side, enforced during database operations.          |

##### **Which One to Use?**

- **Data Annotations** are better for simpler models where basic validation and configuration are enough.
- **Fluent API** should be used for more complex scenarios, such as defining relationships, composite keys, or when dealing with custom database behaviors.

**Best Practice:** In a large project, you can combine both methods. Use **Data Annotations** for simpler, model-specific rules and use **Fluent API** for advanced configurations that require more control or aren't possible with Data Annotations alone.

#### 5.8 Add Data Validation with Data Annotation in CreateUserDto

you should primarily add data annotations to the DTO (Data Transfer Object), like CreateUserDto. The main reason is that the DTO represents the data being transferred between the client and server, so validation should focus on ensuring that the incoming data (from the client) is valid before it is mapped to the actual entity model (User). This keeps your entity models clean from validation logic, and instead focuses them on representing the structure of your data.

- Model Validation: The UserModel uses data annotations to ensure that all necessary fields are present and correctly formatted before the model reaches the service layer.

- Service Layer: The service layer converts the validated CreateUserModel into a User entity, ensuring that the business logic and data storage concerns are cleanly separated.

- Controller Logic: The controller checks ModelState.IsValid to determine if the incoming request data conforms to the defined validation rules. If the data is invalid, it returns a BadRequest response with details of the validation errors.

- Add Data annotation to the CreateUserDto

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace review_ecommerce_api.Models
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username must be between 3 and 50 characters.", MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "Password must be between 6 and 100 characters.", MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Address can't exceed 255 characters.")]
        public string? Address { get; set; }

        // [Url(ErrorMessage = "Invalid URL format for Image.")]
        public string? Image { get; set; }
    }
}
```

- update the controller

```csharp
 [HttpPost]
  public async Task<IActionResult> CreateUser([FromBody] CreateUserDto newUser)
  {
      // Check if the model is valid
      if (!ModelState.IsValid)
      {
          // Log the errors or handle them as needed
          var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
          Console.WriteLine("Validation errors:");
          errors.ForEach(error => Console.WriteLine(error));

          // Return a custom response with validation errors
          return BadRequest(new { Message = "Validation failed", Errors = errors });
      }


      var user = await _userService.CreateUserAsync(newUser);
      var response = new { Message = "User created successfully", User = user };
      return Created($"/api/users/{user.UserId}", response);

  }

```

##### Optionally Disable Automatic Model Validation (Optional)

If you don't want ASP.NET Core to automatically return the 400 Bad Request for validation errors, you can disable this feature by configuring it in Program.cs or in your controller setup:

```csharp
builder.Services.AddControllers()
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true; // Disable automatic model validation response
});

```

#### 5.9 Add Data Validation with Fluent Api in DBcontext

Choose one approach for each concern. For validation, stick with Data Annotations if possible; for complex database rules (e.g., unique constraints, relationships), use Fluent API. This keeps your code organized and avoids redundancy.

```csharp
// CreateUserDto
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace review_ecommerce_api.Models
{
    public class CreateUserDto
    {

        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "Password must be between 6 and 100 characters.", MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Address can't exceed 255 characters.")]
        public string? Address { get; set; }

        // [Url(ErrorMessage = "Invalid URL format for Image.")]
        public string? Image { get; set; }
    }
}

// AppDbContext User Entity
 protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
      modelBuilder.Entity<User>(entity =>
      {
          entity.HasKey(u => u.UserId); // Primary Key configuration
          entity.Property(u => u.UserId).HasDefaultValueSql("uuid_generate_v4()"); // Generate UUID for new records
          entity.Property(u => u.UserName).IsRequired().HasMaxLength(100);
          entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
          entity.HasIndex(u => u.Email).IsUnique();
          entity.Property(u => u.Password).IsRequired();
          entity.Property(u => u.Address).HasMaxLength(255);
          entity.Property(u => u.IsAdmin).HasDefaultValue(false);
          entity.Property(u => u.IsBanned).HasDefaultValue(false);
          entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
      });
  }
```

#### 5.10 Add Exception Handling

```csharp
// Controller
 [HttpPost]
  public async Task<IActionResult> CreateUser([FromBody] CreateUserDto newUser)
  {
      // Check if the model is valid
      if (!ModelState.IsValid)
      {
          // Log the errors or handle them as needed
          var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
          Console.WriteLine("Validation errors:");
          errors.ForEach(error => Console.WriteLine(error));

          // Return a custom response with validation errors
          return BadRequest(new { Message = "Validation failed", Errors = errors });
      }

      try
      {

          var user = await _userService.CreateUserAsync(newUser);
          var response = new { Message = "User created successfully", User = user };
          return Created($"/api/users/{user.UserId}", response);
      }
      catch (ApplicationException ex)
      {

          return StatusCode(500, "An error occured: " + ex.Message);
      }
      catch (Exception ex)
      {

          return StatusCode(500, "An error occured: " + ex.Message);
      }
  }


// Services
 public async Task<UserDto> CreateUserAsync(CreateUserDto newUserDto)
  {

      try
      {
          // Hash the password using a library like BCrypt.Net
          var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUserDto.Password);

          var user = new User
          {
              UserId = Guid.NewGuid(),
              UserName = newUserDto.UserName,
              Email = newUserDto.Email,
              Password = hashedPassword, // Store hashed password
              Address = newUserDto.Address,
              Image = newUserDto.Image
          };

          await _appDbContext.Users.AddAsync(user);
          await _appDbContext.SaveChangesAsync();

          var userResponse = new UserDto
              {
                  UserId = user.UserId,
                  UserName = user.UserName,
                  Email = user.Email,
                  Address = user.Address,
                  Image = user.Image,
                  IsAdmin = user.IsAdmin,
                  IsBanned = user.IsBanned,
              };
         return userResponse;

      }
      catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException postgresException)
      {
          if (postgresException.SqlState == "23505") // PostgreSQL unique constraint violation
          {
              throw new ApplicationException("Duplicate email. Please use a unique email address.");
          }
          else
          {
              // Handle other database-related errors
              throw new ApplicationException("An error occurred while adding the user.");
          }
      }
      catch (Exception ex)
      {
          // Handle any other unexpected exceptions
          Console.WriteLine($"An unexpected error occurred: {ex.Message}");
          throw new ApplicationException("An unexpected error occurred. Please try again later.");
      }
  }
```

#### 5.11 Centralized Response

- Inside the controller create a file called ApiResponseController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using System;

namespace api.Controllers
{
  public static class ApiResponse
  {
    // Central method to handle the creation of ApiResponseTemplate with ObjectResult
    private static IActionResult CreateApiResponse<T>(T? data, string message, int statusCode, bool success)
    {
      var response = new ApiResponseTemplate<T>(success, data, message, statusCode);
      return new ObjectResult(response)
      {
        StatusCode = statusCode
      };
    }

    public static IActionResult Success<T>(T data, string message = "Success")
    {
      return CreateApiResponse(data, message, StatusCodes.Status200OK, true);
    }

    public static IActionResult Created<T>(T data, string message = "Resource Created")
    {
      return CreateApiResponse(data, message, StatusCodes.Status201Created, true);
    }

    public static IActionResult NotFound(string message = "Resource not found")
    {
      return CreateApiResponse<object>(null, message, StatusCodes.Status404NotFound, false);
    }

    public static IActionResult Conflict(string message = "Conflict Detected")
    {
      return CreateApiResponse<object>(null, message, StatusCodes.Status409Conflict, false);
    }

    public static IActionResult BadRequest(string message = "Bad request")
    {
      return CreateApiResponse<object>(null, message, StatusCodes.Status400BadRequest, false);
    }

    public static IActionResult Unauthorized(string message = "Unauthorized access")
    {
      return CreateApiResponse<object>(null, message, StatusCodes.Status401Unauthorized, false);
    }

    public static IActionResult Forbidden(string message = "Forbidden access")
    {
      return CreateApiResponse<object>(null, message, StatusCodes.Status403Forbidden, false);
    }

    public static IActionResult ServerError(string message = "Internal server error")
    {
      return CreateApiResponse<object>(null, message, StatusCodes.Status500InternalServerError, false);
    }
  }

  public class ApiResponseTemplate<T>
  {
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }

    public ApiResponseTemplate(bool success, T? data, string message, int statusCode)
    {
      Success = success;
      Data = data;
      Message = message;
      StatusCode = statusCode;
    }
  }
}

```

- Use them now in controller

```csharp
  // POST => /api/users => Create an User
  [HttpPost]
  public async Task<IActionResult> CreateUser([FromBody] CreateUserDto newUser)
  {
      // Check if the model is valid
      if (!ModelState.IsValid)
      {
          // Log the errors or handle them as needed
          // var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
          // Console.WriteLine("Validation errors:");
          // errors.ForEach(error => Console.WriteLine(error));
          return ApiResponse.BadRequest("Invalid User Data");
      }

      try
      {
          var user = await _userService.CreateUserAsync(newUser);
          return ApiResponse.Created(user, "User created successfully");
      }
      catch (ApplicationException ex)
      {
          return ApiResponse.Conflict(ex.Message);
      }
      catch (Exception ex)
      {
          return ApiResponse.ServerError(ex.Message);
      }
  }
```

#### 5.12 GET => /api/users => Get all the users

- Create the UserDto

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace review_ecommerce_api.Models
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBanned { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
```

- Add a Controller to handle get users request

```csharp
  // GET => /api/users => Get all the users
  [HttpGet]
  public async Task<IActionResult> GetUsers()
  {
      try
      {
          var users = await _userService.GetUsersAsync();
          return ApiResponse.Success(users);
      }
      catch (ApplicationException ex)
      {
          return ApiResponse.ServerError(ex.Message);
      }
      catch (Exception ex)
      {
          // Log the exception details here to debug or trace issues
          Console.WriteLine($"Exception : {ex.Message}");
          return ApiResponse.ServerError("An unexpected error occurred.");
      }
  }

```

- Add a Service to get users from database

```csharp
   public async Task<List<UserDto>> GetUsersAsync()
  {
      try
      {
          var users = await _appDbContext.Users.ToListAsync();
          var requiredUsersData = users.Select(user => new UserDto
          {
              UserId = user.UserId,
              UserName = user.UserName,
              Email = user.Email,
              Address = user.Address,
              Image = user.Image,
              IsAdmin = user.IsAdmin,
              IsBanned = user.IsBanned,
              CreatedAt = DateTime.UtcNow
          }).ToList();
          return requiredUsersData;
      }
      catch (Exception ex)
      {
          // Handle any other unexpected exceptions
          Console.WriteLine($"An unexpected error occurred: {ex.Message}");
          throw new ApplicationException("An unexpected error occurred. Please try again later." + ex.Message);
      }
  }
```

- test the endpoint

```
GET localhost_address_here/api/users
```

#### 5.13 GET => /api/users => Add Pagination

- Add Pagination

```csharp
// create the PaginationResult in dtos/models folder
public class PaginatedResult<T>
{
  public IEnumerable<T> Items { get; set; } = new List<T>();
  public int TotalCount { get; set; }
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
  public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

// Controller
// GET => /api/users => Get all the users
  [HttpGet]
  public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
  {
      try
      {
          var users = await _userService.GetUsersAsync(pageNumber, pageSize);
          return ApiResponse.Success(users);
      }
      catch (ApplicationException ex)
      {
          return ApiResponse.ServerError(ex.Message);
      }
      catch (Exception ex)
      {
          Console.WriteLine($"Exception : {ex.Message}");
          return ApiResponse.ServerError("An unexpected error occurred.");
      }
  }

// Services
   public async Task<PaginatedResult<UserDto>> GetUsersAsync(int pageNumber, int pageSize)
  {
      try
      {
          // Calculate the total count of users
          var totalCount = await _appDbContext.Users.CountAsync();

          // Ensure pageNumber and pageSize are valid
          if (pageNumber < 1) pageNumber = 1;
          if (pageSize < 1) pageSize = 10;

          // Fetch paginated user data
          var users = await _appDbContext.Users
              .Skip((pageNumber - 1) * pageSize) // Skip to the correct page
              .Take(pageSize)                    // Take the correct page size
              .ToListAsync();

          // Map the fetched users to UserDto
          var usersDto = _mapper.Map<List<UserDto>>(users);

          // Return paginated result
          return new PaginatedResult<UserDto>
          {
              Items = usersDto,                   // Return DTOs, not entities
              TotalCount = totalCount,
              PageNumber = pageNumber,
              PageSize = pageSize
          };
      }
      catch (Exception ex)
      {
          // Handle any other unexpected exceptions
          Console.WriteLine($"An unexpected error occurred: {ex.Message}");
          throw new ApplicationException("An unexpected error occurred. Please try again later." + ex.Message);
      }
  }
```

#### 5.14 GET => /api/users => Add Searching and sorting

- Let's create a class which will take care of the query parameters

```csharp
// Create a class for Parameter template
public class QueryParameters
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SearchTerm { get; set; } = string.Empty;
    public string SortBy { get; set; } = "Name"; // Default sorting by name
    public string SortOrder { get; set; } = "asc"; // asc or desc
}
```

- Controller

```csharp
 // GET => /api/users => Get all the users
[HttpGet]
public async Task<IActionResult> GetUsers([FromQuery] QueryParameters queryParameters)
{
    try
    {
        var users = await _userService.GetUsersAsync(queryParameters);
        return ApiResponse.Success(users);
    }
    catch (ApplicationException ex)
    {
        return ApiResponse.ServerError(ex.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception : {ex.Message}");
        return ApiResponse.ServerError("An unexpected error occurred.");
    }
}

```

- Services

```csharp
 public async Task<PaginatedResult<UserDto>> GetUsersAsync(QueryParameters queryParameters)
  {
       public async Task<PaginatedResult<UserDto>> GetUsersAsync(QueryParameters queryParameters)
        {
            try
            {
                var query = _appDbContext.Users.AsQueryable();

                // Search based on name or email
                if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
                {
                    var lowerCaseSearchTerm = queryParameters.SearchTerm.ToLower();
                    query = query.Where(u => u.UserName.ToLower().Contains(lowerCaseSearchTerm) || u.Email.ToLower().Contains(lowerCaseSearchTerm));
                }

                // Sorting
                // if (typeof(Category).GetProperty(queryParameters.SortBy) != null)
                // {
                //     if (queryParameters.SortOrder.ToLower() == "desc")
                //     {
                //         query = query.OrderByDescending(c => EF.Property<object>(c, queryParameters.SortBy));
                //     }
                //     else
                //     {
                //         query = query.OrderBy(c => EF.Property<object>(c, queryParameters.SortBy));
                //     }
                // }

                 // Sorting
                switch (queryParameters.SortBy?.ToLower())
                {
                    case "UserName":
                        query = queryParameters.SortOrder.ToLower() == "desc"
                            ? query.OrderByDescending(u => u.UserName)
                            : query.OrderBy(u => u.UserName);
                        break;
                    case "CreatedAt":
                        query = queryParameters.SortOrder.ToLower() == "desc"
                            ? query.OrderByDescending(u => u.CreatedAt)
                            : query.OrderBy(u => u.CreatedAt);
                        break;

                    // Add other sortable fields if necessary
                    default:
                        query = query.OrderBy(u => u.UserName); // Default sorting
                        break;
                }

                // Calculate the total count of users (after filtering)
                var totalCount = await query.CountAsync();

                // Ensure pageNumber and pageSize are valid
                if (queryParameters.PageNumber < 1) queryParameters.PageNumber = 1;
                if (queryParameters.PageSize < 1) queryParameters.PageSize = 10;

                // Fetch paginated user data
                var users = await query
                    .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize) // Skip to the correct page
                    .Take(queryParameters.PageSize)                    // Take the correct page size
                    .ToListAsync();

                // Map the fetched users to UserDto
                var usersDto = _mapper.Map<List<UserDto>>(users);

                // Return paginated result
                return new PaginatedResult<UserDto>
                {
                    Items = usersDto,                   // Return DTOs, not entities
                    TotalCount = totalCount,
                    PageNumber = queryParameters.PageNumber,
                    PageSize = queryParameters.PageSize
                };
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred. Please try again later." + ex.Message);
            }
        }
      catch (Exception ex)
      {
          // Handle any other unexpected exceptions
          Console.WriteLine($"An unexpected error occurred: {ex.Message}");
          throw new ApplicationException("An unexpected error occurred. Please try again later." + ex.Message);
      }
  }
```

- Test this endpoint

```
GET http://localhost:5109/api/users?pageNumber=1&pageSize=2&searchTerm=anis
GET http://localhost:5109/api/users?SortBy=UserName&SortOrder=desc
GET http://localhost:5109/api/users?pageNumber=2&pageSize=5&searchTerm=anis&sortBy=UserName&sortOrder=asc


```

#### 5.15 GET => /api/users/{userId} => Get a single user

- Find single data from a DB table => context.TableName.FindAsync(identidier);
- Add a request handler method in controller

```csharp
 // GET => /api/users/{userId} => Get a single user by Id
  [HttpGet("{userId:guid}")]
  public async Task<IActionResult> GetUser(Guid userId)
  {
      try
      {
          var user = await _userService.GetUserByIdAsync(userId);
          if (user == null)
          {
              return ApiResponse.NotFound("User not found");
          }
          return ApiResponse.Success(user, "User Retrived successfully");
      }
      catch (ApplicationException ex)
      {
          return ApiResponse.ServerError(ex.Message);
      }
      catch (Exception ex)
      {
          Console.WriteLine($"Exception : {ex.Message}");
          return ApiResponse.ServerError("An unexpected error occurred.");
      }
  }
```

- Add a service handler method in services

```csharp
  public async Task<UserDto?> GetUserByIdAsync(Guid userId)
  {
      try
      {
          var user = await _appDbContext.Users.FindAsync(userId);

          if (user == null)
          {
              return null;
          }

          var singleUser = new UserDto
          {
              UserId = user.UserId,
              UserName = user.UserName,
              Email = user.Email,
              Address = user.Address,
              Image = user.Image,
              IsAdmin = user.IsAdmin,
              IsBanned = user.IsBanned,
              CreatedAt = user.CreatedAt // Return actual CreatedAt from DB
          };

          // Return null if user not found, otherwise return the user
          return singleUser;
      }
      catch (Exception ex)
      {
          // Handle any other unexpected exceptions
          Console.WriteLine($"An unexpected error occurred: {ex.Message}");
          throw new ApplicationException("An unexpected error occurred. Please try again later." + ex.Message);
      }
  }
```

#### 5.16 PUT => /api/users/{userId} => Update a single user by userId

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace review_ecommerce_api.Models
{
  public class UpdateUserDto
  {

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(100, ErrorMessage = "Name must be between 2 and 100 characters.", MinimumLength = 2)]
    public string? UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public string? Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, ErrorMessage = "Password must be between 6 and 100 characters.", MinimumLength = 6)]
    public string? Password { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "Address can't exceed 255 characters.")]
    public string? Address { get; set; } = string.Empty;

    public bool? IsAdmin { get; set; } = false;
    public bool? IsBanned { get; set; } = false;

    // [Url(ErrorMessage = "Invalid URL format for Image.")]
    public string? Image { get; set; } = string.Empty;
  }
}

// Controller
  // PUT => /api/users/{userId} => update a single user by Id
  [HttpPut("{userId:guid}")]
  public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDto userData)
  {
      if (!ModelState.IsValid)
      {
          return ApiResponse.BadRequest("Invalid User Data");
      }

      try
      {
          var updatedUser = await _userService.UpdateUserByIdAsync(userId, userData);
          if (updatedUser == null)
          {
              return ApiResponse.NotFound("User with ID {userId} not found");
          }
          return ApiResponse.Success(updatedUser, "User Updated successfully");
      }
      catch (ApplicationException ex)
      {
          return ApiResponse.ServerError(ex.Message);
      }
      catch (Exception ex)
      {
          Console.WriteLine($"Exception : {ex.Message}");
          return ApiResponse.ServerError("An unexpected error occurred.");
      }
  }


// Service
  public async Task<UserDto?> UpdateUserByIdAsync(Guid userId, UpdateUserDto userData)
    {
        try
        {
            var user = await _appDbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return null;
            }

            // without mapper
            // Update only the fields that are provided in userData
            user.UserName = !string.IsNullOrEmpty(userData.UserName) ? userData.UserName : user.UserName;
            user.Address = !string.IsNullOrEmpty(userData.Address) ? userData.Address : user.Address;
            user.Image = !string.IsNullOrEmpty(userData.Image) ? userData.Image : user.Image;
            user.IsAdmin = userData.IsAdmin;
            user.IsBanned = userData.IsBanned;


            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync();

            var updatedUser = new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Address = user.Address,
                Image = user.Image,
                IsAdmin = user.IsAdmin,
                IsBanned = user.IsBanned,
                CreatedAt = user.CreatedAt // Return actual CreatedAt from DB
            };

            // Return null if user not found, otherwise return the user
            return updatedUser;
        }
        catch (Exception ex)
        {
            // Handle any other unexpected exceptions
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            throw new ApplicationException("An unexpected error occurred. Please try again later." + ex.Message);
        }
    }
```

- test the update

```

PUT  http://localhost:5109/api/users/5673479e-adf9-4157-867c-efc72ef44a2e
Content-Type: application/json

{
  "userName": "ANISUL ISLAM",
  "email": "ANIS2024@example.com",
  "password": "123456",
  "address": "string",
  "image": "https://studywithanis.com"
}
```

#### 5.17 DELETE => /api/users/{userId} => Delete an user account

- Remove data to a table in DB => context.TableName.Remove(dataToBeDeleted); context.TableName.SaveChangesAsync();

```csharp
// controller
 // DELETE => /api/users/{userId} => delete a single user by Id
  [HttpDelete("{userId:guid}")]
  public async Task<IActionResult> DeleteUserAccount(Guid userId)
  {
      try
      {
          bool isUserDeleted = await _userService.DeleteUserByIdAsync(userId);
          if (!isUserDeleted)
          {
              return ApiResponse.NotFound("User not found");
          }
          return ApiResponse.Success("null", "User Deleted successfully");
      }
      catch (ApplicationException ex)
      {
          return ApiResponse.ServerError(ex.Message);
      }
      catch (Exception ex)
      {
          Console.WriteLine($"Exception : {ex.Message}");
          return ApiResponse.ServerError("An unexpected error occurred.");
      }
  }

// services
public async Task<bool> DeleteUserByIdAsync(Guid userId)
{
    try
    {
        var user = await _appDbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return false; // User not found
        }

        _appDbContext.Users.Remove(user);
        await _appDbContext.SaveChangesAsync();
        return true;
    }
    catch (Exception ex)
    {
        // Handle any other unexpected exceptions
        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        throw new ApplicationException("An unexpected error occurred. Please try again later." + ex.Message);
    }
}

```

#### 5.18 Concept => AutoMapper Library in .NET

- AutoMapper is a popular library in .NET that helps simplify the process of transferring data between objects, particularly between data models and data transfer objects (DTOs) in applications. can utilize the IMapper interface to map data models to DTOs and vice versa, simplifying data transformation and reducing boilerplate code.

- Without AutoMapper: You need to manually assign each property from the entity to the DTO. This can be error-prone and cumbersome, especially with complex objects.

Benfits of AutoMapper

1. Reduction of Boilerplate Code
   AutoMapper reduces the amount of manual mapping code you need to write. Without AutoMapper, you would typically write code to map properties from one object to another explicitly. This can be tedious and error-prone, especially with complex objects. AutoMapper automates this by mapping properties based on convention, which can significantly clean up your codebase.

2. Ease of Maintenance
   When your data model changes, maintaining manual mappings can be cumbersome. AutoMapper helps centralize the mapping logic, making it easier to manage and update. Changes in the data model require changes in the mapping configuration rather than throughout the code where data transformations occur.

3. Consistency
   AutoMapper encourages consistency in how mappings are handled across an application. By defining mappings in one place, you ensure that the same mapping logic is applied everywhere in the application, reducing the risk of inconsistencies in data handling and manipulation.

4. Customization and Flexibility
   Although AutoMapper works well with convention-based mapping, it also provides extensive options for customization. You can define custom conversion rules, handle complex type conversions, and conditionally map properties. This flexibility makes it suitable for a wide range of scenarios from simple to complex.

5. Improved Productivity
   Developers can focus more on the business logic rather than the details of converting between object types. AutoMapper handles the mundane task of copying data from one object to another, which can speed up development time and reduce bugs related to data transformation.

6. Integration with LINQ
   AutoMapper integrates well with LINQ, allowing for projections directly from database queries to DTOs. This can optimize performance by avoiding the need to retrieve all columns from the database or load entire entity graphs when only a subset is needed.

7. Support for Nested Objects
   AutoMapper can automatically handle nested objects and collections, which can be complex to map manually. It knows how to traverse these structures and map corresponding elements from source to destination.

- Install the AutoMapper `dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection`

#### 5.19 Use AutoMapper everywhere

- Step 1: Install the AutoMapper `dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection`

- Step 2: Setup AutoMapper `builder.Services.AddAutoMapper(typeof(Program));`

- Step 3: create Map / Mapping Profile: Create a class to define the mapping configurations.

```csharp
// Mappers / MappingProfile
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using review_ecommerce_api.EFCore;
using review_ecommerce_api.Models;

namespace review_ecommerce_api.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().RevereseMap();
            // Category Mapping
            // CreateMap<Category, CategoryDto>().ReverseMap();

            // Product Mapping
            //CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // With AutoMapper now configured, your service methods can utilize the IMapper interface to map data models to DTOs and vice versa, simplifying data transformation and reducing boilerplate code.
        }
    }
}
```

- Step 4: Refactor API to Use AutoMapper

```csharp
// Inside the service
private readonly IMapper _mapper;

public UserService(AppDbContext appDbContext, IMapper mapper)
{
    _appDbContext = appDbContext;
    _mapper = mapper;
}
```

- step 5: ready to use AutoMapper in services

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using review_ecommerce_api.EFCore;
using review_ecommerce_api.Models;

namespace review_ecommerce_api.Services
{
    public class UserService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public UserService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<UserDto>> GetUsersAsync(QueryParameters queryParameters)
        {
            try
            {
                var query = _appDbContext.Users.AsQueryable();

                // Search based on name or email
                if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
                {
                    var lowerCaseSearchTerm = queryParameters.SearchTerm.ToLower();
                    query = query.Where(u => u.UserName.ToLower().Contains(lowerCaseSearchTerm) || u.Email.ToLower().Contains(lowerCaseSearchTerm));
                }

                // Sorting
                if (!string.IsNullOrEmpty(queryParameters.SortBy))
                {
                    if (queryParameters.SortOrder.ToLower() == "desc")
                    {
                        query = query.OrderByDescending(u => EF.Property<object>(u, queryParameters.SortBy));
                    }
                    else
                    {
                        query = query.OrderBy(u => EF.Property<object>(u, queryParameters.SortBy));
                    }
                }

                // Calculate the total count of users (after filtering)
                var totalCount = await query.CountAsync();

                // Ensure pageNumber and pageSize are valid
                if (queryParameters.PageNumber < 1) queryParameters.PageNumber = 1;
                if (queryParameters.PageSize < 1) queryParameters.PageSize = 10;

                // Fetch paginated user data
                var users = await query
                    .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize) // Skip to the correct page
                    .Take(queryParameters.PageSize)                    // Take the correct page size
                    .ToListAsync();

                // Map the fetched users to UserDto
                var usersDto = _mapper.Map<List<UserDto>>(users);

                // Return paginated result
                return new PaginatedResult<UserDto>
                {
                    Items = usersDto,                   // Return DTOs, not entities
                    TotalCount = totalCount,
                    PageNumber = queryParameters.PageNumber,
                    PageSize = queryParameters.PageSize
                };
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred. Please try again later." + ex.Message);
            }
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto newUserDto)
        {

            try
            {
                // Hash the password using a library like BCrypt.Net
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUserDto.Password);

                // convert the CreateUserDto to User
                var user = _mapper.Map<User>(newUserDto);
                await _appDbContext.Users.AddAsync(user);
                await _appDbContext.SaveChangesAsync();

                var userResponse = _mapper.Map<UserDto>(user);
                return userResponse;
            }
            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException postgresException)
            {
                if (postgresException.SqlState == "23505") // PostgreSQL unique constraint violation
                {
                    throw new ApplicationException("Duplicate email. Please use a unique email address.");
                }
                else
                {
                    // Handle other database-related errors
                    throw new ApplicationException("An error occurred while adding the user.");
                }
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred. Please try again later.");
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            try
            {
                var user = await _appDbContext.Users.FindAsync(userId);

                if (user == null)
                {
                    return null;
                }

                var userDto = _mapper.Map<UserDto>(user); // Convert User Entity object to UserDto

                // Return null if user not found, otherwise return the user
                return userDto;
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred. Please try again later." + ex.Message);
            }
        }

        public async Task<bool> DeleteUserByIdAsync(Guid userId)
        {
            try
            {
                var user = await _appDbContext.Users.FindAsync(userId);
                if (user == null)
                {
                    return false; // User not found
                }

                _appDbContext.Users.Remove(user);
                await _appDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred. Please try again later." + ex.Message);
            }
        }
        public async Task<UserDto?> UpdateUserByIdAsync(Guid userId, UpdateUserDto userData)
        {
            try
            {
                var user = await _appDbContext.Users.FindAsync(userId);
                if (user == null)
                {
                    return null;
                }

                if(userData.Password != null){
                    // Hash the password using a library like BCrypt.Net
                    user.Password = BCrypt.Net.BCrypt.HashPassword(userData.Password);
                }

                // with mapper convert the UpdateUserDto to User Entity
                _mapper.Map<UserDto>(user);
                _appDbContext.Users.Update(user);
                await _appDbContext.SaveChangesAsync();
                var updatedUser = _mapper.Map<UserDto>(userData);
                return updatedUser;
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred. Please try again later." + ex.Message);
            }
        }

    }
}
```

#### 5.20 POST => /api/users/login => User login (working)

#### 5.21 => How to generate JWT (working)

#### 5.22 POST => /api/users/logout => User logout (working)

#### 5.23 Authorization Option in Swagger (working)

#### 5.24 PUT => /api/users/ban-unban/{userId} => Ban/Unban the user by the userId (working)

#### 5.25 GET => /api/users/banned-users => List of ban users (working)

#### 5.26 POST => /api/users/reset-password => Password Reset (working)

#### 5.27 Middleware (working)

#### 5.28 Environment Variable Setup (working)

#### 5.29 CORS Setup (working)

### 6. Profile API (Not Ready)

#### 6.1 Define Entity

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace review_ecommerce_api.EFCore
{
    public class UserProfile
    {
         public Guid ProfileId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        // public string Occupation { get; set; } = string.Empty;
        // public string PhoneNumber { get; set; } = string.Empty;
        // public string ProfilePictureUrl { get; set; } = string.Empty;
        // public string Bio { get; set; } = string.Empty;
        // public string Gender { get; set; } = string.Empty;
        // public DateTime DateOfBirth { get; set; }
        // public string TwitterHandle { get; set; } = string.Empty;
        // public string LinkedInProfile { get; set; } = string.Empty;
        // public string WebsiteUrl { get; set; } = string.Empty;
        // public string Interests { get; set; } = string.Empty;
        // public string Location { get; set; } = string.Empty;

        public User User { get; set; } // 1-to-1 relationship with User


    }
}

// Update the User Entity

public Guid ProfileId { get; set; }
public UserProfile UserProfile{ get; set; } // 1-1 with UserProfile
```

- update the context

```csharp

// DbSet properties

public DbSet<Profile> Profiles { get; set; }

modelBuilder.Entity<UserProfile>(entity =>
{
    entity.HasKey(up => up.ProfileId); // Primary Key configuration
    entity.Property(up => up.ProfileId).HasDefaultValueSql("uuid_generate_v4()"); // Generate UUID for new records
});


// One-to-One: User <-> Profile
modelBuilder.Entity<User>()
      .HasOne(u => u.UserProfile)
      .WithOne(up => up.User)
      .HasForeignKey<User>(u => u.ProfileId);
```

#### 6.2 Create CreateUserProfileDto, UserProfileDto and Update Dtos

Updating your DTOs ensures that both the User and the associated Profile are handled together during create and update operations, following the 1-to-1 relationship between them. This way, when a user is created or updated, their profile is also processed accordingly.

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace review_ecommerce_api.Models.profiles
{
    public class CreateUserProfileDto
    {

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; } = string.Empty;

        // [Phone(ErrorMessage = "Invalid phone number.")]
        // public string PhoneNumber { get; set; } = string.Empty;

        // [Url(ErrorMessage = "Invalid URL format for Profile Picture.")]
        // public string ProfilePictureUrl { get; set; } = string.Empty;

        // [StringLength(500, ErrorMessage = "Bio can't exceed 500 characters.")]
        // public string Bio { get; set; } = string.Empty;

        // [Required(ErrorMessage = "Gender is required.")]
        // public string Gender { get; set; } = string.Empty;

        // [Required(ErrorMessage = "Date of Birth is required.")]
        // public DateTime DateOfBirth { get; set; }

        // [Url(ErrorMessage = "Invalid URL format for Twitter Handle.")]
        // public string TwitterHandle { get; set; } = string.Empty;

        // [Url(ErrorMessage = "Invalid URL format for LinkedIn Profile.")]
        // public string LinkedInProfile { get; set; } = string.Empty;

        // public string Occupation { get; set; } = string.Empty;
        // public string WebsiteUrl { get; set; } = string.Empty;
        // public string Interests { get; set; } = string.Empty;
        // public string Location { get; set; } = string.Empty;

    }
}

// Update the CreateUser Dto
public class CreateUserDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "Password must be between 6 and 100 characters.", MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Address can't exceed 255 characters.")]
        public string Address { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        // Profile information
        [Required]
        public CreateUserProfileDto UserProfile { get; set; } = new CreateUserProfileDto();
    }


// Create the UserProfileDto
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace review_ecommerce_api.Models.profiles
{
    public class UserProfileDto
    {
        public Guid ProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // public string PhoneNumber { get; set; }
        // public string ProfilePictureUrl { get; set; }
        // public string Bio { get; set; }
        // public string Gender { get; set; }
        // public DateTime DateOfBirth { get; set; }
        // public string TwitterHandle { get; set; }
        // public string LinkedInProfile { get; set; }
        // public string Occupation { get; set; }
        // public string WebsiteUrl { get; set; }
        // public string Interests { get; set; }
        // public string Location { get; set; }
    }
}

// Update the UserDto
public class UserDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Image { get; set; }

    public UserProfileDto UserProfile { get; set; } = new UserProfileDto();
}


```

#### 6.3 Update User Controllers and Services

- update the atomapper

```csharp


  // User mappings
  CreateMap<User, UserDto>()
      .ForMember(dest => dest.UserProfile, opt => opt.MapFrom(src => src.UserProfile));  // Map UserProfile as part of User
  CreateMap<CreateUserDto, User>()
      .ForMember(dest => dest.UserProfile, opt => opt.MapFrom(src => src.UserProfile));  // Map Profile as part of User
  CreateMap<UpdateUserDto, User>()
      .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
  // .ForMember(dest => dest.UserProfile, opt => opt.MapFrom(src => src.UserProfile));  // Map Profile as part of User

  // Profile mappings
            CreateMap<UserProfile, UserProfileDto>().ReverseMap();
            CreateMap<CreateUserProfileDto, UserProfile>();
```

#### 6.4 Update User Controllers and Services

```csharp

// no need to change in Controllers
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Controllers;
using Microsoft.AspNetCore.Mvc;
using review_ecommerce_api.Models;
using review_ecommerce_api.Services;

namespace review_ecommerce_api.Controllers
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
                return ApiResponse.BadRequest("Invalid User Data");
            }

            try
            {
                var user = await _userService.CreateUserAsync(newUser);
                return ApiResponse.Created(user, "User created successfully");
            }
            catch (ApplicationException ex)
            {
                return ApiResponse.Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : {ex.Message}");
                return ApiResponse.ServerError(ex.Message);
            }
        }

        // GET => /api/users => Get all the users
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] QueryParameters queryParameters)
        {
            try
            {
                var users = await _userService.GetUsersAsync(queryParameters);
                return ApiResponse.Success(users);
            }
            catch (ApplicationException ex)
            {
                return ApiResponse.ServerError(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : {ex.Message}");
                return ApiResponse.ServerError("An unexpected error occurred.");
            }
        }

        // GET => /api/users/{userId} => Get a single user by Id
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse.NotFound("User not found");
                }
                return ApiResponse.Success(user, "User Retrived successfully");
            }
            catch (ApplicationException ex)
            {
                return ApiResponse.ServerError(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : {ex.Message}");
                return ApiResponse.ServerError("An unexpected error occurred.");
            }
        }

        // DELETE => /api/users/{userId} => delete a single user by Id
        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> DeleteUserAccount(Guid userId)
        {
            try
            {
                bool isUserDeleted = await _userService.DeleteUserByIdAsync(userId);
                if (!isUserDeleted)
                {
                    return ApiResponse.NotFound("User not found");
                }
                return ApiResponse.Success("null", "User Deleted successfully");
            }
            catch (ApplicationException ex)
            {
                return ApiResponse.ServerError(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : {ex.Message}");
                return ApiResponse.ServerError("An unexpected error occurred.");
            }
        }

        // PUT => /api/users/{userId} => update a single user by Id
        [HttpPut("{userId:guid}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDto userData)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse.BadRequest("Invalid User Data");
            }

            try
            {
                var updatedUser = await _userService.UpdateUserByIdAsync(userId, userData);
                if (updatedUser == null)
                {
                    return ApiResponse.NotFound("User with ID {userId} not found");
                }
                return ApiResponse.Success(updatedUser, "User Updated successfully");
            }
            catch (ApplicationException ex)
            {
                return ApiResponse.ServerError(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : {ex.Message}");
                return ApiResponse.ServerError("An unexpected error occurred.");
            }
        }

    }
}


// Services
```

- test this endpoint

```json
{
  "userName": "johndoe",
  "email": "johndoe@example.com",
  "password": "StrongPassword123!",
  "address": "123 Main St, Springfield",
  "image": "https://example.com/johndoe.jpg",
  "profile": {
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "+1234567890",
    "profilePictureUrl": "https://example.com/profile/johndoe.jpg",
    "bio": "A passionate software developer and tech enthusiast.",
    "gender": "Male",
    "dateOfBirth": "1990-04-15",
    "twitterHandle": "@johndoe",
    "linkedInProfile": "https://linkedin.com/in/johndoe",
    "occupation": "Software Engineer",
    "websiteUrl": "https://johndoe.dev",
    "interests": "Programming, Blogging, Traveling",
    "location": "New York, USA"
  }
}
```

#### Profile CRUD operations (Working)

### 7 Category and Product API

#### 7.0 add this to the Program.cs

```csharp
builder.Services.AddControllers()
 .AddJsonOptions(options =>
 {
     options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
     options.JsonSerializerOptions.WriteIndented = true;
 });
```

#### 7.1 Create the Category and Product Entity

- Create the category and product Entity

```csharp
// Inside EFCore folder Category Entity
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace review_ecommerce_api.EFCore
{
    public class Category
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore] // Prevent serialization of Category to avoid circular reference
        public List<Product> Products { get; set; }  // Many-to-many with Product
    }
}

// Inside the EFCore Product Entity
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace review_ecommerce_api.EFCore
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 0;
        public int Sold { get; set; } = 0;
        public decimal Shipping { get; set; } = 0;

        public Guid CategoryId { get; set; }  // Foreign Key for the Category

        [JsonIgnore] // Prevent serialization of Category to avoid circular reference
        public Category Category { get; set; } // Navigation property

        // public int OrderId { get; set; }  // Foreign key
        // public Order Order { get; set; } = new Order();  // Navigation property
        // public List<Category> Categories { get; set; } = new List<Category>();  // Many-to-many with Category

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
```

#### 7.2 Adjust the dbcontext

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace review_ecommerce_api.EFCore
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } // DbSet will convert LINQ TO SQL Queries
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId); // Primary Key configuration
                entity.Property(u => u.UserId).HasDefaultValueSql("uuid_generate_v4()"); // Generate UUID for new records
                entity.Property(u => u.UserName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Password).IsRequired();
                entity.Property(u => u.Address).HasMaxLength(255);
                entity.Property(u => u.IsAdmin).HasDefaultValue(false);
                entity.Property(u => u.IsBanned).HasDefaultValue(false);
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Category>(entity =>
           {
               entity.HasKey(c => c.CategoryId); // Primary Key
               entity.Property(c => c.CategoryId).HasDefaultValueSql("uuid_generate_v4()");
               entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
               entity.HasIndex(c => c.Name).IsUnique();
               entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
           });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductId); // Primary Key
                entity.Property(p => p.ProductId).HasDefaultValueSql("uuid_generate_v4()");
                entity.Property(p => p.Name).IsRequired().HasMaxLength(255);
                entity.HasIndex(p => p.Name).IsUnique();
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // 1-Many Relationship
            modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);// optional
        }
    }
}
```

- Add and run the migration script

#### Interface for the Service / Registry Pattern

- Step 1: Create the interface
- Step 2: Use the interface in Service
- Step 3: Add the interface as a dependency injection inside the controller
- Step 4: Add the builder.Services.AddScoped<interface,Service>() in the startup file

#### 7.3 POST => /api/categories => Create the Category

- create the CreateCategoryDto and CategoryDto

```csharp
// Inside the DTOs/categories/CreateDto
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace review_ecommerce_api.EFCore
{
  public class CreateCategory
  {
    [Required]
    [StringLength(100, ErrorMessage = "Category Name must be between 2 and 200 characters.", MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

  }
}

// Inside the EFCore/Dtos/categories/CategoryDto.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace review_ecommerce_api.EFCore
{
  public class CategoryDto
  {
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }

  }
}
```

- Adjust the Mapper

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using review_ecommerce_api.EFCore;
using review_ecommerce_api.Models;

namespace review_ecommerce_api.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>();
            // Add this line to map UpdateUserDto to User
            CreateMap<UpdateUserDto, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            // CreateMap<CreateCategory, Category>().ReverseMap(); // equivalent to the following 2 lines
            CreateMap<CreateCategory, Category>();
            CreateMap<Category, CategoryDto>();

            // With AutoMapper now configured, your service methods can utilize the IMapper interface to map data models to DTOs and vice versa, simplifying data transformation and reducing boilerplate code.
        }
    }
}
```

- Create the helper method for generating the slug

```csharp
// helper for generating slug
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace review_ecommerce_api.utilities
{
    public class Helper
    {
        // Utility function to generate slug from a name
        public string GenerateSlug(string name)
        {
            // Implement a simple slug generator (convert to lowercase, replace spaces with dashes, remove invalid characters, etc.)
            return name.ToLower().Replace(" ", "-").Replace("--", "-");
        }
    }
}
```

- Create the Interfaces and Services

```csharp
public interface ICategoryService
{
    Task<PaginatedResult<CategoryDto>> GetCategoriesAsync(QueryParameters queryParameters);
    Task<CategoryDto> CreateCategoryAsync(CreateCategory newCategory);
    Task<CategoryDto?> GetCategoryBySlug(string slugOrId);
    Task<bool> DeleteCategoryBySlugOrId(string slugOrId);
    // Uncomment the method if you plan to implement category updates in the future
    Task<CategoryDto?> UpdateCategorySlugOrIdAsync(string slugOrId, UpdateCategoryDto updateCategoryData);
}

 public class CategoryService : ICategoryService

{
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public CategoryService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

  public async Task<CategoryDto> CreateCategoryAsync(CreateCategory newCategory)
  {

      try
      {
          var category = _mapper.Map<Category>(newCategory);
          category.Slug = Helper.GenerateSlug(newCategory.Name);
          await _appDbContext.Categories.AddAsync(category);
          await _appDbContext.SaveChangesAsync();
          return _mapper.Map<CategoryDto>(category);
      }
      catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException postgresException)
      {
          if (postgresException.SqlState == "23505") // PostgreSQL unique constraint violation
          {
              throw new ApplicationException("Duplicate Category Name. Please use a unique category name.");
          }
          else
          {
              // Handle other database-related errors
              throw new ApplicationException("An error occurred while adding the user.");
          }
      }
      catch (Exception ex)
      {
          // Handle any other unexpected exceptions
          Console.WriteLine($"An unexpected error occurred: {ex.Message}");
          throw new ApplicationException("An unexpected error occurred. Please try again later.");
      }
  }
}

// Inside the controller make sure to use ICategoryService
// POST => /api/categories => Create a category
[HttpPost]
public async Task<IActionResult> CreateCategory([FromBody] CreateCategory newCategory)
{
    if (!ModelState.IsValid)
    {
        return ApiResponse.BadRequest("Invalid Category Data");
    }

    try
    {
        var category = await _categoryService.CreateCategoryAsync(newCategory);
        return ApiResponse.Created(category, "Category created successfully");
    }
    catch (ApplicationException ex)
    {
        return ApiResponse.Conflict(ex.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception : {ex.Message}");
        return ApiResponse.ServerError(ex.Message);
    }
}
```

- register the service to Program.cs

```csharp
builder.Services.AddScoped<ICategoryService,CategoryService>();
```

- here is some example JSON data for creating categories:

Example 1: Electronics Category

```json
{
  "name": "Electronics",
  "description": "Devices and gadgets including phones, laptops, and other electronic equipment"
}
```

Example 2: Clothing Category

```json
{
  "name": "Clothing",
  "description": "Apparel including shirts, pants, dresses, and more"
}
```

Example 3: Books Category

```json
{
  "name": "Books",
  "description": "Various genres of books including fiction, non-fiction, educational, and more"
}
```

Example 4: Home & Kitchen Category

```json
{
  "name": "Home & Kitchen",
  "description": "Items for home improvement, kitchen tools, and appliances"
}
```

Example 5: Sports & Outdoors Category

```json
{
  "name": "Sports & Outdoors",
  "slug": "sports-outdoors",
  "description": "Equipment and gear for sports and outdoor activities"
}
```

#### 7.4 GET => /api/categories => Get all the categories

```csharp

// controller
  public async Task<IActionResult> GetCategories([FromQuery] QueryParameters queryParameters)
  {
      try
      {
          var categories = await _categoryService.GetCategoriesAsync(queryParameters);
          return ApiResponse.Success(categories);
      }
      catch (ApplicationException ex)
      {
          return ApiResponse.ServerError(ex.Message);
      }
      catch (Exception ex)
      {
          Console.WriteLine($"Exception : {ex.Message}");
          return ApiResponse.ServerError("An unexpected error occurred.");
      }
  }

// services

  public async Task<PaginatedResult<CategoryDto>> GetCategoriesAsync(QueryParameters queryParameters)
  {
      try
      {
          var query = _appDbContext.Categories.AsQueryable();

          // Search based on name or email
          if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
          {
              var lowerCaseSearchTerm = queryParameters.SearchTerm.ToLower();
              query = query.Where(u => u.Name.ToLower().Contains(lowerCaseSearchTerm));
          }

          // Sorting (working)
          switch (queryParameters.SortBy?.ToLower())
          {
              case "Name":
                  query = queryParameters.SortOrder.ToLower() == "desc"
                      ? query.OrderByDescending(c => c.Name)
                      : query.OrderBy(c => c.Name);
                  break;
              case "CreatedAt":
                  query = queryParameters.SortOrder.ToLower() == "desc"
                      ? query.OrderByDescending(c => c.CreatedAt)
                      : query.OrderBy(c => c.CreatedAt);
                  break;

              // Add other sortable fields if necessary
              default:
                  query = query.OrderBy(c => c.Name); // Default sorting
                  break;
          }


          // Calculate the total count of users (after filtering)
          var totalCount = await query.CountAsync();

          // Ensure pageNumber and pageSize are valid
          if (queryParameters.PageNumber < 1) queryParameters.PageNumber = 1;
          if (queryParameters.PageSize < 1) queryParameters.PageSize = 10;

          // Fetch paginated categories data
          var categories = await query
              .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize) // Skip to the correct page
              .Take(queryParameters.PageSize)                    // Take the correct page size
              .ToListAsync();

          // Map the fetched users to UserDto
          var categoriesDto = _mapper.Map<List<CategoryDto>>(categories);

          Console.WriteLine("Received categories: " + JsonConvert.SerializeObject(categoriesDto));


          // Return paginated result
          return new PaginatedResult<CategoryDto>
          {
              Items = categoriesDto,                   // Return DTOs, not entities
              TotalCount = totalCount,
              PageNumber = queryParameters.PageNumber,
              PageSize = queryParameters.PageSize
          };
      }
      catch (Exception ex)
      {
          // Handle any other unexpected exceptions
          Console.WriteLine($"An unexpected error occurred: {ex.Message}");
          throw new ApplicationException("An unexpected error occurred. Please try again later." + ex.Message);
      }
  }


```

- test this endpoint

```csharp
https://your-api-url.com/api/categories?sortBy=CategoryName&sortOrder=asc
```

#### 7.5 GET => /api/categories/{slugOrId} => Get a single category by slug or id

```csharp
// controller
// GET => /api/categories/{slugOrId} => Get a single category by slug or id
[HttpGet("{slugOrId}")]
public async Task<IActionResult> GetCategory(string slugOrId)
{
    try
    {
        var category = await _categoryService.GetCategoryBySlugOrId(slugOrId);
        if (category == null)
        {
            return ApiResponse.NotFound("Category not found");
        }
        return ApiResponse.Success(category, "Category retrieved successfully");
    }
    catch (ApplicationException ex)
    {
        return ApiResponse.ServerError(ex.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {ex.Message}");
        return ApiResponse.ServerError("An unexpected error occurred.");
    }
}

// services
 public async Task<CategoryDto?> GetCategoryBySlug(string slugOrId)
  {
      try
      {
          Category? category;

          // Check if the slugOrId is a valid Guid, meaning it's an ID.
          if (Guid.TryParse(slugOrId, out var categoryId))
          {
              category = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryId);
          }
          else
          {
              // Otherwise, assume it's a slug and search by slug
              category = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.Slug == slugOrId);
          }

          if (category == null)
          {
              return null;
          }

          var categoryData = _mapper.Map<CategoryDto>(category);
          return categoryData;
      }
      catch (Exception ex)
      {
          // Handle any other unexpected exceptions
          Console.WriteLine($"An unexpected error occurred: {ex.Message}");
          throw new ApplicationException("An unexpected error occurred. Please try again later." + ex.Message);
      }
  }

```

- test this endpoint: `GET http://localhost:5109/api/categories/books`

#### 7.6 Delete => /api/categories/{slug} => delete category by slug or id

```csharp
// Controller
 // DELETE => /api/categories/{slugOrId} => delete a single user by Id or slug
[HttpDelete("{slugOrId}")]
public async Task<IActionResult> DeleteCategory(string slugOrId)
{
    try
    {
        bool isCategoryDeleted = await _categoryService.DeleteCategoryBySlugOrId(slugOrId);
        if (!isCategoryDeleted)
        {
            return ApiResponse.NotFound("Category not found");
        }
        return ApiResponse.Success("null", "Category Deleted successfully");
    }
    catch (ApplicationException ex)
    {
        return ApiResponse.ServerError(ex.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception : {ex.Message}");
        return ApiResponse.ServerError("An unexpected error occurred.");
    }
}

// Service
 public async Task<bool> DeleteCategoryBySlugOrId(string slugOrId)
  {
      try
      {
          Category? category;

          // Check if the slugOrId is a valid Guid, meaning it's an ID.
          if (Guid.TryParse(slugOrId, out var categoryId))
          {
              category = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryId);
          }
          else
          {
              // Otherwise, assume it's a slug and search by slug
              category = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.Slug == slugOrId);
          }

          if (category == null)
          {
              return false; // Return false if the category was not found
          }

          // Remove the category
          _appDbContext.Categories.Remove(category);
          await _appDbContext.SaveChangesAsync();

          return true; // Return true to indicate the category was deleted
      }
      catch (Exception ex)
      {
          // Handle any unexpected exceptions
          Console.WriteLine($"An unexpected error occurred: {ex.Message}");
          throw new ApplicationException("An unexpected error occurred. Please try again later." + ex.Message);
      }
  }
```

#### 7.7 PUT => /api/categories/{slug} => update category by slug or id

- create an UpdatecategoryDto

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace review_ecommerce_api.Models.categories
{
    public class UpdateCategoryDto
    {
        public string? Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;

    }
}
```

- Add the Controller

```csharp
  // Update => /api/categories/{slugOrId} => update a single user by Id or slug
  [HttpPut("{slugOrId}")]
  public async Task<IActionResult> UpdateCategory(string slugOrId, [FromBody] UpdateCategoryDto updateCategoryData)
  {
      if (!ModelState.IsValid)
      {
          return ApiResponse.BadRequest("Invalid Category Data");
      }

      try
      {
          var updatedCategory = await _categoryService.UpdateCategorySlugOrIdAsync(slugOrId, updateCategoryData);
          if (updatedCategory == null)
          {
              return ApiResponse.NotFound("Category with ID {userId} not found");
          }
          return ApiResponse.Success(updatedCategory, "Category Updated successfully");
      }
      catch (ApplicationException ex)
      {
          return ApiResponse.ServerError(ex.Message);
      }
      catch (Exception ex)
      {
          Console.WriteLine($"Exception : {ex.Message}");
          return ApiResponse.ServerError("An unexpected error occurred.");
      }
  }
```

- Modify the mapper

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using review_ecommerce_api.EFCore;
using review_ecommerce_api.Migrations;
using review_ecommerce_api.Models;
using review_ecommerce_api.Models.categories;

namespace review_ecommerce_api.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>();
            // Add this line to map UpdateUserDto to User
            CreateMap<UpdateUserDto, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<CreateCategory, Category>();
            CreateMap<Category, CategoryDto>();
            CreateMap<UpdateCategoryDto, Category>()
           .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // With AutoMapper now configured, your service methods can utilize the IMapper interface to map data models to DTOs and vice versa, simplifying data transformation and reducing boilerplate code.
        }
    }
}
```

- Add the service

```csharp
    public async Task<CategoryDto?> UpdateCategorySlugOrIdAsync(string slugOrId, UpdateCategoryDto updateCategoryData)
  {
      try
      {
          // Try to find the category by ID or slug
          Category? category;

          // Check if the slugOrId is a valid Guid, meaning it's an ID.
          if (Guid.TryParse(slugOrId, out var categoryId))
          {
              category = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryId);
          }
          else
          {
              // Otherwise, assume it's a slug and search by slug
              category = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.Slug == slugOrId);
          }

          if (category == null)
          {
              return null; // Return null if the category was not found
          }

          // Update the category fields
          if (!string.IsNullOrEmpty(updateCategoryData.Name) && updateCategoryData.Name != category.Name)
          {
              // Update the category name
              category.Name = updateCategoryData.Name;

              // Automatically generate a new slug based on the updated name
              category.Slug = Helper.GenerateSlug(updateCategoryData.Name);
          }

          // Use the mapper to update any other fields from UpdateCategoryDto to the existing category
          _mapper.Map(updateCategoryData, category);

          // Save updated category to the database
          _appDbContext.Categories.Update(category);
          await _appDbContext.SaveChangesAsync();

          // Return the updated category as a CategoryDto
          var updatedCategoryDto = _mapper.Map<CategoryDto>(category);
          return updatedCategoryDto;
      }
      catch (Exception ex)
      {
          // Handle any other unexpected exceptions
          Console.WriteLine($"An unexpected error occurred: {ex.Message}");
          throw new ApplicationException("An unexpected error occurred. Please try again later." + ex.Message);
      }
  }
```

#### 7.8 POST => /api/products => Create the Product

- make sure the Product entity and dbcontext is setup
- create the CreateProductDto and ProductDto

```csharp
// Models/products/CreateProductDto
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace review_ecommerce_api.Models.products
{
    public class CreateProductDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Product Name must be between 2 and 100 characters.", MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        // [Url(ErrorMessage = "Image must be a valid URL.")]
        public string Image { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description can't be longer than 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10000.00.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative number.")]
        public int Quantity { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "Sold must be a non-negative number.")]
        public int Sold { get; set; } = 0;

        [Range(0, 1000.00, ErrorMessage = "Shipping must be between 0 and 1000.00.")]
        public decimal Shipping { get; set; } = 0;

        // public int OrderId { get; set; }  // Foreign key

        // public OrderModel Order { get; set; } = new OrderModel();  // Navigation property

        [Required]
        [MinLength(1, ErrorMessage = "CategoryIds must contain at least one category.")]
        public Guid CategoryId { get; set; } // 1-to-many with CategoryMany-to-many with Category
    }
}

// Models/products/ProductDto
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using review_ecommerce_api.EFCore;

namespace review_ecommerce_api.Models.products
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Sold { get; set; }
        public decimal Shipping { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

```

- modify the mapper

```csharp

CreateMap<CreateProductDto, Product>();
CreateMap<Product, ProductDto>();
```

- Controller

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Controllers;
using Microsoft.AspNetCore.Mvc;
using review_ecommerce_api.Models;
using review_ecommerce_api.Models.products;
using review_ecommerce_api.Services;

namespace review_ecommerce_api.Controllers
{
    [ApiController, Route("/api/products")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        // POST => /api/products => Create a product
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto newProduct)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                foreach (var error in errors)
                {
                    Console.WriteLine($"Error : {error}");
                }

                // return BadRequest(new { Message = "Invalid data", Errors = errors });
                return ApiResponse.BadRequest("Invalid Product Data");
            }

            try
            {
                var product = await _productService.CreateProductAsync(newProduct);
                return ApiResponse.Created(product, "product created successfully");
            }
            catch (ApplicationException ex)
            {
                return ApiResponse.Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : {ex.Message}");
                return ApiResponse.ServerError(ex.Message);
            }
        }

    }
}

```

- Create and add the Service to Program.cs

```csharp
 public async Task<ProductDto> CreateProductAsync(CreateProductDto newProduct)
  {

      try
      {
          // Check if the CategoryId provided exists in the database
          var category = await _appDbContext.Categories.FindAsync(newProduct.CategoryId);

          if (category == null)
          {
              throw new ApplicationException("The specified category does not exist.");
          }

          // Map the DTO to the Product entity
          var product = _mapper.Map<Product>(newProduct);
          product.Slug = Helper.GenerateSlug(newProduct.Name);

          // Add the new product to the database
          await _appDbContext.Products.AddAsync(product);
          await _appDbContext.SaveChangesAsync();


          // Map the created product entity back to ProductDto and return
          return _mapper.Map<ProductDto>(product);
      }
      catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException postgresException)
      {
          if (postgresException.SqlState == "23505") // PostgreSQL unique constraint violation
          {
              throw new ApplicationException("Duplicate Product Name. Please use a unique product name.");
          }
          else
          {
              // Handle other database-related errors
              throw new ApplicationException("An error occurred while adding the user.");
          }
      }
      catch (Exception ex)
      {
          // Handle any other unexpected exceptions
          Console.WriteLine($"An unexpected error occurred: {ex.Message}");
          throw new ApplicationException("An unexpected error occurred. Please try again later.");
      }
  }
```

- test the endpoint with the follwing data

  ```json

  {
    "name": "Fjallraven - Foldsack No. 1 Backpack, Fits 15 Laptops",
    "image": "https://fakestoreapi.com/img/81fPKd-2AYL._AC_SL1500_.jpg",
    "description": "Your perfect pack for everyday use and walks in the forest. Stash your laptop (up to 15 inches) in the padded sleeve, your everyday",
    "price": 100.95,
    "quantity": 10,
    "shipping": 2.5,
   "categoryId": "3439780b-5683-472c-9142-faa86a75f3c7"
  }

  {
    "name": "Mens Casual Premium Slim Fit T-Shirts ",
    "image": "https://fakestoreapi.com/img/81fPKd-2AYL._AC_SL1500_.jpg",
    "description": "Slim-fitting style, contrast raglan long sleeve, three-button henley placket, light weight & soft fabric for breathable and comfortable wearing. And Solid stitched shirts with round neck made for durability and a great fit for casual fashion wear and diehard baseball fans. The Henley style round neckline includes a three-button placket.",
    "price": 22.3,
    "quantity": 12,
    "shipping": 0,
    "categoryId": "3439780b-5683-472c-9142-faa86a75f3c7"
  }


  {

  "name": "Mens Cotton Jacket",
  "price": 55.99,
  "description": "great outerwear jackets for Spring/Autumn/Winter, suitable for many occasions, such as working, hiking, camping, mountain/rock climbing, cycling, traveling or other outdoors. Good gift choice for you or your family member. A warm hearted love to Father, husband or son in this thanksgiving or Christmas Day.",
  "image": "https://fakestoreapi.com/img/71li-ujtlUL._AC_UX679_.jpg",
  "quantity": 12,
    "shipping": 0,
    "categoryId": "3439780b-5683-472c-9142-faa86a75f3c7"
  }

  {
  "name": "Mens Casual Slim Fit",
  "price": 15.99,
  "description": "The color could be slightly different between on the screen and in practice. / Please note that body builds vary by person, therefore, detailed size information should be reviewed below on the product description.",
  "image": "https://fakestoreapi.com/img/71YXzeOuslL._AC_UY879_.jpg",
  "quantity": 12,
  "shipping": 0,
  "categoryId": "d8309b09-2f25-4a2a-b3cd-951876cd422d"
  }



  {
      "name": "WD 2TB Elements Portable External Hard Drive - USB 3.0 ",
      "price": 64,
      "description": "USB 3.0 and USB 2.0 Compatibility Fast data transfers Improve PC Performance High Capacity; Compatibility Formatted NTFS for Windows 10, Windows 8.1, Windows 7; Reformatting may be required for other operating systems; Compatibility may vary depending on user’s hardware configuration and operating system",
      "image": "https://fakestoreapi.com/img/61IBBVJvSDL._AC_SY879_.jpg",
      "quantity": 12,
      "shipping": 0,
      "categoryId": "39655df3-9fe8-4451-971d-8af25979b104"
  }
  {

      "name": "SanDisk SSD PLUS 1TB Internal SSD - SATA III 6 Gb/s",
      "price": 109,
      "description": "Easy upgrade for faster boot up, shutdown, application load and response (As compared to 5400 RPM SATA 2.5” hard drive; Based on published specifications and internal benchmarking tests using PCMark vantage scores) Boosts burst write performance, making it ideal for typical PC workloads The perfect balance of performance and reliability Read/write speeds of up to 535MB/s/450MB/s (Based on internal testing; Performance may vary depending upon drive capacity, host device, OS and application.)",
      "image": "https://fakestoreapi.com/img/61U7T1koQqL._AC_SX679_.jpg",
      "quantity": 10,
      "shipping": 3.5,
     "categoryId": "39655df3-9fe8-4451-971d-8af25979b104"
  }

  {
      "name": "Silicon Power 256GB SSD 3D NAND A55 SLC Cache Performance Boost SATA III 2.5",
      "price": 109,
      "description": "3D NAND flash are applied to deliver high transfer speeds Remarkable transfer speeds that enable faster bootup and improved overall system performance. The advanced SLC Cache Technology allows performance boost and longer lifespan 7mm slim design suitable for Ultrabooks and Ultra-slim notebooks. Supports TRIM command, Garbage Collection technology, RAID, and ECC (Error Checking & Correction) to provide the optimized performance and enhanced reliability.",
      "image": "https://fakestoreapi.com/img/71kWymZ+c+L._AC_SX679_.jpg",
      "quantity": 22,
      "shipping": 2.5,
      "categoryId": "39655df3-9fe8-4451-971d-8af25979b104"
  }

  {
      "name": "High-Speed Blender",
      "slug": "high-speed-blender",
      "image": "https://example.com/images/high_speed_blender.jpg",
      "description": "A versatile blender perfect for smoothies and more.",
      "price": 99.99,
      "quantity": 60,
      "sold": 15,
      "shipping": 12.00,
      "categoryId": "39655df3-9fe8-4451-971d-8af25979b104"
  }

  {
      "name": "Robotic Vacuum Cleaner",
      "slug": "robotic-vacuum-cleaner",
      "image": "https://example.com/images/robotic_vacuum_cleaner.jpg",
      "description": "An efficient robotic vacuum cleaner to keep your home spotless.",
      "price": 299.99,
      "quantity": 40,
      "sold": 10,
      "shipping": 20.00,
      "categoryId": "39655df3-9fe8-4451-971d-8af25979b104"
  }



  {
      "name": "Mystery Novel",
      "slug": "mystery-novel",
      "image": "https://example.com/images/mystery_novel.jpg",
      "description": "An intriguing mystery novel that keeps you on the edge of your seat.",
      "price": 19.99,
      "quantity": 120,
      "sold": 40,
      "shipping": 3.00,
      "categoryId": "c5dea427-d68e-4cdd-aff9-d479e3e21888"
  }

  {
      "name": "Gourmet Cookbook",
      "slug": "gourmet-cookbook",
      "image": "https://example.com/images/gourmet_cookbook.jpg",
      "description": "A cookbook filled with gourmet recipes for all occasions.",
      "price": 25.99,
      "quantity": 80,
      "sold": 25,
      "shipping": 4.00,
      "categoryId": "c5dea427-d68e-4cdd-aff9-d479e3e21888"
  }


  ```

#### 7.9 GET => api/products => Get all the products

```csharp
// Controller
  // GET => /api/users => Get all the users
  [HttpGet]
  public async Task<IActionResult> GetProducts([FromQuery] QueryParameters queryParameters)
  {
      try
      {
          var products = await _productService.GetProductsAsync(queryParameters);
          return ApiResponse.Success(products);
      }
      catch (ApplicationException ex)
      {
          return ApiResponse.ServerError(ex.Message);
      }
      catch (Exception ex)
      {
          Console.WriteLine($"Exception : {ex.Message}");
          return ApiResponse.ServerError("An unexpected error occurred.");
      }
  }

// Service
 public async Task<PaginatedResult<ProductDto>> GetProductsAsync(QueryParameters queryParameters)
  {
      try
      {
          var query = _appDbContext.Products.Include(p => p.Category).AsQueryable();

          // Search based on name or email
          if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
          {
              var lowerCaseSearchTerm = queryParameters.SearchTerm.ToLower();
              query = query.Where(u => u.Name.ToLower().Contains(lowerCaseSearchTerm));
          }

          // Sorting (working)
          switch (queryParameters.SortBy?.ToLower())
          {
              case "Name":
                  query = queryParameters.SortOrder.ToLower() == "desc"
                      ? query.OrderByDescending(p => p.Name)
                      : query.OrderBy(p => p.Name);
                  break;
              case "CreatedAt":
                  query = queryParameters.SortOrder.ToLower() == "desc"
                      ? query.OrderByDescending(p => p.CreatedAt)
                      : query.OrderBy(p => p.CreatedAt);
                  break;

              // Add other sortable fields if necessary
              default:
                  query = query.OrderBy(p => p.Name); // Default sorting
                  break;
          }


          // Calculate the total count of users (after filtering)
          var totalCount = await query.CountAsync();

          // Ensure pageNumber and pageSize are valid
          if (queryParameters.PageNumber < 1) queryParameters.PageNumber = 1;
          if (queryParameters.PageSize < 1) queryParameters.PageSize = 10;

          // Fetch paginated categories data
          var products = await query
              .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize) // Skip to the correct page
              .Take(queryParameters.PageSize)                    // Take the correct page size
              .ToListAsync();

          // Map the fetched users to UserDto
          var productsDto = _mapper.Map<List<ProductDto>>(products);

          // Return paginated result
          return new PaginatedResult<ProductDto>
          {
              Items = productsDto,                   // Return DTOs, not entities
              TotalCount = totalCount,
              PageNumber = queryParameters.PageNumber,
              PageSize = queryParameters.PageSize
          };
      }
      catch (Exception ex)
      {
          // Handle any other unexpected exceptions
          Console.WriteLine($"An unexpected error occurred: {ex.Message}");
          throw new ApplicationException("An unexpected error occurred. Please try again later." + ex.Message);
      }
  }
```

#### 7.10 GET => api/products/{productId} => Get a single product

```csharp
// Controller
// GET => /api/products/{productId} => Get a single product by Id
[HttpGet("{productIdOrSlug}")]
public async Task<IActionResult> GetProducts(string productIdOrSlug)
{
    try
    {
        var product = await _productService.GetProductByIdOrSlug(productIdOrSlug);
        if (product == null)
        {
            return ApiResponse.NotFound("Product not found");
        }
        return ApiResponse.Success(product, "Product Retrived successfully");
    }
    catch (ApplicationException ex)
    {
        return ApiResponse.ServerError(ex.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception : {ex.Message}");
        return ApiResponse.ServerError("An unexpected error occurred.");
    }
}

// Service
 public async Task<ProductDto?> GetProductByIdOrSlug(string slugOrId)
  {
      try
      {
          Product? product;

          // Check if the slugOrId is a valid Guid, meaning it's an ID.
          if (Guid.TryParse(slugOrId, out var productId))
          {
              product = await _appDbContext.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == productId);
          }
          else
          {
              // Otherwise, assume it's a slug and search by slug
              product = await _appDbContext.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Slug == slugOrId);
          }

          if (product == null)
          {
              return null;
          }

          var productData = _mapper.Map<ProductDto>(product);

          // Return null if user not found, otherwise return the user
          return productData;
      }
      catch (Exception ex)
      {
          // Handle any other unexpected exceptions
          Console.WriteLine($"An unexpected error occurred: {ex.Message}");
          throw new ApplicationException("An unexpected error occurred. Please try again later." + ex.Message);
      }
  }

```

#### 7.11 DELETE => api/products/{productId} => Delete a single product by slugOrId (working)

#### 7.12 PUT => api/products/{productId} => Update a single product by slugOrId (working)

### 8 Order API (1-Many Relation => Update this based on your need)

- All the User EndPoints

  - GET /api/users => get all users with their orders
  - GET /api/users/{id} => get a specific users with their orders
  - POST /api/users/ => Create a new User
  - PUT /api/users/{id} => Update an existing User
  - DELETE /api/users/{id} => Delete an exisiting User

- All the Order EndPoints
  - GET /api/orders => get all orders with user information
  - GET /api/orders/{id} => get a specific orders with user information
  - POST /api/orders => Create a new Order for a user
  - PUT /api/orders/{id} => Update an exisiting Order
  - DELETE /api/orders/{id} => Delete an exisiting Order

#### 8.1 Create the Order Entity

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_db_api.EFCore;

namespace ecommerce_db_api.Models.orders
{
    public class Order
    {
        public int OrderId { get; set; } // Primary Key
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime OrderDate { get; set; }

        // Foreign Key
        public int UserId { get; set; }

        // Navigation Property
        public User User { get; set; }
    }
}
```

#### 8.2 Make the adjustment in User and dbcontext

```csharp
 public List<Order> orders { get; set; } = new List<Order>();

 // In AppDbContext
  modelBuilder.Entity<User>()
  .HasMany(u => u.orders)
  .WithOne(o => o.User)
  .HasForeignKey(o => o.UserId)
  .OnDelete(DeleteBehavior.Cascade);
```

#### 8.3 POST => Create an Order

```csharp
// CreateOrderDto

// OrderDto

// Service


// Controller

```

#### 8.4 Order CRUD API

```csharp
// Interface
public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetOrdersAsync();
    Task<Order> GetOrderByIdAsync(Guid id);
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
    Task CreateOrderAsync(Order order);
    Task UpdateOrderAsync(Order order);
    Task DeleteOrderAsync(Guid id);
}

public class OrderRepository : IOrderRepository
{
    private readonly OrdersDbContext _context;

    public OrderRepository(OrdersDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        return await _context.Orders.Include(o => o.User).ToListAsync();
    }

    public async Task<Order> GetOrderByIdAsync(Guid id)
    {
        return await _context.Orders.Include(o => o.User)
                                    .FirstOrDefaultAsync(o => o.OrderId == id);
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId)
    {
        return await _context.Orders.Where(o => o.UserId == userId)
                                    .Include(o => o.User)
                                    .ToListAsync();
    }

    public async Task CreateOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateOrderAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOrderAsync(Guid id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;

    public OrdersController(IOrderRepository orderRepository, IUserRepository userRepository)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
    }

    // GET: api/orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        var orders = await _orderRepository.GetOrdersAsync();
        return Ok(orders);
    }

    // GET: api/orders/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await _orderRepository.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound();
        return Ok(order);
    }

    // GET: api/orders/user/{userId}
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUser(int userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound($"User with ID {userId} not found.");

        var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
        return Ok(orders);
    }

    // POST: api/orders
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(Order order)
    {
        // Optionally, validate if the User exists
        var user = await _userRepository.GetUserByIdAsync(order.UserId);
        if (user == null)
            return BadRequest($"User with ID {order.UserId} does not exist.");

        await _orderRepository.CreateOrderAsync(order);
        return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
    }

    // PUT: api/orders/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, Order order)
    {
        if (id != order.OrderId)
            return BadRequest();

        // Optionally, validate if the User exists
        var user = await _userRepository.GetUserByIdAsync(order.UserId);
        if (user == null)
            return BadRequest($"User with ID {order.UserId} does not exist.");

        await _orderRepository.UpdateOrderAsync(order);
        return NoContent();
    }

    // DELETE: api/orders/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        await _orderRepository.DeleteOrderAsync(id);
        return NoContent();
    }
}
```

#### 8.5 User CRUD API

Create an interface for user-related data operations.

```csharp
public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsersAsync();
    Task<User> GetUserByIdAsync(int id);
    Task CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
}
```

- **User Repository Implementation**

Implement the `IUserRepository`.

```csharp
public class UserRepository : IUserRepository
{
    private readonly OrdersDbContext _context;

    public UserRepository(OrdersDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _context.Users
                             .Include(u => u.Orders) // Eager loading orders
                             .ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users
                             .Include(u => u.Orders)
                             .FirstOrDefaultAsync(u => u.UserId == id);
    }

    public async Task CreateUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
// UserController
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _userRepository.GetUsersAsync();
        return Ok(users);
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        await _userRepository.CreateUserAsync(user);
        return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
    }

    // PUT: api/users/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User user)
    {
        if (id != user.UserId)
            return BadRequest();

        await _userRepository.UpdateUserAsync(user);
        return NoContent();
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userRepository.DeleteUserAsync(id);
        return NoContent();
    }
}
```

#### 8.6. Configure Services in `Program.cs` or `Startup.cs`

Register both repositories and configure the `DbContext`.

```csharp
public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        // Configure DbContext with SQL Server
        services.AddDbContext<OrdersDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddControllers();

        // (Optional) Add Swagger for API documentation
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            // (Optional) Enable Swagger in development
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
```

#### 8.7 Update Database with Migrations

After updating the models and `DbContext`, create and apply migrations to update the database schema.

```bash
# Install Entity Framework CLI tools if not already installed
dotnet tool install --global dotnet-ef

# Add a new migration
dotnet ef migrations add AddUserAndOrderRelationship

# Apply the migration to update the database
dotnet ef database update
```

---

#### 8.8 Testing the API

- **Example JSON for Creating a User:**

```json
POST /api/users
Content-Type: application/json

{
    "username": "john_doe",
    "email": "john.doe@example.com"
}
```

- Example JSON for Creating an Order:\*\*

```json
POST /api/orders
Content-Type: application/json

{
    "productId": "11111",
    "quantity": 1,
    "price": 1200.00,
    "orderDate": "2024-10-01T10:00:00Z",
    "userId": 1
}
```

#### 8.9 Additional Considerations

#### **Validation**

Implement data validation to ensure that inputs are correct. You can use data annotations in your models or implement Fluent Validation.

**Example with Data Annotations:**

```csharp
public class User
{
    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public ICollection<Order> Orders { get; set; }
}

public class Order
{
    public int OrderId { get; set; }

    [Required]
    public string ProductName { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be positive")]
    public decimal Price { get; set; }

    public DateTime OrderDate { get; set; }

    // Foreign Key
    [Required]
    public int UserId { get; set; }

    public User User { get; set; }
}
```

#### **DTOs (Data Transfer Objects)**

For better separation of concerns and to avoid exposing internal models, consider using DTOs.

**Example:**

```csharp
public class UserDto
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public IEnumerable<OrderDto> Orders { get; set; }
}

public class OrderDto
{
    public int OrderId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime OrderDate { get; set; }
}
```

#### Implement mapping between entities and DTOs using libraries like AutoMapper.

#### **Authentication & Authorization**

To ensure that users can only access their own orders, implement authentication (e.g., JWT) and authorization mechanisms.

#### **Error Handling**

Implement global error handling to manage exceptions gracefully and provide meaningful error messages.

#### **Logging**

Incorporate logging to monitor the application's behavior and troubleshoot issues.

---

By following the steps above, you establish a robust relationship between **Users** and **Orders** in your ASP.NET Core API, enabling comprehensive CRUD operations that reflect real-world scenarios.

### 9 Order<=>Product (Many-Many Relationship)

- Target CRUD Operations Summary:

  - POST: /api/orders — Create a new order.
  - GET: /api/orders/{id} — Get a specific order by ID.
  - PUT: /api/orders/{id} — Update an existing order by ID.
  - DELETE: /api/orders/{id} — Delete an order by ID.
  - GET: /api/orders — (Optional) Retrieve all orders.

- Many-to-many relationship between Order and Product, where an order can contain multiple products, and a product can appear in multiple orders.

To achieve this, you need to create a join table (also called a linking entity) between Order and Product that holds the quantity and price for each product in the order. This join table is typically called something like OrderProduct.

### 9.1 Creating Entities

```csharp
// Order Entity
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_db_api.EFCore
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public Guid UserId { get; set; } // foregin id for the User
        public User User { get; set; } // reference for the User Entity

        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();  // Collection of products in the order
    }
}

// Product Entity
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ecommerce_db_api.EFCore
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 0;
        public int Sold { get; set; } = 0;
        public decimal Shipping { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid CategoryId { get; set; }  // Foreign Key for the Category

        [JsonIgnore]
        public Category? Category { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();  // Many-to-Many relationship with orders
    }
}

// OrderProduct Entity
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_db_api.EFCore
{
    public class OrderProduct
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; }  // Navigation property

        public Guid ProductId { get; set; }
        public Product Product { get; set; }  // Navigation property

        public int Quantity { get; set; }  // Quantity of this product in the order
        public decimal Price { get; set; }  // Price of this product in the order
    }
}

```

### 9.2 Testing with some data

```json
{
  "userId": "b1a1b1e1-c1c1-11eb-b8bc-0242ac130003",
  "orderDate": "2024-09-30T00:00:00Z",
  "orderProducts": [
    {
      "productId": "d1a1d1f1-c1c1-11eb-b8bc-0242ac130003",
      "quantity": 2,
      "price": 100.0
    },
    {
      "productId": "e1a1e1f1-c1c1-11eb-b8bc-0242ac130003",
      "quantity": 1,
      "price": 200.0
    }
  ]
}
```

### 9.3 Create DTO's

- Represents the order data, including user and product details

```csharp

public class OrderProductDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class OrderDto
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public Guid UserId { get; set; }
    public List<OrderProductDto> OrderProducts { get; set; } = new List<OrderProductDto>();
}
```

- Represents the input data required to create a new order

```csharp
public class CreateOrderProductDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class CreateOrderDto
{
    public Guid UserId { get; set; }
    public List<CreateOrderProductDto> OrderProducts { get; set; } = new List<CreateOrderProductDto>();
}
```

### 9.4 Setup the context for many-many realtionship

```csharp
// Many-to-Many relationship between Order and Product using OrderProduct as join table
  modelBuilder.Entity<OrderProduct>()
      .HasKey(op => new { op.OrderId, op.ProductId });  // Composite primary key

  modelBuilder.Entity<OrderProduct>()
      .HasOne(op => op.Order)
      .WithMany(o => o.OrderProducts)
      .HasForeignKey(op => op.OrderId);

  modelBuilder.Entity<OrderProduct>()
      .HasOne(op => op.Product)
      .WithMany(p => p.OrderProducts)
      .HasForeignKey(op => op.ProductId);
```

### 9.5 Setup the IOrderSevice

```csharp
 public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
    Task<OrderDto?> GetOrderByIdAsync(Guid orderId);
    Task<OrderDto?> UpdateOrderAsync(Guid orderId, CreateOrderDto updateOrderDto);
    Task<bool> DeleteOrderByIdAsync(Guid orderId);
}
```

### 9.6 Setup the Order Service

```csharp
public class OrderService
{
    private readonly AppDbContext _appDbContext;
    private readonly IMapper _mapper;

    public OrderService(AppDbContext appDbContext, IMapper mapper)
    {
        _appDbContext = appDbContext;
        _mapper = mapper;
    }

    // Create a new order
    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        var order = new Order
        {
            UserId = createOrderDto.UserId,
            OrderDate = DateTime.UtcNow
        };

        foreach (var orderProductDto in createOrderDto.OrderProducts)
        {
            var orderProduct = new OrderProduct
            {
                ProductId = orderProductDto.ProductId,
                Quantity = orderProductDto.Quantity,
                Price = orderProductDto.Price
            };

            order.OrderProducts.Add(orderProduct);
        }

        await _appDbContext.Orders.AddAsync(order);
        await _appDbContext.SaveChangesAsync();

        var orderDto = _mapper.Map<OrderDto>(order);
        return orderDto;
    }

    // Get order by ID
    public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId)
    {
       var order = await _appDbContext.Orders
        .Include(o => o.OrderProducts)
        .ThenInclude(op => op.Product)  // Include Product details
        .Include(o => o.User)  // Include User details
        .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null)
            return null;

        return _mapper.Map<OrderDto>(order);
    }

    // Update an order
    public async Task<OrderDto?> UpdateOrderAsync(Guid orderId, CreateOrderDto updateOrderDto)
    {
        var order = await _appDbContext.Orders
            .Include(o => o.OrderProducts)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null)
            return null;

        // Clear existing products and add updated products
        order.OrderProducts.Clear();
        foreach (var orderProductDto in updateOrderDto.OrderProducts)
        {
            var orderProduct = new OrderProduct
            {
                ProductId = orderProductDto.ProductId,
                Quantity = orderProductDto.Quantity,
                Price = orderProductDto.Price
            };

            order.OrderProducts.Add(orderProduct);
        }

        _appDbContext.Orders.Update(order);
        await _appDbContext.SaveChangesAsync();

        return _mapper.Map<OrderDto>(order);
    }

    // Delete an order by ID
    public async Task<bool> DeleteOrderByIdAsync(Guid orderId)
    {
        var order = await _appDbContext.Orders
            .Include(o => o.OrderProducts)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null)
            return false;

        _appDbContext.Orders.Remove(order);
        await _appDbContext.SaveChangesAsync();
        return true;
    }
}

```

### 9.7 Setup the Order Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    // POST: /api/orders
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var order = await _orderService.CreateOrderAsync(createOrderDto);
        return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
    }

    // GET: /api/orders/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound();

        return Ok(order);
    }

    // PUT: /api/orders/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] CreateOrderDto updateOrderDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updatedOrder = await _orderService.UpdateOrderAsync(id, updateOrderDto);
        if (updatedOrder == null)
            return NotFound();

        return Ok(updatedOrder);
    }

    // DELETE: /api/orders/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        var result = await _orderService.DeleteOrderByIdAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
```

### 9.8 Add Enum for OrderStatus

Here are the codes for implementing the `OrderStatus` enum in your project:

##### **1. Define the Enum**

```csharp
public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}
```

##### **2. Update the Order Model**

```csharp
public class Order
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User User { get; set; }

    public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public OrderStatus Status { get; set; } = OrderStatus.Pending;  // Default to Pending
}
```

##### **3. Update DTOs**

```csharp
public class OrderDto
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public UserDto User { get; set; }
    public List<OrderProductDto> OrderProducts { get; set; } = new List<OrderProductDto>();
    public OrderStatus Status { get; set; }
}

public class CreateOrderDto
{
    public Guid UserId { get; set; }
    public List<CreateOrderProductDto> OrderProducts { get; set; } = new List<CreateOrderProductDto>();
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}
```

##### **4. Update Mapping Profile**

```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Order, OrderDto>();
        CreateMap<CreateOrderDto, Order>();
        CreateMap<User, UserDto>();
        CreateMap<OrderProduct, OrderProductDto>();
        CreateMap<Product, ProductDto>();
    }
}
```

##### **5. Sample Controller Update**

```csharp
[HttpPut("{id}/status")]
public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] OrderStatus status)
{
    var result = await _orderService.UpdateOrderStatusAsync(id, status);
    if (!result)
        return NotFound();

    return NoContent();
}
```

##### **6. Update Service for Status**

```csharp
public async Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
{
    var order = await _appDbContext.Orders.FindAsync(orderId);
    if (order == null) return false;

    order.Status = newStatus;
    await _appDbContext.SaveChangesAsync();
    return true;
}
```

These are the necessary code changes for adding the `OrderStatus` enum to your project.

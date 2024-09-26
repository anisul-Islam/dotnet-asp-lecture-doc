## 5. ASP.NET and REST API with Database (2024)

### 5.1 Create and Run the web api

- Create a webapi project: `dotnet new console -o ecommerce-api`
- Run the webapi project: `dotnet watch`
- Stop opening the tab everytime => Properties folder => launchSettings.json => profiles => launchBrowser => false
- Add .gitignore => [check this doc](https://github.com/anisul-Islam/dotnet-gitignore/blob/main/README.md)
- Modify the startup file

  ```csharp
  // Program.cs
  var builder = WebApplication.CreateBuilder(args);

  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen();

  var app = builder.Build();

  // Configure the HTTP request pipeline.
  if (app.Environment.IsDevelopment())
  {
      app.UseSwagger();
      app.UseSwaggerUI();
  }

  app.UseHttpsRedirection();


  app.MapGet("/", () =>
  {
      var response = new
      {
          StatusCode = 200,
          Message = "Api is running fine"
      };
      return Results.Ok(response);
  })
  .WithOpenApi();

  app.Run();
  ```

To set up your ASP.NET Core project with database connectivity using Entity Framework Core (EF Core) and PostgreSQL, you’ll need the following dependencies.

### 5.2 Concept: What is Entity and EF Core?

#### EFCore

- Entity Framework Core (EF Core) is a modern, open-source, object-relational mapping (ORM) framework for .NET applications, developed by Microsoft.
- It allows developers to interact with databases using .NET objects and eliminates the need to write most of the raw SQL queries.
- EF Core enables developers to work with databases using C# classes (referred to as entities), and it handles the conversion of the data between the database and the application automatically.

#### What is an Entity?

In the context of databases and Entity Framework Core (EF Core), an entity refers to a class that represents a table in a database. Each instance of the entity class corresponds to a row in the table, and the properties of the entity represent the columns of the table.

#### Alternative of EFCore

Apart from **Entity Framework Core (EF Core)**, there are several other ways to connect to a database in .NET applications. Some of these methods provide more control and flexibility over database operations. Below are some common alternatives to EF Core for database connectivity in .NET:

##### 1. **ADO.NET (ActiveX Data Objects)**

- **ADO.NET** is a low-level, foundational data access technology for .NET. It provides a set of classes for accessing databases, executing SQL queries, and retrieving results. ADO.NET offers full control over database operations and is often used when developers need high performance or more direct control over SQL execution.

   **Example:**

   ```csharp
   using (SqlConnection connection = new SqlConnection("YourConnectionString"))
   {
       SqlCommand command = new SqlCommand("SELECT * FROM Products", connection);
       connection.Open();
       SqlDataReader reader = command.ExecuteReader();
       while (reader.Read())
       {
           Console.WriteLine($"{reader["Name"]} - {reader["Price"]}");
       }
   }
   ```

   **Advantages:**

- Full control over SQL and database operations.
- Lightweight and high-performance for large-scale operations.

   **Disadvantages:**

- Requires manual handling of SQL queries, parameters, and result sets.
- Higher risk of SQL injection if not handled properly.

##### 2. **Dapper**

- **Dapper** is a popular micro ORM (Object-Relational Mapper) that simplifies data access by mapping objects to database tables, similar to EF Core, but with less abstraction. Dapper is lightweight and faster than EF Core because it doesn't do as much under the hood.

   **Example:**

   ```csharp
   using (var connection = new SqlConnection("YourConnectionString"))
   {
       var products = connection.Query<Product>("SELECT * FROM Products").ToList();
       foreach (var product in products)
       {
           Console.WriteLine($"{product.Name} - {product.Price}");
       }
   }
   ```

   **Advantages:**

- Lightweight and fast compared to full ORMs like EF Core.
- Simple to use and integrate with existing SQL queries.
- Less overhead, making it ideal for performance-sensitive applications.

   **Disadvantages:**

- Lacks some advanced features like change tracking and migrations found in EF Core.
- Requires more manual SQL compared to EF Core.

##### 3. **NHibernate**

- **NHibernate** is a full-featured ORM for .NET, similar to EF Core. It provides rich functionality for mapping objects to database tables, but it tends to be more complex than EF Core.

   **Example:**

   ```csharp
   using (ISession session = sessionFactory.OpenSession())
   {
       using (ITransaction transaction = session.BeginTransaction())
       {
           var products = session.Query<Product>().ToList();
           foreach (var product in products)
           {
               Console.WriteLine($"{product.Name} - {product.Price}");
           }
           transaction.Commit();
       }
   }
   ```

   **Advantages:**

- Highly customizable and powerful ORM.
- Supports advanced mapping features, caching, and lazy loading.

   **Disadvantages:**

- Complex to set up and use compared to simpler tools like Dapper or ADO.NET.
- Slower performance than micro ORMs like Dapper.

##### 4. **LINQ to SQL**

- **LINQ to SQL** is an ORM that was introduced in .NET 3.5. It provides a way to map classes to SQL Server tables using LINQ queries, but it's limited to SQL Server. LINQ to SQL is simpler than EF Core but also more limited in functionality.

   **Example:**

   ```csharp
   using (var db = new DataContext("YourConnectionString"))
   {
       var products = db.GetTable<Product>().ToList();
       foreach (var product in products)
       {
           Console.WriteLine($"{product.Name} - {product.Price}");
       }
   }
   ```

   **Advantages:**

- Simple and easy-to-use ORM.
- Integrates well with LINQ queries.

   **Disadvantages:**

- Limited to SQL Server.
- Not as feature-rich as EF Core or NHibernate.

##### 5. **Micro ORMs (e.g., PetaPoco, ServiceStack.OrmLite)**

- Micro ORMs are lightweight data access libraries that provide object mapping without the overhead of full ORMs like EF Core or NHibernate. They are designed to provide basic functionality (CRUD operations) without compromising performance.

   **PetaPoco Example:**

   ```csharp
   using (var db = new Database("YourConnectionString"))
   {
       var products = db.Query<Product>("SELECT * FROM Products").ToList();
       foreach (var product in products)
       {
           Console.WriteLine($"{product.Name} - {product.Price}");
       }
   }
   ```

   **Advantages:**

- Lightweight and fast.
- Less complex than full ORMs.

   **Disadvantages:**

- Fewer features than full ORMs (no change tracking, migrations, etc.).

##### 6. **Raw SQL Queries**

- You can execute raw SQL queries directly in .NET without using any ORM. This gives full control over the database but requires manual query building, parameterization, and result mapping.

   **Example:**

   ```csharp
   using (SqlConnection connection = new SqlConnection("YourConnectionString"))
   {
       SqlCommand command = new SqlCommand("SELECT * FROM Products WHERE Price > @price", connection);
       command.Parameters.AddWithValue("@price", 100);
       connection.Open();
       SqlDataReader reader = command.ExecuteReader();
       while (reader.Read())
       {
           Console.WriteLine($"{reader["Name"]} - {reader["Price"]}");
       }
   }
   ```

   **Advantages:**

- Full control over SQL queries and database operations.
- No abstraction, making it ideal for complex queries.

   **Disadvantages:**

- Higher risk of errors and security issues (e.g., SQL injection) if not handled correctly.
- Manual mapping of results to objects.

### 5.3 Dependencies for Database Connectivity and EF Core

Here are all the installation commands you might need for the setup:

```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet tool install --global dotnet-ef

dotnet add package Microsoft.AspNetCore.OData --version 8.0.0-preview3
dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson --version 6.0.0-preview.6.21355.2
dotnet add package Swashbuckle.AspNetCore --version 6.2.3
```

1. **`Npgsql.EntityFrameworkCore.PostgreSQL`**:
   - Provides PostgreSQL database provider for EF Core, enabling you to interact with PostgreSQL databases.
   - Install Command:

     ```bash
     dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
     ```

2. **`Microsoft.EntityFrameworkCore.Design`**:
   - Required for design-time tools such as `dotnet ef` commands (migrations, scaffolding).
   - Install Command:

     ```bash
     dotnet add package Microsoft.EntityFrameworkCore.Design
     ```

3. **`Microsoft.EntityFrameworkCore.Tools`**:
   - Provides command-line tools for EF Core commands (e.g., migrations).
   - Install Command:

     ```bash
     dotnet add package Microsoft.EntityFrameworkCore.Tools
     ```

4. **`Microsoft.Extensions.Configuration`** (already included in ASP.NET Core projects):
   - This library is used for loading configurations from `appsettings.json`, environment variables, etc., which is needed for database connection strings.
   - Usually, you don’t need to explicitly install it, but if needed:

     ```bash
     dotnet add package Microsoft.Extensions.Configuration
     ```

#### Dependencies for API and Swagger Support

5. **`Swashbuckle.AspNetCore`**:
   - Provides tools to generate Swagger documentation for your API.
   - Install Command:

     ```bash
     dotnet add package Swashbuckle.AspNetCore --version 6.2.3
     ```

6. **`Microsoft.AspNetCore.OData`**:
   - Adds support for OData in ASP.NET Core Web APIs.
   - Install Command:

     ```bash
     dotnet add package Microsoft.AspNetCore.OData --version 8.0.0-preview3
     ```

7. **`Microsoft.AspNetCore.Mvc.NewtonsoftJson`**:
   - Adds support for using Newtonsoft.Json in ASP.NET Core for JSON serialization and deserialization.
   - Install Command:

     ```bash
     dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson --version 6.0.0-preview.6.21355.2
     ```

#### Additional Dependencies (if required)

8. **`dotnet-ef` (tool)**:
   - Command-line tool used to create migrations, update the database schema, and scaffold models.
   - Install Command:

     ```bash
     dotnet tool install --global dotnet-ef
     ```

### 5.4 Connect Database

#### Concept: What is DbContext?

- What is Context? Why do we need Context?

  In Entity Framework Core (EF Core), a DbContext is a crucial component because it acts as a bridge between your application and the database. It represents a session with the database and is used to query and save data. Creating a DbContext is necessary for several reasons:

  1. Managing Database Connections:
  The DbContext manages the connection to the database. It opens and closes the connection as needed when querying or saving data. You don't have to manually handle database connections in your code, as the DbContext does it for you, making the process simpler and less error-prone.

  2. Mapping Entities to Database Tables (Object Relational Mapper = ORM):
  The DbContext allows you to define how your C# classes (entities) map to database tables, columns, and relationships. This mapping is essential because Entity Framework Core is an Object-Relational Mapper (ORM), which means it converts C# objects into database records and vice versa.

#### Create the DbContext

  ```csharp
  // EFCore/AppDbContext.cs
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
  ```

#### Configure the Database

```csharp
// appsettings.json
 "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=full-ecommerce-db;Username=postgres;Password=new_password;"
  }

// inside the Startup file / Program.cs
builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

#### Migration scripts

- Install Entity Framework Core Tools: Ensure that you have installed the Entity Framework Core Tools globally. You can do this by running the following command: dotnet tool install --global dotnet-ef

```shell
# Generate the migration
dotnet ef migrations add InitialCreate

# Run the Migration and Update the Database:
dotnet ef database update; 

# if you wish to undo this action use 
dotnet ef migrations remove
```

Here's a list of commonly used Entity Framework Core migration commands:

1. **Adding a Migration**: Create a new migration based on the changes to your model.

   ```
   dotnet ef migrations add <NameOfMigration>
   ```

   Replace `<NameOfMigration>` with a descriptive name for your migration.

2. **Applying Migrations**: Update the database to apply pending migrations.

   ```
   dotnet ef database update
   ```

3. **Reverting Migrations**: Rollback the last applied migration.

   ```
   dotnet ef migrations remove
   ```

4. **Applying Migrations to a Specific Version**: Update the database to a specific migration.

   ```
   dotnet ef database update <TargetMigration>
   ```

   Replace `<TargetMigration>` with the name of the migration you want to update to.

5. **Generating a Script**: Generate a SQL script for a migration without applying it to the database.

   ```
   dotnet ef migrations script
   ```

6. **Applying Migrations for a Specific Environment**: Update the database for a specific environment (e.g., Development, Staging, Production).

   ```
   dotnet ef database update --environment <EnvironmentName>
   ```

7. **Applying Migrations for a Specific DbContext**: Update the database for a specific DbContext.

   ```
   dotnet ef database update --context <DbContextName>
   ```

8. **Applying Migrations for a Specific Project**: Update the database for a specific project within a solution.

   ```
   dotnet ef database update --project <ProjectName>
   ```

9. **Generating a Migration for a Specific DbContext**: Create a migration for a specific DbContext.

   ```
   dotnet ef migrations add <NameOfMigration> --context <DbContextName>
   ```

10. **Generating a Migration for a Specific Project**: Create a migration for a specific project within a solution.

   ```
   dotnet ef migrations add <NameOfMigration> --project <ProjectName>
   ```

#### Add UUID

- you can setup it in pgadmin or in the terminal `CREATE EXTENSION IF NOT EXISTS "uuid-ossp";`

### 5.5 User API

#### 5.5.1 Create the User Entity

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

#### 5.5.2 Create the User Table in Database

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

#### 5.5.3 Setup the Service and Controller for User (MVC Pattern)

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

#### 5.5.4 POST /api/users => Create an user

- workflow of MVC

![alt text](image-17.png)

- Create a DTO to get the necessary data to create an user

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CreateUserDto
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Image { get; set; }
}
```

- Create a Controller to handle request for creating an user

```csharp
[HttpPost]
public async Task<IActionResult> CreateUser([FromBody] CreateUserDto newUser)
{
    var user = await _userService.CreateUserAsync(newUser);
    var response = new { Message = "An user created successfully", User = user };
    return Created($"/api/users/{user.UserId}", response);
}
```

- Create a Service to store the user in DB

```csharp
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

    await _appDbContext.Users.AddAsync(user);
    await _appDbContext.SaveChangesAsync();
    return user;
}
```

#### 5.5.5 Concept: Data Validation With Data Annotation vs Fluent API

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

| **Aspect**                | **Data Annotations**                                       | **Fluent API**                                              |
|---------------------------|------------------------------------------------------------|-------------------------------------------------------------|
| **Configuration Location** | Applied directly to the model properties using attributes. | Configured centrally inside `OnModelCreating` in `DbContext`.|
| **Complexity**             | Simple to use for basic configurations and validations.    | Suitable for complex configurations and relationships.       |
| **Scope of Application**   | Works at the **model** level and applies rules at **compile time**. | Works at the **database** level and applies rules at **runtime**.|
| **Exception Handling**     | Throws `ValidationException` before interacting with the database. | Throws `DbUpdateException` when saving to the database.       |
| **Flexibility**            | Limited in terms of complex configurations (e.g., unique constraints). | Full flexibility to define more advanced configurations.      |
| **Validation Location**    | Mostly client-side, before interaction with the database.   | Mostly server-side, enforced during database operations.      |

##### **Which One to Use?**

- **Data Annotations** are better for simpler models where basic validation and configuration are enough.
- **Fluent API** should be used for more complex scenarios, such as defining relationships, composite keys, or when dealing with custom database behaviors.

**Best Practice:** In a large project, you can combine both methods. Use **Data Annotations** for simpler, model-specific rules and use **Fluent API** for advanced configurations that require more control or aren't possible with Data Annotations alone.

#### 5.5.6 Add Data Validation with Data Annotation in CreateUserDto

you should primarily add data annotations to the DTO (Data Transfer Object), like CreateUserDto. The main reason is that the DTO represents the data being transferred between the client and server, so validation should focus on ensuring that the incoming data (from the client) is valid before it is mapped to the actual entity model (User). This keeps your entity models clean from validation logic, and instead focuses them on representing the structure of your data.

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

#### 5.5.7 Add Data Validation with Fluent Api in DBcontext (working)

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

#### 5.5.8 Add Exception Handling

- Create the UserDto

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
 public async Task<User> CreateUserAsync(CreateUserDto newUserDto)
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
```

#### 5.5.6 Centralized Response

```csharp
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

- Use them now in controller (working on this)

```csharp

```

#### 5.5.7 GET /api/users => Get all the users

- Create the UserDto

```csharp

```

- Add a Controller to handle get users request

```csharp
 [HttpGet]
  public async Task<IActionResult> GetAllUsers()
  {
      var users = await _userService.GetUsersAsync();
      var response = new { StatusCode = 200, Message = "Users are returned successfully", Users = users };
      return Ok(response);
  }
```

- Add a Service to get users from database

```csharp
  public async Task<List<User>> GetUsersAsync()
  {
      return await _appDbContext.Users.ToListAsync();
  }
```

- test the endpoint

```
GET localhost_address_here/api/users
```

#### 5.5.8 GET /api/users => Add Pagination, Searching, Sorting

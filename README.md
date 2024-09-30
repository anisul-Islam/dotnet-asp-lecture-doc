## ASP.NET and REST API with Database (2024)

### 1 Create and Run the web api

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

  services.AddControllers()
  .AddJsonOptions(options =>
  {
      options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
      options.JsonSerializerOptions.WriteIndented = true;
  });

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

### 2 Concept: What is Entity and EF Core?

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

### 3 Dependencies for Database Connectivity and EF Core

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

### 4 Connect Database

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
    public string Address { get; set; } = string.Empty;
    public string? Image { get; set; } = string.Empty;
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

  - AddAsync: Marks the entity as to be inserted in the context.
  - SaveChangesAsync: Actually commits the changes (including the new user) to the database.

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

    // - AddAsync: Marks the entity as to be inserted in the context.
    // - SaveChangesAsync: Actually commits the changes (including the new user) to the database.
    await _appDbContext.Users.AddAsync(user);
    await _appDbContext.SaveChangesAsync();
    return user;
}
```

#### 5.5 How to console the data in api

```
dotnet add package Newtonsoft.Json
using Newtonsoft.Json;
 Console.WriteLine("Received user: " + JsonConvert.SerializeObject(user));
```

#### 5.6 How to Respond with the desried data

- Respond with the desried data after the user is created

```csharp
// Create the Models/UserDto.cs
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

 // Hash the password using a library like BCrypt.Net
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

- create a dto for update data

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
```

- create the controller method

```csharp
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
```

- create the Service method

```csharp
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
###

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

### 6 Category and Product API

#### 6.1 Create the Category and Product Entity

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

#### 6.2 Adjust the dbcontext and create Interface for CategoryService

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

            modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);// optional
        }
    }
}
```

- Add the migration script

- Interface for CategoryService

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
```

#### 6.3 POST => /api/categories => Create the Category

- create the CreateCategoryDto

```csharp
// Inside the Models/categories
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
```

- Create the CategoriesDto to returned the desired data after the category created

```csharp
// Inside the EFCore/Models
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


            CreateMap<CreateCategory, Category>();
            CreateMap<Category, CategoryDto>();

            // With AutoMapper now configured, your service methods can utilize the IMapper interface to map data models to DTOs and vice versa, simplifying data transformation and reducing boilerplate code.
        }
    }
}
```

- Create the helper method

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

- Create the Services

```csharp
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
```

- register the service to Program.cs

```csharp
builder.Services.AddScoped<CategoryService>();
```

- create the controller

```csharp
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

#### 6.4 GET => /api/categories => Get all the categories

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

#### 6.5 GET => /api/categories/{slugOrId} => Get a single category by slug or id

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

#### 6.6 Delete => /api/categories/{slug} => delete category by slug or id

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

#### 6.7 PUT => /api/categories/{slug} => update category by slug or id

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

#### 6.8 POST => /api/products => Create the Product

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

- add json serializer in Program.cs

```csharp
builder.Services.AddControllers()
 .AddJsonOptions(options =>
 {
     options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
     options.JsonSerializerOptions.WriteIndented = true;
 });
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

#### 6.9 GET => api/products => Get all the products

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

#### 6.10 GET => api/products/{productId} => Get a single product

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

### 7 Profile API

- define the entity

```csharp
public class User
{
    public Guid UserId { get; set; }
    // ....

    // Navigation property
    public Profile Profile { get; set; }
}

public class Profile
{
    public Guid Id { get; set; }
    // public string Address { get; set; }
    public string PhoneNumber { get; set; }

    // Foreign key
    public Guid UserId { get; set; }

    // Navigation property
    public User User { get; set; }
}
```

- update the context

```csharp

// DbSet properties

public DbSet<Profile> Profiles { get; set; }

    // One-to-One: User <-> Profile
    modelBuilder.Entity<User>()
        .HasOne(u => u.Profile)
        .WithOne(p => p.User)
        .HasForeignKey<Profile>(p => p.UserId);
```

#### Get Profile

#### Create Profile

#### Delete Profile

#### Update Profile

### 8 Order API

### (ADD SOMEWHERE) try to insert data in pgadmin

```sql
INSERT INTO "Users" ("UserId", "Name", "Email", "Password", "Address", "Image", "IsAdmin", "IsBanned", "CreatedAt")
VALUES
    ('a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11', 'John Doe', 'john@example.com', 'password123', '123 Main St', 'john.jpg', false, false, '2024-04-30 12:00:00'::timestamp),

    ('b6e9f696-6dbb-49b2-bb55-19c13c8e1701', 'Jane Smith', 'jane@example.com', 'password456', '456 Elm St', 'jane.jpg', true, false, '2024-04-30 13:00:00'::timestamp);
```

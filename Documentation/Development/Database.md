# Database

This application uses [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) as an object-relational mapper
to work with an SQL Server database.

## Migrating the Database (Or Creating)
There are two ways to migrate the database:

1. Using the Entity Framework Tools
    1. Enter the project source directory
    2. Run `dotnet ef database update`
2. Using the CLI
    1. Enter the project source directory
    2. Run `dotnet run migrate` 

## Dropping the Database
1. Enter the project source directory
2. Run `dotnet ef database drop`

## Seeders
Seeders are stored in the `Data.Seeders` namespace. Seeders must implement the `ISeeder` interface. Seeders
can be run via the Command Line Interface (CLI). By default the `DatabaseSeeder` seeder is run.

### Running a Seeder
1. Enter the project source directory
2. Run `dotnet run seed [OptionalSeederClassName]` 

### Adding a Seeder
1. Create the seeder in the `Data.Seeders` namespace. Seeders support dependency injection.
2. Implement the `ISeeder` interface.
3. (Optional) If you want the seeder to run on the default `seed` command, register the seeder in the `DatabaseSeeder` seeder.

## Code
### Models
Models are stored in the `Data.Models` namespace. Models represent tables (or more accurately, rows of a table) in the database.

### ApplicationDbContext
The `DbContext` represents a database and is the object used to access the database. The context contains
many `DbSet<Model>` properties which represent tables in the database. This class should never be
instantiated manually, and should instead be access using dependency injection.

### Querying
Instances of the models are retrieved from the database using Language Integrated Query (LINQ). Additional
documentation on query can be found here: [Querying Data](https://docs.microsoft.com/en-us/ef/core/querying/index).

### Saving Data
Data is created, deleted, and modified in the database using instances of your entity classes. After modifying
the entity or a `DbSet`, changes can be committed by running `await context.SaveChangesAsync()`. Adding or
removing entities from/to a `DbSet` should not be done using their corresponding async methods.
Additional documentation can be found here: [Saving Data](https://docs.microsoft.com/en-us/ef/core/saving/index).

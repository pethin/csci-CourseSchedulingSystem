# Course Scheduling System

## Setting-up your development environment
1. Install SSMS (https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-2017)
2. Install SQL Server 2017 Express edition (https://www.microsoft.com/en-us/sql-server/sql-server-editions-express)
3. Install .NET Core SDK (https://dotnet.microsoft.com/download)
4. Install Rider (https://www.jetbrains.com/rider/)

### Creating the database
1. Open SSMS
2. Connect to `.\SQLEXPRESS` with **Windows Authentication**
3. Right-click **Databases** in **Object Explorer** and select **New database**
4. Enter `CourseSchedulingSystemDev` for the **Database name** and click **OK**

## Developing
### Running migrations
1. Enter the project directory
2. Run `dotnet ef database update`

### Starting the development server
1. Enter the project directory
2. Run `dotnet watch run`
3. The project will run at http://localhost:5000 and https://localhost:5001. It will automatically redirect to the HTTPS server.

### Managing JS dependencies
JS dependencies are managed via [LibMan](https://docs.microsoft.com/en-us/aspnet/core/client-side/libman/libman-cli)

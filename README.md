# Course Scheduling System

## Setting-up your development environment
1. Install [Visual Studio 2017 Enterprise](https://winthrop.onthehub.com/WebStore/OfferingsOfMajorVersionList.aspx?pmv=4fec9f1d-6d0a-e711-9427-b8ca3a5db7a1&cmi_mnuMain=3e6b4796-9ea9-e511-9413-b8ca3a5db7a1&cmi_mnuMain_child=1d5f75a1-e3db-e511-9416-b8ca3a5db7a1&cmi_mnuMain_child_child=c304d5c0-a7d9-e511-9416-b8ca3a5db7a1)
    * Select **ASP.NET and web development**
    * Under the **Individual Components** tab and under the **Code tools** section, check **Git for Windows** and **GitHub extension for Visual Studio**
2. Install [SQL Server 2017 Express Edition](https://www.microsoft.com/en-us/sql-server/sql-server-editions-express)

### Cloning and Opening the Solution
1. Open Visual Studio
2. In the right pane select the **Team Explorer** tab
3. Under the **GitHub** section sign into your GitHub account
4. Clone the repository
5. Double-click the repository
6. Under the **Solutions** section, double-click the Project solution

## Developing
### Running migrations (Creating the database)
1. Open PowerShell in the project directory
2. Run `dotnet ef database update`
    * This will create the local database if it's not yet created

### Starting the development server
1. Open PowserShell in the project directory
2. Run `dotnet watch run`
3. The project will run at http://localhost:5000 and https://localhost:5001. It will automatically redirect to the HTTPS server.

### Managing JS dependencies
JS dependencies are managed via [LibMan](https://docs.microsoft.com/en-us/aspnet/core/client-side/libman/libman-cli)

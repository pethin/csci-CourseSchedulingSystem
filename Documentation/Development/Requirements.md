# Requirements

This project can be run on any platform.

1. [.NET Core SDK 2.2](https://dotnet.microsoft.com/download)
2. [SQL Server 2017 Express Edition](https://www.microsoft.com/en-us/sql-server/sql-server-editions-express)

**NOTE:** For Linux, SQL Server Express can be installed as a package or run in a Docker container. For Mac, SQL Server
Express can only be run as a Docker container. In both cases, the ConnectionString in the appsettings.json must be
configured accordingly.

## Tooling
### Windows and Mac
If using Windows or Mac, [Visual Studio 2017 Enterprise](https://visualstudio.microsoft.com/) is recommended.

1. Get a key from [Azure Education](https://portal.azure.com/#blade/Microsoft_Azure_Education/EducationMenuBlade/software)
    * Sign in with your `@winthrop.edu` credentials
2. Install Visual Studio
    * Select **ASP.NET and web development**
    * Under the **Individual Components** tab and under the **Code tools** section, check **Git for Windows** and **GitHub extension for Visual Studio**

### Linux
1. Install the [.NET Core 2.2 SDK](https://dotnet.microsoft.com/download)
2. Install [JetBrains Rider](https://www.jetbrains.com/rider/)

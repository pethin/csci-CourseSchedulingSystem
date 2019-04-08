# Authentication

The system is designed to authenticate against a ADFS/WS-Federation server. The system uses
[ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-2.2&tabs=visual-studio)
to provide user whitelisting and user groups. 

## Log-in Process
1. The user clicks the Sign In button.
2. The user gets redirected to ADFS.
3. The user enters their Winthrop credentials.
4. The user gets redirected to the ExternalLogin page.
5. The username is captured from the claims.
    1. If the user exists and a corresponding external login exists for the user
        * Sign them in
    2. If the user exists with a password, but the external login does not exist
        * Make the user confirm their local password, then associate the login provider
        * Sign them in
    3. If the user exists without a password, but the external login does not exist
        * Associate the external login provider
        * Sign them in.
    4. If the user does not exist
        * Create a password-less locked out user

# Authorization

Users that are locked-out can not access the system. By default there are no user groups. Therefore,
**all users have all permissions**.

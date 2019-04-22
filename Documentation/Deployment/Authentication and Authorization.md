# Authentication

The system is designed to authenticate against a ADFS/WS-Federation server. The system uses
[ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-2.2&tabs=visual-studio)
to provide user whitelisting and user groups. 

A guide on configuring WS-Federation can be found here: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/ws-federation?view=aspnetcore-2.2

## Reply URLs
The following reply URLs must be registered in the ADFS/WS-Federation server.

1. <ROOT_PATH>/Identity/AccountLogin
2. <ROOT_PATH>/signin-wsfed

## App Settings
The following settings must be configured in the app settings.

```json
{
  "WsFederation": {
    "MetadataAddress": "<REPLACE_ME>",
    "Wtrealm": "<REPLACE_ME>"
  }
}
```

Directions to find the setting values can be found in the
[ASP.NET Core Identity guide on configuring WS-Federation](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/ws-federation?view=aspnetcore-2.2).

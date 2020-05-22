# UZI Card Authentication Server
> 💳 dotnet core implementation for handling UZI Card Client Certificate authentication.

## Usage

1. Change JWT secret as described in `AuthenticationServer/launchSettings.json`
2. Start server by running following commands in root of repo:
    ```bash
    dotnet build Authentication-Server && dotnet run --project Authentication-Server 
    ```

3. Open client page `Authorization-Client/client_demo.html`


## Todo
- [ ] Check revocation with hosting ISS Server @ windows
- [ ] [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows) implementation 
- [ ] ~~Client request JWT key generation~~ -> optional, but unnecessary

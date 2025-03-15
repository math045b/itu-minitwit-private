using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components;
using Web.Services;

namespace Web.Components.Pages;

public class LoginBase : ComponentBase
{
    [Inject] protected NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private UserState UserState { get; set; } = null!;
    [SupplyParameterFromForm(FormName = "SignIn")]
    protected UserModel UserModel { get; set; } = new UserModel();
    protected string? ErrorMessage { get; set; }

    protected void HandleValidSubmit()
    {
        if (!UserModel.IsValid())
        {
            ErrorMessage = "Please fill out all fields";
            return;
        }
        
        if (UserModel.Username == "admin" && UserModel.Password == "password")
        {
            UserState.SetUser(UserModel.Username);
            NavigationManager.NavigateTo("/");
        }
        else
        {
            ErrorMessage = "Invalid username or password.";
        }
    }
}

public class UserModel
{
    public string? Username { get; set; }
    public string? Password { get; set; }

    public bool IsValid()
    { 
        return !(string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(Password));
    }
}


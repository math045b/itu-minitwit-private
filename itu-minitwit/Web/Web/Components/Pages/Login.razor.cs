using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components;
using Web.Services;
using Web.Services.DTO_s;

namespace Web.Components.Pages;

public class LoginBase : ComponentBase
{
    [Inject] protected NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private UserState UserState { get; set; } = null!;
    [Inject] private IUserService UserService { get; set; } = null!;
    [SupplyParameterFromForm(FormName = "SignIn")]
    protected UserModel UserModel { get; set; } = new UserModel();
    protected string? ErrorMessage { get; set; }

    protected async Task HandleValidSubmit()
    {
        if (!UserModel.IsValid())
        {
            ErrorMessage = "Please fill out all fields";
            return;
        }
        
        if (await UserService.Login(new LoginUserDTO(UserModel.Username!,UserModel.Password!)))
        {
            UserState.SetUser(UserModel.Username!);
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


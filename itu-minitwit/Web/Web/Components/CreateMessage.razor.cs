using Microsoft.AspNetCore.Components;
using Web.Services;
using Web.Services.DTO_s;

namespace Web.Components;

public class CreateMessageBase : ComponentBase
{
    [Inject] protected UserState UserState { get; set; } = default!;
    [Inject] protected IMessageService MessageService { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    
    
    [Parameter] public required string Username { get; set; } // Parent sets this
    protected string Message { get; set; } = "";
    protected string StatusMessage { get; set; } = "";
    
    protected void OnInputHandler(ChangeEventArgs e)
    {
        if (e.Value != null) Message = e.Value.ToString() ?? "";
    }

    protected async Task PostMessage()
    {
        if (string.IsNullOrWhiteSpace(Message))
        {
            StatusMessage = "Message cannot be empty.";
            Console.WriteLine("Message cannot be empty.");
            return;
        }

        var newMessage = new CreateMessageDto
        {
            Username = UserState.Username!,
            Content = Message
        };

        try
        {
            await MessageService.CreateMessage(newMessage, newMessage.Username);
            
            //Console.WriteLine("Message sent successfully!"); //for debugging
            
            Message = ""; // Clear input field after sending
            await Task.Delay(300);
            Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine($"HTTP Error: {httpEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

}
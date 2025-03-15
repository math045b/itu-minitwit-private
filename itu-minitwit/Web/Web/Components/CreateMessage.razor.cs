using Microsoft.AspNetCore.Components;
using Web.Services;
using Web.Services.DTO_s;

namespace Web.Components;

public class CreateMessageBase : ComponentBase
{
    [Inject] protected IMessageService MessageService { get; set; }
    [Inject] protected NavigationManager Navigation { get; set; }
    
    protected async Task CreateMessage(string message)
    {
        var newMessage = new CreateMessageDto
        {
            Content = message
        };
        
        var createdMessage = await MessageService.CreateMessage(newMessage);
        Navigation.NavigateTo($"/message/{createdMessage.Text}");
    }
}
using Microsoft.AspNetCore.Components;
using Web.Services;
using Web.Services.DTO_s;

namespace Web.Components.Pages;

public class PublicTimelineBase : ComponentBase
{
    public IEnumerable<DisplayMessageDto>? Messages { get; set; }
    [Inject] private IMessageService MessageService { get; set;  } = null!;

    protected override async Task OnInitializedAsync()
    { 
        Messages = await MessageService.GetMessages();
    }

}
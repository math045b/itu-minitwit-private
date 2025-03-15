using Microsoft.AspNetCore.Components;
using Web.Services;
using Web.Services.DTO_s;

namespace Web.Components;

public class MessageBase : ComponentBase
{
    [Parameter]
    public DisplayMessageDto MessageDto { get; set; } = new DisplayMessageDto
    {
        Text =
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras cursus justo ante, et sollicitudin arcu mollis sit amet. Sed luctus tempor nisi et dignissim. Etia",
        Username = "Mr. Test",
        PubDate = (int)new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()
    };

    [Inject] protected UserState Userstate { get; set; } = null!;

    [Inject] protected IFollowService FollowService { get; set; } = null!;

    [Inject] private NavigationManager Navigation { get; set; } = null!;

    protected bool DoseLoggedInUserFollowUser()
    {
        return FollowService.DoesFollow(Userstate.Username!, MessageDto.Username).Result;
    }

    protected async Task Follow()
    {
        await FollowService.Follow(Userstate.Username!, MessageDto.Username);
        Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
    }

    protected async Task Unfollow()
    {
        await FollowService.UnFollow(Userstate.Username!, MessageDto.Username);
        Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
    }
}

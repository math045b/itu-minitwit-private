using Microsoft.AspNetCore.Components;

namespace Web.Components.Pages;

public partial class PrivateTimeline : ComponentBase
{
    [Parameter] public required string Author { get; set; }
}
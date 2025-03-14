using Microsoft.AspNetCore.Components;

namespace Web.Components.Pages;

public partial class PrivateTimeline : ComponentBase
{
    [Parameter] public string Author { get; set; }
}
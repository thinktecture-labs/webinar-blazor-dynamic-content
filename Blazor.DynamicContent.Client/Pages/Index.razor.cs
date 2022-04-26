using Blazor.DynamicContent.Client.Services;
using Microsoft.AspNetCore.Components;

namespace Blazor.DynamicContent.Client.Pages
{
    public partial class Index
    {
        [Inject] public NavigationManager _navigationManager { get; set; } = default!;
        [Inject] public DynamicControlDataService _dynamicDataService { get; set; } = default!;

        private List<IDictionary<string, object>> _items = new List<IDictionary<string, object>>();


        private static RenderFragment _title => __builder =>
        {
            __builder.OpenElement(0, "p");
            __builder.AddContent(1, "Hello World");
            __builder.CloseComponent();
        };

        protected override void OnInitialized()
        {
            _items = _dynamicDataService.GetDynamicData();
            base.OnInitialized();
        }

        private void NavigateTo(string url)
        {
            _navigationManager.NavigateTo(url);
        }
    }
}
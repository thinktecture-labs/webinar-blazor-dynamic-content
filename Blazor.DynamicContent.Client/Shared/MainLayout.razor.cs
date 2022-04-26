using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Blazor.DynamicContent.Client.Shared
{
    public partial class MainLayout
    {
        private MudTheme _theme = new MudTheme
        {
            Palette = new Palette
            {
                AppbarText = "#ff584f",
                AppbarBackground = "#ffffff",
                Primary = "#3d6fb4",
                Secondary =  "#ff584f",
            }
        };

        private bool _isDarkMode;
        private bool _drawerOpen;

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        public static RenderFragment _version => __builder =>
        {
            __builder.OpenElement(0, "p");
            __builder.AddAttribute(1, "style", "position: fixed;bottom: 0.25rem;right: 0.25rem;");
            __builder.AddContent(2, typeof(Program).Assembly.GetName().Version);
            __builder.CloseElement();
        };
    }
}
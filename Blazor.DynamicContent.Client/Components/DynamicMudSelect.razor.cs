using Blazor.DynamicContent.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.DynamicContent.Client.Components
{
    public partial class DynamicMudSelect<T>
    {
        [Parameter] public T Value { get; set; } = default!;
        [Parameter] public string Label { get; set; } = string.Empty;
        [Parameter] public EventCallback<T> ValueChanged { get; set; }
        [Parameter] public List<Item> Items { get; set; } = new List<Item>();

        private void OnSelectionChanged(T value)
        {
            Value = value;
            ValueChanged.InvokeAsync(Value);
        }
    }
}
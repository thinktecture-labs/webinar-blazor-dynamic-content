using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.DynamicContent.Client.Components
{
    public partial class DynamicMudTextField
    {
        [Parameter] public string Label { get; set; } = string.Empty;
        [Parameter] public string Placeholder { get; set; } = string.Empty;
        [Parameter] public string Value { get; set; } = string.Empty;
        [Parameter] public EventCallback<string> ValueChanged { get; set; }

        private string _currentValue = string.Empty;
        protected override void OnInitialized()
        {
            _currentValue = Value;
            base.OnInitialized();
        }

        private void OnValueChanged(string value)
        {
            _currentValue = value;
            ValueChanged.InvokeAsync(_currentValue);
        }
    }
}
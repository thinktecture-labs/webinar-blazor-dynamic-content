using Blazor.DynamicContent.Client.Components;
using Blazor.DynamicContent.Client.Models;
using Blazor.DynamicContent.Client.Services;
using Blazor.DynamicContent.Client.Utils;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Dynamic;
using System.Text.Json;

namespace Blazor.DynamicContent.Client.Pages
{
    public partial class DynamicContentWithComponent
    {
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private ISnackbar _snackbar { get; set; } = default!;
        [Inject] private DynamicControlDataService _dynamicControlDataService { get; set; } = default!;

        private Section[] _sections = Array.Empty<Section>();
        private ExpandoObject _model = new ExpandoObject();
        private IDictionary<string, object> _accessor = new Dictionary<string, object>();

        protected async override Task OnInitializedAsync()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new SystemObjectCompatibleConverter());

            _accessor = _model!;
            _sections = await _dynamicControlDataService.LoadFormDataForDynamicComponent() ?? Array.Empty<Section>();
            foreach (var section in _sections)
            {
                section.Components.ForEach(component =>
                {
                    AddDynamicComponentProperties(component);
                });
            }
        }


        private void AddDynamicComponentProperties(DynamicComponentModel component)
        {

            switch (component.ComponentType)
            {
                case "textbox":
                    component.DynamicComponentType = typeof(MudTextField<string>);
                    component.Parameters.Add(nameof(MudTextField<string>.ValueChanged), EventCallback.Factory.Create(this, EventUtils.AsNonRenderingEventHandler((string value) =>
                    {
                        _accessor[component.FieldId] = value;
                    })));
                    _accessor[component.FieldId] = component.Parameters.TryGetValue(nameof(MudTextField<string>.Value), out object? textFieldValue)
                        ? textFieldValue
                        : string.Empty;
                    break;
                case "date":
                    component.DynamicComponentType = typeof(MudDatePicker);
                    component.Parameters.Add(nameof(MudDatePicker.DateChanged), EventCallback.Factory.Create(this, (DateTime? value) =>
                    {
                        _accessor[component.FieldId] = value ?? default(DateTime?)!;
                    }));
                    if (component.Parameters.ContainsKey(nameof(MudDatePicker.Date))
                        && DateTime.TryParse(component.Parameters[nameof(MudDatePicker.Date)].ToString(), out var date))
                    {
                        _accessor[component.FieldId] = date;
                    }
                    else
                    {
                        _accessor[component.FieldId] = default(DateTime?)!;
                    }
                    break;
                case "dropdown":
                    component.DynamicComponentType = typeof(DynamicMudSelect<string>);
                    component.Parameters.Add(nameof(DynamicMudSelect<string>.ValueChanged), new EventCallbackFactory().Create(this, (string value) =>
                    {
                        _accessor[component.FieldId] = value;
                    }));
                    if (component.Parameters.ContainsKey(nameof(DynamicMudSelect<string>.Items)))
                    {
                        var element = (JsonElement)component.Parameters[nameof(DynamicMudSelect<string>.Items)];
                        component.Parameters[nameof(DynamicMudSelect<string>.Items)] = element.ConvertToObject<List<Item>>() ?? new List<Item>();
                    }
                    _accessor[component.FieldId] = component.Parameters.TryGetValue(nameof(DynamicMudSelect<string>.Value), out object? selectValue)
                        ? selectValue
                        : default!;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        private void Save()
        {
            _dynamicControlDataService.AddDynamicValue(_model);
            _snackbar.Add("Form saved successfull with DynamicComponent.", Severity.Success);
            _navigationManager.NavigateTo("/");
        }
    }
}
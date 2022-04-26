using Blazor.DynamicContent.Client.Models;
using Blazor.DynamicContent.Client.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Dynamic;
using System.Text.Json;

namespace Blazor.DynamicContent.Client.Pages
{
    public partial class DynamicContentWithRenderFragment
    {
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] public DynamicControlDataService _dynamicControlDataService { get; set; } = default!;
        [Inject] public DynamicFormGeneratorService _dynamicFormGeneratorService { get; set; } = default!;
        [Inject] public ISnackbar _snackbar { get; set; } = default!;

        private Section[] _sections = Array.Empty<Section>();
        private ExpandoObject _model = new();

        protected override async Task OnInitializedAsync()
        {
            _sections = await _dynamicControlDataService.LoadFormDataForRenderFragment();
            var data = await _dynamicControlDataService.LoadFormDataValues();
            var dict = (IDictionary<string, object>)_model!;
            if (data != null)
            {
                foreach (var key in data)
                {
                    dict.Add(key);
                }
            }
            await base.OnInitializedAsync();
        }

        private RenderFragment CreateForm()
        {
            return _dynamicFormGeneratorService.RenderControls(_sections, _model);
        }

        private void SubmitData()
        {
            _dynamicControlDataService.AddDynamicValue(_model);
            _snackbar.Add("Form saved successfull with RenderFragment.", Severity.Success);
            _navigationManager.NavigateTo("/");
        }

    }
}
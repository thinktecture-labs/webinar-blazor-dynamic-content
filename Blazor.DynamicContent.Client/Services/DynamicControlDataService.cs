using Blazor.DynamicContent.Client.Models;
using Blazor.DynamicContent.Client.Utils;
using System.Dynamic;
using System.Net.Http.Json;
using System.Text.Json;

namespace Blazor.DynamicContent.Client.Services
{
    public class DynamicControlDataService
    {
        private readonly HttpClient _httpClient;
        private static List<IDictionary<string, object>> _dynamicData = new();

        public DynamicControlDataService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<Section[]> LoadFormDataForRenderFragment()
        {
            var result = await _httpClient.GetFromJsonAsync<Section[]>("sample-data/sample-data.json");
            return result ?? Array.Empty<Section>();
        }

        public async Task<Section[]> LoadFormDataForDynamicComponent()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new SystemObjectCompatibleConverter());
            var result = await _httpClient.GetFromJsonAsync<Section[]>("sample-data/sample-data-dynamic.json", options);
            return result ?? Array.Empty<Section>();
        }

        public async Task<Dictionary<string, object>> LoadFormDataValues()
        {
            var result = await _httpClient.GetFromJsonAsync<Dictionary<string, object>>($"sample-data/sample-data-values.json")!;
            return result ?? new Dictionary<string, object>();
        }

        public List<IDictionary<string, object>> GetDynamicData()
        {
            return _dynamicData;
        }

        public void AddDynamicValue(ExpandoObject data)
        {
            var newData = data?.ToDictionary(d => d.Key, d => d.Value) ?? new Dictionary<string, object?>();
            _dynamicData.Add(newData!);
        }
    }
}
using System.Text.Json.Serialization;

namespace Blazor.DynamicContent.Client.Models
{
    public class Section
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("sectionName")]
        public string SectionName { get; set; } = string.Empty;

        [JsonPropertyName("components")]
        public List<DynamicComponentModel> Components { get; set; } = new();
    }    
}

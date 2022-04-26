using System.Text.Json.Serialization;

namespace Blazor.DynamicContent.Client.Models
{
    public class DynamicComponentModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty; 

        [JsonPropertyName("fieldId")]
        public string FieldId { get; set; } = string.Empty;

        [JsonPropertyName("componentType")]
        public string ComponentType { get; set; } = string.Empty;        

        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;

        [JsonPropertyName("items")]
        public List<Item> Items { get; set; } = new List<Item>();

        [JsonPropertyName("parameters")]
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        [JsonIgnore]
        public Type DynamicComponentType { get; set; } = default!;
    }

    public class Item
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
    }
}

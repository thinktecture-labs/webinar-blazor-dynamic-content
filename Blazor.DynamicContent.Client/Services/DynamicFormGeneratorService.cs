using System.Dynamic;
using System.Text.Json;
using Blazor.DynamicContent.Client.Models;
using Blazor.DynamicContent.Client.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazor.DynamicContent.Client.Services
{
    public class DynamicFormGeneratorService
    {
        public RenderFragment RenderControls(Section[] sections, ExpandoObject data) => builder =>
        {
            builder.OpenComponent<MudBlazor.MudExpansionPanels>(0);
            builder.AddAttribute(1, nameof(MudBlazor.MudExpansionPanels.MultiExpansion), true);
            builder.AddAttribute(2, nameof(MudBlazor.MudExpansionPanels.ChildContent), new RenderFragment((panelBuilder =>
            {
                foreach (var section in sections)
                {
                    panelBuilder.OpenRegion(0);
                    panelBuilder.OpenComponent<MudBlazor.MudExpansionPanel>(8);
                    panelBuilder.AddAttribute(1, nameof(MudBlazor.MudExpansionPanel.IsInitiallyExpanded), true);
                    panelBuilder.AddAttribute(2, nameof(MudBlazor.MudExpansionPanel.TitleContent),
                        new RenderFragment(titleBuilder =>
                        {
                            titleBuilder.OpenRegion(0);
                            titleBuilder.OpenComponent<MudBlazor.MudText>(1);
                            titleBuilder.AddAttribute(2, nameof(MudBlazor.MudText.Typo), MudBlazor.Typo.h5);
                            titleBuilder.AddAttribute(3, nameof(MudBlazor.MudText.ChildContent), new RenderFragment(titleContentBuilder =>
                            {
                                titleContentBuilder.OpenElement(4, "span");
                                titleContentBuilder.AddContent(5, section.SectionName);
                                titleContentBuilder.CloseElement();
                            }));
                            titleBuilder.CloseComponent();
                            titleBuilder.CloseRegion();
                        }));
                        panelBuilder.AddAttribute(3, nameof(MudBlazor.MudExpansionPanel.ChildContent),
                        new RenderFragment(contentBuilder =>
                        {
                            foreach (var component in section.Components)
                            {
                                RenderInputComponent(data, component, contentBuilder);
                            }

                        }));
                    panelBuilder.CloseComponent();
                    panelBuilder.CloseRegion();
                }
            })));
            builder.CloseComponent();
        };

        private void RenderInputComponent(ExpandoObject data, DynamicComponentModel component,
            RenderTreeBuilder builder)
        {
            builder.OpenRegion(0);
            switch (component.ComponentType)
            {
                case "textbox":
                    builder.OpenComponent(1, typeof(MudBlazor.MudTextField<string>));
                    builder.AddAttribute(2, nameof(MudBlazor.MudTextField<string>.Label), component.Label);
                    BindDataValue<string>(data, component.FieldId, builder);
                    builder.CloseComponent();                    
                    break;

                case "date":
                    builder.OpenComponent(1, typeof(MudBlazor.MudDatePicker));
                    builder.AddAttribute(2, nameof(MudBlazor.MudDatePicker.Label), component.Label);
                    BindDataValue<DateTime?>(data, component.FieldId, builder, nameof(MudBlazor.MudDatePicker.Date), nameof(MudBlazor.MudDatePicker.DateChanged));
                    builder.CloseComponent();
                    break;
                case "dropdown":
                    builder.OpenComponent(1, typeof(MudBlazor.MudSelect<string>));
                    builder.AddAttribute(2, nameof(MudBlazor.MudSelect<string>.Label), component.Label);
                    BindDataValue<string>(data, component.FieldId, builder);
                    builder.AddAttribute(6, nameof(MudBlazor.MudSelect<string>.ChildContent), new RenderFragment(childBuilder =>
                    {
                        foreach (var item in component.Items)
                        {
                            childBuilder.OpenRegion(0);
                            childBuilder.OpenComponent(1, typeof(MudBlazor.MudSelectItem<string>));
                            childBuilder.AddAttribute(2, nameof(MudBlazor.MudSelect<string>.Value), item.Key);
                            childBuilder.AddAttribute(3, nameof(MudBlazor.MudSelect<string>.ChildContent), new RenderFragment(radiobuilder =>
                            {
                                radiobuilder.OpenRegion(0);
                                radiobuilder.AddContent(1, item.Value);
                                radiobuilder.CloseRegion();
                            }));
                            childBuilder.CloseComponent();
                            childBuilder.CloseRegion();
                        }
                    }));
                    builder.CloseComponent();
                    break;
                default:
                    builder.OpenRegion(0);
                    builder.OpenElement(1, "p");
                    builder.AddAttribute(2, "style", "padding: 8px 12px  24px");
                    builder.AddContent(3, $"Control type {component.ComponentType} is to be implemented :-)");
                    builder.CloseElement();
                    builder.CloseRegion();
                    break;
            }
            builder.CloseRegion();
        }

        private void BindDataValue<T>(ExpandoObject data, string key, RenderTreeBuilder builder,
            string valuePropertyName = "Value", string valueChangedPropertyName = "ValueChanged")
        {
            var accessor = (IDictionary<string, object>)data!;
            var value = GetValue<T>(accessor, key);
            accessor[key] = value ?? default(T)!;

            var valueChanged = EventCallback.Factory.Create(this, (T value) =>
            {
                accessor[key] = value ?? default!;
            });

            builder.AddAttribute(4, valuePropertyName, value);
            builder.AddAttribute(5, valueChangedPropertyName, valueChanged);
            builder.SetUpdatesAttributeName(valuePropertyName);
        }

        private T GetValue<T>(IDictionary<string, object> data, string key)
        {            
            if (data.ContainsKey(key))
            {
                if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
                {
                    try
                    {
                        if (data[key] != null)
                        {
                            var dateString = ((JsonElement)data[key]).ConvertToObject<string>();
                            if (DateTime.TryParse(dateString, out var result))
                            {
                                var jsonString = JsonSerializer.Serialize(result);
                                return JsonSerializer.Deserialize<T>(jsonString) ?? default(T)!;
                            }
                        }
                    }
                    catch (InvalidCastException)
                    {
                        Console.WriteLine($"Failed to cast DateTime value for {key} with value {data[key]}");
                    }
                }
                else
                {
                    return data[key] is JsonElement
                        ? ((JsonElement)data[key]).ConvertToObject<T>()
                        : (T)data[key];
                }
            }

            return default(T)!;
        }
    }
}
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Json;

public static class ConfigurationExtensions
{
    public static readonly JsonSerializerOptions JsonSettings = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    
    public static PropertyBuilder<T> StoreAsJson<T>(this PropertyBuilder<T> propertyBuilder) where T : class
    {
        var conversion = propertyBuilder.HasConversion(item => item == null ? null : JsonSerializer.Serialize(item, JsonSettings),
                                                        json => json == null ? null : JsonSerializer.Deserialize<T>(json, JsonSettings));
        return conversion;
    }
}
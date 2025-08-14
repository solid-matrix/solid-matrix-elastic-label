using System.Text.Json.Serialization;

namespace SolidMatrix.ElasticLabels;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Alignment
{
    Center,
    Left,
    Right,
}

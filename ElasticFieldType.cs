using System.Text.Json.Serialization;

namespace SolidMatrix.ElasticLabels;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ElasticFieldType
{
    Attribute,
    Text,
    Image,
    Rect,
    TextSlot,
    QrCodeSlot,
}

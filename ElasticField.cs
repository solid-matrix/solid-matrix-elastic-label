using System.Buffers.Text;
using System.Text.Json.Serialization;

namespace SolidMatrix.ElasticLabels;

public class ElasticField
{
    [JsonInclude]
    public ElasticFieldType Type { get; set; }

    [JsonInclude]
    public required string Entry { get; set; }

    [JsonInclude]
    public required string Value { get; set; }

    [JsonInclude]
    public uint FontSize { get; set; }

    [JsonInclude]
    public Rect Rect { get; set; }

    public Alignment Alignment { get; set; }

    [JsonConstructor]
    private ElasticField() { }

    public static ElasticField NewAttribute(string entry, string value)
    {
        return new ElasticField
        {
            Type = ElasticFieldType.Attribute,
            Entry = entry,
            Value = value,
        };
    }

    public static ElasticField NewText(Rect rect, string value, uint fontSize = 0, Alignment align = Alignment.Left)
    {
        return new ElasticField
        {
            Type = ElasticFieldType.Text,
            Entry = "txt",
            Value = value,
            Rect = rect,
            FontSize = fontSize,
            Alignment = align
        };
    }

    public static ElasticField NewImageFromFile(Rect rect, string path)
    {
        var bytes = File.ReadAllBytes(path);
        var base64url = Base64Url.EncodeToString(bytes);

        return new ElasticField
        {
            Type = ElasticFieldType.Image,
            Entry = "img",
            Value = base64url,
            Rect = rect,
        };
    }

    public static ElasticField NewRect(Rect rect)
    {
        return new ElasticField
        {
            Type = ElasticFieldType.Rect,
            Rect = rect,
            Entry = "rec",
            Value = "",
        };
    }

    public static ElasticField NewTextSlot(Rect rect, string entry, string previewValue, uint fontSize = 0, Alignment align = Alignment.Left)
    {
        return new ElasticField
        {
            Type = ElasticFieldType.TextSlot,
            Entry = entry,
            Value = previewValue,
            Rect = rect,
            FontSize = fontSize,
            Alignment = align
        };
    }

    public static ElasticField NewQrCodeSlot(Rect rect, string entry, string previewValue)
    {
        return new ElasticField
        {
            Type = ElasticFieldType.QrCodeSlot,
            Entry = entry,
            Value = previewValue,
            Rect = rect,
        };
    }
}

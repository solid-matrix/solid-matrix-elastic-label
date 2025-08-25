using QRCoder;
using System.Buffers.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SolidMatrix.ElasticLabels;

public class ElasticLabel
{
    [JsonInclude]
    public Guid Guid { get; internal set; }

    [JsonInclude]
    public int Width { get; internal set; }

    [JsonInclude]
    public int Height { get; internal set; }

    [JsonInclude]
    public float WidthCm { get; internal set; }

    [JsonInclude]
    public float HeightCm { get; internal set; }

    [JsonInclude]
    public List<ElasticField> Fields { get; internal set; }

    [JsonConstructor]
    internal ElasticLabel()
    {
        Fields = [];
    }
    public ElasticLabel(int width, int height, float res_cm)
    {
        Guid = Guid.NewGuid();
        Width = width;
        Height = height;
        WidthCm = width / res_cm;
        HeightCm = height / res_cm;
        Fields = [];
    }

    public ElasticLabel(int width, int height, float width_cm, float height_cm)
    {
        Guid = Guid.NewGuid();
        Width = width;
        Height = height;
        WidthCm = width_cm;
        HeightCm = height_cm;
        Fields = [];
    }

    public static byte[] Encode(ElasticLabel label)
    {
        return JsonSerializer.SerializeToUtf8Bytes(label);
    }

    public static void Save(ElasticLabel label, string path)
    {
        File.WriteAllBytes(path, Encode(label));
    }

    public static ElasticLabel Decode(string path)
    {
        return Decode(File.ReadAllBytes(path));
    }

    public static ElasticLabel Decode(ReadOnlySpan<byte> bytes)
    {
        return JsonSerializer.Deserialize<ElasticLabel>(bytes) ?? throw new Exception("failed to decode");
    }

    internal static StringAlignment AlignmentSwitch(Alignment alignment)
    {
        return alignment switch
        {
            Alignment.Center => StringAlignment.Center,
            Alignment.Left => StringAlignment.Near,
            Alignment.Right => StringAlignment.Far,
            _ => throw new Exception("unsupported alignment"),
        };
    }
    internal static void RenderText(Graphics g, ElasticField field)
    {
        var fontSize = field.FontSize == 0 ? field.Rect.Height / 2.0f : field.FontSize;
        //var fontStyle = fontSize >= 30 ? FontStyle.Bold : FontStyle.Regular;
        using var font = new Font(FontFamily.GenericSerif, fontSize, FontStyle.Bold);

        using var format = StringFormat.GenericTypographic;
        format.LineAlignment = StringAlignment.Center;
        format.Alignment = AlignmentSwitch(field.Alignment);
        format.FormatFlags = StringFormatFlags.NoWrap;

        g.DrawString(field.Value, font, Brushes.Black, field.Rect, format);
    }

    internal static void RenderImage(Graphics g, ElasticField field)
    {
        var bytes = Base64Url.DecodeFromChars(field.Value);
        using var ms = new MemoryStream(bytes);
        using var bmp = new Bitmap(ms);

        int width;
        int height;
        double rx = (double)bmp.Width / field.Rect.Width;
        double ry = (double)bmp.Height / field.Rect.Height;

        if (rx >= ry)
        {
            width = field.Rect.Width;
            height = (int)Math.Round((double)field.Rect.Width * bmp.Height / bmp.Width);
        }
        else
        {
            width = (int)Math.Round((double)field.Rect.Height * bmp.Width / bmp.Height);
            height = field.Rect.Height;
        }

        int x = field.Rect.X + field.Rect.Width / 2 - width / 2;
        int y = field.Rect.Y + field.Rect.Height / 2 - height / 2;

        g.DrawImage(bmp, new Rectangle(x, y, width, height), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
    }

    internal static void RenderRect(Graphics g, ElasticField field)
    {
        var width = int.Parse(field.Value);
        using var pen = new Pen(Color.Black, width);
        g.DrawRectangle(pen, field.Rect);
    }

    internal static void RenderTextSlotPreview(Graphics g, ElasticField field)
    {
        var fontSize = field.FontSize == 0 ? field.Rect.Height / 2.0f : field.FontSize;
        using var font = new Font(FontFamily.GenericSerif, fontSize, FontStyle.Bold);

        using var format = StringFormat.GenericTypographic;
        format.LineAlignment = StringAlignment.Center;
        format.Alignment = AlignmentSwitch(field.Alignment);
        format.FormatFlags = StringFormatFlags.NoWrap;

        g.FillRectangle(Brushes.LightGray, field.Rect);
        g.DrawString(field.Value, font, Brushes.Black, field.Rect, format);
    }

    internal static void RenderTextSlot(Graphics g, ElasticField field, string? value)
    {
        if (value == null) return;

        var fontSize = field.FontSize == 0 ? field.Rect.Height / 2.0f : field.FontSize;
        using var font = new Font(FontFamily.GenericSerif, fontSize, FontStyle.Bold);

        using var format = StringFormat.GenericTypographic;
        format.LineAlignment = StringAlignment.Center;
        format.Alignment = AlignmentSwitch(field.Alignment);
        format.FormatFlags = StringFormatFlags.NoWrap;

        g.DrawString(value, font, Brushes.Black, field.Rect, format);
    }

    internal static void RenderQrCodeSlotPreview(Graphics g, ElasticField field)
    {
        using var qrCodeGenerator = new QRCodeGenerator();
        using var qrCodeData = qrCodeGenerator.CreateQrCode(field.Value, QRCodeGenerator.ECCLevel.L);
        using var qrCode = new QRCode(qrCodeData);
        using var bmp = qrCode.GetGraphic(1, Color.Black, Color.White, false);

        var size = Math.Min(field.Rect.Width, field.Rect.Height);
        int x = field.Rect.X + field.Rect.Width / 2 - size / 2;
        int y = field.Rect.Y + field.Rect.Height / 2 - size / 2;

        g.DrawImage(bmp, new Rectangle(x, y, size, size), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
    }

    internal static void RenderQrCodeSlot(Graphics g, ElasticField field, string? value)
    {
        if (value == null) return;

        using var qrCodeGenerator = new QRCodeGenerator();
        using var qrCodeData = qrCodeGenerator.CreateQrCode(value, QRCodeGenerator.ECCLevel.L);
        using var qrCode = new QRCode(qrCodeData);
        using var bmp = qrCode.GetGraphic(1, Color.Black, Color.White, false);

        var size = Math.Min(field.Rect.Width, field.Rect.Height);
        int x = field.Rect.X + field.Rect.Width / 2 - size / 2;
        int y = field.Rect.Y + field.Rect.Height / 2 - size / 2;

        g.DrawImage(bmp, new Rectangle(x, y, size, size), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
    }

    internal static void RenderPreviewOnGraphics(ElasticLabel label, Graphics g, int width, int height)
    {
        g.ScaleTransform((float)width / label.Width, (float)height / label.Height);
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.Clear(Color.White);

        foreach (var field in label.Fields.Where(field => field.Type == ElasticFieldType.Text)) RenderText(g, field);
        foreach (var field in label.Fields.Where(field => field.Type == ElasticFieldType.Image)) RenderImage(g, field);

        foreach (var field in label.Fields.Where(field => field.Type == ElasticFieldType.TextSlot)) RenderTextSlotPreview(g, field);
        foreach (var field in label.Fields.Where(field => field.Type == ElasticFieldType.QrCodeSlot)) RenderQrCodeSlotPreview(g, field);

        foreach (var field in label.Fields.Where(field => field.Type == ElasticFieldType.Rect)) RenderRect(g, field);
    }

    internal static void RenderOnGraphics(ElasticLabel label, Graphics g, int width, int height, IDictionary<string, string> slotValues)
    {
        g.ScaleTransform((float)width / label.Width, (float)height / label.Height);
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.Clear(Color.White);

        foreach (var field in label.Fields.Where(field => field.Type == ElasticFieldType.Text)) RenderText(g, field);
        foreach (var field in label.Fields.Where(field => field.Type == ElasticFieldType.Image)) RenderImage(g, field);

        foreach (var field in label.Fields.Where(field => field.Type == ElasticFieldType.TextSlot))
        {
            if (slotValues.TryGetValue(field.Entry, out var value))
            {
                RenderTextSlot(g, field, value);
            }
        }

        foreach (var field in label.Fields.Where(field => field.Type == ElasticFieldType.QrCodeSlot))
        {
            if (slotValues.TryGetValue(field.Entry, out var value))
            {
                RenderQrCodeSlot(g, field, value);
            }
        }

        foreach (var field in label.Fields.Where(field => field.Type == ElasticFieldType.Rect)) RenderRect(g, field);
    }
}

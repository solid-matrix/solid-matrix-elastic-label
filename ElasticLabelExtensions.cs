using System.Drawing;
using System.Drawing.Imaging;

namespace SolidMatrix.ElasticLabels;

public static class ElasticLabelExtensions
{
    public static Image RenderPreview(this ElasticLabel label)
    {
        var dst = new Bitmap(label.Width, label.Height, PixelFormat.Format24bppRgb);

        using var g = Graphics.FromImage(dst);

        RenderPreviewOnGraphics(label, g, label.Width, label.Height);

        dst.SetResolution(label.Width / label.WidthCm * 2.54f, label.Height / label.HeightCm * 2.54f);

        return dst;
    }

    public static Image Render(this ElasticLabel label, IDictionary<string, string> slotValues)
    {
        var dst = new Bitmap(label.Width, label.Height, PixelFormat.Format24bppRgb);

        using var g = Graphics.FromImage(dst);

        RenderOnGraphics(label, g, label.Width, label.Height, slotValues);

        dst.SetResolution(label.Width / label.WidthCm * 2.54f, label.Height / label.HeightCm * 2.54f);

        return dst;
    }

    public static void RenderPreviewOnGraphics(this ElasticLabel label, Graphics g, int width, int height)
    {
        ElasticLabel.RenderPreviewOnGraphics(label, g, width, height);
    }

    public static void RenderOnGraphics(this ElasticLabel label, Graphics g, int width, int height, IDictionary<string, string> slotValues)
    {
        ElasticLabel.RenderOnGraphics(label, g, width, height, slotValues);
    }

    public static byte[] Encode(this ElasticLabel label)
    {
        return ElasticLabel.Encode(label);
    }

    public static void Save(this ElasticLabel label, string path)
    {
        ElasticLabel.Save(label, path);
    }

    public static string? GetAttributeValue(this ElasticLabel label, string entry)
    {
        return label.Fields.Where(field => field.Type == ElasticFieldType.Attribute && field.Entry == entry).FirstOrDefault()?.Value;
    }

    public static string[] GetAttributeEntries(this ElasticLabel label)
    {
        return [.. label.Fields.Where(field => field.Type == ElasticFieldType.Attribute).Select(field => field.Entry)];
    }

    public static string[] GetTextSlotEntries(this ElasticLabel label)
    {
        return [.. label.Fields.Where(field => field.Type == ElasticFieldType.TextSlot).Select(field => field.Entry)];
    }

    public static string[] GetQrCodeSlotEntries(this ElasticLabel label)
    {
        return [.. label.Fields.Where(field => field.Type == ElasticFieldType.QrCodeSlot).Select(field => field.Entry)];
    }

    public static string[] GetSlotEntries(this ElasticLabel label)
    {
        return [.. label.Fields.Where(field => field.Type == ElasticFieldType.TextSlot || field.Type == ElasticFieldType.QrCodeSlot).Select(field => field.Entry)];
    }
}

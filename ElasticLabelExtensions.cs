using System.Drawing;

namespace SolidMatrix.ElasticLabels;

public static class ElasticLabelExtensions
{
    public static Image Render(this ElasticLabel label, Dictionary<string, string> slotValues)
    {
        return ElasticLabel.Render(label, slotValues);
    }

    public static Image RenderPreview(this ElasticLabel label)
    {
        return ElasticLabel.RenderPreview(label);
    }

    public static byte[] Encode(this ElasticLabel label)
    {
        return ElasticLabel.Encode(label);
    }

    public static void Save(this ElasticLabel label, string path)
    {
        ElasticLabel.Save(label, path);
    }
}

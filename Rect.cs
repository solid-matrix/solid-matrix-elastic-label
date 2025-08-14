using System.Drawing;

namespace SolidMatrix.ElasticLabels;

public struct Rect(int x, int y, int width, int height)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public int Width { get; set; } = width;
    public int Height { get; set; } = height;

    public static implicit operator RectangleF(Rect rect)
    {
        return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
    }

    public static implicit operator Rectangle(Rect rect)
    {
        return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
    }
}

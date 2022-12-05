namespace grafic_lab4.Figures;

public static class PointExtensions
{
    public static PointF Add(this PointF a, PointF b)
    {
        return new PointF(a.X + b.X, a.Y + b.Y);
    }

    public static PointF Sub(this PointF a, PointF b)
    {
        return new PointF(a.X - b.X, a.Y - b.Y);
    }

    public static PointF Mul(this PointF a, float b)
    {
        return new PointF(a.X * b, a.Y * b);
    }

    public static float Dot(this PointF a, PointF b)
    {
        return a.X * b.X + a.Y * b.Y;
    }

    public static float Cross(this PointF a, PointF b)
    {
        return a.X * b.Y - a.Y * b.X;
    }

    public static PointF Offset(this PointF point, float dx, float dy)
    {
        point.X += dx;
        point.Y += dy;

        return point;
    }

    public static void Swap(this ref PointF left, ref PointF right)
    {
        float temp = left.X;
        left.X = right.X;
        right.X = temp;

        temp = left.Y;
        left.Y = right.Y;
        right.Y = temp;
    }
}

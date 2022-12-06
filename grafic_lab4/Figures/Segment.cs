namespace grafic_lab4.Figures;

public class Segment
{
    public const float BEGIN = 0;
    public const float END = 1;

    public Color? Color { set; get; }

    public PointF A, B;

    public Segment(PointF a, PointF b)
    {
        A = a;
        B = b;
    }

    public void Offset(PointF derection)
    {
        A.X += derection.X;
        B.X += derection.X;

        A.Y += derection.Y;
        B.Y += derection.Y;
    }

    public bool OnLeft(PointF p)
    {
        var ab = B.Sub(A);
        var ap = p.Sub(A);
        return ab.Cross(ap) >= 0;
    }

    public PointF Normal => new PointF(B.Y - A.Y, A.X - B.X);

    public PointF Direction => new PointF(B.X - A.X, B.Y - A.Y);

    public float IntersectionParameter(Segment that)
    {
        var segment = this;
        var edge = that;

        var segmentToEdge = edge.A.Sub(segment.A);
        var segmentDir = segment.Direction;
        var edgeDir = edge.Direction;

        var t = edgeDir.Cross(segmentToEdge) / edgeDir.Cross(segmentDir);

        if (float.IsNaN(t))
        {
            t = 0;
        }

        return t;
    }

    public Segment? Morph(float tA, float tB)
    {
        if (Math2d.AreEqual(tA, tB))
        {
            return null;
        }

        var d = Direction;

        var res = new Segment(A.Add(d.Mul(tA)), A.Add(d.Mul(tB)));
        res.Color = Color;

        return res;
    }

    public PointF Morph(float t)
    {
        return A.Add(Direction.Mul(t));
    }

    public double Lenght
    {
        get
        {
            double dx = A.X - B.X;
            double dy = A.Y - B.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}

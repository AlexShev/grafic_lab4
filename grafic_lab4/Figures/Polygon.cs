namespace grafic_lab4.Figures;

public class Polygon
{
    private List<PointF> points;

    public Color? Color { set; get; }

    public Polygon(IEnumerable<PointF> points)
    {
        this.points = new List<PointF>(points);
    }

    public Segment GetEdge(int index)
    {
        int a = index % points.Count;
        int b = (index + 1) % points.Count;

        return new Segment(points[a], points[b]);
    }

    public PointF GetPoint(int index) => points[index % points.Count];
    public List<PointF> GetPoints() => points;

    public void AddAfter(int index, PointF point)
    {
        points.Insert(index + 1, point);
    }

    public void Offset(PointF derection)
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i] = points[i].Offset(derection.X, derection.Y);
        }
    }

    public int Size => points.Count;

    public Polygon Clone()
    {
        var res = new Polygon(points);
        res.Color = Color;

        return res;
    }

    public Rect GetDimensions()
    {
        float minX = points.MinBy(point => point.X).X;
        float minY = points.MinBy(point => point.Y).Y;

        float maxX = points.MaxBy(point => point.X).X;
        float maxY = points.MaxBy(point => point.Y).Y;

        return new Rect(Math2d.Average(minX, maxX), Math2d.Average(minY, maxY), 
                        maxX - minX, maxY - minY);
    }

    public PointF GetLeftButtonCorner()
    {
        float minX = points.MinBy(point => point.X).X;
        float minY = points.MinBy(point => point.Y).Y;

        return new PointF(minX, minY);
    }

    public PointF GetRightTopCorner()
    {
        float maxX = points.MaxBy(point => point.X).X;
        float maxY = points.MaxBy(point => point.Y).Y;

        return new PointF(maxX, maxY);
    }
}

/*
    public void SplitSegment(int index, float tA, float tB)
    {
        var newPoints = GetEdge(index).Morph(tA, tB);

        if (newPoints != null)
        {
            points.Insert(index + 1, newPoints.A);
            points.Insert(index + 2, newPoints.B);
        }
    }

    public void SplitSegment(int index, float t)
    {
        var newPoints = GetEdge(index).Morph(0, t);

        if (newPoints != null)
        {
            points.Insert(index + 1, newPoints.B);
        }
    }

 */
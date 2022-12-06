using static grafic_lab4.Figures.FigureMover;

namespace grafic_lab4.Figures;

public class PolygonWithSegments : IMovable
{
    public Polygon Polygon;
    public List<Segment> Segments;

    public PolygonWithSegments(Polygon polygon) : this(polygon, new List<Segment>())
    {
    }

    public PolygonWithSegments(Polygon polygon, List<Segment> segments)
    {
        Polygon = polygon;
        Segments = segments;
    }

    public void MoveTo(float leftX, float topY)
    {
        PointF leftTopCorner = new PointF(leftX, topY);
        var derection = leftTopCorner.Sub(Polygon.GetLeftButtonCorner());

        Polygon.Offset(derection);
        Segments.ForEach((segment) => segment.Offset(derection));
    }

    public static PolygonWithSegments Create(Polygon polygon)
    {
        PolygonWithSegments res = new PolygonWithSegments(polygon);

        var begin = polygon.GetPoint(0);
        var color = InvertMeAColour(polygon.Color);

        for (int i = 2; i < polygon.Size - 1; i++)
        {
            var segment = new Segment(begin, polygon.GetPoint(i));
            segment.Color = color;
            res.Segments.Add(segment);
        }

        return res;
    }

    public static Color? InvertMeAColour(Color? ColourToInvert)
    {
        if (ColourToInvert == null) return null;

        return Color.FromArgb((byte)~ColourToInvert?.R, (byte)~ColourToInvert?.G, (byte)~ColourToInvert?.B);
    }
}

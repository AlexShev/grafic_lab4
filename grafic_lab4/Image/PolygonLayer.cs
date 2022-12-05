
using grafic_lab4.Figures;

namespace grafic_lab4.Image;

public class PolygonLayer : ILayer
{
    private PolygonWithSegments polygon;

    public PolygonLayer(PolygonWithSegments polygon)
    {
        this.polygon = polygon;
    }

    public void AddTo(GeometryImage image)
    {
        image.AddUpper(polygon.Polygon);
        image.AddUpper(polygon.Segments);
    }
}


/*

    public void MoveTo(PointF leftTopCorner)
    {
        var derection = polygon.Polygon.GetRightTopCorner().Sub(leftTopCorner);

        polygon.Polygon.Offset(derection);
        polygon.Segments.ForEach((segment) => segment.Offset(derection));
    }
 */
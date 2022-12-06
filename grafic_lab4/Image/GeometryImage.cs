using grafic_lab4.Figures;
using grafic_lab4.CrossInspectors;

namespace grafic_lab4.Image;

public class GeometryImage
{
    public double Hight { get; private set; }
    public double Wight { get; private set; }

    public List<Polygon> VisiblePolygons;
    public List<Segment> VisibleSegments;

    public List<List<Segment>> InvisibleLines;
    public List<Polygon> InvisiblePolygons;

    public Color Background;

    public GeometryImage()
    {
        VisiblePolygons = new List<Polygon>();
        VisibleSegments = new List<Segment>();

        InvisibleLines = new List<List<Segment>>();
        InvisiblePolygons = new List<Polygon>();
    }

    public void AddUpper(Polygon polygon)
    {
        var maxCoordinat = polygon.GetRightTopCorner();
        Hight = Math.Max(Hight, maxCoordinat.Y);
        Wight = Math.Max(Wight, maxCoordinat.X);

        List<Polygon> layerVisiblePolygons = new List<Polygon>();

        for (int i = 0; i < VisiblePolygons.Count; i++)
        {
            if (polygon.GetDimensions().IsIntersected(VisiblePolygons[i].GetDimensions()))
            {
                var visiblePolygon = new WeilerAzerton(VisiblePolygons[i], polygon);

                if (visiblePolygon.IsClip)
                {
                    layerVisiblePolygons.AddRange(visiblePolygon.VisiblePolygons);
                    InvisibleLines.AddRange(visiblePolygon.InvisibleLines);
                }
                else
                {
                    bool isVisible = false;

                    for (int j = 0; j < VisiblePolygons[i].Size; j++)
                    {
                        if (!polygon.IsInside(VisiblePolygons[i].GetPoint(j)))
                        {
                            isVisible = true;
                            break;
                        }
                    }

                    if (isVisible)
                    {
                        layerVisiblePolygons.Add(VisiblePolygons[i]);
                    }
                    else
                    {
                        InvisiblePolygons.Add(VisiblePolygons[i]);
                    }
                }
            }
            else
            {
                // проверить на полигон внутри
                layerVisiblePolygons.Add(VisiblePolygons[i]);
            }
        }

        layerVisiblePolygons.Add(polygon);

        VisiblePolygons = layerVisiblePolygons;

        List<Segment> layerVisibleSegments = new List<Segment>();

        for (int i = 0; i < VisibleSegments.Count; i++)
        {
            var visibleSegment = new CyrusBeckClip(VisibleSegments[i], polygon);

            if (visibleSegment.IsClip)
            {
                var visiblePart1 = visibleSegment.GetFirstClipSegment();
                var visiblePart2 = visibleSegment.GetSecondClipSegment();
                var invisiblePart = visibleSegment.GetSegmentInPolygon();

                if (visiblePart1 != null)
                {
                    layerVisibleSegments.Add(visiblePart1);
                }
                if (visiblePart2 != null)
                {
                    layerVisibleSegments.Add(visiblePart2);
                }
                if (invisiblePart != null)
                {
                    InvisibleLines.Add(new List<Segment>() { invisiblePart });
                }
            }
            else if (visibleSegment.IsOutOfPolygon)
            {
                layerVisibleSegments.Add(VisibleSegments[i]);
            }
            else
            {
                InvisibleLines.Add(new List<Segment>() { VisibleSegments[i] });
            }
        }

        VisibleSegments = layerVisibleSegments;
    }

    public void AddUpper(List<Segment> segments)
    {
        VisibleSegments.AddRange(segments);
    }

    public void SetBackground(Color background)
    {

        Background = background;
    }
}

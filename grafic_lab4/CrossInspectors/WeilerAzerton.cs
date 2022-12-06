using grafic_lab4.Figures;
using System.Linq;

namespace grafic_lab4.CrossInspectors;

public class WeilerAzerton
{
    private Polygon lower;
    private Polygon upper;

    public bool IsClip { get; private set; }

    public List<Polygon> VisiblePolygons { get; private set; }
    public List<List<Segment>> InvisibleLines { get; private set; }

    public WeilerAzerton(Polygon lower, Polygon upper)
    {
        VisiblePolygons = new List<Polygon>();
        InvisibleLines = new List<List<Segment>>();

        IsClip = IntersectedPoint(lower, upper);
    }

    private bool IntersectedPoint(Polygon lowerOrigin, Polygon upperOrigin)
    {
        upper = upperOrigin.Clone();
        lower = lowerOrigin.Clone();

        // каждый список - точки пересечения для определённой стороны
        var intersectionsLower = CreateArrayList<PointF>(lower.Size);
        var intersectionsUpper = CreateArrayList<PointF>(upper.Size);

        HashSet<PointF> pointsOnEdge = new HashSet<PointF>();
        HashSet<PointF> pointsOnLower = new HashSet<PointF>();
        (int interCounter, int pointCounter) = FindIntersections(intersectionsLower, intersectionsUpper, pointsOnEdge, pointsOnLower);

        if (interCounter == 0 && pointCounter == 0)
        {
            return false;
        }

        InsertIntersections(lower, intersectionsLower);
        InsertIntersections(upper, intersectionsUpper);

        HashSet<PointF> inPoints = new HashSet<PointF>();
        HashSet<PointF> outPoints = new HashSet<PointF>();

        inPoints = inPoints.Concat(pointsOnEdge).ToHashSet();
        outPoints = outPoints.Concat(pointsOnEdge).ToHashSet();

        FindInOutPoints(lowerOrigin, upper, intersectionsLower, inPoints, outPoints);

        if (!inPoints.Any() && !outPoints.Any())
        {
            return false;
        }


        if (inPoints.Count != outPoints.Count)
        {
            return false;
        }

        if (pointsOnEdge.Count == inPoints.Count)
        {
            return false;
        }


        Dictionary<PointF, int> pointsIndexLower = CreatePointIndexe(lower);
        Dictionary<PointF, int> pointsIndexUpper = CreatePointIndexe(upper);

        ClipLower(inPoints, outPoints, pointsIndexLower, pointsIndexUpper, pointsOnLower);

        FindInvisibleSegments(inPoints, outPoints, pointsOnEdge);

        return true;
    }

    private (int, int) FindIntersections(List<PointF>[] intersectionsLower, List<PointF>[] intersectionsUpper, HashSet<PointF> pointsOnEdge, HashSet<PointF> pointsOnUpper)
    {
        int interCounter = 0;
        int pointCounter = 0;

        for (int i = 0; i < lower.Size; i++)
        {
            var edge = lower.GetEdge(i);

            CyrusBeckClip cyrusBeckClip = new CyrusBeckClip(edge, upper);

            if (Math2d.AreEqual(cyrusBeckClip.tA, cyrusBeckClip.tB))
            {
                if (cyrusBeckClip.tA == Segment.BEGIN)
                {
                    pointsOnEdge.Add(edge.A);

                    ++pointCounter;
                }
                else if (cyrusBeckClip.tB == Segment.END)
                {
                    pointsOnEdge.Add(edge.B);

                    ++pointCounter;
                }
                else
                {
                    PointF point = edge.Morph(cyrusBeckClip.tA);

                    pointsOnEdge.Add(point);
                    intersectionsLower[i].Add(point);

                    ++pointCounter;
                }

                continue;
            }
            
            if (cyrusBeckClip.IsClip)
            {
                Segment? firstVisibleSeg = cyrusBeckClip.GetFirstClipSegment();
                Segment? secondVisibleSeg = cyrusBeckClip.GetSecondClipSegment();

                if (firstVisibleSeg != null)
                {
                    ++interCounter;
                    intersectionsLower[i].Add(firstVisibleSeg.B);
                    intersectionsUpper[cyrusBeckClip.IndexCrosingPolygonA].Add(firstVisibleSeg.B);
                }

                if (secondVisibleSeg != null)
                {
                    ++interCounter;
                    intersectionsLower[i].Add(secondVisibleSeg.A);
                    intersectionsUpper[cyrusBeckClip.IndexCrosingPolygonB].Add(secondVisibleSeg.A);
                }
            }
            
            if (Math2d.AreEqual(cyrusBeckClip.tA, Segment.BEGIN) && cyrusBeckClip.IndexCrosingPolygonA != -1)
            {
                double d = Math2d.DistanceToSegment(edge.A, upper.GetEdge(cyrusBeckClip.IndexCrosingPolygonA));

                if (Math2d.AreEqual(d, 0) && !pointsOnEdge.Contains(edge.A))
                {
                    pointsOnEdge.Add(edge.A);
                    ++pointCounter;
                    intersectionsUpper[cyrusBeckClip.IndexCrosingPolygonA].Add(edge.A);
                    pointsOnUpper.Add(edge.A);
                }
            }

            //if (Math2d.AreEqual(cyrusBeckClip.tB, Segment.END) && cyrusBeckClip.IndexCrosingPolygonB != -1)
            //{
            //    double d = Math2d.DistanceToSegment(edge.B, upper.GetEdge(cyrusBeckClip.IndexCrosingPolygonB));

            //    if (Math2d.AreEqual(d, 0)/* && ! added.Contains(edge.B)*/)
            //    {
            //        //added.Add(edge.B);
            //        ++pointCounter;
            //        intersectionsLower[i].Add(edge.B);
            //        intersectionsUpper[cyrusBeckClip.IndexCrosingPolygonB].Add(edge.B);
            //    }
            //}
        }

        return (interCounter, pointCounter);
    }

    private void FindInvisibleSegments(HashSet<PointF> inPoints, HashSet<PointF> outPoints, HashSet<PointF> pointsOnEdge)
    {
        for (int invisibleLowIndex = 0; invisibleLowIndex < lower.Size; ++invisibleLowIndex)
        {
            if (!pointsOnEdge.Contains(lower.GetPoint(invisibleLowIndex)) && outPoints.Contains(lower.GetPoint(invisibleLowIndex)))
            {
                List<Segment> invisibleLine = new List<Segment>();
                
                do
                {
                    Segment segment = lower.GetEdge(invisibleLowIndex);
                    segment.Color = lower.Color;

                    invisibleLine.Add(segment);

                    ++invisibleLowIndex;
                } while (pointsOnEdge.Contains(lower.GetPoint(invisibleLowIndex)) || !inPoints.Contains(lower.GetPoint(invisibleLowIndex))) ;

                if (invisibleLine.Any())
                {
                    --invisibleLowIndex;
                    InvisibleLines.Add(invisibleLine);
                }
            }
        }
    }

    private static List<T>[] CreateArrayList<T>(int size)
    {
        List<T>[] res = new List<T>[size];

        for (int i = 0; i < size; i++)
        {
            res[i] = new List<T>();
        }

        return res;
    }

    private static void FindInOutPoints(Polygon lowerOrigin, Polygon upperWithIntersection, List<PointF>[] intersectionsLower, HashSet<PointF> inPoints, HashSet<PointF> outPoints)
    {
        int lowerEdgeIndex = 0;

        for (int i = 0; i < intersectionsLower.Length; i++)
        {
            intersectionsLower[i].Reverse();
        }

        while (lowerEdgeIndex < intersectionsLower.Length && intersectionsLower[lowerEdgeIndex].Count == 0)
        {
            ++lowerEdgeIndex;
        }

        if (lowerEdgeIndex >= intersectionsLower.Length)
        {
            return;
        }

        int upperEdgeIndex = 0;
        PointF firstIntersections = intersectionsLower[lowerEdgeIndex][0];

        for (; upperEdgeIndex < upperWithIntersection.Size; upperEdgeIndex++)
        {
            if (firstIntersections == upperWithIntersection.GetPoint(upperEdgeIndex))
            {
                break;
            }
        }

        bool isIn = upperWithIntersection.GetEdge(upperEdgeIndex).Normal.Dot(lowerOrigin.GetEdge(lowerEdgeIndex).Direction) > 0;

        for (int lowerEdge = lowerEdgeIndex; lowerEdge < intersectionsLower.Length; lowerEdge++)
        {
            foreach (PointF lowerEdgeIntersection in intersectionsLower[lowerEdge])
            {
                if (!inPoints.Contains(lowerEdgeIntersection) && !outPoints.Contains(lowerEdgeIntersection))
                {
                    if (isIn)
                    {
                        inPoints.Add(lowerEdgeIntersection);
                    }
                    else
                    {
                        outPoints.Add(lowerEdgeIntersection);
                    }

                    isIn = !isIn;
                }
            }
        }
    }

    private Dictionary<PointF, int> CreatePointIndexe(Polygon polygon)
    {
        Dictionary<PointF, int> res = new Dictionary<PointF, int>();

        for (int i = 0; i < polygon.Size; i++)
        {
            res[polygon.GetPoint(i)] = i;
        }

        return res;
    }

    private void ClipLower(HashSet<PointF> inPoints, HashSet<PointF> outPoints, Dictionary<PointF, int> pointsIndexLower, Dictionary<PointF, int> pointsIndexUpper, HashSet<PointF> pointsOnUpper)
    {
        inPoints = new HashSet<PointF>(inPoints);
        outPoints = new HashSet<PointF>(outPoints);

        while (inPoints.Any() && outPoints.Any())
        {
            PointF begin = outPoints.First();

            foreach (PointF point in outPoints)
            {
                if (!pointsOnUpper.Contains(point))
                {
                    begin = point;
                    
                    break;
                }
            }

            int beginIndex = pointsIndexLower[begin];
            int lowerIndex = beginIndex;

            // отсечённая фигура
            List<PointF> figure = new List<PointF>();

            do
            {
                PointF currPoint = lower.GetPoint(lowerIndex);

                // обход по контурам фигуры lower
                do {
                    figure.Add(currPoint);

                    --lowerIndex;

                    if (lowerIndex < 0)
                    {
                        lowerIndex = lower.Size - 1;
                    }

                    currPoint = lower.GetPoint(lowerIndex);
                }
                while (!inPoints.Contains(currPoint) && inPoints.Any()) ;

                inPoints.Remove(currPoint);

                int upperIndex = pointsIndexUpper[currPoint];

                currPoint = upper.GetPoint(upperIndex);

                // обход по контурам фигур upper
                while (!outPoints.Contains(currPoint) && outPoints.Any())
                {
                    figure.Add(upper.GetPoint(upperIndex));

                    upperIndex = (upperIndex + 1) % upper.Size;

                    currPoint = upper.GetPoint(upperIndex);
                }

                outPoints.Remove(currPoint);

                lowerIndex = pointsIndexLower[currPoint];

            } while (beginIndex != lowerIndex);

            if (figure.Any())
            {
                figure.Reverse();
                Polygon visiblePolygon = new Polygon(figure);
                visiblePolygon.Color = lower.Color;
                VisiblePolygons.Add(visiblePolygon);
            }
        }
    }

    private static int InsertIntersections(Polygon polygon, List<PointF>[] intersections)
    {
        int count = 0;

        for (int i = polygon.Size - 1; i >= 0; i--)
        {
            if (intersections[i].Count == 0)
            {
                continue;
            }

            PointF direction = polygon.GetEdge(i).Direction;

            int xDirectionSort = -Math.Sign(direction.X);
            int yDirectionSort = -Math.Sign(direction.Y);

            intersections[i].Sort(Comparer<PointF>.Create((first, second) =>
            {
                int res = xDirectionSort * first.X.CompareTo(second.X);

                if (res == 0)
                {
                    res = yDirectionSort * first.Y.CompareTo(second.Y);
                }

                return res;
            }));

            for (int j = 0; j < intersections[i].Count; j++)
            {
                if (polygon.GetPoint(i) != intersections[i][j] && polygon.GetPoint(i + 1) != intersections[i][j])
                {
                    polygon.AddAfter(i, intersections[i][j]);
                    ++count;
                }
            }
        }

        return count;
    }
}


/*
    private void IntersectedPoint(Polygon lowerOrigin, Polygon upperOrigin)
    {
        upper = upperOrigin.Clone();
        lower = lowerOrigin.Clone();

        // каждый список - точки пересечения для определённой стороны
        List<PointF>[] intersectionsLower = new List<PointF>[lower.Size];

        for (int i = 0; i < intersectionsLower.Length; i++)
        {
            intersectionsLower[i] = new List<PointF>();
        }

        var intersectionsUpper = new Dictionary<PointF, int>();

        for (int i = 0, upperOriginIndex = 0; i < upper.Size; ++i, ++upperOriginIndex)
        {
            var edge = upper.GetEdge(i);

            CyrusBeckClip cyrusBeckClip = new CyrusBeckClip(edge, lower);

            if (cyrusBeckClip.IsClip)
            {
                Segment? firstVisibleSeg = cyrusBeckClip.GetFirstClipSegment();
                Segment? secondVisibleSeg = cyrusBeckClip.GetSecondClipSegment();

                if (firstVisibleSeg != null && secondVisibleSeg != null)
                {
                    upper.SplitSegment(i, cyrusBeckClip.tA, cyrusBeckClip.tB);
                    intersectionsUpper[firstVisibleSeg.B] = upperOriginIndex;
                    intersectionsUpper[secondVisibleSeg.A] = upperOriginIndex;
                    i += 2;
                }
                else if (firstVisibleSeg != null)
                {
                    upper.SplitSegment(i, cyrusBeckClip.tA);
                    intersectionsUpper[firstVisibleSeg.B] = upperOriginIndex;
                    ++i;
                }
                else if (secondVisibleSeg != null)
                {
                    upper.SplitSegment(i, cyrusBeckClip.tB);
                    intersectionsUpper[secondVisibleSeg.A] = upperOriginIndex;
                    ++i;
                }

                if (firstVisibleSeg != null)
                {
                    intersectionsLower[cyrusBeckClip.IndexCrosingPolygonA].Add(firstVisibleSeg.B);
                }

                if (secondVisibleSeg != null)
                {
                    intersectionsLower[cyrusBeckClip.IndexCrosingPolygonB].Add(secondVisibleSeg.A);
                }
            }
        }

        InsertIntersections(lower, intersectionsLower);

        HashSet<PointF> inPoints = new HashSet<PointF>();
        HashSet<PointF> outPoints = new HashSet<PointF>();

        FindInOutPoints(lowerOrigin, upperOrigin, intersectionsLower, intersectionsUpper, inPoints, outPoints);

        Dictionary<PointF, int> pointsIndexLower = CreatePointIndexe(lower);
        Dictionary<PointF, int> pointsIndexUpper = CreatePointIndexe(upper);

        ClipLower(new HashSet<PointF>(inPoints), new HashSet<PointF>(outPoints), pointsIndexLower, pointsIndexUpper);
    
        
    }
 */


//private static void FindInOutPoints(Polygon upperOriginal, Polygon lowerWithIntersection, List<PointF>[] intersectionsUpper, HashSet<PointF> inPoints, HashSet<PointF> outPoints)
//{
//    int upperEdgeIndex = 0;

//    for (int i = 0; i < intersectionsUpper.Length; i++)
//    {
//        intersectionsUpper[i].Reverse();
//    }

//    while (upperEdgeIndex < intersectionsUpper.Length && intersectionsUpper[upperEdgeIndex].Count == 0)
//    {
//        ++upperEdgeIndex;
//    }

//    int lowerEdgeIndex = 0;
//    PointF firstIntersections = intersectionsUpper[upperEdgeIndex][0];

//    for (; lowerEdgeIndex < lowerWithIntersection.Size; lowerEdgeIndex++)
//    {
//        if (firstIntersections == lowerWithIntersection.GetPoint(lowerEdgeIndex))
//        {
//            break;
//        }
//    }

//    bool isIn = lowerWithIntersection.GetEdge(lowerEdgeIndex).Normal.Dot(upperOriginal.GetEdge(upperEdgeIndex).Direction) < 0;

//    for (int lowerEdge = upperEdgeIndex; lowerEdge < intersectionsUpper.Length; lowerEdge++)
//    {
//        foreach (PointF lowerEdgeIntersection in intersectionsUpper[lowerEdge])
//        {
//            if (isIn)
//            {
//                inPoints.Add(lowerEdgeIntersection);
//            }
//            else
//            {
//                outPoints.Add(lowerEdgeIntersection);
//            }

//            isIn = !isIn;
//        }
//    }
//}


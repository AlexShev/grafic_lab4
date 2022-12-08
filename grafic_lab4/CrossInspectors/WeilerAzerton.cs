using grafic_lab4.Figures;

namespace grafic_lab4.CrossInspectors;

public class WeilerAzerton
{
    private Polygon lower;
    private Polygon upper;

    private HashSet<PointF> inPoints = new HashSet<PointF>();
    private HashSet<PointF> outPoints = new HashSet<PointF>();


    public bool IsClip { get; private set; }
    public bool IsLowerInUpper { get; private set; }

    public List<Polygon> VisiblePolygons { get; private set; }
    public List<List<Segment>> InvisibleLines { get; private set; }

    public WeilerAzerton(Polygon lower, Polygon upper)
    {
        VisiblePolygons = new List<Polygon>();
        InvisibleLines = new List<List<Segment>>();
        
        IsLowerInUpper = false;
        IsClip = IntersectedPoint(lower, upper);
    }

    private bool IntersectedPoint(Polygon lowerOrigin, Polygon upperOrigin)
    {
        upper = upperOrigin.Clone();
        lower = lowerOrigin.Clone();

        // каждый список - точки пересечения для определённой стороны
        var intersectionsLower = CreateArray<HashSet<PointF>>(lower.Size);
        var intersectionsUpper = CreateArray<HashSet<PointF>>(upper.Size);
        
        FindIntersections(intersectionsLower, intersectionsUpper);

        if (IsLowerInUpper)
        {
            return false;
        }

        InsertIntersections(lower, intersectionsLower);
        InsertIntersections(upper, intersectionsUpper);

        if (!inPoints.Any() && !outPoints.Any())
        {
            return false;
        }

        // в алгоритме что-то не то
        if (inPoints.Count != outPoints.Count)
        {
            return false;
        }

        Dictionary<PointF, int> pointsIndexLower = CreatePointIndexe(lower);
        Dictionary<PointF, int> pointsIndexUpper = CreatePointIndexe(upper);

        ClipLower(inPoints, outPoints, pointsIndexLower, pointsIndexUpper);

        FindInvisibleSegments(inPoints, outPoints);

        return true;
    }

    private void FindIntersections(HashSet<PointF>[] intersectionsLower, HashSet<PointF>[] intersectionsUpper)
    {
        int tA_0_counter = 0;
        int tB_1_counter = 0;

        for (int i = 0; i < lower.Size; i++)
        {
            var edge = lower.GetEdge(i);

            CyrusBeckClip cyrusBeckClip = new CyrusBeckClip(edge, upper);

            if (Math2d.AreEqual(cyrusBeckClip.tA, Segment.BEGIN))
            {
                ++tA_0_counter;
            }
            if (Math2d.AreEqual(cyrusBeckClip.tB, Segment.END))
            {
                ++tB_1_counter;
            }


            if (Math2d.AreEqual(cyrusBeckClip.tA, cyrusBeckClip.tB))
            {
                if (Math2d.AreEqual(cyrusBeckClip.tA, Segment.BEGIN))
                {
                    inPoints.Add(edge.A);

                    int index = cyrusBeckClip.IndexCrosingPolygonA;

                    if (index == -1)
                    {
                        index = cyrusBeckClip.IndexCrosingPolygonB;
                    }

                    intersectionsUpper[index].Add(edge.A);
                }
                else if (Math2d.AreEqual(cyrusBeckClip.tB, Segment.END))
                {
                    outPoints.Add(edge.B);

                    int index = cyrusBeckClip.IndexCrosingPolygonB;

                    if (index == -1)
                    {
                        index = cyrusBeckClip.IndexCrosingPolygonA;
                    }

                    intersectionsUpper[index].Add(edge.B);
                }
                else
                {
                    PointF point = edge.Morph(cyrusBeckClip.tA);

                    outPoints.Add(point);
                    inPoints.Add(point);

                    intersectionsLower[i].Add(point);
                }

                continue;
            }

            if (cyrusBeckClip.IsClip)
            {
                // проверка что конец принадлежит ребру
                if (Math2d.AreEqual(cyrusBeckClip.tA, Segment.BEGIN))
                {
                    if (cyrusBeckClip.IndexCrosingPolygonA != -1)
                    {
                        double d = Math2d.DistanceToSegment(edge.A, upper.GetEdge(cyrusBeckClip.IndexCrosingPolygonA));

                        if (Math2d.AreEqual(d, 0))
                        {
                            intersectionsUpper[cyrusBeckClip.IndexCrosingPolygonA].Add(edge.A);
                            outPoints.Add(edge.A);
                        }
                    }
                }
                else
                {
                    var point = edge.Morph(cyrusBeckClip.tA);

                    outPoints.Add(point);
                    intersectionsLower[i].Add(point);

                    int index = cyrusBeckClip.IndexCrosingPolygonA;

                    if (index == -1)
                    {
                        index = cyrusBeckClip.IndexCrosingPolygonB;
                    }

                    // проследить чтоб не добавилась точки верхнего!
                    intersectionsUpper[index].Add(point);
                }


                // проверка что конец принадлежит ребру
                if (Math2d.AreEqual(cyrusBeckClip.tB, Segment.END))
                {
                    int index = cyrusBeckClip.IndexCrosingPolygonB;

                    //if (index == -1)
                    //{
                    //    index = cyrusBeckClip.IndexCrosingPolygonA;
                    //}

                    if (index != -1)
                    {
                        double d = Math2d.DistanceToSegment(edge.B, upper.GetEdge(index));

                        if (Math2d.AreEqual(d, 0))
                        {
                            intersectionsUpper[index].Add(edge.B);
                            inPoints.Add(edge.B);
                        }
                    }
                }
                else
                {
                    var point = edge.Morph(cyrusBeckClip.tB);

                    inPoints.Add(point);
                    intersectionsLower[i].Add(point);

                    int index = cyrusBeckClip.IndexCrosingPolygonB;

                    if (index == -1)
                    {
                        index = cyrusBeckClip.IndexCrosingPolygonA;
                    }

                    intersectionsUpper[index].Add(point);
                }
            }
        }

        IsLowerInUpper = tA_0_counter ==tB_1_counter && tA_0_counter == lower.Size;
    }

    private void FindInvisibleSegments(HashSet<PointF> inPoints, HashSet<PointF> outPoints)
    {
        for (int invisibleLowIndex = 0; invisibleLowIndex < lower.Size; ++invisibleLowIndex)
        {
            if (outPoints.Contains(lower.GetPoint(invisibleLowIndex)) && !inPoints.Contains(lower.GetPoint(invisibleLowIndex)))
            {
                List<Segment> invisibleLine = new List<Segment>();

                while (!inPoints.Contains(lower.GetPoint(invisibleLowIndex)) 
                    || inPoints.Contains(lower.GetPoint(invisibleLowIndex)) && outPoints.Contains(lower.GetPoint(invisibleLowIndex)))
                {
                    Segment segment = lower.GetEdge(invisibleLowIndex);
                    segment.Color = lower.Color;

                    invisibleLine.Add(segment);

                    ++invisibleLowIndex;
                }

                if (invisibleLine.Any())
                {
                    --invisibleLowIndex;
                    InvisibleLines.Add(invisibleLine);
                }
            }
        }
    }

    private static T [] CreateArray<T>(int size) where T : new()
    {
        T[] res = new T[size];

        for (int i = 0; i < size; i++)
        {
            res[i] = new T();
        }

        return res;
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

    private void ClipLower(HashSet<PointF> inPoints, HashSet<PointF> outPoints, Dictionary<PointF, int> pointsIndexLower, Dictionary<PointF, int> pointsIndexUpper)
    {
        inPoints = new HashSet<PointF>(inPoints);
        outPoints = new HashSet<PointF>(outPoints);

        while (inPoints.Any() && outPoints.Any())
        {
            PointF begin = inPoints.First();

            //int i = 0;
            //foreach (var point in inPoints)
            //{
            //    if (! outPoints.Contains(point))
            //    {
            //        begin = point;
            //        break;
            //    }

            //    ++i;
            //}

            //if (i == inPoints.Count)
            //{
            //    break;
            //}

            int beginIndex = pointsIndexLower[begin];
            
            int upperIndex = 0;
            int lowerIndex = beginIndex;
            // отсечённая фигура
            List<PointF> figure = new List<PointF>();

            do
            {
                PointF currPoint = lower.GetPoint(lowerIndex);

                // обход по контурам фигуры lower
                do
                {
                    figure.Add(currPoint);

                    lowerIndex = (lowerIndex + 1) % lower.Size;

                    currPoint = lower.GetPoint(lowerIndex);
                }
                while (!outPoints.Contains(currPoint) && outPoints.Any());

                outPoints.Remove(currPoint);

                upperIndex = pointsIndexUpper[currPoint];

                currPoint = lower.GetPoint(lowerIndex);

                // обход по контурам фигур upper
                do
                {
                    figure.Add(currPoint);

                    --upperIndex;

                    if (upperIndex < 0)
                    {
                        upperIndex = upper.Size - 1;
                    }

                    currPoint = upper.GetPoint(upperIndex);
                }
                while (!inPoints.Contains(currPoint) && inPoints.Any());

                inPoints.Remove(currPoint);

                lowerIndex = pointsIndexLower[currPoint];

            } while (beginIndex != lowerIndex);

            if (figure.Count > 2)
            {
                figure.Reverse();
                Polygon visiblePolygon = new Polygon(figure);
                visiblePolygon.Color = lower.Color;
                VisiblePolygons.Add(visiblePolygon);
            }
        }
    }

    private static int InsertIntersections(Polygon polygon, HashSet<PointF>[] intersections)
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

            var pointsOnSegment = intersections[i].ToList();
            pointsOnSegment.Sort(Comparer<PointF>.Create((first, second) =>
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
                if (polygon.GetPoint(i) != pointsOnSegment[j] && polygon.GetPoint(i + 1) != pointsOnSegment[j])
                {
                    polygon.AddAfter(i, pointsOnSegment[j]);
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



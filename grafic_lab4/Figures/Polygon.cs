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

    public bool IsIn(PointF point)
    {
        bool result = false;

        int j = Size - 1;

        for (int i = 0; i < Size; i++)
        {
            if ((points[i].Y <= point.Y && points[j].Y > point.Y 
                 || points[j].Y <= point.Y && points[i].Y > point.Y)
                 && (point.X < ((points[j].Y - points[i].Y) * (points[j].X - points[i].X)) / (points[i].X + (point.Y - points[i].Y))))
                result = !result;
            j = i;
        }

        return result;
    }


    // Given three colinear points p, q, r,
    // the function checks if point q lies
    // on line segment "pr"
    static bool onSegment(PointF p, PointF q, PointF r)
    {
        if (q.X <= Math.Max(p.X, r.X) &&
            q.X >= Math.Min(p.X, r.X) &&
            q.Y <= Math.Max(p.Y, r.Y) &&
            q.Y >= Math.Min(p.Y, r.Y))
        {
            return true;
        }
        return false;
    }

    // To find orientation of ordered triplet (p, q, r).
    // The function returns following values
    // 0 --> p, q and r are colinear
    // 1 --> Clockwise
    // 2 --> Counterclockwise
    static int orientation(PointF p, PointF q, PointF r)
    {
        float val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);

        if (Math2d.AreEqual(val, 0))
        {
            return 0; // colinear
        }
        return (val > 0) ? 1 : 2; // clock or counterclock wise
    }

    // The function that returns true if
    // line segment "p1q1" and "p2q2" intersect.
    static bool doIntersect(PointF p1, PointF q1, PointF p2, PointF q2)
    {
        // Find the four orientations needed for
        // general and special cases
        int o1 = orientation(p1, q1, p2);
        int o2 = orientation(p1, q1, q2);
        int o3 = orientation(p2, q2, p1);
        int o4 = orientation(p2, q2, q1);

        // General case
        if (o1 != o2 && o3 != o4)
        {
            return true;
        }
        // Special Cases
        // p1, q1 and p2 are colinear and
        // p2 lies on segment p1q1
        if (o1 == 0 && onSegment(p1, p2, q1))
        {
            return true;
        }

        // p1, q1 and p2 are colinear and
        // q2 lies on segment p1q1
        if (o2 == 0 && onSegment(p1, q2, q1))
        {
            return true;
        }

        // p2, q2 and p1 are colinear and
        // p1 lies on segment p2q2
        if (o3 == 0 && onSegment(p2, p1, q2))
        {
            return true;
        }

        // p2, q2 and q1 are colinear and
        // q1 lies on segment p2q2
        if (o4 == 0 && onSegment(p2, q1, q2))
        {
            return true;
        }

        // Doesn"t fall in any of the above cases
        return false;
    }


    // Returns true if the point p lies
    // inside the points[] with n vertices
    public bool IsInside(PointF p)
    {
        // Create a point for line segment from p to infinite
        PointF extreme = new PointF(float.MaxValue, p.Y);

        // Count intersections of the above line
        // with sides of points
        int count = 0, i = 0;

        do
        {
            int next = (i + 1) % Size;

            // Check if the line segment from "p" to
            // "extreme" intersects with the line
            // segment from "points[i]" to "points[next]"
            if (doIntersect(points[i], points[next], p, extreme))
            {
                // If the point "p" is colinear with line
                // segment "i-next", then check if it lies
                // on segment. If it lies, return true, otherwise false
                if (orientation(points[i], p, points[next]) == 0)
                {
                    return onSegment(points[i], p, points[next]);
                }

                count++;
            }
            i = next;
        } while (i != 0);

        // Return true if count is odd, false otherwise
        return (count % 2 == 1); // Same as (count%2 == 1)
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
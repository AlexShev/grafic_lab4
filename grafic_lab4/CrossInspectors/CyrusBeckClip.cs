using grafic_lab4.Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grafic_lab4.CrossInspectors;

public class CyrusBeckClip
{
    private readonly Segment subject;
    private readonly Polygon polygon;

    public bool IsClip { get; private set; }
    public bool IsOutOfPolygon { get; private set; }

    public int IndexCrosingPolygonA { get; private set; } = -1;
    public int IndexCrosingPolygonB { get; private set; } = -1;

    public float tA { get; private set; }
    public float tB { get; private set; }

    public CyrusBeckClip(Segment segment, Polygon polygon)
    {
        this.subject = segment;
        this.polygon = polygon;
        IsOutOfPolygon = false;

        IsClip = Clip();
    }

    private bool Clip()
    {
        var subjDir = subject.Direction;

        tA = Segment.BEGIN;
        tB = Segment.END;

        for (int i = 0; i < polygon.Size; ++i)
        {   
            var edge = polygon.GetEdge(i);

            switch (Math.Sign(edge.Normal.Dot(subjDir)))
            {
                case -1:
                {
                    var t = subject.IntersectionParameter(edge);

                    if (Math2d.AreEqual(tA, Segment.BEGIN) && Math2d.AreEqual(t, Segment.BEGIN))
                    {
                        IndexCrosingPolygonA = i;
                    }

                    if (t > tA)
                    {
                        tA = t;
                        IndexCrosingPolygonA = i;
                    }
                    break;
                }
                case +1:
                {
                    var t = subject.IntersectionParameter(edge);

                    if (Math2d.AreEqual(tB, Segment.END) && Math2d.AreEqual(t, Segment.END))
                    {
                        IndexCrosingPolygonB = i;
                    }

                    if (t < tB)
                    {
                        tB = t;
                        IndexCrosingPolygonB = i;
                    }

                    break;
                }
                case 0:
                {
                    if (!edge.OnLeft(subject.A))
                    {
                        IsOutOfPolygon = true;
                        return false;
                    }
                    break;
                }
            }
        }
        if (tA > tB)
        {
            IsOutOfPolygon = true;

            return false;
        }

        return true;
    }

    public Segment? GetFirstClipSegment()
    {
        return subject.Morph(Segment.BEGIN, tA);
    }

    public Segment? GetSecondClipSegment()
    {
        return subject.Morph(tB, Segment.END);
    }

    public Segment? GetSegmentInPolygon()
    {
        return tA < tB ? subject.Morph(tA, tB) : null;
    }
}

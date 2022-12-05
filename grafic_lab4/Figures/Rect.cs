using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grafic_lab4.Figures;

public struct Rect
{
    float x; // the center in x axis
    float y; // the center in y axis
    float width;
    float height;

    public Rect(float x, float y, float width, float height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    public bool IsIntersected(Rect orther)
    {
        return Math.Abs(x - orther.x) < (Math.Abs(width + orther.width) / 2)
               && (Math.Abs(y - orther.y) < (Math.Abs(height + orther.height) / 2));
    }
}

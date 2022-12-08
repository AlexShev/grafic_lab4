using grafic_lab4.Figures;

namespace grafic_lab4.Image;

public class Printer
{
    public readonly Bitmap Image;

    public Printer(GeometryImage image, int wight, int hight, bool withVisible)
    {
        Image = new Bitmap((int)image.Wight, (int)image.Hight);
        Graphics graphics = Graphics.FromImage(Image);

        graphics.Clear(image.Background);

        PrintVisible(graphics, image.VisiblePolygons);
        PrintVisible(graphics, image.VisibleSegments);

        if (withVisible)
        {
            PrintInvisible(graphics, image.InvisiblePolygons);
            PrintInvisible(graphics, image.InvisibleLines);
        }
    }

    public void PrintVisible(Graphics graphics, List<Polygon> polygons)
    {
        using SolidBrush brush = new SolidBrush(Color.White);

        for (int i = 0; i < polygons.Count; i++)
        {
            if (polygons[i].Size > 0)
            {
                brush.Color = polygons[i].Color.Value;
                graphics.FillPolygon(brush, polygons[i].GetPoints().ToArray());
            }
        }
    }

    public void PrintVisible(Graphics graphics, List<Segment> segments)
    {
        using Pen pen = new Pen(Color.White, 2);

        for (int i = 0; i < segments.Count; i++)
        {
            pen.Color = segments[i].Color.Value;
            graphics.DrawLine(pen, segments[i].A, segments[i].B);
        }
    }

    public void PrintInvisible(Graphics graphics, List<Polygon> polygons)
    {
        float[] dashValues = { 4, 2 };
        Pen dashPen = new Pen(Color.White, 2);
        dashPen.DashPattern = dashValues;

        for (int i = 0; i < polygons.Count; i++)
        {
            dashPen.Color = polygons[i].Color.Value;
            graphics.DrawPolygon(dashPen, polygons[i].GetPoints().ToArray());
        }
    }

    public void PrintInvisible(Graphics graphics, List<List<Segment>> lines)
    {
        float[] dashValues = { 4, 2 };
        Pen dashPen = new Pen(Color.White, 2);
        dashPen.DashPattern = dashValues;

        for (int i = 0; i < lines.Count; i++)
        {
            for (int j = 0; j < lines[i].Count; j++)
            {
                dashPen.Color = lines[i][j].Color.Value;
                graphics.DrawLine(dashPen, lines[i][j].A, lines[i][j].B);
            }
        }
    }
}

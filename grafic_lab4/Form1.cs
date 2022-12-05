using grafic_lab4.CrossInspectors;
using grafic_lab4.Figures;
using grafic_lab4.Image;
using System.Drawing;

namespace grafic_lab4;

public partial class Form1 : Form
{
    private LayerCollection _layers;
    private FigureMover[] _figureMovers;

    public Form1()
    {
        InitializeComponent();

        pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);

        InitPolygons();
    }

    private void InitPolygons()
    {
        _layers = new LayerCollection();

        List<PointF>[] polylins = new List<PointF>[]
        {
            new List<PointF> { new PointF(200, 50), new PointF(350, 200), new PointF(350, 400), new PointF(200, 200), new PointF(50, 400), new PointF(50, 200) },
            new List<PointF> { new PointF(100, 100), new PointF(300, 100), new PointF(300, 300), new PointF(100, 300) },
            //new List<PointF> { new PointF(50, 125), new PointF(350, 125), new PointF(200, 350) },
            //new List<PointF> { new PointF(0, 0), new PointF(500, 0), new PointF(500, 500), new PointF(0, 500) },
        };

        Color[] colors = new Color[]
        {
            Color.Red,
            Color.Blue,
            Color.Yellow,
            Color.Green,
            Color.Violet
        };

        _figureMovers = new FigureMover[polylins.Length];

        _layers.AddLayer(new BackgroundLayer(Color.Silver));

        for (int i = 0; i < polylins.Length; i++)
        {
            Polygon polygon = new Polygon(polylins[i]);
            polygon.Color = colors[i];

            var polygonWithSegments =  PolygonWithSegments.Create(polygon);
            _figureMovers[i] = new FigureMover(polygonWithSegments);

            _layers.AddLayer(new PolygonLayer(polygonWithSegments));
        }

      // _figureMovers.Last().MoveTo(new PointF(200, 200));
    }

    private void button1_Click(object sender, EventArgs e)
    {
        pictureBox1.Image = new Printer(_layers.Create(), true).Image;
    }
}
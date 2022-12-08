using grafic_lab4.Figures;
using grafic_lab4.Image;

namespace grafic_lab4;

public partial class Form1 : Form
{
    private LayerCollection _layers;

    private FigureMoveControl[] _figureMoveControls;
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

        (List<PointF>[] polylins, Color[] colors) = InitInfor();


        _figureMovers = new FigureMover[polylins.Length];
        _figureMoveControls = new FigureMoveControl[polylins.Length];

        _layers.AddLayer(new BackgroundLayer(Color.Silver));

        for (int i = 0; i < polylins.Length; i++)
        {
            Polygon polygon = new Polygon(polylins[i]);
            polygon.Color = colors[i];

            var polygonWithSegments = PolygonWithSegments.Create(polygon);
            _figureMovers[i] = new FigureMover(polygonWithSegments);

            _layers.AddLayer(new PolygonLayer(polygonWithSegments));

            _figureMoveControls[i] = new FigureMoveControl(i + 1);
            flowLayoutPanel1.Controls.Add(_figureMoveControls[i]);
        }
    }

    private static (List<PointF>[], Color[]) InitInfor()
    {
        List<PointF>[] polylins = new List<PointF>[]
        {
            // new List<PointF> { new PointF(50, 200), new PointF(0, 80), new PointF(100, 100) },

            // new List<PointF> { new PointF(0, 100), new PointF(100, 100), new PointF(50, 200) },

            new List<PointF> { new PointF(200, 55), new PointF(350, 200), new PointF(350, 400), new PointF(200, 500), new PointF(50, 400), new PointF(50, 205) },
            new List<PointF> { new PointF(200, 0), new PointF(400, 200), new PointF(200, 400), new PointF(0, 200) },
            new List<PointF> { new PointF(50, 120), new PointF(350, 120), new PointF(200, 350) },
            new List<PointF> { new PointF(0, 0), new PointF(500, 0), new PointF(500, 500), new PointF(0, 500) },
        };

        Color[] colors = new Color[]
        {
            Color.Red,
            Color.Blue,
            Color.Yellow,
            Color.Green,
            Color.Violet,
            Color.Gold,
            Color.Orange
        };

        return (polylins, colors);
    }

    private void button1_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < _figureMovers.Length; i++)
        {
            _figureMovers[i].MoveTo((float)_figureMoveControls[i].X, (float)_figureMoveControls[i].Y);
        }

        pictureBox1.Image = new Printer(_layers.Create(pictureBox1.Width, pictureBox1.Height), pictureBox1.Width, pictureBox1.Height, checkBox1.Checked).Image;
    }
}
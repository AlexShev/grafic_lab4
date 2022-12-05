namespace grafic_lab4.Image;

public class BackgroundLayer : ILayer
{
    private Color background;

    public BackgroundLayer(Color background)
    {
        this.background = background;
    }

    public void AddTo(GeometryImage image)
    {
        image.SetBackground(background);
    }
}

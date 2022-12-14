namespace grafic_lab4.Image;

public class LayerCollection
{
    List<ILayer> layers;

    public LayerCollection()
    {
        layers = new List<ILayer>();
    }

    public void AddLayer(ILayer layer)
    {
        layers.Add(layer);
    }

    public GeometryImage Create(int wight, int hight)
    {
        GeometryImage image = new GeometryImage(wight, hight);

        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].AddTo(image);
        }

        return image;
    }
}

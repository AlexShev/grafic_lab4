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

    public GeometryImage Create()
    {
        GeometryImage image = new GeometryImage();

        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].AddTo(image);
        }

        return image;
    }
}

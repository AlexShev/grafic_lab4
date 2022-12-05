namespace grafic_lab4.Figures;

public class FigureMover
{
    public interface IMovable
    {
        public void MoveTo(PointF point);
    }

    private readonly IMovable movable;

    public FigureMover(IMovable movable)
    {
        this.movable = movable;
    }

    public void MoveTo(PointF point)
    {
        movable.MoveTo(point);
    }
}

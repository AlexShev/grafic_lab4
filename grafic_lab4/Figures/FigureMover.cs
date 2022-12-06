namespace grafic_lab4.Figures;

public class FigureMover
{
    public interface IMovable
    {
        public void MoveTo(float leftX, float topY);
    }

    private readonly IMovable movable;

    public FigureMover(IMovable movable)
    {
        this.movable = movable;
    }

    public void MoveTo(float leftX, float topY)
    {
        movable.MoveTo(leftX, topY);
    }
}

using grafic_lab4.Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grafic_lab4;

public class Math2d
{
    public static bool AreEqual(float a, float b)
    {
        return Math.Abs(a - b) < 1e-9;
    }

    public static bool AreEqual(PointF a, PointF b)
    {
        return new Segment(a, b).Lenght < 1;
    }

    public static float Average(float a, float b)
    {
        return (a + b) / 2;
    }

    /// Вычислить расстояние от точки до отрезка
    public static double DistanceToSegment(PointF point, Segment segment)
    {
        PointF left = segment.A;
        PointF right = segment.B;

        if (right.X < left.X)
        {
            left.Swap(ref right);
        }

        // Нахождение квадратичного расстояния - так как сама длинна нужна не всегда
        double squareDistanceToLeftPoint = SquareDistanceBetween(point, left);
        double squareDistanceToRightPoint = SquareDistanceBetween(point, right);
        double squareLength = SquareDistanceBetween(left, right);

        double distance = 0;

        // рассматривается треугольник, если одно из расстояний оказываеться длиннее гипатенузы
        // треугольника образованного катитами - отрезок и расстояние до другоко конца
        if (AreGreaterOrEqual(squareDistanceToLeftPoint + squareLength, squareDistanceToRightPoint)
            && AreGreaterOrEqual(squareDistanceToRightPoint + squareLength, squareDistanceToLeftPoint))
        {
            // Перпендикуляр лежит на отрезке, величина перпендикуляра - искомая величина
            distance = DistanceToLine(point, segment);
        }
        else
        {
            // Перпендикуляр не лежит на отрезке, нужно выбрать ближайшее расстояние
            distance = Math.Sqrt(Math.Min(squareDistanceToLeftPoint, squareDistanceToRightPoint));
        }

        return distance;
    }

    /// Вычислить расстояние от точки до прямой заданной двумя точками
    public static double DistanceToLine(PointF point, Segment line)
    {
        // расстояние можно найти из площади парралелограмма заданного двумя точками на прямой
        // и задданой точкой (положение 4-ой точки не имеет значения)

        double square = Math.Abs((line.A.X - point.X) * (line.B.Y - line.A.Y) - (line.B.X - line.A.X) * (line.A.Y - point.Y));

        return square / line.Lenght;
    }

    public static double SquareDistanceBetween(PointF first, PointF second)
    {
        // разница координат по осям
        double dx = first.X - second.X;
        double dy = first.Y - second.Y;

        return dx* dx + dy* dy;
    }

    public static bool AreGreaterOrEqual(double first, double second)
    {
        return first > second || AreEqual(first, second); ;
    }

    public static bool AreEqual(double first, double second)
    {
        return Math.Abs(first - second) < 1e-9;
    }
}

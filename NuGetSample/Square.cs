namespace NuGetSample
{
    public class Square
    {
        public int SideLength { get; set; } //Длина ребра квадрата
        public int X { get; set; } //Абсцисса левого нижнего угла квадрата
        public int Y { get; set; } //Ордината левого нижнего угла квадрата

        public Square(int sideLength, int x, int y)
        {
            this.SideLength = sideLength;
            this.X = x;
            this.Y = y;
        }
    }
}
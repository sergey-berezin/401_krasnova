using System.Text;

namespace NuGetSample
{
    public class Solution
    {
        //Экземпляр расстановки квадратов
        public List<Square> Squares { get; set; }
        public int Fitnes { get; set; }
        public Solution(List<Square> squares)
        {
            this.Squares = squares;
        }

        // Рассчёт площади минимального прямоугольника, содержащего все квадраты
        public void CalculateFitness()
        {
            int maxX = Squares.Max(s => s.X + s.SideLength);
            int minX = Squares.Min(s => s.X);
            int maxY = Squares.Max(s => s.Y + s.SideLength);
            int minY = Squares.Min(s => s.Y);
            Fitnes = (maxX - minX) * (maxY - minY); // Площадь прямоугольника
        }

        // Проверка на пересечение квадратов
        public bool IsValid()
        {
            for (int i = 0; i < Squares.Count; ++i)
            {
                for (int j = i + 1; j < Squares.Count; ++j)
                {
                    if (AreIntersecting(Squares[i], Squares[j]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // Проверка пересечения двух квадратов
        private static bool AreIntersecting(Square a, Square b)
        {
            return !(a.X + a.SideLength <= b.X || b.X + b.SideLength <= a.X ||
                     a.Y + a.SideLength <= b.Y || b.Y + b.SideLength <= a.Y);
        }
        public override string ToString()
        {
            int k = 0;
            var res = new StringBuilder();
            foreach (var s in Squares) 
            {
                k += 1;
                res.Append($"{s.SideLength}: ({s.X}, {s.Y}); ");
                if (k == 5 || k == 10)
                {
                    res.Append("\n");
                }
            }
            return res.ToString();
        }
    }

}

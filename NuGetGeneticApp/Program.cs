using NuGetSample;
public class Program
{
    public static void Main()
    {
        var geneticAlgorithm = new GeneticAlgorithm();
        // Инициализация популяции
        geneticAlgorithm.InitializePopulation(100);
        // Ограничение на число поколений
        var bestSolution = geneticAlgorithm.Run(1000);

        Console.WriteLine("Полученное решение:");
        foreach (var square in bestSolution.Squares)
        {
            Console.WriteLine($"Квадрат с длиной стороны {square.SideLength}, X: {square.X}, Y: {square.Y}");
        }

        Console.WriteLine($"Минимальная площадь прямоугольника, содержащего" +
            $" все квадраты: {bestSolution.CalculateFitness()}");
    }
}

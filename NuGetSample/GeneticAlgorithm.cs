namespace NuGetSample
{
    public class GeneticAlgorithm
    {
        private List<Solution> population = new ();
        private readonly Random random = new ();

        // Инициализация популяции случайными решениями
        public void InitializePopulation(int populationSize)
        {
            int index = 0;
            while(index < populationSize)
            {
                var squares = new List<Square>
                {
                    new Square(1, random.Next(0, 10), random.Next(0, 10)),
                    new Square(1, random.Next(0, 10), random.Next(0, 10)),
                    new Square(2, random.Next(0, 10), random.Next(0, 10)),
                    new Square(2, random.Next(0, 10), random.Next(0, 10)),
                    new Square(3, random.Next(0, 10), random.Next(0, 10))
                };

                var solution = new Solution(squares);
                if (solution.IsValid()) // Добавляем решение, если оно валидно
                {
                    population.Add(solution);
                    ++index;
                }
            }
        }

        // Основной метод для запуска алгоритма
        public Solution Run(int maxGenerations)
        {
            int generation = 0;
            while (generation < maxGenerations)
            {
                generation++;

                // Выбор лучшего решения в текущем поколении
                Solution bestSolution = population.OrderBy(s => s.CalculateFitness()).First();

                Console.WriteLine($"Поколение №: {generation}, Мин. площадь: {bestSolution.CalculateFitness()}");

                // Следующий этап эволюции популяции
                EvolvePopulation();

                // Проверка нажатия клавиши для завершения
                if (Console.KeyAvailable)
                {
                    Console.ReadKey();
                    return bestSolution;
                }
            }

            // Возвращение лучшего решения по окончании заданного числа поколений
            return population.OrderBy(s => s.CalculateFitness()).First();
        }

        // Эволюция популяции
        private void EvolvePopulation()
        {
            List<Solution> newPopulation = new List<Solution>();

            // Отбор лучших решений
            var selectedSolutions = population.OrderBy(s => s.CalculateFitness()).Take(population.Count / 2).ToList();

            // Кроссовер и мутация
            while (newPopulation.Count < population.Count)
            {
                var parent1 = selectedSolutions[random.Next(selectedSolutions.Count)];
                var parent2 = selectedSolutions[random.Next(selectedSolutions.Count)];

                var child = Crossover(parent1, parent2);
                Mutate(child);

                if (child.IsValid()) // Добавляем решение, если оно валидно
                {
                    newPopulation.Add(child);
                }
            }

            population = newPopulation;
        }

        // Кроссовер: объединение двух родителей
        private Solution Crossover(Solution parent1, Solution parent2)
        {
            var squares = new List<Square>();

            int crossoverPoint = random.Next(1, parent1.Squares.Count-1); // Выбор случайной точки разделения

            // Первую часть берём от первого родителя
            for (int i = 0; i < crossoverPoint; i++)
            {
                squares.Add(new Square(parent1.Squares[i].SideLength, parent1.Squares[i].X, parent1.Squares[i].Y));
            }

            // Вторую часть берём от второго родителя
            for (int i = crossoverPoint; i < parent2.Squares.Count; i++)
            {
                squares.Add(new Square(parent2.Squares[i].SideLength, parent2.Squares[i].X, parent2.Squares[i].Y));
            }

            return new Solution(squares);
        }
        
        // Мутация: случайное изменение координат квадрата
        private void Mutate(Solution solution)
        {
            foreach (var square in solution.Squares)
            {
                if (random.NextDouble() < 0.1) // 10% - шанс мутации
                {
                    square.X += random.Next(-1, 2);
                    square.Y += random.Next(-1, 2);
                }
            }
        }
    }
}
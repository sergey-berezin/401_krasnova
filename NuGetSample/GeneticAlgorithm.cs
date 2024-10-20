using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace NuGetSample
{
    public class GeneticAlgorithm
    {
        private List<Solution> population = new ();
        private readonly Random random = new ();
        private object populationLock = new object(); // Для синхронизации доступа к списку популяции

        // Инициализация популяции случайными решениями
        public void InitializePopulation(int populationSize, int a, int b, int c)
        {
            int index = 0;
            Parallel.For(0, populationSize, i =>
            {
                while (index < populationSize)
                {
                    var squares = new List<Square>();

                    for (int j = 0; j < a; j++)
                    {
                        squares.Add(new Square(1, random.Next(0, 10), random.Next(0, 10)));
                    }
                    for (int j = 0; j < b; j++)
                    {
                        squares.Add(new Square(2, random.Next(0, 10), random.Next(0, 10)));
                    }
                    for (int j = 0; j < c; j++)
                    {
                        squares.Add(new Square(3, random.Next(0, 10), random.Next(0, 10)));
                    }

                    var solution = new Solution(squares);


                    // Синхронизируем доступ к общему ресурсу (список популяции)
                    if (solution.IsValid())
                    {
                        lock (populationLock) // Блокировка для защиты доступа
                        {
                            if (index < populationSize) // Проверяем лимит популяции
                            {
                                population.Add(solution);
                                index++;
                            }
                        }
                    }
                }
            });
        }

        // Основной метод для запуска алгоритма
        public Solution Run(int maxGenerations, CancellationToken token, Action<int, int, Solution> onProgressUpdate)
        {
            int generation = 0;
            while (generation < maxGenerations)
            {
                generation++;

                Parallel.ForEach(population, solution =>
                {
                    solution.CalculateFitness();
                });
                
                // Выбор лучшего решения в текущем поколении
                Solution bestSolution = population.OrderBy(s => s.Fitnes).First();
                onProgressUpdate(generation, bestSolution.Fitnes, bestSolution);
                
                if(token.IsCancellationRequested)
                {
                    return bestSolution;
                }

                if (generation < maxGenerations)
                {
                    EvolvePopulation();
                }
            }
            return population.OrderBy(s => s.Fitnes).First();
        }

        // Эволюция популяции
        private void EvolvePopulation()
        {
            List<Solution> newPopulation = new List<Solution>();
            var selectedSolutions = population.OrderBy(s => s.Fitnes).Take(population.Count / 2).ToList();
            Parallel.For(0, population.Count, (i) =>
            {
                while (newPopulation.Count < population.Count)
                {
                    // Выбор родителей случайным образом
                    var parent1 = selectedSolutions[random.Next(selectedSolutions.Count)];
                    var parent2 = selectedSolutions[random.Next(selectedSolutions.Count)];

                    // Выполняем кроссовер
                    var child = Crossover(parent1, parent2);

                    // Выполняем мутацию
                    Mutate(child);

                    // Проверяем валидность ребенка
                    if (child.IsValid())
                    {
                        // Блокируем доступ к общему ресурсу newPopulation
                        lock (populationLock)
                        {
                            if (newPopulation.Count < population.Count)
                            // Добавляем ребенка в новую популяцию
                            { newPopulation.Add(child); }
                        }
                    }
                }
            });
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
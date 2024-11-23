using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NuGetSample;
using System.IO;
using System.Text.Json;

namespace NuGetParallelWnd
{
    public partial class MainWindow : Window
    {
        private const string ExperimentsFilePath = "experiments.json";
        public List<Experiment> Experiments { get; set; } = new();
        private GeneticAlgorithm geneticAlgorithm = new GeneticAlgorithm();
        private CancellationTokenSource cancellationTokenSource;
        public MainWindow()
        {
            InitializeComponent();
            LoadExperiments();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int count1x1 = int.Parse(((ComboBoxItem)comboBox1x1.SelectedItem).Content.ToString());
            int count2x2 = int.Parse(((ComboBoxItem)comboBox2x2.SelectedItem).Content.ToString());
            int count3x3 = int.Parse(((ComboBoxItem)comboBox3x3.SelectedItem).Content.ToString());

            cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    RunGeneticAlgorithm(count1x1, count2x2, count3x3, cancellationTokenSource.Token);
                }, cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }
        private void RunGeneticAlgorithm(int side1, int side2, int side3, CancellationToken token)
        {
            geneticAlgorithm = new GeneticAlgorithm();
            // Инициализация популяции
            geneticAlgorithm.InitializePopulation(100, side1, side2, side3);
            // Ограничение на число поколений
            Solution bestSolution = geneticAlgorithm.Run(1000, token, (generation, bestFitness, bestSolution) =>
            {
                Dispatcher.Invoke(() =>
                {
                    ProgressTextBlock.Text = $"{generation}";
                    BestMetricTextBlock.Text = $"{bestFitness}";
                    BestSolution.Text = bestSolution.ToString();
                });
            });
            Dispatcher.Invoke(() =>
            {
                canvas.Children.Clear();
            });
            foreach (var square in bestSolution.Squares)
            {
                Dispatcher.Invoke(() =>
                {
                    DrawSquare(square);
                });
            }
        }
        private void DrawSquare(Square square)
        {

            Brush color;
            switch(square.SideLength)
            {
                case 1:
                    color = Brushes.LightSeaGreen; break;
                case 2:
                    color = Brushes.Coral; break;
                case 3:
                    color = Brushes.Gold; break;
                default:
                    color = Brushes.Gray;
                    break;
            }
            Rectangle rect = new Rectangle
            {
                Width = square.SideLength * 50,
                Height = square.SideLength * 50,
                Stroke = Brushes.Black,
                Fill = color
            };

            Canvas.SetLeft(rect, square.X * 50);
            Canvas.SetBottom(rect, square.Y * 50);

            canvas.Children.Add(rect);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource?.Cancel();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
        }

        private void SaveExperiments()
        {
            var json = JsonSerializer.Serialize(Experiments, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ExperimentsFilePath, json);
        }

        private void SaveExperiment(Experiment experiment)
        {
            Experiments.Add(experiment);
            SaveExperiments();
            ExperimentsListBox.ItemsSource = null;
            ExperimentsListBox.ItemsSource = Experiments;
        }
        
        private void LoadExperiments()
        {
            if (File.Exists(ExperimentsFilePath))
            {
                var json = File.ReadAllText(ExperimentsFilePath);
                Experiments = JsonSerializer.Deserialize<List<Experiment>>(json);
                ExperimentsListBox.ItemsSource = null;
                ExperimentsListBox.ItemsSource = Experiments;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var experimentName = Microsoft.VisualBasic.Interaction.InputBox("Введите имя эксперимента:", "Сохранение эксперимента");
            if (!string.IsNullOrEmpty(experimentName))
            {
                var experiment = new Experiment
                {
                    Name = experimentName,
                    Population = geneticAlgorithm.population,
                    Generation = 0,
                    MaxGenerations = 1000,
                };
                SaveExperiment(experiment);
            }
        }

        private void RunLoadedGeneticAlgorithm(Experiment exp, CancellationToken token)
        {
            geneticAlgorithm = new GeneticAlgorithm();
            geneticAlgorithm.population = exp.Population;
            Solution bestSolution = geneticAlgorithm.Run(1000, token, (generation, bestFitness, bestSolution) =>
            {
                Dispatcher.Invoke(() =>
                {
                    ProgressTextBlock.Text = $"{generation}";
                    BestMetricTextBlock.Text = $"{bestFitness}";
                    BestSolution.Text = bestSolution.ToString();
                });
            });
            Dispatcher.Invoke(() =>
            {
                canvas.Children.Clear();
            });
            foreach (var square in bestSolution.Squares)
            {
                Dispatcher.Invoke(() =>
                {
                    DrawSquare(square);
                });
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var selectedExperiment = ExperimentsListBox.SelectedItem as Experiment;
            if (selectedExperiment != null)
            {
                cancellationTokenSource = new CancellationTokenSource();
                try
                {
                    await Task.Factory.StartNew(() =>
                    {
                        RunLoadedGeneticAlgorithm(selectedExperiment, cancellationTokenSource.Token);
                    }, cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }
                finally
                {
                    cancellationTokenSource.Dispose();
                }
            }
        }
        
    }
}

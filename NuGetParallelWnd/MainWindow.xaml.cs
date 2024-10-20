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

namespace NuGetParallelWnd
{
    public partial class MainWindow : Window
    {
        private CancellationTokenSource cancellationTokenSource;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int count1x1 = int.Parse(((ComboBoxItem)comboBox1x1.SelectedItem).Content.ToString());
            int count2x2 = int.Parse(((ComboBoxItem)comboBox2x2.SelectedItem).Content.ToString());
            int count3x3 = int.Parse(((ComboBoxItem)comboBox3x3.SelectedItem).Content.ToString());

            cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => RunGeneticAlgorithm(count1x1, count2x2, count3x3, cancellationTokenSource.Token), cancellationTokenSource.Token);
        }
        private void RunGeneticAlgorithm(int side1, int side2, int side3, CancellationToken token)
        {
            var geneticAlgorithm = new GeneticAlgorithm();
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
                    color = Brushes.Gray;       // Цвет по умолчанию
                    break;
            }
            Rectangle rect = new Rectangle
            {
                Width = square.SideLength * 50,  // Умножаем размер стороны на коэффициент для наглядности
                Height = square.SideLength * 50,
                Stroke = Brushes.Black,    // Черная рамка квадрата
                Fill = color   // Заполнение квадрата
            };

            // Устанавливаем координаты квадрата на Canvas
            Canvas.SetLeft(rect, square.X * 50);
            Canvas.SetTop(rect, square.Y * 50);

            // Добавляем квадрат на Canvas
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
    }
}

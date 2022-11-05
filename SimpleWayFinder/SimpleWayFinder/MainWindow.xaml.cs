using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;
using System.Security;
using System.Windows.Media.Imaging;

namespace SimpleWayFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Размер клеток, размер сетки
        private double cellSize;
        private int gridRows;
        private int gridColumns;

        // Структура для открытия референса
        private OpenFileDialog ofd;

        // Сетка
        private ColorCell[,] ccArray;

        // Путь
        private List<Position> result_path;

        public MainWindow()
        {
            InitializeComponent();

            // Первичная инициализация
            buttonStart.IsEnabled = false;
            wayGrid.IsEnabled = false;
            cellSize = 30;
            gridRows = 2;
            gridColumns = 2;
        }

        // Изменяет ширину
        private void sliderCol_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbSliderCol.Content = sliderCol.Value;
            gridColumns = (int)sliderCol.Value;
        }

        // Изменяет высоту
        private void sliderRow_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbSliderRow.Content = sliderRow.Value;
            gridRows = (int)sliderRow.Value;
        }

        // Инициализировать сетку
        private void buttonCreateGrid_Click(object sender, RoutedEventArgs e)
        {
            buttonStart.IsEnabled = true;
            wayGrid.IsEnabled = true;

            ccArray = new ColorCell[gridRows, gridColumns];

            wayGrid.Children.Clear();

            // Необходимо изменить (плохая реализация масштабирования)
            if (gridColumns > gridRows) {
                cellSize = (window.ActualWidth - 20) / gridColumns;
            } else
            {
                cellSize = (window.ActualHeight - 70)/ gridRows;
            }

            double left = 0, top = 0;

            for (int i = 0; i < gridRows; i++)
            {
                for (int j = 0; j < gridColumns; j++)
                {
                    ccArray[i, j] = new ColorCell();
                    ccArray[i, j].Margin = new Thickness(left, top, 0, 0);
                    ccArray[i, j].Height = cellSize;
                    ccArray[i, j].Width = cellSize;
                    left += cellSize;

                    wayGrid.Children.Add(ccArray[i, j]);
                }
                left = 0;
                top += cellSize;
            }
        }

        // Реализация открытия референса
        private void buttonOpenRef_Click(object sender, RoutedEventArgs e)
        {
            ofd = new OpenFileDialog();
            // Форматы
            ofd.Title = "Выбор референса";
            ofd.Filter = "Все изображения |*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";

            if (ofd.ShowDialog() == true)
            {
                try
                {
                    imgRef.Source = new BitmapImage(new Uri(ofd.FileName));
                    imgRef.Width = window.ActualWidth - 20;
                    imgRef.Height = window.ActualHeight - 70;
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Ошибка доступа.\n\nТекст ошибки: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        // Реализация поиска
        private bool makeSearch()
        {
            // Прямоугольный граф-сетка
            var grid = new GridGraph(gridColumns, gridRows);

            // Список препятствий
            List<Tuple<int, int>> list_of_obstacles = new List<Tuple<int, int>>();

            // Начальная и конечная точки
            Tuple<int, int> start = null;
            Tuple<int, int> target = null;

            // Сбор данных
            for (int i = 0; i < gridRows; i++)
            {
                for (int j = 0; j < gridColumns; j++)
                {
                    if (ccArray[i, j].state == CellStates.OBSTACLE) {
                        list_of_obstacles.Add(Tuple.Create(j, i));
                    }

                    if (ccArray[i, j].state == CellStates.START) {
                        start = Tuple.Create(j, i);
                    }

                    if (ccArray[i, j].state == CellStates.END) {
                        target = Tuple.Create(j, i);
                    }
                }
            }

            // Обработка некорректного ввода
            if (start == null || target == null)
            {
                MessageBox.Show("Нет отправной/целевой точки!");
                return false;
            }

            // Добавление препятствий в граф
            foreach (var obstacle in list_of_obstacles) {
                grid.obstacles.Add(new Position(obstacle.Item1, obstacle.Item2));
            }

            // Выполнение поиска
            var se = new SearchEngine(grid, new Position(start.Item1, start.Item2),
                                    new Position(target.Item1, target.Item2));

            // Построение пути
            Position current = new Position(target.Item1, target.Item2);
            Position s = new Position(start.Item1, start.Item2);
            result_path = new List<Position>();

            while (current.x != s.x || current.y != s.y)
            {
                try
                {
                    current = se.prevNode[current];
                }
                catch 
                {
                    MessageBox.Show("Поиск не дал результатов...");
                    return false;
                }
                
                result_path.Add(current);
            }
            return true;
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            if (makeSearch()) {
                // Обновление сетки (вывод пути на экран)

                foreach (Position tuple in result_path)
                {
                    if (ccArray[tuple.y, tuple.x].state != CellStates.START)
                    {
                        ccArray[tuple.y, tuple.x].state = CellStates.PATH;
                        ccArray[tuple.y, tuple.x].updateColor();
                    }
                }

                wayGrid.Children.Clear();

                for (int i = 0; i < gridRows; i++)
                {
                    for (int j = 0; j < gridColumns; j++)
                    {
                        wayGrid.Children.Add(ccArray[i, j]);
                    }
                }

                wayGrid.IsEnabled = false;

                buttonStart.IsEnabled = false;
            }
        }
    }
}

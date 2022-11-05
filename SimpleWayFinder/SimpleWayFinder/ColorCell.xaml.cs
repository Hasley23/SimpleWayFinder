using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SimpleWayFinder
{
    /// <summary>
    /// Possible cell states for the algorithm
    /// </summary>
    public enum CellStates { EMPTY, OBSTACLE, START, END, PATH}
    /// <summary>
    /// Interaction logic for ColorCell.xaml
    /// </summary>
    public partial class ColorCell : UserControl
    {
        public CellStates state { get; set; }

        public ColorCell()
        {
            InitializeComponent();

            // Инициализация пустой клетки
            state = CellStates.EMPTY;
        }

        public void updateColor() {
            // Обновить цвет клетки по ее состоянию
            switch (state)
            {
                case CellStates.EMPTY:
                    Background = Brushes.Transparent;
                    break;
                case CellStates.OBSTACLE:
                    Background = Brushes.Black;
                    break;
                case CellStates.START:
                    Background = Brushes.Blue;
                    break;
                case CellStates.END:
                    Background = Brushes.Red;
                    break;
                case CellStates.PATH:
                    Background = Brushes.Yellow;
                    break;
            }
        }

        private void changeObstacleState() {
            // Создать или уничтожить препятствие
            if (state != CellStates.OBSTACLE)
            {
                state = CellStates.OBSTACLE;
            }
            else
            {
                state = CellStates.EMPTY;
            }
        }

        private void changeWayPointState() {
            // Создать начальную или конечную точку
            if (state != CellStates.START)
            {
                state = CellStates.START;
            }
            else
            {
                state = CellStates.END;
            }
        }

        private void onMouseEnter(object sender, MouseEventArgs e) {
            // Режим рисования или подсветка курсора 
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                changeObstacleState();
                updateColor();
            }
            else
            {
                // Цвет подсветки курсора
                Background = Brushes.Gray;
            }
        }

        private void onMouseLeave(object sender, MouseEventArgs e)
        {
            // Убрать подсветку курсора
            updateColor();
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Нарисовать препятствие
            changeObstacleState();
            updateColor();
        }

        private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Установить отправную или конечную точку
            changeWayPointState();
            updateColor();
        }
    }
}

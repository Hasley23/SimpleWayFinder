using System;
using System.Collections.Generic;

namespace SimpleWayFinder
{
    // Структура, определяющая позицию в сетке (графе)
    public struct Position
    {
        public readonly int x, y;
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }


    public class GridGraph
    {
        // Направления для работы с соседними клетками в сетке
        private Position[] directions = new[] {
            new Position(1, 0), // вправо
            new Position(0, 1), // вниз
            new Position(0, -1), // вверх
            new Position(-1, 0), // влево
            new Position(1, 1), // вправо вниз
            new Position(-1, 1), // влево вниз
            new Position(1, -1), // вправо вверх
            new Position(-1, -1) // влево вверх
        };

        // Ширина и высота сетки
        private int width, height;

        // Препятствия 
        public HashSet<Position> obstacles = new HashSet<Position>();

        public GridGraph(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        // Проверка выхода за границы сетки
        private bool isValidPosition(Position pos)
        {
            return 0 <= pos.y && pos.y < height && 0 <= pos.x && pos.x < width;
        }

        // Проверка на препятствие
        private bool isNotObstacle(Position pos)
        {
            return !obstacles.Contains(pos);
        }

        // Возвращает перечисление соседних клеток
        public IEnumerable<Position> findNeighbors(Position pos)
        {
            foreach (var direction in directions)
            {
                Position neighbor = new Position(pos.x + direction.x, pos.y + direction.y);
                // Добавляем позицию в перечисление, если она находится в границах сетки и пуста
                if (isValidPosition(neighbor) && isNotObstacle(neighbor))
                {
                    yield return neighbor;
                }
            }
        }
    }

    public class SearchEngine
    {
        // Указывает на предыдущий узел
        public Dictionary<Position, Position> prevNode = new Dictionary<Position, Position>();
        // Хранит стоимости перемещений
        public Dictionary<Position, double> nodeCosts = new Dictionary<Position, double>();

        // Эвристическая функция
        static public double heuristic(Position a, Position b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }
        
        // Алгоритм A*
        public SearchEngine(GridGraph graph, Position start, Position target)
        {
            // Список вместо двоичной кучи (отсутствует в C#)
            List<Tuple<Position, double>> processingQueue = new List<Tuple<Position, double>>();

            // Начальная позиция
            processingQueue.Add(Tuple.Create(start, 0.0));

            // Начальная позиция указывает на себя
            prevNode[start] = start;

            // Начальная стоимость нулевая
            nodeCosts[start] = 0;

            // Пока очередь не кончится
            while (processingQueue.Count > 0)
            {
                // *** Ищем минимальное значение priority ***
                int minValue = 0;

                for (int i = 0; i < processingQueue.Count; i++)
                {
                    if (processingQueue[i].Item2 < processingQueue[minValue].Item2)
                    {
                        minValue = i;
                    }
                }
                // ***

                var current = processingQueue[minValue].Item1;
                processingQueue.RemoveAt(minValue);
                

                // Если дошли до цели - конец
                if (current.Equals(target))
                {
                    break;
                }

                // Добавляем и оцениваем соседей
                foreach (var neighbor in graph.findNeighbors(current))
                {
                    double newCost = nodeCosts[current];
                    if (!nodeCosts.ContainsKey(neighbor) || newCost < nodeCosts[neighbor])
                    {
                        nodeCosts[neighbor] = newCost;
                        double priority = newCost + heuristic(neighbor, target);
                        processingQueue.Add(Tuple.Create(neighbor, priority));
                        prevNode[neighbor] = current;
                    }
                }
            }
        }
    }
}

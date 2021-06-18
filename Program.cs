using System;
using System.Collections.Generic;
using System.Text;

namespace Snake
{
    class Program
    {
        private static int width;
        private static int height;
        private static int[] grid;
        private static List<char> snake;
        private static Heading heading;
        private static int headPos;
        private static Random random;
        private static int fps;
        private static int score;

        static void Main(string[] args)
        {
            while (true)
            {
                Setup();
                Run();
                if (Console.ReadKey(true).Key == ConsoleKey.Escape) break;
                Console.Clear();
            }
        }

        private static void Run()
        {
            while (true)
            {
                HandleKey();

                try { Move(); }
                catch (Exception e)
                {
                    Console.SetCursorPosition(width / 2, height / 2);
                    Console.Write("Game Over");
                    Console.SetCursorPosition(width / 2, height / 2 + 1);
                    if (e is IndexOutOfRangeException) Console.WriteLine("Hit Wall");
                    else Console.WriteLine(e.Message);
                    break;
                }

                Draw();

                System.Threading.Thread.Sleep(1000 / fps);
            }
        }

        private static void HandleKey()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow && heading != Heading.Down) heading = Heading.Up;
                if (key == ConsoleKey.RightArrow && heading != Heading.Left) heading = Heading.Right;
                if (key == ConsoleKey.DownArrow && heading != Heading.Up) heading = Heading.Down;
                if (key == ConsoleKey.LeftArrow && heading != Heading.Right) heading = Heading.Left;
            }
        }

        private static void Draw()
        {
            StringBuilder buffer = new StringBuilder(width + grid.Length + 4 * height + width);
            for (int i = 0; i < width + 1; i++) buffer.Append("X");
            for (int i = 0; i < grid.Length; i++)
            {
                if (i % width == 0) buffer.Append("X" + Environment.NewLine + "X");
                if (grid[i] == -1) buffer.Append('#');
                else if (grid[i] > 0) buffer.Append(snake[grid[i]]);
                else buffer.Append(' ');
            }
            buffer.Append("X" + Environment.NewLine);
            for (int i = 0; i < width + 2; i++) buffer.Append("X");
            Console.SetCursorPosition(0, 0);
            Console.Write(buffer);
        }

        private static void Move()
        {
            for (int i = 0; i < grid.Length; i++)
            {
                if (grid[i] > 0) grid[i]--;
            }
            int lookup = -1;
            switch (heading)
            {
                case Heading.Up: lookup = headPos - width; break;
                case Heading.Right: lookup = headPos + 1; if (lookup % width == 0) throw new Exception("Hit Wall"); break;
                case Heading.Down: lookup = headPos + width; break;
                case Heading.Left: lookup = headPos - 1; if (lookup % width == width - 1) throw new Exception("Hit Wall"); break;
            }

            if (grid[lookup] == -1)
            {
                snake.Insert(2, 'o');
                grid[random.Next(grid.Length)] = -1;
                grid[lookup] = 0;
                if (fps < 60) fps++;
                Console.Title = $"Snake Score: {++score}";
            }

            if (grid[lookup] == 0) grid[lookup] = snake.Count - 1;
            else throw new Exception("Hit self");
            headPos = lookup;
        }

        private static void Setup()
        {
            Console.CursorVisible = false;
            width = 50;
            height = 30;
            Console.WindowHeight = height + 5;
            Console.WindowWidth = width + 5;

            grid = new int[width * height]; 
            for (int i = 0; i < grid.Length; i++) grid[i] = 0;
            grid[grid.Length / 2] = 4;
            headPos = grid.Length / 2;
            snake = new List<char>() { ' ', '.', 'o', 'O', '@' };
            heading = Heading.Right;
            random = new Random();
            grid[random.Next(grid.Length)] = -1;
            fps = 10;
            score = 0;
            Console.Title = "Snake";
        }

        enum Heading
        {
            Up,
            Right,
            Down,
            Left
        }
    }
}

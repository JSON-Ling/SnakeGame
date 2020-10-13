﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.IO;
using System.Data;
using System.Drawing;

namespace SnakeGame
{
    //Defining a struct for the position of the snake
    struct Position
    {
        public int row;
        public int col;
        public Position(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }

    class Snake
    {

        private static int MainMenu()
        {
            //main menu options
            Console.Clear();
            Console.WriteLine("\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "Choose an option:");
            Console.WriteLine("\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "1) Start Game");
            Console.WriteLine("\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "2) Exit");
            Console.Write("\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "Select an option: ");

            switch (Console.ReadLine())
            {
                //level selector
                case "1":
                    return 1;
                //exit the game
                case "2":
                    Environment.Exit(0);
                    return 1;
                default:
                    return 1;
            }
        }

        //Method for the directional positions
        public void Direction(Position[] directions)
        {
            directions[0] = new Position(0, 1);
            directions[1] = new Position(0, -1);
            directions[2] = new Position(1, 0);
            directions[3] = new Position(-1, 0);
        }

        public void DrawFood()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("@");
        }

        //Method to draw the obstacle
        public void DrawObstacle()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("=");
        }

        public void DrawSnakeBody()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("*");
        }

        public void Obstacles(List<Position> obstacles)
        {
            Random rand = new Random();
            obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
            obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
            obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
            obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
            obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
        }

        public void CheckUserInput(ref int direction, byte right, byte left, byte down, byte up)
        {
            //User key pressed statement: depends on which direction the user want to go to get food or avoid obstacle
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo userInput = Console.ReadKey();
                if (userInput.Key == ConsoleKey.LeftArrow)
                {
                    if (direction != right) direction = left;
                }
                if (userInput.Key == ConsoleKey.RightArrow)
                {
                    if (direction != left) direction = right;
                }
                if (userInput.Key == ConsoleKey.UpArrow)
                {
                    if (direction != down) direction = up;
                }
                if (userInput.Key == ConsoleKey.DownArrow)
                {
                    if (direction != up) direction = down;
                }
            }
        }
        /// <summary>
        /// win condition
        /// </summary>
        ///<param name="snakeElements"></param>
        ///<param name="negativePoints"></param>
        public int Wincond(Queue<Position> snakeElements, int negativePoints)
        {
            // initial value = 4, value add per food = 1, win condition = 9
            if(snakeElements.Count==9)
            {
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.Yellow; //text color when display

                int userPoints = (snakeElements.Count - 4) * 100 - negativePoints;
                userPoints = Math.Max(userPoints, 0);

                PrintLinesInCenter("You Win!", "Your points are:" + userPoints, "Press enter to exit the game!");

                while(Console.ReadKey().Key != ConsoleKey.Enter) { }//close the program when "enter" is pressed
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// display text on the screen
        /// </summary>
        /// <param name="lines"></param>
        public static void PrintLinesInCenter(params string[] lines)
        {
            int verticalStart = (Console.WindowHeight - lines.Length) / 2;
            int verticalPosition = verticalStart;
            foreach (var line in lines)
            {
                int horizontalStart = (Console.WindowWidth - line.Length) / 2; //start display from horizontal
                Console.SetCursorPosition(horizontalStart, verticalPosition); //position for the text
                Console.Write(line); // write text
                ++verticalPosition; // next line
            }
        }
        static void Main(string[] args)
        {

            // Set the Foreground color to blue 
            Console.BackgroundColor
                = ConsoleColor.Black;

            // Display current Foreground color 
            Console.ForegroundColor
                     = ConsoleColor.Gray;

            byte right = 0;
            byte left = 1;
            byte down = 2;
            byte up = 3;
            int lastFoodTime = 0;
            int negativePoints = 0;
            int foodDissapearTime = 0;
            double sleepTime = 0;
            Position[] directions = new Position[4];
            Random rand = new Random();
            int showMenu = 0;
            while (showMenu == 0)
            {
                showMenu = MainMenu();//main menu
            }
            if (showMenu == 1)
            { // Normal Mode
                foodDissapearTime = 15000;
                sleepTime = 100;
            }
            if (showMenu == 2)
            { // Hard Mode
                foodDissapearTime = 7500;
                sleepTime = 50;
            }

            Snake s = new Snake();

            // Define direction with characteristic of index of array
            s.Direction(directions);
            List<Position> obstacles = new List<Position>();
            if (showMenu == 1)
            {
                s.Obstacles(obstacles);
            }
            if (showMenu == 2)
            {
                s.Obstacles(obstacles);
                obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
                obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
                obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
                obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
                obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
            }

            //Initializes the direction of the snakes head and the food timer and the speed of the snake.
            int direction = right;
            Console.BufferHeight = Console.WindowHeight;
            lastFoodTime = Environment.TickCount;
            Console.Clear();
            Thread.Sleep(2000);

            //Loop to show obstacles in the console window
            foreach (Position obstacle in obstacles)
            {
                Console.SetCursorPosition(obstacle.col, obstacle.row);
                s.DrawObstacle();
            }

            //Initialise the snake position in top left corner of the windows
            //The snakes length is reduced to 3* instead of 5.
            Queue<Position> snakeElements = new Queue<Position>();
            for (int i = 0; i <= 3; i++)
            {
                snakeElements.Enqueue(new Position(0, i));
            }
            //To position food randomly in the console
            Position food = new Position();
            do
            {
                food = new Position(rand.Next(0, Console.WindowHeight), //Food generated within limits of the console height
                    rand.Next(0, Console.WindowWidth)); //Food generated within the limits of the console width
            }
            //loop to continue putting food in the game
            //put food in random places with "@" symbol
            while (snakeElements.Contains(food));
            Console.SetCursorPosition(food.col, food.row);
            s.DrawFood();
            //during the game, the snake is shown with "*" symbol
            foreach (Position position in snakeElements)
            {
                Console.SetCursorPosition(position.col, position.row);
                s.DrawSnakeBody();
            }
            while (true)
            {
                //negative points increased if the food is not eaten in time
                negativePoints++;
                s.CheckUserInput(ref direction, right, left, down, up);
                //Manages the position of the snakes head.
                Position snakeHead = snakeElements.Last();
                Position nextDirection = directions[direction];
                //Snake position when it goes through the terminal sides
                Position snakeNewHead = new Position(snakeHead.row + nextDirection.row,
                    snakeHead.col + nextDirection.col);
                if (snakeNewHead.col < 0) snakeNewHead.col = Console.WindowWidth - 2;
                if (snakeNewHead.row < 0) snakeNewHead.row = Console.WindowHeight - 2;
                if (snakeNewHead.row >= Console.WindowHeight) snakeNewHead.row = 0;
                if (snakeNewHead.col >= Console.WindowWidth) snakeNewHead.col = 0;

                //Check winning condition
                int winning = s.Wincond(snakeElements, negativePoints);
                if (winning == 1) return;

                //The position of the snake head according the body
                Console.SetCursorPosition(snakeHead.col, snakeHead.row);
                s.DrawSnakeBody();
                
                //Snake head shape when the user presses the key to change his direction
                snakeElements.Enqueue(snakeNewHead);
                Console.SetCursorPosition(snakeNewHead.col, snakeNewHead.row);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                if (direction == right) Console.Write(">"); //Snakes head when moving right
                if (direction == left) Console.Write("<");//Snakes head when moving left
                if (direction == up) Console.Write("^");//Snakes head when moving up
                if (direction == down) Console.Write("v");//Snakes head when moving down
                // food will be positioned in different column and row from snakes head
                if (snakeNewHead.col == food.col && snakeNewHead.row == food.row)
                {
                    Console.Beep();//Beep when food is eaten
                    do
                    {
                        food = new Position(rand.Next(0, Console.WindowHeight),
                            rand.Next(0, Console.WindowWidth));
                    }
                    //if the snake consumes the food, lastfoodtime will be reset
                    //new food will be drawn and the snakes speed will increase
                    while (snakeElements.Contains(food));
                    lastFoodTime = Environment.TickCount;
                    Console.SetCursorPosition(food.col, food.row);
                    s.DrawFood();
                    sleepTime--;
                    Position obstacle = new Position();
                    do
                    {
                        obstacle = new Position(rand.Next(0, Console.WindowHeight),
                            rand.Next(0, Console.WindowWidth));
                    }
                    //new obstacle will not be placed in the current position of the snake and previous obstacles.
                    //new obstacle will not be placed at the same row & column of food
                    while (snakeElements.Contains(obstacle) ||
                        obstacles.Contains(obstacle) ||
                        (food.row != obstacle.row && food.col != obstacle.row));
                    obstacles.Add(obstacle);
                    Console.SetCursorPosition(obstacle.col, obstacle.row);
                    s.DrawObstacle();
                }
                else
                {
                    // snakes movement shown by blank spaces
                    Position last = snakeElements.Dequeue();
                    Console.SetCursorPosition(last.col, last.row);
                    Console.Write(" ");
                }
                //if snake didnt eat in time, 50 will be added to negative points
                //draw new food randomly after the previous one is eaten
                if (Environment.TickCount - lastFoodTime >= foodDissapearTime)
                {
                    negativePoints = negativePoints + 50;
                    Console.SetCursorPosition(food.col, food.row);
                    Console.Write(" ");
                    do
                    {
                        food = new Position(rand.Next(0, Console.WindowHeight),
                            rand.Next(0, Console.WindowWidth));
                    }
                    while (snakeElements.Contains(food));
                    lastFoodTime = Environment.TickCount;
                }
                //draw food with @ symbol
                Console.SetCursorPosition(food.col, food.row);
                s.DrawFood();
                //snake moving speed increased by 0.01.
                sleepTime -= 0.01;
                //pause the execution of snake moving speed
                Thread.Sleep((int)sleepTime);
            }
        }
    }
}
    



using static System.Formats.Asn1.AsnWriter;

namespace DrivegameProjectFinal
{
    internal class Program
    {
        static int SYNC_FPS = 0;
        static int SYNC_CAR = 500;

            static int width;
            static int height;

            static int windowWidth;
            static int windowHeight;
            static readonly Random random = new();
            static char[,] road;
            static int score;
            static int carPosition;
            static int carVelocity;
            static bool gameRunning;
            static bool keepPlaying = true;
            static int previousRoadUpdate;

            static void Main()
            {
                Console.CursorVisible = false;

                Initialize();
                LaunchScreen();
                while (keepPlaying)
                {

                    
                    InitializeScene();
                    while (gameRunning)
                    {

                        SYNC_FPS++;

                        HandleInput();
                        Update();
                        Render();
                        if (gameRunning)
                        {

                         Thread.Sleep(33); //change this to synchronization
                        }
                    }
                    if (keepPlaying)
                    {
                        GameOverScreen();
                    }
                }
                Console.Clear();
                Console.WriteLine("Drive was closed.");

                Console.CursorVisible = true;
            }

            static void Initialize()
            {
                windowWidth = Console.WindowWidth;
                windowHeight = Console.WindowHeight;

                width = windowWidth;
                height = windowHeight - 2;

                Console.BufferWidth = windowWidth;
                Console.BufferHeight = windowHeight;
            }

            static void LaunchScreen()
            {
                Console.Clear();
                Console.WriteLine("This is a driving game.");
                Console.WriteLine();
                Console.WriteLine("Stay on the road!");
                Console.WriteLine();
                Console.WriteLine("Use A, W, and D to control your velocity.");
                Console.WriteLine();
                Console.Write("Press [enter] to start...");
                PressEnterToContinue();
            }

            static void InitializeScene()
            {
                const int roadWidth = 10;
                gameRunning = true;
                carPosition = width / 2;
                carVelocity = 0;
                score = 0;
                previousRoadUpdate = 0;
                int leftSide = (width - roadWidth) / 2;
                int rightSide = leftSide + roadWidth + 1;
                road = new char[height, width];
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (j < leftSide || j > rightSide)
                        {
                            road[i, j] = '.';
                        }
                        else
                        {
                            road[i, j] = ' ';
                        }
                    }
                }
            }

            static void Render()
            {
                Console.SetCursorPosition(0, 0);
                for (int i = height - 1; i >= 0; i--)
                {
                    string line = "";
                    for (int j = 0; j < width; j++)
                    {
                        if (i == 1 && j == carPosition)
                        {
                            if (!gameRunning)
                            {
                                line += 'X';
                            }
                            else if (carVelocity < 0)
                            {
                                line += '<';
                            }
                            else if (carVelocity > 0)
                            {
                                line += '>';
                            }
                            else
                            {
                                line += '^';
                            }
                        }
                        else
                        {
                            line += road[i, j];
                        }
                    }
                    Console.WriteLine(line);
                }
                Console.WriteLine("Score: " + score);
            }

            static void HandleInput()
            {


               /* if (SYNC_FPS % SYNC_CAR != 0)
                {
                    return;
                }*/
                while (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.A:
                        case ConsoleKey.LeftArrow:
                            carVelocity = -1;
                            break;
                        case ConsoleKey.D:
                        case ConsoleKey.RightArrow:
                            carVelocity = +1;
                            break;
                        case ConsoleKey.W:
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.S:
                        case ConsoleKey.DownArrow:
                            carVelocity = 0;
                            break;
                        case ConsoleKey.Escape:
                            gameRunning = false;
                            keepPlaying = false;
                            break;
                        case ConsoleKey.Enter:
                            Console.ReadLine();
                            break;
                    }
                }
            }

            static void GameOverScreen()
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Game Over");
                Console.WriteLine("Score: " + score);
                Console.WriteLine("Play Again (Y/N)?");

                bool validInput = false;
                while (!validInput)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Y)
                    {
                        keepPlaying = true;
                        validInput = true;
                    }
                    else if (key == ConsoleKey.N || key == ConsoleKey.Escape)
                    {
                        keepPlaying = false;
                        validInput = true;
                    }
                }
            }

            static void Update()
            {
                // Scroll the road upward
                for (int i = 0; i < height - 1; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        road[i, j] = road[i + 1, j];
                    }
                }

                // Determine road movement
                int roadUpdate;
                if (random.Next(5) < 4)
                {
                    roadUpdate = previousRoadUpdate;
                }
                else
                {
                    roadUpdate = random.Next(3) - 1;
                }

                if (roadUpdate == -1 && road[height - 1, 0] == ' ')
                {
                    roadUpdate = 1;
                }
                if (roadUpdate == 1 && road[height - 1, width - 1] == ' ')
                {
                    roadUpdate = -1;
                }

                if (roadUpdate == -1)
                {
                    // Shift road left
                    for (int i = 0; i < width - 1; i++)
                    {
                        road[height - 1, i] = road[height - 1, i + 1];
                    }
                    road[height - 1, width - 1] = '.';
                }
                else if (roadUpdate == 1)
                {
                    // Shift road right
                    for (int i = width - 1; i > 0; i--)
                    {
                        road[height - 1, i] = road[height - 1, i - 1];
                    }
                    road[height - 1, 0] = '.';
                }

                previousRoadUpdate = roadUpdate;
                carPosition += carVelocity;

                // Check collision
                if (road[1, carPosition] != ' ')
                {
                    gameRunning = false;
                }

                score++;
            }

            static void PressEnterToContinue()
            {
                bool validInput = false;
                while (!validInput)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Enter)
                    {
                        validInput = true;
                    }
                    else if (key == ConsoleKey.Escape)
                    {
                        keepPlaying = false;
                        validInput = true;
                    }
                }
            }
        
    }

}


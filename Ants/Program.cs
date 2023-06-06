using System;
using System.Collections.Generic;
using System.Text;
using static Ants.Program;

namespace Ants;
class Program
{
    static void Main(string[] args)
    {
        World world = new World(31, 31);
        Resource resource = new Resource(9, 5, world);
        Resource resource2 = new Resource(30, 23, world);
        Resource resource3 = new Resource(17, 8, world);
        Resource resource4 = new Resource(29, 15, world);
        Agent agent = new Agent(5, 5, 'A', world);

        while (true)
        {
            Console.Clear();
            Console.WriteLine(agent.currentDirection);
            Console.WriteLine((agent.Y, agent.X));
            Console.WriteLine(world.ToString());
            agent.Move(agent.Think(agent.Look(4)));
            Thread.Sleep(120);
        }
    }

    public class StaticObject
    {
        public World World;
        public int X;
        public int Y;
        public char Tile;
    }

    public class Pheromone : StaticObject
    {
        public Pheromone(int x, int y, World world)
        {
            World = world;
            X = x;
            Y = y;
            Tile = '*';
            World.Instance[X][Y] = Tile;
        }
    }

    public class Resource : StaticObject
    {
        public Resource(int x, int y, World world)
        {
            World = world;
            X = x;
            Y = y;
            Tile = 'R';
            World.Instance[X][Y] = Tile;
        }
    }

    /// <summary>
    /// A class representing an agent in a world, with the ability to move and search for targets within a certain radius.
    /// </summary>
    public class Agent : StaticObject
    {

        public (int dx, int dy) currentDirection = (0, 0); // Initialize with no movement

        public void Move((int x, int y) target)
        {
            // Clear agent on map.
            World.Instance[X][Y] = '*';

            // Calculate the difference between the current position and the target position
            int dx = target.x - X;
            int dy = target.y - Y;

            // Determine the direction of movement (left, right, up, down)
            int dirX = dx > 0 ? 1 : dx < 0 ? -1 : 0;
            int dirY = dy > 0 ? 1 : dy < 0 ? -1 : 0;

            // Update the agent's position
            X += dirX;
            Y += dirY;

            // Handle out of bounds movement
            if (X < 0)
            {
                X = World.Instance.Count - 1;
            }
            else if (X >= World.Instance.Count)
            {
                X = 0;
            }

            if (Y < 0)
            {
                Y = World.Instance[0].Count - 1;
            }
            else if (Y >= World.Instance[0].Count)
            {
                Y = 0;
            }

            // Update agent on map
            World.Instance[X][Y] = Tile;
        }

        public (int x, int y) Think(List<(int x, int y)> options)
        {
            Random random = new Random();

            // Randomly change the direction by 45 degrees
            void randomize()
            {
                if (random.NextDouble() < 0.1) // 10% chance to change direction
                {
                    int angle = random.Next(2); // Randomly choose clockwise or counterclockwise direction
                    if (angle == 0)
                    {
                        currentDirection = (-currentDirection.dy, currentDirection.dx);
                    }
                    else
                    {
                        currentDirection = (currentDirection.dy, -currentDirection.dx);
                    }
                }
            }


            if (options.Count == 0)
            {
                // If there are no options, continue moving in the current direction.
                randomize();
                return (X + currentDirection.dx, Y + currentDirection.dy);
            }

            // Find the option closest to the agent.
            int minDistance = int.MaxValue;
            (int x, int y) closestOption = options[0];
            foreach (var option in options)
            {
                int distance = Math.Abs(X - option.x) + Math.Abs(Y - option.y);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestOption = option;
                }
            }

            // Calculate the difference between the current position and the target position
            int dx = closestOption.x - X;
            int dy = closestOption.y - Y;

            // Update the current movement direction based on the distance to the target
            if (dx > 0 && dy == 0)
            {
                currentDirection = (1, 0);
            }
            else if (dx > 0 && dy > 0)
            {
                currentDirection = (1, 1);
            }
            else if (dx == 0 && dy > 0)
            {
                currentDirection = (0, 1);
            }
            else if (dx < 0 && dy > 0)
            {
                currentDirection = (-1, 1);
            }
            else if (dx < 0 && dy == 0)
            {
                currentDirection = (-1, 0);
            }
            else if (dx < 0 && dy < 0)
            {
                currentDirection = (-1, -1);
            }
            else if (dx == 0 && dy < 0)
            {
                currentDirection = (0, -1);
            }
            else if (dx > 0 && dy < 0)
            {
                currentDirection = (1, -1);
            }
            // Return the position to move to based on the current movement direction
            return (X + currentDirection.dx, Y + currentDirection.dy);
        }

        /// <summary>
        /// Searches the area in front of the agent within a certain range for a target character.
        /// </summary>
        /// <param name="range">The range to search within in front of the agent.</param>
        /// <returns>A list of (x, y) coordinates for each target found.</returns>
        public List<(int y, int x)> Look(int range)
        {
            List<List<char>> matrix = World.Instance;
            List<(int y, int x)> targets = new List<(int y, int x)>();
            int rows = matrix.Count;
            int cols = matrix[0].Count;
            (int y, int x) agentDir = currentDirection;
            int agentX = Y; // Does this need a reversal?
            int agentY = X; // Fuck, seems so

            if(agentDir.y != 0)
            {
                for (int y = (agentY + agentDir.y); Math.Abs(y - (agentY + agentDir.y)) < range; y += agentDir.y)
                {
                    for (int x = agentX - 1; x <= agentX + 1; x++)
                    {
                        if (y < 0 || y >= matrix.Count) break;
                        if (x < 0 || x >= matrix[y].Count) continue;
                        if (matrix[y][x] == 'R')
                        {
                            targets.Add((y, x));
                            Console.WriteLine($"added {(y, x)}");
                        }
                    }
                }
            }

            if(agentDir.x != 0)
            {
                for (int x = (agentX + agentDir.x); Math.Abs(x - (agentX + agentDir.x)) < range; x += agentDir.x)
                {
                    for (int y = agentY - 1; y <= agentY + 1; y++)
                    {
                        if (y < 0 || y >= matrix.Count) continue;
                        if (x < 0 || x >= matrix[y].Count) break;
                        if (matrix[y][x] == 'R')
                        {
                            targets.Add((y, x));
                            Console.WriteLine($"added {(y, x)}");
                        }
                    }
                }
            }
            return targets;
        }

        /// <summary>
        /// Creates a new instance of the Agent class in a world with the given X and Y coordinates.
        /// </summary>
        /// <param name="x">The X coordinate of the agent.</param>
        /// <param name="y">The Y coordinate of the agent.</param>
        public Agent(int x, int y, char tile, World world)
        {
            X = x;
            Y = y;
            Tile = tile;
            World = world;
            world.Instance[X][Y] = Tile;
            currentDirection = (-1, 0);
        }
    }

    /// <summary>
    /// Represents a two-dimensional grid of characters that can be used to create a game world.
    /// </summary>
    /// <remarks>
    /// The World class uses a List of List of characters to represent the grid.
    /// Each cell in the grid can hold a single character.
    /// The grid can be created with a specified number of rows and columns.
    /// </remarks>
    public class World
    {
        public List<List<char>> Instance;

        /// <summary>
        /// Creates a new instance of the World class with the specified dimensions.
        /// </summary>
        /// <param name="sizeX">The number of columns in the world.</param>
        /// <param name="sizeY">The number of rows in the world.</param>
        /// <remarks>
        /// The world is represented as a two-dimensional List of characters.
        /// Each cell in the world is initialized to a space character.
        /// </remarks>
        public World(int sizeX, int sizeY)
        {
            Instance = new List<List<char>>();

            for (int i = 0; i < sizeY; i++)
            {
                List<char> row = new List<char>();
                for (int j = 0; j < sizeX; j++)
                {
                    row.Add('.');
                }
                Instance.Add(row);
            }
        }

        /// <summary>
        /// Returns a string representation of the world, with each cell represented by its character value
        /// and arranged in a top-down visualization of the grid.
        /// </summary>
        /// <returns>A string representation of the world.</returns>
        /// <remarks>
        /// This method iterates over each row and column in the grid, appending the character at each cell to a StringBuilder
        /// object. It adds a newline character after each row to create a top-down visualization of the grid.
        /// </remarks>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int y = 0; y < Instance.Count; y++)
            {
                for (int x = 0; x < Instance[y].Count; x++)
                {
                    sb.Append(Instance[y][x]);
                }

                sb.Append('\n');
            }

            return sb.ToString();
        }
    }
}


using System;
using System.Collections.Generic;
using System.Text;
using static Ants.Program;

// TODO: Being able to directly place objects through constructor is cool, but feels abit like an anti-pattern?
// TODO: Agent shouldn't be able to just see through walls
// TODO: Agent SHOULD be able to look past bounds

namespace Ants;
class Program
{
    static void Main(string[] args)
    {
        Logger logger = new Logger();
        World world = new World(51, 151);
        world.SetLogger(logger);
        GameObject[,] initialState = world.GetInstance;

        Thread mainThread = new Thread(() =>
        {
            while (true)
            {
                if(world.Paused == true)
                {
                        Thread.Sleep(240);
                }
                world.Next();
            }
        });
        mainThread.Start();

        Thread logThread = new Thread(() =>
        {
            logger.Run(initialState);
        });
        logThread.Start();
    }

    public static GameObject[,] DeepCopy2DArray(GameObject[,] originalArray)
    {
        int rows = originalArray.GetLength(0);
        int columns = originalArray.GetLength(1);

        // Create a new 2D array with the same dimensions as the original array
        GameObject[,] copiedArray = new GameObject[rows, columns];

        // Copy the elements from the original array to the copied array
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // For value types like 'int', a simple assignment is enough for deep copy
                copiedArray[i, j] = originalArray[i, j];
            }
        }

        return copiedArray;
    }
}
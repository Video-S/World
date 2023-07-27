using System;
using System.Collections.Generic;
using System.Text;
using static Ants.Program;

// TODO: Being able to directly place objects through constructor is cool, but feels abit like an anti-pattern?
// TODO: Agent shouldn't be able to just see through walls
// TODO: Agent SHOULD be able to look past bounds
// TODO: In the World changes list, vision debug needs to be added.

namespace Ants;
class Program
{
    static void Main(string[] args)
    {
        World world = new World(41, 141);

        _ = new Agent(20, 5, world);

        for (int x = 0; x < world.GetSize.x; x++)
        {
            _ = new Wall(world, 0, x);
        }

        for (int x = world.GetSize.x - 1; x >= 0; x--)
        {
            _ = new Wall(world, world.GetSize.y - 1, x);
        }

        //for (int y = 0; y < world.Size().y; y++)
        //{
        //    _ = new Wall(world, y, world.Size().x - 1);
        //}

        for (int y = 0; y < world.GetSize.y; y++)
        {
            _ = new Wall(world, y, world.GetSize.x / 2);
        }

        // Start off with random resources (every 20th cell)
        int toSpawn = world.GetSize.y * world.GetSize.x / 20;
        for(int i = toSpawn; i >= 0; i--)
        {
            world.SpawnResourceAtRandom();
        }

        // SMALL TEST WORLD FOR BOUND AND BLOCK TESTING //

        //World world = new World(4, 2);
        //new Agent(1, 0, world);
        //new Agent(1, 1, world);
        //new Agent(2, 0, world);
        //new Agent(2, 1, world);

        //for (int x = 0; x < world.Size().x; x++)
        //{
        //    new Wall(world, 0, x);
        //}

        //for (int x = world.Size().x - 1; x >= 0; x--)
        //{
        //    new Wall(world, world.Size().y - 1, x);
        //}

        world.ToConsole();
        while (true)
        {
            world.Next();
            Thread.Sleep(60);
            //Console.ReadKey();
        }
    }
}
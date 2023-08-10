using System;
namespace Ants
{
    public class Hunter : Agent
    {
        public Hunter(World world, int y, int x) : base(y, x, world)
        {
            this.tile = 'H';
            this.color = ConsoleColor.DarkBlue;
            this.range = 6;
            this.resources = 4001;
            this.resourceBaseLine = 6000;
            this.resourcesFromEating = 1750;
            this.reproductionCost = 5500;
            this.livingCost = 10;
            this.AddInterest((typeof(Pheromone), 0.5f));
            this.AddInterest((typeof(Ant), 1f));
        }

        protected override void SpawnNewInstance(int y, int x)
        {
            _ = new Hunter(this.GetWorld, y, x);
        }
    }
}


using System;
namespace Ants
{
    public class Resource : GameObject, IPickUp, IDynamicObject
    {
        public Resource(World world, int y, int x) : base(world, y, x)
        {
            this.tile = 'R';
            this.color = ConsoleColor.DarkGreen;
            this.isBlocker = true;
            this.GetWorld.SetCell(this, y, x);
            this.GetWorld.AddDynamicObject(this);
            this.resources = new Random().Next(150);
        }

        private bool markedForDel = false;
        bool IDynamicObject.MarkedForDel { get => markedForDel; set => markedForDel = value; }
        void IDynamicObject.Run()
        {
            reproduce();
        }

        private int maxReproduction = 3;
        private int currentReproduction = 0;
        private int resources = 0;
        private int resourceBaseLine = 110;
        private int reproductionCost = 110;
        private int livingCost = 1;

        private void reproduce()
        {
            this.resources += livingCost;

            int range = 3;
            int y = this.GetPos.y;
            int x = this.GetPos.x;

            if (currentReproduction >= maxReproduction)
            {
                this.GetWorld.RemoveDynamicObject(this);
                return;
            }
            if (resources < resourceBaseLine) return;

            Random r = new Random();
            y += r.Next(-(range + 1), range + 1);
            x += r.Next(-(range + 1), range + 1);

            (y, x) = this.GetWorld.HandleBounds(y, x);

            if (this.GetWorld.GetCell(y, x) is Floor || this.GetWorld.GetCell(y, x) is Pheromone)
            {
                resources -= reproductionCost;
                this.GetWorld.SetCell(new Resource(this.GetWorld, y, x), y, x);
                currentReproduction++;
            }
        }
    }
}


using System;
namespace Ants
{
    public class Ant : Agent, IPickUp
    {
        public Ant(World world, int y, int x) : base(y, x, world)
        {
            this.tile = 'A';
            this.resources = 501;
            this.resourceBaseLine = 2000;
            this.resourcesFromEating = 100;
            this.reproductionCost = 1500;
            this.livingCost = 7;
            this.range = 4;
            this.AddInterest((typeof(Pheromone), 0.5f));
            this.AddInterest((typeof(Resource), 1f));
        }

        public void PickUp(IInventory? pickedUpBy)
        {
            if (pickedUpBy != null)
            {
                pickedUpBy.AddToInventory(this); // TODO: Do this well. Needs some standard means of cleaning shit up. 
                GameObject obj = this as GameObject;
                _ = new Floor(obj.GetWorld, obj.GetPos.y, obj.GetPos.x);
                obj.GetWorld.RemoveDynamicObject(obj);
            }
        }

        protected override void SpawnNewInstance(int y, int x)
        {
            _ = new Ant(this.GetWorld, y, x);
        }

        protected override void SpawnTrail(int y, int x)
        {
            _ = new Pheromone(this.GetWorld, y, x);
        }
    }
}


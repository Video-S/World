using System;
namespace Ants
{
    public class Pheromone : GameObject, IDynamicObject
    {
        private int fullTimer = 100;
        public int Timer;
        private bool markedForDel = false;
        public Pheromone(World world, int y, int x) : base(world, y, x)
        {
            GameObject? alreadyMarked = world.GetDynamicObjects.Find(item =>
            {
                if (item is Pheromone)
                {
                    if (((Pheromone)item).GetPos == (y, x)) return true;
                    else return false;
                }
                else
                {
                    return false;
                }
            });

            if (alreadyMarked != null)
            {
                ((Pheromone)alreadyMarked).Timer = fullTimer;
                this.GetWorld.SetCell(alreadyMarked, y, x);
            }
            else
            {
                this.Timer = this.fullTimer;
                this.tile = '.';
                this.interestRating = 0.5F;
                this.GetWorld.AddDynamicObject(this);
                this.GetWorld.SetCell(this, y, x);
            }
        }

        bool IDynamicObject.MarkedForDel { get => markedForDel; set => markedForDel = value; }

        void IDynamicObject.Run()
        {
            if (Timer == 0)
            {
                _ = new Floor(this.GetWorld, this.GetPos.y, this.GetPos.x);
                this.markedForDel = true;
            }
            Timer--;
        }
    }
}


using System;
namespace Ants
{
    public class Resource : GameObject, IPickUp
    {
        public Resource(World world, int y, int x) : base(world, y, x)
        {
            this.tile = 'R';
            this.color = ConsoleColor.DarkBlue;
            this.interestRating = 1.0F;
            this.GetWorld.SetCell(this, y, x);
        }
    }
}


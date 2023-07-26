using System;
namespace Ants
{
    public class Floor : GameObject
    {
        public Floor(World world, int y, int x) : base(world, y, x)
        {
            this.tile = '.';
            this.interestRating = 0F;
            this.color = ConsoleColor.DarkGray;
            this.GetWorld.SetCell(this, y, x);
        }
    }
}


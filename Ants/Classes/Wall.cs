using System;
namespace Ants
{
    public class Wall : GameObject
    {
        public Wall(World world, int y, int x) : base(world, y, x)
        {
            this.tile = '*';
            this.isBlocker = true;
            this.GetWorld.SetCell(this, y, x);
        }
    }

}
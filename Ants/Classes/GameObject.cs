using System;
namespace Ants
{
    public class GameObject
    {
        private static int idCount = 0;
        private int id;
        public int Id => this.id;

        private World world;
        public World GetWorld => world;

        private (int y, int x) pos;
        public (int y, int x) GetPos => this.pos;
        public (int y, int x) SetPos { set { this.pos = value; } }

        protected char tile;
        public char GetTile => tile;
        public char SetTile { set { tile = value; } }

        protected ConsoleColor color = Console.ForegroundColor;
        public ConsoleColor GetColor => this.color;
        public ConsoleColor SetColor { set { this.color = value; } }

        protected float interestRating = 0.0F;
        public float InterestRating => interestRating;

        protected bool isBlocker = false;
        public bool IsBlocker => isBlocker;

        public GameObject(World world, int y, int x)
        {
            this.world = world;
            this.pos.y = y;
            this.pos.x = x;
            this.id = idCount;
            idCount++;
        }

        public void ToConsole()
        {
            Console.ForegroundColor = this.color;
            Console.Write($"{this.tile}");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}


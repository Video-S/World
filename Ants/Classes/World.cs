using System;
using System.Runtime.CompilerServices;
using static Ants.Program;

namespace Ants
{
    public class World
    {
        private GameObject[,] instance;
        public GameObject[,] GetInstance => this.instance;

        private Random r = new Random();
        public bool Paused = false;

        private Logger logger;
        public void SetLogger(Logger logger)
        {
            this.logger = logger;
        }

        public World(int height, int width)
        {
            this.instance = new GameObject[height, width];
            this.size = (height, width);
            this.dynamicObjects = new List<GameObject>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _ = new Floor(this, y, x);
                }
            }

            int initialAnts = 3;
            int initialHunters = 3;

            for(int i = 0; i <= initialAnts; i++)
            {
                (int y, int x) posToSpawn = this.GetRandomFreeCell();
                _ = new Ant(this, posToSpawn.y, posToSpawn.x);
            }

            for (int i = 0; i <= initialHunters; i++)
            {
                (int y, int x) posToSpawn = this.GetRandomFreeCell();
                _ = new Hunter(this, posToSpawn.y, posToSpawn.x);
            }

            for (int x = 0; x < this.GetSize.x; x++)
            {
                _ = new Wall(this, 0, x);
            }

            for (int x = this.GetSize.x - 1; x >= 0; x--)
            {
                _ = new Wall(this, this.GetSize.y - 1, x);
            }

            for (int y = 0; y < this.GetSize.y; y++)
            {
                _ = new Wall(this, y, this.GetSize.x / 2);
            }

            // Start off with random resources 
            int resourcesToSpawn = this.GetSize.y * this.GetSize.x / 60;
            for (int i = resourcesToSpawn; i >= 0; i--)
            {
                (int y, int x) posToSpawn = this.GetRandomFreeCell();
                _ = new Resource(this, posToSpawn.y, posToSpawn.x);
            }
        }

        private List<(char ch, (ConsoleColor fg, ConsoleColor bg) color, int y, int x)> consoleChanges = new List<(char ch, (ConsoleColor fg, ConsoleColor bg) color, int y, int x)>();
        public void AddConsoleChange(GameObject obj, int y, int x)
        { 
            consoleChanges.Add((obj.GetTile, (obj.GetColor, obj.GetBGColor), y, x)); 
        }

        public GameObject GetCell(int y, int x)
        {
            (y, x) = this.HandleBounds(y, x);
            return instance[y, x];
        }
        public void SetCell(GameObject obj, int y, int x)
        {
            (y, x) = this.HandleBounds(y, x);
            this.instance[y, x] = obj;
            AddConsoleChange(obj, y, x);
        }

        private List<GameObject> dynamicObjects;
        public List<GameObject> GetDynamicObjects => dynamicObjects;
        public List<GameObject> AddDynamicObject(GameObject obj)
        {
            dynamicObjects.Add(obj);
            return dynamicObjects;
        }
        public List<GameObject> RemoveDynamicObject(GameObject obj)
        {
            dynamicObjects.Remove(obj);
            return dynamicObjects;
        }

        private (int y, int x) size;
        public (int y, int x) GetSize => this.size;

        public (int y, int x) HandleBounds(int y, int x)
        {
            // Handle out of bounds positions
            if (y < 0) y = this.GetSize.y + y;
            else if (y >= this.GetSize.y) y = y - this.GetSize.y;

            if (x < 0) x = this.GetSize.x + x;
            else if (x >= this.GetSize.x) x = x - this.GetSize.x;

            return (y, x);
        }
        public (int y, int x, int dy, int dx) HandleBlocked(int y, int x, int dy, int dx)
        {
            // Handle blocked cells
            GameObject? newPos = this.instance[y, x];
            if (newPos.IsBlocker)
            {
                dy = -dy;
                dx = -dx;

                y += dy * 2;
                x += dx * 2;

                (y, x) = this.HandleBounds(y, x);

                // Handle blocked cells for new y, x
                newPos = this.instance[y, x];
                if (newPos.IsBlocker)
                {
                    y -= dy; // by making it sit in place :)
                    x -= dx;
                }

                return (y, x, dy, dx);
            }
            return (y, x, dy, dx);
        }

        public void Next()
        {
            if (logger.GetQueueLength > 600) this.Paused = true;
            else this.Paused = false;

            // cleanup
            this.dynamicObjects.RemoveAll(item =>
            {
                if (item is IDynamicObject) return ((IDynamicObject)item).MarkedForDel;
                else return false;
            });

            // run all dynamicobjects
            for (int i = 0; i < this.dynamicObjects.Count; i++)
            {
                ((IDynamicObject)this.dynamicObjects[i]).Run();
            }

            logger.AddToQueue(new List<(char ch, (ConsoleColor fg, ConsoleColor bg), int y, int x)>(consoleChanges));
            this.consoleChanges.Clear();
        }

        public (int y, int x) GetRandomFreeCell()
        {
            bool isFree = false;
            int rY = 0;
            int rX = 0;

            while(!isFree)
            {
                rY = this.r.Next(this.size.y);
                rX = this.r.Next(this.size.x - 1);
                GameObject? pos = this.instance[rY, rX];
                if (pos is Floor || pos is Pheromone) isFree = true;
            }

            return (rY, rX);
        }
    }
}


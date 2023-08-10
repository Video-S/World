using System;
namespace Ants
{
    public class Agent : GameObject, IDynamicObject, IInventory
    {
        private Random r = new Random();

        private (int dy, int dx) currentDirection = (0, 0);

        protected virtual void SpawnNewInstance(int y, int x)
        {
            _ = new Agent(y, x, this.GetWorld);
        }

        protected virtual void SpawnTrail(int y, int x)
        {
            _ = new Floor(this.GetWorld, y, x);
        }

        protected int resources = 601;
        protected int resourceBaseLine = 1000;
        protected int resourcesFromEating = 100;
        protected int reproductionCost = 200;
        protected int livingCost = 5;
        private void reproduce()
        {
            resources = resources - livingCost;
            if (this.inventory.Count > 0 && this.resources <= resourceBaseLine)
            {
                resources = resources + resourcesFromEating;
                this.inventory.Dequeue();
            }
            if (resources <= 0)
            {
                _ = new Floor(this.GetWorld, this.GetPos.y, this.GetPos.x);
                this.markedForDel = true;
            }
            if (resources > resourceBaseLine)
            {
                for (int i = this.GetPos.y - 1; i <= this.GetPos.y + 1; i++)
                {
                    if (i < 0 || i > this.GetWorld.GetSize.y - 1) continue;
                    if (GetWorld.GetCell(i, this.GetPos.x) is Floor || GetWorld.GetCell(i, this.GetPos.x) is Pheromone)
                    {
                        this.SpawnNewInstance(this.GetPos.y, this.GetPos.x);
                        this.resources = resources - reproductionCost;
                        return;
                    }
                }

                for (int i = this.GetPos.x - 1; i <= this.GetPos.x + 1; i++)
                {
                    if (i < 0 || i > this.GetWorld.GetSize.x - 1) continue;
                    if (this.GetWorld.GetCell(this.GetPos.y, i) is Floor || GetWorld.GetCell(this.GetPos.y, i) is Pheromone)
                    {
                        this.SpawnNewInstance(this.GetPos.y, this.GetPos.x);
                        this.resources = resources - 200;
                        return;
                    }
                }

            }
        }

        private bool markedForDel = false;
        bool IDynamicObject.MarkedForDel { get => markedForDel; set => markedForDel = value; }

        private Queue<IPickUp> inventory = new Queue<IPickUp>();
        public Queue<IPickUp> Inventory
        {
            get
            {
                return inventory;
            }
        }
        public void AddToInventory(IPickUp item)
        {
            inventory.Enqueue(item);
            item.pickUp(this);
        }
        public void RemoveFromInventory(IPickUp item)
        {
            inventory.Dequeue();
        }

        private void Move(GameObject? target)
        {
            (int y, int x) = this.GetPos;
            (int dy, int dx) = this.currentDirection;

            if (target == null)
            {
                // Keep current direction with a chance of randomization
                (dy, dx) = randomizeDirection(dy, dx);
            }
            else
            {
                // Calculate the difference between the current position and the target position
                dy = target.GetPos.y - y;
                dx = target.GetPos.x - x;

                // dy and dx should not be more than 1 or -1
                if (dy < -1) dy = -1;
                if (dy > 1) dy = 1;
                if (dx < -1) dx = -1;
                if (dx > 1) dx = 1;

                // If there is a target, set currentDirection here
                this.currentDirection = (dy, dx);

                // Can't both be positive or negative. Picks only y or x axis to change.
                if (dy > 0 && dx > 0 || dy < 0 && dx < 0 || dy < 0 && dx > 0 || dy > 0 && dx < 0)
                {
                    int r = this.r.Next(2); // Generates numbers below 2, so 0 and 1
                    if (r == 0) dy = 0;
                    if (r == 1) dx = 0;
                }
            }

            // Update the agent's position
            y += dy;
            x += dx;

            // Handle out of bounds
            (y, x) = this.GetWorld.HandleBounds(y, x);

            // Check if new pos is a pickup and interesting
            bool isInteresting = this.interests.Any((item) =>
            {
                return this.GetWorld.GetCell(y, x).GetType() == item.type;
            });

            if (this.GetWorld.GetCell(y, x) is IPickUp && isInteresting)
            {
                this.AddToInventory((IPickUp)this.GetWorld.GetCell(y, x));
                this.GetWorld.SetCell(new Floor(this.GetWorld, y, x), y, x);
            }

            // Handle blocked cells
            (y, x, dy, dx) = this.GetWorld.HandleBlocked(y, x, dy, dx);

            // Handle out of bounds... AGAIN
            (y, x) = this.GetWorld.HandleBounds(y, x);

            // Update agent on map
            this.GetWorld.SetCell(this, y, x);

            // Clear agent on map.
            this.SpawnTrail(this.GetPos.y, this.GetPos.x);

            // Set new agent pos
            this.SetPos = (y, x);

            // If no target, update currentDirection here
            if (target == null) this.currentDirection = (dy, dx);

            (int dy, int dx) randomizeDirection(int dy, int dx)
            {
                if (this.r.NextDouble() < 0.05) // 10% chance to change direction
                {
                    int angle = this.r.Next(5); // Randomly choose clockwise or counterclockwise direction
                    if (angle == 0) return (-1, 0);
                    else if (angle == 1) return (1, 0);
                    else if (angle == 2) return (0, 1);
                    else return (0, -1);
                }
                else
                {
                    return (dy, dx);
                }
            }
        }

        private List<(Type type, float rating)> interests = new List<(Type type, float rating)>();
        public List<(Type type, float rating)> GetInterests => this.interests;
        public void AddInterest((Type type, float rating) interest)
        {
            interests.Add(interest);
        }
        private GameObject? Think(List<GameObject> options)
        {
            if (options.Count == 0) return null;

            // Get all options with highest interest rating
            float highestRating = 0;
            GameObject highestRated = null;
            foreach(GameObject option in options)
            {
                Type type = option.GetType();
                (Type type, float rating) item = interests.Find((item) =>
                {
                    return item.type == type;
                });
                if (item.rating > highestRating)
                {
                    highestRating = item.rating;
                    highestRated = option;
                }
            }

            if (highestRated == null) return null;

            options = options.FindAll((GameObject option) =>
            {
                return option.GetType() == highestRated.GetType();
            });

            // Always take closest?
            int closestSum = int.MaxValue;
            GameObject closestGO = null;
            foreach (var option in options)
            {
                int yOption = option.GetPos.y;
                int xOption = option.GetPos.x;
                int yAgent = this.GetPos.y;
                int xAgent = this.GetPos.x;

                int diffY = Math.Abs(yAgent - yOption);
                int diffX = Math.Abs(xAgent - xOption);
                int diffSum = diffY + diffX;

                if (diffSum < closestSum)
                {
                    closestSum = diffSum;
                    closestGO = option;
                }
            }

            // Return the position to move to based on the current movement direction
            return closestGO;
        }

        protected int range = 3;
        private List<GameObject> Look()
        {
            List<GameObject> targets = new List<GameObject>();
            (int y, int x) agentDir = this.currentDirection;
            (int agentY, int agentX) = this.GetPos;

            if (agentDir.y != 0)
            {
                for (int x = agentX - 1; x <= agentX + 1; x++)
                {
                    for (int y = (agentY + agentDir.y); Math.Abs(y - (agentY + agentDir.y)) < this.range; y += agentDir.y) 
                    {
                        int boundedY = y;
                        int boundedX = x;

                        if (y < 0 || y >= GetWorld.GetSize.y) (boundedY, boundedX) = this.GetWorld.HandleBounds(y, x);
                        if (x < 0 || x >= GetWorld.GetSize.x) (boundedY, boundedX) = this.GetWorld.HandleBounds(y, x);

                        GameObject obj = this.GetWorld.GetCell(boundedY, boundedX);
                        targets.Add(obj);
                        //_ = new DebugVision(this.GetWorld, boundedY, boundedX);
                        if (obj.IsBlocker) break;                        
                    }
                }
            }

            if (agentDir.x != 0)
            {
                for (int y = agentY - 1; y <= agentY + 1; y++)
                {
                    for (int x = (agentX + agentDir.x); Math.Abs(x - (agentX + agentDir.x)) < range; x += agentDir.x) 
                    {
                        int boundedY = y;
                        int boundedX = x;

                        if (y < 0 || y >= GetWorld.GetSize.y) (boundedY, boundedX) = this.GetWorld.HandleBounds(y, x);
                        if (x < 0 || x >= GetWorld.GetSize.x) (boundedY, boundedX) = this.GetWorld.HandleBounds(y, x);

                        GameObject obj = this.GetWorld.GetCell(boundedY, boundedX);
                        targets.Add(obj);
                        //_ = new DebugVision(this.GetWorld, boundedY, boundedX);
                        if (obj.IsBlocker) break;                       

                    }
                }
            }

            return targets;
        }

        void IDynamicObject.Run()
        {
            this.Move(this.Think(this.Look()));
            this.reproduce();
        }

        public Agent(int y, int x, World world) : base(world, y, x)
        {
            this.tile = 'A';
            this.currentDirection = (new Random().Next(-1, 2), new Random().Next(-1, 2));
            this.color = ConsoleColor.Gray;
            this.isBlocker = true;

            world.SetCell(this, y, x);
            world.AddDynamicObject(this);
        }
    }
}


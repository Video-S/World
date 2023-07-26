using System;
namespace Ants
{
    public class Agent : GameObject, IDynamicObject, IInventory
    {
        private (int dy, int dx) currentDirection = (0, 0);
        private Random r = new Random();
        private bool markedForDel = false;

        private int resources;

        private void reproduce()
        {
            resources = resources - 5;
            if (this.inventory.Count > 0 && this.resources <= 1000)
            {
                resources = resources + 100;
                this.inventory.RemoveAt(this.inventory.Count - 1);
            }
            if (resources <= 0)
            {
                _ = new Floor(this.GetWorld, this.GetPos.y, this.GetPos.x);
                this.markedForDel = true;
            }
            if (resources > 1000)
            {
                for(int i = this.GetPos.y - 1; i <= this.GetPos.y + 1; i++)
                {
                    if (i < 0 || i > this.GetWorld.GetSize.y - 1) continue;
                    if (GetWorld.GetCell(i, this.GetPos.x) is Floor || GetWorld.GetCell(i, this.GetPos.x) is Pheromone)
                    {
                        _ = new Agent(this.GetPos.y, this.GetPos.x, this.GetWorld);
                        this.resources = resources - 200;
                        return;
                    }
                }
                for(int i = this.GetPos.x - 1; i <= this.GetPos.x + 1; i++)
                {
                    if (i < 0 || i > this.GetWorld.GetSize.x - 1) continue;
                    if (this.GetWorld.GetCell(this.GetPos.y, i) is Floor || GetWorld.GetCell(this.GetPos.y, i) is Pheromone)
                    {
                        _ = new Agent(this.GetPos.y, this.GetPos.x, this.GetWorld);
                        this.resources = resources - 200;
                        return;
                    }
                }
                
            }
        }

        private List<IPickUp> inventory = new List<IPickUp>();

        public List<IPickUp> Inventory
        {
            get
            {
                return inventory;
            }
        }

        bool IDynamicObject.MarkedForDel { get => markedForDel; set => markedForDel = value; }

        public void AddToInventory(IPickUp item)
        {
            inventory.Add(item);
        }

        public void RemoveFromInventory(IPickUp item)
        {
            inventory.Remove(item);
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

            // Handle blocked cells
            (y, x, dy, dx) = this.GetWorld.HandleBlocked(y, x, dy, dx);

            // Handle out of bounds... AGAIN
            (y, x) = this.GetWorld.HandleBounds(y, x);

            // Check if new pos is a pickup
            if(this.GetWorld.GetCell(y, x) is IPickUp)
            {
                this.AddToInventory((IPickUp)this.GetWorld.GetCell(y, x));
            }

            // Update agent on map
            this.GetWorld.SetCell(this, y, x);

            // Clear agent on map.
            _ = new Pheromone(this.GetWorld, this.GetPos.y, this.GetPos.x);

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

        private GameObject? Think(List<GameObject> options)
        {
            if (options.Count == 0) return null;
            
            // Determine highest interest rating
            float highestInterest = 0F;
            foreach (var option in options)
            {
                if (option.InterestRating > highestInterest)
                {
                    highestInterest = option.InterestRating;
                }
            }

            // Select all options with highest interest rating
            options = options.FindAll
            (
                delegate (GameObject obj)
                {
                    return obj.InterestRating == highestInterest;
                }
            );

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

        private List<GameObject> Look(int range)
        {
            List<GameObject> targets = new List<GameObject>();
            (int y, int x) agentDir = this.currentDirection;
            (int agentY, int agentX) = this.GetPos;

            if (agentDir.y != 0)
            {
                for (int y = (agentY + agentDir.y); Math.Abs(y - (agentY + agentDir.y)) < range; y += agentDir.y)
                {
                    for (int x = agentX - 1; x <= agentX + 1; x++)
                    {
                        if (y < 0 || y >= GetWorld.GetSize.y) continue;
                        if (x < 0 || x >= GetWorld.GetSize.x) continue;

                        _ = new DebugVision(this.GetWorld, y, x);
                        GameObject obj = this.GetWorld.GetCell(y, x);
                        if (obj is Resource || obj is Pheromone)
                        {
                            targets.Add(obj);
                        }
                    }
                }
            }

            if (agentDir.x != 0)
            {
                for (int x = (agentX + agentDir.x); Math.Abs(x - (agentX + agentDir.x)) < range; x += agentDir.x)
                {
                    for (int y = agentY - 1; y <= agentY + 1; y++)
                    {
                        if (y < 0 || y >= GetWorld.GetSize.y) continue;
                        if (x < 0 || x >= GetWorld.GetSize.x) continue;

                        _ = new DebugVision(this.GetWorld, y, x);
                        GameObject obj = this.GetWorld.GetCell(y, x);
                        if (obj is Resource || obj is Pheromone)
                        {
                            targets.Add(obj);
                        }
                    }
                }
            }

            return targets;
        }

        void IDynamicObject.Run()
        {
            this.Move(this.Think(this.Look(4)));
            this.reproduce();
        }

        public Agent(int y, int x, World world) : base(world, y, x)
        {
            this.tile = 'A';
            this.currentDirection = (-1, 0);
            this.isBlocker = true;
            this.resources = 601;

            world.SetCell(this, y, x);
            world.AddDynamicObject(this);
        }
    }
}


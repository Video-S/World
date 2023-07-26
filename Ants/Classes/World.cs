﻿using System;
using static Ants.Program;

namespace Ants
{
    public class World
    {
        private GameObject[,] instance;

        private List<(int y, int x)> consoleChanges = new List<(int y, int x)>();
        public void AddConsoleChange(int y, int x)
        {
            consoleChanges.Add((y, x));
        }

        public GameObject GetCell(int y, int x)
        {
            return instance[y, x];
        }
        public void SetCell(GameObject obj, int y, int x)
        {
            this.instance[y, x] = obj;
            this.consoleChanges.Add((y, x));
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

        private Random r = new Random();

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
        }

        public (int y, int x) HandleBounds(int y, int x)
        {
            // Handle out of bounds positions
            if (y < 0) y = this.GetSize.y - 1;
            else if (y >= this.GetSize.y) y = 0;

            if (x < 0) x = this.GetSize.x - 1;
            else if (x >= this.GetSize.x) x = 0;

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

            // spawn resources
            this.SpawnResourceAtRandom();

            // Update console
            foreach((int y, int x) change in this.consoleChanges)
            {
                this.RefreshConsole(change.y, change.x);
            }

            this.consoleChanges.Clear();
        }

        public void SpawnResourceAtRandom()
        {
            int rY = this.r.Next(this.size.y);
            int rX = this.r.Next(this.size.x - 1);

            GameObject? pos = this.instance[rY,rX];
            if (pos is Floor || pos is Pheromone) _ = new Resource(this, rY, rX);
        }

        public void RefreshConsole(int y, int x)
        {
            Console.SetCursorPosition(x, y);
            GameObject target = this.GetCell(y, x);

            Console.ForegroundColor = target.GetColor;
            Console.Write(this.GetCell(y, x).GetTile);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void ToConsole()
        {
            Console.SetCursorPosition(0, 0);

            for (int y = 0; y < this.size.y; y++)
            {
                for (int x = 0; x < this.size.x; x++)
                {
                    GameObject? obj = this.instance[y, x];
                    obj.ToConsole();
                }
                Console.Write('\n');
            }
        }
    }
}

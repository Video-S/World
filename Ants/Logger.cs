using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ants
{
	public class Logger
	{
        private Queue<List<(char ch, (ConsoleColor fg, ConsoleColor bg), int y, int x)>> queue;
        public int GetQueueLength => queue.Count;
        public void AddToQueue(List<(char ch, (ConsoleColor fg, ConsoleColor bg), int y, int x)> changes)
        {
            queue.Enqueue(changes);
        }

        public Logger()
        {
            this.queue = new Queue<List<(char ch, (ConsoleColor fg, ConsoleColor bg), int y, int x)>>();
        }
        
        public void Run(GameObject[,] initialState)
        {
            PrintToConsole(initialState);
            while(true)
            {
                if(this.queue.Count() > 0)
                {
                    List<(char ch, (ConsoleColor fg, ConsoleColor bg), int y, int x)> changes = queue.Dequeue();
                    for(int i = 0; i < changes.Count; i++)
                    {
                        this.PrintUpdate(changes[i]);
                    }
                    
                }
                Thread.Sleep(60);
            }
        }

        public void PrintUpdate((char ch, (ConsoleColor fg, ConsoleColor bg) color, int y, int x) change)
        {
            Console.SetCursorPosition(change.x, change.y);
            Console.ForegroundColor = change.color.fg;
            Console.BackgroundColor = change.color.bg;
            Console.Write($"{change.ch}");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void PrintToConsole(GameObject[,] state)
        {
            Console.SetCursorPosition(0, 0);

            for (int y = 0; y < state.GetLength(0); y++)
            {
                for (int x = 0; x < state.GetLength(1); x++)
                {
                    GameObject target = state[y, x];
                    target.ToConsole();
                }
                Console.Write('\n');
            }
        }
    }
}


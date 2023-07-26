using System;
namespace Ants
{
	public class DebugVision: GameObject, IDynamicObject
	{
		private int fullTimer = 1;
		public int Timer;
		private bool markedForDel = false;
		private GameObject target;
		private ConsoleColor targetOrgColor;
		public DebugVision(World world, int y, int x): base(world, y, x)
		{
            GameObject? alreadyMarked = world.GetDynamicObjects.Find(item =>
			{
				if (item is DebugVision)
				{
					if (((DebugVision)item).GetPos == (y, x)) return true;
					else return false;
				}
				else
				{
					return false;
				}
			});

			if(alreadyMarked != null)
			{
				((DebugVision)alreadyMarked).Timer = fullTimer;
			}
			else
			{
                this.Timer = this.fullTimer;
                this.color = ConsoleColor.DarkCyan;
                this.target = world.GetCell(y, x);
                this.targetOrgColor = target.GetColor;
                this.target.SetColor = this.color;
                world.AddDynamicObject(this);
				world.AddConsoleChange(y, x);
            }
		}

        bool IDynamicObject.MarkedForDel { get => markedForDel; set => markedForDel = value; }

        void IDynamicObject.Run()
        {
            if (this.Timer == 0)
			{
				target.SetColor = targetOrgColor;
				this.markedForDel = true;
				this.GetWorld.AddConsoleChange(this.GetPos.y, this.GetPos.x);
            }
            this.Timer--;
        }
    }
}
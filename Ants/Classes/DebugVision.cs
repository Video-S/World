using System;
namespace Ants
{
	// TODO: DebugVision depends on references. No longer works that way.
	// Perhaps I should work within the console changes list, and add and remove things there
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
                this.bgColor = ConsoleColor.DarkCyan;
                this.target = world.GetCell(y, x);
                this.targetOrgColor = target.GetBGColor;
                this.target.SetBGColor = this.bgColor;
                world.AddDynamicObject(this);
				world.AddConsoleChange(target, y, x);
            }
		}

        bool IDynamicObject.MarkedForDel { get => markedForDel; set => markedForDel = value; }

        void IDynamicObject.Run()
        {
            if (this.Timer == 0)
			{
				target.SetBGColor = targetOrgColor;
				this.markedForDel = true;
				this.GetWorld.AddConsoleChange(target, this.GetPos.y, this.GetPos.x);
            }
            this.Timer--;
        }
    }
}
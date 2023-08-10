using System;
namespace Ants
{
	public interface IPickUp
	{
		void pickUp(IInventory? pickedUpBy)
		{
			if(pickedUpBy != null)
			{
				GameObject obj = this as GameObject;
				if(obj != null)
				{
                    _ = new Floor(obj.GetWorld, obj.GetPos.y, obj.GetPos.x);
                    obj.GetWorld.RemoveDynamicObject(obj);
                }
			}
		}
	}
}
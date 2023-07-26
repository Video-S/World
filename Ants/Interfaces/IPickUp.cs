using System;
namespace Ants
{
	// TODO: How will I make this work?
	public interface IPickUp
	{
		void pickUp(IInventory? pickedUpBy)
		{
			if(pickedUpBy != null)
			{
				pickedUpBy.AddToInventory(this);
			}
		}
	}
}

using System;
namespace Ants
{
	public interface IInventory
	{
		public List<IPickUp> Inventory
		{
			get;
		}

		public bool isPickUp(GameObject item)
		{
			if (item is IPickUp) return true;
			else return false;
		}

		public void AddToInventory(IPickUp item)
		{
			Inventory.Add(item);
		}

		public void RemoveFromInventory(IPickUp item)
		{
			Inventory.Remove(item);
		}
	}
}


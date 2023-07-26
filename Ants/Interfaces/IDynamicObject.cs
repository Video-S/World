using System;
namespace Ants
{
	public interface IDynamicObject
	{
		bool MarkedForDel { get; set; }
		void Run();
	}
}


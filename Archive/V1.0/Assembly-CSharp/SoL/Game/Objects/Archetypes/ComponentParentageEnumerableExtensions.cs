using System;
using System.Runtime.CompilerServices;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A4E RID: 2638
	public static class ComponentParentageEnumerableExtensions
	{
		// Token: 0x060051A7 RID: 20903 RVA: 0x001D0FE4 File Offset: 0x001CF1E4
		[return: TupleElementNames(new string[]
		{
			"shouldFilter",
			"matchesAnyFilters"
		})]
		public static ValueTuple<bool, bool> ShouldFilterOut(this ComponentParentage[] parentages, UniqueId componentId, int componentDepth)
		{
			bool flag = false;
			bool flag2 = false;
			if (parentages != null)
			{
				foreach (ComponentParentage componentParentage in parentages)
				{
					flag = (flag || (((componentParentage != null) ? componentParentage.ComponentList : null) != null && componentDepth < componentParentage.ComponentList.Length));
					if (((componentParentage != null) ? componentParentage.ComponentList : null) != null && componentDepth < componentParentage.ComponentList.Length)
					{
						flag2 = (flag2 || componentId == componentParentage.ComponentList[componentDepth].ComponentId);
					}
				}
			}
			return new ValueTuple<bool, bool>(flag, flag2);
		}
	}
}

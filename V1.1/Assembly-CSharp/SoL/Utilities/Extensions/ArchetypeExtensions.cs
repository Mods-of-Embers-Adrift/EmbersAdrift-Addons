using System;
using SoL.Game.Objects.Archetypes;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000323 RID: 803
	public static class ArchetypeExtensions
	{
		// Token: 0x06001631 RID: 5681 RVA: 0x00051849 File Offset: 0x0004FA49
		public static bool TryGetAsType<T>(this BaseArchetype archetype, out T value) where T : class
		{
			value = (archetype as T);
			return value != null;
		}
	}
}

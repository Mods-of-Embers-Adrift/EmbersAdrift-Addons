using System;
using SoL.Game.Objects.Archetypes;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CE4 RID: 3300
	public struct ItemUsage
	{
		// Token: 0x170017F2 RID: 6130
		// (get) Token: 0x060063F5 RID: 25589 RVA: 0x000834EE File Offset: 0x000816EE
		// (set) Token: 0x060063F6 RID: 25590 RVA: 0x000834F6 File Offset: 0x000816F6
		public ArchetypeInstance Instance { readonly get; set; }

		// Token: 0x170017F3 RID: 6131
		// (get) Token: 0x060063F7 RID: 25591 RVA: 0x000834FF File Offset: 0x000816FF
		// (set) Token: 0x060063F8 RID: 25592 RVA: 0x00083507 File Offset: 0x00081707
		public RecipeComponent UsedFor { readonly get; set; }

		// Token: 0x170017F4 RID: 6132
		// (get) Token: 0x060063F9 RID: 25593 RVA: 0x00083510 File Offset: 0x00081710
		// (set) Token: 0x060063FA RID: 25594 RVA: 0x00083518 File Offset: 0x00081718
		public int AmountUsed { readonly get; set; }

		// Token: 0x060063FB RID: 25595 RVA: 0x002081A8 File Offset: 0x002063A8
		public ComponentMaterial FindFulfilledMaterial()
		{
			foreach (ComponentMaterial componentMaterial in this.UsedFor.AcceptableMaterials)
			{
				if (this.Instance.ArchetypeId == componentMaterial.Archetype.Id)
				{
					return componentMaterial;
				}
			}
			return null;
		}
	}
}

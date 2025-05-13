using System;
using SoL.Utilities.Extensions;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AD3 RID: 2771
	public static class MasteryArchetypeExtensions
	{
		// Token: 0x0600559A RID: 21914 RVA: 0x001DF130 File Offset: 0x001DD330
		public static bool CanBeLearnedBy(this MasteryArchetype mastery, GameEntity entity, out string msg)
		{
			msg = null;
			ArchetypeInstance archetypeInstance;
			if (entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(mastery.Id, out archetypeInstance))
			{
				msg = "You already know " + mastery.DisplayName + "!";
				return false;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			MasterySphere masterySphere = mastery.Type.GetMasterySphere();
			foreach (ArchetypeInstance archetypeInstance2 in entity.CollectionController.Masteries.Instances)
			{
				MasteryArchetype masteryArchetype;
				if (archetypeInstance2.Archetype.TryGetAsType(out masteryArchetype))
				{
					if (masteryArchetype.Type == mastery.Type)
					{
						num++;
					}
					if (masteryArchetype.Type.GetMasterySphere() == masterySphere)
					{
						num2++;
						num3 += archetypeInstance2.GetAssociatedLevelInteger(entity);
					}
				}
			}
			if (num2 >= masterySphere.GetMaximumForSphere())
			{
				int maximumForSphere = masterySphere.GetMaximumForSphere();
				string text = string.Empty;
				if (masterySphere != MasterySphere.Adventuring)
				{
					if (masterySphere == MasterySphere.Crafting)
					{
						text = ((maximumForSphere == 1) ? "profession" : "professions");
					}
				}
				else
				{
					text = ((maximumForSphere == 1) ? "role" : "roles");
				}
				msg = string.Concat(new string[]
				{
					"You can only learn ",
					masterySphere.GetMaximumForSphere().ToString(),
					" ",
					masterySphere.ToString(),
					" ",
					text,
					"!"
				});
				return false;
			}
			if (masterySphere == MasterySphere.Crafting)
			{
				int num4 = (num2 == 1) ? 3 : 6;
				foreach (ArchetypeInstance archetypeInstance3 in entity.CollectionController.Masteries.Instances)
				{
					MasteryArchetype masteryArchetype2;
					if (archetypeInstance3.Archetype.TryGetAsType(out masteryArchetype2) && masteryArchetype2.Type.GetMasterySphere() == MasterySphere.Crafting && archetypeInstance3.GetAssociatedLevelInteger(entity) < num4)
					{
						msg = "All gathering & crafting professions must be level " + num4.ToString() + " before you can learn another!";
						return false;
					}
				}
				return true;
			}
			return true;
		}
	}
}

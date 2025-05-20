using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000860 RID: 2144
	public class CodexMasterySummary : MonoBehaviour
	{
		// Token: 0x06003DE5 RID: 15845 RVA: 0x00183DD8 File Offset: 0x00181FD8
		private void Awake()
		{
			this.m_codexMasteryUI.InstanceAdded += this.CodexMasteryUiOnInstanceAdded;
			this.m_codexMasteryUI.InstanceRemoved += this.CodexMasteryUiOnInstanceRemoved;
			this.InitializeMasteryDetails(this.m_adventuringLeftDetails);
			this.InitializeMasteryDetails(this.m_adventuringRightDetails);
			this.InitializeMasteryDetails(this.m_harvestingDetails);
			this.InitializeMasteryDetails(this.m_craftingDetails);
		}

		// Token: 0x06003DE6 RID: 15846 RVA: 0x00069E40 File Offset: 0x00068040
		private void OnDestroy()
		{
			this.m_codexMasteryUI.InstanceAdded -= this.CodexMasteryUiOnInstanceAdded;
			this.m_codexMasteryUI.InstanceRemoved -= this.CodexMasteryUiOnInstanceRemoved;
		}

		// Token: 0x06003DE7 RID: 15847 RVA: 0x00183E44 File Offset: 0x00182044
		private void InitializeMasteryDetails(MasteryDetailsUI[] details)
		{
			for (int i = 0; i < details.Length; i++)
			{
				details[i].gameObject.SetActive(false);
				if (details[i].Tab != null)
				{
					details[i].Tab.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06003DE8 RID: 15848 RVA: 0x00183E90 File Offset: 0x00182090
		private void CodexMasteryUiOnInstanceRemoved(ArchetypeInstance obj)
		{
			MasteryDetailsUI masteryDetailsUI;
			if (this.m_masteries.TryGetValue(obj.ArchetypeId, out masteryDetailsUI))
			{
				masteryDetailsUI.UnregisterMastery();
				masteryDetailsUI.gameObject.SetActive(false);
				this.m_masteries.Remove(obj.ArchetypeId);
			}
		}

		// Token: 0x06003DE9 RID: 15849 RVA: 0x00183ED8 File Offset: 0x001820D8
		private void CodexMasteryUiOnInstanceAdded(ArchetypeInstance obj)
		{
			MasteryDetailsUI masteryDetailsUI;
			if (this.m_masteries.TryGetValue(obj.ArchetypeId, out masteryDetailsUI))
			{
				masteryDetailsUI.RegisterMastery(obj);
				return;
			}
			MasteryDetailsUI[] array = null;
			MasteryArchetype masteryArchetype;
			if (obj.Archetype.TryGetAsType(out masteryArchetype))
			{
				switch (masteryArchetype.Type)
				{
				case MasteryType.Combat:
					array = this.m_adventuringLeftDetails;
					break;
				case MasteryType.Utility:
					array = this.m_adventuringRightDetails;
					break;
				case MasteryType.Trade:
					array = this.m_craftingDetails;
					break;
				case MasteryType.Harvesting:
					array = this.m_harvestingDetails;
					break;
				}
			}
			if (array == null)
			{
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Instance == null)
				{
					array[i].gameObject.SetActive(true);
					array[i].RegisterMastery(obj);
					this.m_masteries.Add(obj.ArchetypeId, array[i]);
					return;
				}
			}
		}

		// Token: 0x04003C54 RID: 15444
		[SerializeField]
		private CodexMasteryUI m_codexMasteryUI;

		// Token: 0x04003C55 RID: 15445
		[SerializeField]
		private MasteryDetailsUI[] m_adventuringLeftDetails;

		// Token: 0x04003C56 RID: 15446
		[SerializeField]
		private MasteryDetailsUI[] m_adventuringRightDetails;

		// Token: 0x04003C57 RID: 15447
		[SerializeField]
		private MasteryDetailsUI[] m_harvestingDetails;

		// Token: 0x04003C58 RID: 15448
		[SerializeField]
		private MasteryDetailsUI[] m_craftingDetails;

		// Token: 0x04003C59 RID: 15449
		private Dictionary<UniqueId, MasteryDetailsUI> m_masteries = new Dictionary<UniqueId, MasteryDetailsUI>(default(UniqueIdComparer));
	}
}

using System;
using System.Collections.Generic;
using SoL.Game.Login.Client.Creation.NewCreation;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Game.UMA;
using SoL.Networking.Database;
using UMA.CharacterSystem;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005E5 RID: 1509
	[CreateAssetMenu(menuName = "SoL/UMA/Wardrobe Recipe Pair Ensemble", order = 6)]
	public class WardrobeRecipePairEnsemble : BaseArchetype
	{
		// Token: 0x17000A1E RID: 2590
		// (get) Token: 0x06002FDB RID: 12251 RVA: 0x00060FCB File Offset: 0x0005F1CB
		public EmberStone HipMountedEmberStone
		{
			get
			{
				return this.m_hipMountedEmberStone;
			}
		}

		// Token: 0x06002FDC RID: 12252 RVA: 0x001582F0 File Offset: 0x001564F0
		public void AddEnsembleToDca(DynamicCharacterAvatar dca, CharacterSex sex, System.Random seed)
		{
			if (this.m_entries == null)
			{
				return;
			}
			if (this.m_colorSamplerRuleDict == null && this.m_colorSamplerRules.Length != 0)
			{
				this.m_colorSamplerRuleDict = new Dictionary<MaterialColorType, WardrobeRecipePairEnsemble.ColorSamplerRule>(default(MaterialColorTypeComparer));
				for (int i = 0; i < this.m_colorSamplerRules.Length; i++)
				{
					ColorSampler colorSampler;
					if (this.m_colorSamplerRules[i].MaterialColorType != MaterialColorType.None && GlobalSettings.Values.Uma.MaterialColorTypeSamplerDict.TryGetValue(this.m_colorSamplerRules[i].MaterialColorType, out colorSampler))
					{
						this.m_colorSamplerRules[i].Sampler = colorSampler;
						if (this.m_colorSamplerRules[i].UniformColor)
						{
							this.m_colorSamplerRules[i].Color = new Color?(colorSampler.GetRandomColor(seed));
						}
						this.m_colorSamplerRuleDict.Add(this.m_colorSamplerRules[i].MaterialColorType, this.m_colorSamplerRules[i]);
					}
				}
			}
			for (int j = 0; j < this.m_entries.Length; j++)
			{
				if (this.m_entries[j] != null)
				{
					WardrobeRecipePairEnsemble.RecipePairEntry entry = this.m_entries[j].GetEntry(seed, false);
					if (entry != null && !(entry.Obj == null))
					{
						Color? overrideColor = null;
						WardrobeRecipePairEnsemble.ColorSamplerRule colorSamplerRule;
						if (this.m_colorSamplerRuleDict != null && this.m_colorSamplerRuleDict.TryGetValue(entry.Obj.MaterialColorType, out colorSamplerRule))
						{
							overrideColor = ((colorSamplerRule.UniformColor && colorSamplerRule.Color != null) ? colorSamplerRule.Color : new Color?(colorSamplerRule.Sampler.GetRandomColor(seed)));
						}
						entry.Obj.OnEquipVisuals(sex, dca, false, overrideColor);
					}
				}
			}
		}

		// Token: 0x04002EC2 RID: 11970
		[SerializeField]
		private EmberStone m_hipMountedEmberStone;

		// Token: 0x04002EC3 RID: 11971
		[SerializeField]
		private WardrobeRecipePairEnsemble.ColorSamplerRule[] m_colorSamplerRules;

		// Token: 0x04002EC4 RID: 11972
		[SerializeField]
		private WardrobeRecipePairEnsemble.RecipePairCollection[] m_entries;

		// Token: 0x04002EC5 RID: 11973
		private Dictionary<MaterialColorType, WardrobeRecipePairEnsemble.ColorSamplerRule> m_colorSamplerRuleDict;

		// Token: 0x020005E6 RID: 1510
		[Serializable]
		private class ColorSamplerRule
		{
			// Token: 0x17000A1F RID: 2591
			// (get) Token: 0x06002FDE RID: 12254 RVA: 0x00060FD3 File Offset: 0x0005F1D3
			public MaterialColorType MaterialColorType
			{
				get
				{
					return this.m_materialColorType;
				}
			}

			// Token: 0x17000A20 RID: 2592
			// (get) Token: 0x06002FDF RID: 12255 RVA: 0x00060FDB File Offset: 0x0005F1DB
			public bool UniformColor
			{
				get
				{
					return this.m_uniformColor;
				}
			}

			// Token: 0x17000A21 RID: 2593
			// (get) Token: 0x06002FE0 RID: 12256 RVA: 0x00060FE3 File Offset: 0x0005F1E3
			// (set) Token: 0x06002FE1 RID: 12257 RVA: 0x00060FEB File Offset: 0x0005F1EB
			public Color? Color { get; set; }

			// Token: 0x17000A22 RID: 2594
			// (get) Token: 0x06002FE2 RID: 12258 RVA: 0x00060FF4 File Offset: 0x0005F1F4
			// (set) Token: 0x06002FE3 RID: 12259 RVA: 0x00060FFC File Offset: 0x0005F1FC
			public ColorSampler Sampler { get; set; }

			// Token: 0x04002EC8 RID: 11976
			[SerializeField]
			private MaterialColorType m_materialColorType;

			// Token: 0x04002EC9 RID: 11977
			[SerializeField]
			private bool m_uniformColor;
		}

		// Token: 0x020005E7 RID: 1511
		[Serializable]
		private class RecipePairEntry : ProbabilityEntryObject<WardrobeRecipePair>
		{
		}

		// Token: 0x020005E8 RID: 1512
		[Serializable]
		private class RecipePairCollection : ProbabilityCollection<WardrobeRecipePairEnsemble.RecipePairEntry>
		{
		}
	}
}

using System;
using System.Collections.Generic;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Login.Client.Creation
{
	// Token: 0x02000B58 RID: 2904
	public class DnaClusterSlider : Feature
	{
		// Token: 0x06005966 RID: 22886 RVA: 0x001E9D64 File Offset: 0x001E7F64
		public override void Initialize(CreationDirector director, CreationDirector.CharacterToCreate toCreate, CreationDirector.FeatureSetting settings)
		{
			base.Initialize(director, toCreate, settings);
			for (int i = 0; i < this.m_settings.Sliders.Length; i++)
			{
				Feature component = UnityEngine.Object.Instantiate<GameObject>(this.m_director.SliderPrefab, base.gameObject.transform).GetComponent<Feature>();
				component.Initialize(this.m_director, toCreate, new CreationDirector.FeatureSetting
				{
					Label = this.m_settings.Sliders[i].ToString(),
					Sliders = new UMADnaType[]
					{
						this.m_settings.Sliders[i]
					}
				});
				this.m_subFeatures.Add(component);
			}
		}

		// Token: 0x06005967 RID: 22887 RVA: 0x001E9E18 File Offset: 0x001E8018
		public override void Reset()
		{
			if (base.m_locked)
			{
				return;
			}
			base.Reset();
			for (int i = 0; i < this.m_subFeatures.Count; i++)
			{
				this.m_subFeatures[i].Reset();
			}
		}

		// Token: 0x06005968 RID: 22888 RVA: 0x001E9E5C File Offset: 0x001E805C
		public override void Randomize()
		{
			if (base.m_locked)
			{
				return;
			}
			base.Randomize();
			for (int i = 0; i < this.m_subFeatures.Count; i++)
			{
				this.m_subFeatures[i].Randomize();
			}
		}

		// Token: 0x06005969 RID: 22889 RVA: 0x001E9EA0 File Offset: 0x001E80A0
		public override void OnLockStateChanged(ToggleController.ToggleState obj)
		{
			base.OnLockStateChanged(obj);
			bool flag = obj == ToggleController.ToggleState.ON;
			for (int i = 0; i < this.m_subFeatures.Count; i++)
			{
				this.m_subFeatures[i].ToggleLock(flag);
				this.m_subFeatures[i].ToggleLockInteractable(!flag);
			}
		}

		// Token: 0x04004EAC RID: 20140
		private readonly List<Feature> m_subFeatures = new List<Feature>();
	}
}

using System;
using BehaviorDesigner.Runtime;
using SoL.Game.EffectSystem;
using SoL.Game.Influence;
using SoL.Game.NPCs;
using SoL.Game.Randomization;
using SoL.Game.Settings;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006A1 RID: 1697
	[CreateAssetMenu(menuName = "SoL/Profiles/Flatworm")]
	public class FlatwormSpawnProfile : SpawnProfile
	{
		// Token: 0x060033DE RID: 13278 RVA: 0x00162918 File Offset: 0x00160B18
		protected override void SpawnInternal(ISpawnController controller, GameEntity gameEntity)
		{
			base.SpawnInternal(controller, gameEntity);
			FlatwormController componentInChildren = gameEntity.GetComponentInChildren<FlatwormController>();
			if (componentInChildren)
			{
				int level = controller.GetLevel();
				componentInChildren.Initialize(level, this.m_fleeRadius, this.m_effects);
			}
			if (gameEntity.CharacterData != null)
			{
				gameEntity.CharacterData.Name.Value = this.m_nameSettings.Name;
				gameEntity.CharacterData.Title.Value = this.m_nameSettings.Title;
			}
			if (gameEntity.ServerNpcController != null)
			{
				BehaviorTree tree = gameEntity.ServerNpcController.Tree;
				if (tree != null)
				{
					tree.StartWhenEnabled = false;
					tree.ExternalBehavior = this.m_behaviorTree;
					SharedVariable variable = gameEntity.ServerNpcController.Tree.GetVariable("LeashDistance");
					if (variable != null)
					{
						float? leashDistance = controller.LeashDistance;
						float num = (leashDistance != null) ? leashDistance.Value : this.m_leashDistance;
						variable.SetValue(num);
						SharedVariable variable2 = gameEntity.ServerNpcController.Tree.GetVariable("ResetDistance");
						if (variable2 != null)
						{
							float num2 = Mathf.Max(GlobalSettings.Values.Npcs.ResetDistanceMinimum, num * GlobalSettings.Values.Npcs.ResetDistanceLeashMultiplier);
							variable2.SetValue(num2);
						}
					}
					gameEntity.ServerNpcController.Tree.enabled = true;
				}
			}
			int num3;
			if (gameEntity.SeedReplicator != null && this.m_visualIndex != null && this.m_visualIndex.TryGetVisualIndex(this.m_prefabReference, out num3))
			{
				gameEntity.SeedReplicator.VisualIndexOverride = new byte?((byte)num3);
			}
			if (gameEntity.InfluenceSource != null)
			{
				gameEntity.InfluenceSource.InfluenceProfile = this.m_influenceProfile;
			}
		}

		// Token: 0x040031AF RID: 12719
		private const string kColorIndex = "Color Index";

		// Token: 0x040031B0 RID: 12720
		private const string kScalingGroup = "Scaling Properties";

		// Token: 0x040031B1 RID: 12721
		private const string kNpcGroup = "Npc Stuff";

		// Token: 0x040031B2 RID: 12722
		[SerializeField]
		private VisualIndex m_visualIndex;

		// Token: 0x040031B3 RID: 12723
		[SerializeField]
		private AnimationCurve m_transformScale = AnimationCurve.Linear(1f, 0f, 50f, 1f);

		// Token: 0x040031B4 RID: 12724
		[SerializeField]
		private NpcSpawnProfileV2.MetadataSettings m_nameSettings;

		// Token: 0x040031B5 RID: 12725
		[SerializeField]
		private InfluenceProfile m_influenceProfile;

		// Token: 0x040031B6 RID: 12726
		[SerializeField]
		private float m_fleeRadius = 30f;

		// Token: 0x040031B7 RID: 12727
		[SerializeField]
		private float m_leashDistance = 100f;

		// Token: 0x040031B8 RID: 12728
		[SerializeField]
		private ExternalBehavior m_behaviorTree;

		// Token: 0x040031B9 RID: 12729
		[SerializeField]
		private FlatwormSpawnProfile.ScriptableEffectsByLevel[] m_effects;

		// Token: 0x020006A2 RID: 1698
		[Serializable]
		public class ScriptableEffectsByLevel
		{
			// Token: 0x17000B36 RID: 2870
			// (get) Token: 0x060033E0 RID: 13280 RVA: 0x00063A13 File Offset: 0x00061C13
			public int LevelThreshold
			{
				get
				{
					return this.m_levelThreshold;
				}
			}

			// Token: 0x17000B37 RID: 2871
			// (get) Token: 0x060033E1 RID: 13281 RVA: 0x00063A1B File Offset: 0x00061C1B
			public ScriptableEffectData Effect
			{
				get
				{
					return this.m_effect;
				}
			}

			// Token: 0x040031BA RID: 12730
			[Range(1f, 50f)]
			[SerializeField]
			private int m_levelThreshold = 1;

			// Token: 0x040031BB RID: 12731
			[SerializeField]
			private ScriptableEffectData m_effect;
		}
	}
}

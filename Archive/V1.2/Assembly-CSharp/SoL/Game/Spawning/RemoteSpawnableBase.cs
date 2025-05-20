using System;
using System.Collections.Generic;
using SoL.Game.NPCs.Interactions;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Spawning.Behavior;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006B7 RID: 1719
	public abstract class RemoteSpawnableBase : ScriptableObject, ISpawnControllerRemoteSpawns, ISpawnController
	{
		// Token: 0x17000B58 RID: 2904
		// (get) Token: 0x0600344B RID: 13387 RVA: 0x00063DE2 File Offset: 0x00061FE2
		// (set) Token: 0x0600344C RID: 13388 RVA: 0x00063DEA File Offset: 0x00061FEA
		public int Level { get; set; } = 10;

		// Token: 0x17000B59 RID: 2905
		// (get) Token: 0x0600344D RID: 13389 RVA: 0x00063DF3 File Offset: 0x00061FF3
		// (set) Token: 0x0600344E RID: 13390 RVA: 0x00063DFB File Offset: 0x00061FFB
		public SpawnBehaviorType BehaviorType { get; set; }

		// Token: 0x17000B5A RID: 2906
		// (get) Token: 0x0600344F RID: 13391
		protected abstract RemoteSpawnProfile.RemoteSpawnProfileType Type { get; }

		// Token: 0x17000B5B RID: 2907
		// (get) Token: 0x06003450 RID: 13392 RVA: 0x00049FFA File Offset: 0x000481FA
		protected virtual BehaviorSubTreeCollection BehaviorOverrides
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000B5C RID: 2908
		// (get) Token: 0x06003451 RID: 13393 RVA: 0x00164964 File Offset: 0x00162B64
		protected virtual MinMaxIntRange LevelRange
		{
			get
			{
				return default(MinMaxIntRange);
			}
		}

		// Token: 0x17000B5D RID: 2909
		// (get) Token: 0x06003452 RID: 13394 RVA: 0x00154B50 File Offset: 0x00152D50
		protected virtual float? LeashDistance
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000B5E RID: 2910
		// (get) Token: 0x06003453 RID: 13395 RVA: 0x00154B50 File Offset: 0x00152D50
		protected virtual float? ResetDistance
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003454 RID: 13396 RVA: 0x0016497C File Offset: 0x00162B7C
		public string GetRemoteNames(string filter)
		{
			string result = "UNKNOWN";
			if (string.IsNullOrEmpty(filter))
			{
				if (string.IsNullOrEmpty(this.m_spawnableNames) && this.m_profiles != null && this.m_profiles.Length != 0)
				{
					List<string> list = new List<string>(this.m_profiles.Length);
					for (int i = 0; i < this.m_profiles.Length; i++)
					{
						list.Add(string.Concat(new string[]
						{
							"<color=",
							RemoteSpawnProfile.GetNextColor(i == 0),
							">",
							this.m_profiles[i].BaseName,
							"</color>"
						}));
					}
					this.m_spawnableNames = "Spawnable: " + string.Join(" | ", list);
				}
				result = this.m_spawnableNames;
			}
			else
			{
				string[] array = filter.Split('.', StringSplitOptions.None);
				string baseName = array[0].ToLowerInvariant();
				string category = (array.Length > 1) ? array[1].ToLowerInvariant() : string.Empty;
				for (int j = 0; j < this.m_profiles.Length; j++)
				{
					string text;
					if (this.m_profiles[j].GetRemoteNames(baseName, category, out text))
					{
						result = text;
						break;
					}
				}
			}
			return result;
		}

		// Token: 0x06003455 RID: 13397 RVA: 0x00164AAC File Offset: 0x00162CAC
		public SpawnProfile GetSpawnProfile(string request)
		{
			if (string.IsNullOrEmpty(request))
			{
				return null;
			}
			string[] array = request.Split('.', StringSplitOptions.None);
			string baseName = array[0].ToLowerInvariant();
			string category = string.Empty;
			string alias = string.Empty;
			int num = array.Length;
			if (num != 2)
			{
				if (num == 3)
				{
					category = array[1].ToLowerInvariant();
					alias = array[2].ToLowerInvariant();
				}
			}
			else
			{
				alias = array[1].ToLowerInvariant();
			}
			for (int i = 0; i < this.m_profiles.Length; i++)
			{
				SpawnProfile result;
				if (this.m_profiles[i].TryGetSpawnProfile(baseName, category, alias, out result))
				{
					return result;
				}
			}
			return null;
		}

		// Token: 0x06003456 RID: 13398 RVA: 0x0005897E File Offset: 0x00056B7E
		bool ISpawnController.TryGetBehaviorProfile(out BehaviorProfile profile)
		{
			profile = null;
			return false;
		}

		// Token: 0x17000B5F RID: 2911
		// (get) Token: 0x06003457 RID: 13399 RVA: 0x00063E04 File Offset: 0x00062004
		BehaviorSubTreeCollection ISpawnController.BehaviorOverrides
		{
			get
			{
				return this.BehaviorOverrides;
			}
		}

		// Token: 0x06003458 RID: 13400 RVA: 0x00063E0C File Offset: 0x0006200C
		bool ISpawnController.TryGetLevel(out int level)
		{
			level = ((this.Type == RemoteSpawnProfile.RemoteSpawnProfileType.Npc) ? this.Level : 0);
			return false;
		}

		// Token: 0x06003459 RID: 13401 RVA: 0x00063E22 File Offset: 0x00062022
		int ISpawnController.GetLevel()
		{
			if (this.Type != RemoteSpawnProfile.RemoteSpawnProfileType.Npc)
			{
				return 0;
			}
			return this.Level;
		}

		// Token: 0x17000B60 RID: 2912
		// (get) Token: 0x0600345A RID: 13402 RVA: 0x00063E34 File Offset: 0x00062034
		MinMaxIntRange ISpawnController.LevelRange
		{
			get
			{
				return this.LevelRange;
			}
		}

		// Token: 0x17000B61 RID: 2913
		// (get) Token: 0x0600345B RID: 13403 RVA: 0x00164B40 File Offset: 0x00162D40
		Vector3? ISpawnController.CurrentPosition
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000B62 RID: 2914
		// (get) Token: 0x0600345C RID: 13404 RVA: 0x00063E3C File Offset: 0x0006203C
		float? ISpawnController.LeashDistance
		{
			get
			{
				return this.LeashDistance;
			}
		}

		// Token: 0x17000B63 RID: 2915
		// (get) Token: 0x0600345D RID: 13405 RVA: 0x00063E44 File Offset: 0x00062044
		float? ISpawnController.ResetDistance
		{
			get
			{
				return this.ResetDistance;
			}
		}

		// Token: 0x17000B64 RID: 2916
		// (get) Token: 0x0600345E RID: 13406 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool ISpawnController.DespawnOnDeath
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000B65 RID: 2917
		// (get) Token: 0x0600345F RID: 13407 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool ISpawnController.CallForHelpRequiresLos
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000B66 RID: 2918
		// (get) Token: 0x06003460 RID: 13408 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool ISpawnController.ForceIndoorProfiles
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000B67 RID: 2919
		// (get) Token: 0x06003461 RID: 13409 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool ISpawnController.MatchAttackerLevel
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000B68 RID: 2920
		// (get) Token: 0x06003462 RID: 13410 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool ISpawnController.LogSpawns
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000B69 RID: 2921
		// (get) Token: 0x06003463 RID: 13411 RVA: 0x00049FFA File Offset: 0x000481FA
		DialogueSource ISpawnController.OverrideDialogue
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003464 RID: 13412 RVA: 0x00063E4C File Offset: 0x0006204C
		public bool OverrideInteractionFlags(out NpcInteractionFlags flags)
		{
			flags = NpcInteractionFlags.None;
			return false;
		}

		// Token: 0x06003465 RID: 13413 RVA: 0x0004475B File Offset: 0x0004295B
		void ISpawnController.NotifyOfDeath(GameEntity entity)
		{
		}

		// Token: 0x17000B6A RID: 2922
		// (get) Token: 0x06003466 RID: 13414 RVA: 0x00045BCA File Offset: 0x00043DCA
		int ISpawnController.XpAdjustment
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06003467 RID: 13415 RVA: 0x0005BFF3 File Offset: 0x0005A1F3
		bool ISpawnController.TryGetOverrideData(SpawnProfile profile, out SpawnControllerOverrideData data)
		{
			data = null;
			return false;
		}

		// Token: 0x04003266 RID: 12902
		[NonSerialized]
		private string m_spawnableNames = string.Empty;

		// Token: 0x04003267 RID: 12903
		[SerializeField]
		private RemoteSpawnProfile[] m_profiles;
	}
}

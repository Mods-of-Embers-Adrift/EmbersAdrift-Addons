using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.NPCs;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Spawning;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities;
using UMA.CharacterSystem;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000D9B RID: 3483
	public class DCATester : MonoBehaviour
	{
		// Token: 0x1700190F RID: 6415
		// (get) Token: 0x06006889 RID: 26761 RVA: 0x000862EC File Offset: 0x000844EC
		private bool m_showEnsemble
		{
			get
			{
				return this.m_ensembles == null || this.m_ensembles.Count <= 0;
			}
		}

		// Token: 0x0600688A RID: 26762 RVA: 0x00214CCC File Offset: 0x00212ECC
		private void SpawnMany()
		{
			System.Random incomingRandom = new System.Random(this.m_seed);
			for (int i = 0; i < this.m_nToSpawn; i++)
			{
				this.InitNpcRandomInternal(incomingRandom);
			}
		}

		// Token: 0x0600688B RID: 26763 RVA: 0x00214D00 File Offset: 0x00212F00
		private void InitPlayerFromDB()
		{
			if (this.m_prefab == null || string.IsNullOrEmpty(this.m_dbId))
			{
				throw new ArgumentException("Stuff");
			}
			CustomSerialization.Initialize();
			CharacterRecord characterRecord = CharacterRecord.Load(ExternalGameDatabase.Database, this.m_dbId);
			DynamicCharacterAvatar newDCA = this.GetNewDCA(null);
			List<ArchetypeInstance> itemInstances = null;
			ContainerRecord containerRecord;
			if (characterRecord.Storage.TryGetValue(ContainerType.Equipment, out containerRecord))
			{
				itemInstances = containerRecord.Instances;
			}
			UMAManager.BuildBaseDca(null, newDCA, characterRecord.Visuals, itemInstances, null);
		}

		// Token: 0x0600688C RID: 26764 RVA: 0x00214D7C File Offset: 0x00212F7C
		private void InitNpcFromProfile()
		{
			if (this.m_prefab == null)
			{
				throw new ArgumentNullException("m_prefab");
			}
			if (this.m_npcProfile != null)
			{
				UMAManager.BuildNpcFromProfile(null, this.GetNewDCA(null), this.m_npcProfile, new System.Random(this.m_npcProfile.ProfileSeed));
				return;
			}
			if (this.m_npcPopulationVisualsProfile != null)
			{
				System.Random seed = new System.Random(this.m_seed);
				WardrobeRecipePairEnsemble wardrobeRecipePairEnsemble = this.m_showEnsemble ? this.m_ensemble : this.m_ensembles.GetEntry(seed, false).Obj;
				UniqueId ensembleId = (wardrobeRecipePairEnsemble == null) ? UniqueId.Empty : wardrobeRecipePairEnsemble.Id;
				UMAManager.BuildNpcFromPopulationVisualsProfile(null, this.GetNewDCA(null), this.m_npcPopulationVisualsProfile, ensembleId, seed);
			}
		}

		// Token: 0x0600688D RID: 26765 RVA: 0x00086309 File Offset: 0x00084509
		private void RandomizeSeed()
		{
			this.m_seed = UnityEngine.Random.Range(0, int.MaxValue);
		}

		// Token: 0x0600688E RID: 26766 RVA: 0x0008631C File Offset: 0x0008451C
		private void InitNpcRandom()
		{
			this.InitNpcRandomInternal(null);
		}

		// Token: 0x0600688F RID: 26767 RVA: 0x00214E40 File Offset: 0x00213040
		public DynamicCharacterAvatar InitNpcRandomInternal(System.Random incomingRandom)
		{
			if (this.m_prefab == null)
			{
				throw new ArgumentException("Stuff");
			}
			WardrobeRecipePairEnsemble wardrobeRecipePairEnsemble = this.m_ensemble;
			if (this.m_ensembles != null && this.m_ensembles.Count > 0)
			{
				EnsembleProbabilityEntry entry = this.m_ensembles.GetEntry(incomingRandom, false);
				if (entry != null)
				{
					wardrobeRecipePairEnsemble = entry.Obj;
				}
			}
			System.Random random = (incomingRandom == null) ? new System.Random(this.m_seed) : incomingRandom;
			DynamicCharacterAvatar newDCA = this.GetNewDCA(random);
			UMAManager.BuildRandomNpc(null, newDCA, random, wardrobeRecipePairEnsemble ? wardrobeRecipePairEnsemble.Id : UniqueId.Empty, new CharacterSex?(this.m_sex));
			return newDCA;
		}

		// Token: 0x06006890 RID: 26768 RVA: 0x00214EE0 File Offset: 0x002130E0
		private DynamicCharacterAvatar GetNewDCA(System.Random random)
		{
			if (this.m_dcaToUse != null)
			{
				return this.m_dcaToUse;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_prefab);
			DCATester.PlacementType placementType = this.m_placementType;
			if (placementType != DCATester.PlacementType.Line)
			{
				if (placementType == DCATester.PlacementType.Circle)
				{
					Vector2 vector = Vector2.zero;
					if (random == null)
					{
						vector = UnityEngine.Random.insideUnitCircle * this.m_radius;
					}
					else
					{
						float num = (random.NextDouble() < 0.5) ? -1f : 1f;
						float num2 = (random.NextDouble() < 0.5) ? -1f : 1f;
						vector = new Vector2((float)random.NextDouble() * num, (float)random.NextDouble() * num2) * this.m_radius;
					}
					gameObject.gameObject.transform.SetPositionAndRotation(new Vector3(vector.x, 0f, vector.y), Quaternion.Euler(new Vector3(0f, 180f, 0f)));
				}
			}
			else
			{
				gameObject.gameObject.transform.SetPositionAndRotation(new Vector3(DCATester.m_pos, 0f, 0f), Quaternion.Euler(new Vector3(0f, 180f, 0f)));
				DCATester.m_pos -= 1f;
			}
			return gameObject.GetComponent<DynamicCharacterAvatar>();
		}

		// Token: 0x06006891 RID: 26769 RVA: 0x00063B05 File Offset: 0x00061D05
		private IEnumerable GetNpcProfiles()
		{
			return SolOdinUtilities.GetDropdownItems<NpcProfile>();
		}

		// Token: 0x06006892 RID: 26770 RVA: 0x00063BEB File Offset: 0x00061DEB
		private IEnumerable GetEnsemble()
		{
			return SolOdinUtilities.GetDropdownItems<WardrobeRecipePairEnsemble>();
		}

		// Token: 0x06006893 RID: 26771 RVA: 0x00086326 File Offset: 0x00084526
		private void OnValidate()
		{
			this.m_ensembles.Normalize();
		}

		// Token: 0x04005ACA RID: 23242
		private const string kPlacement = "Placement";

		// Token: 0x04005ACB RID: 23243
		private const string kPlayer = "Player";

		// Token: 0x04005ACC RID: 23244
		private const string kNpcProfile = "Npc Profile";

		// Token: 0x04005ACD RID: 23245
		private const string kNpcRandom = "Npc Random";

		// Token: 0x04005ACE RID: 23246
		[SerializeField]
		private DCATester.PlacementType m_placementType;

		// Token: 0x04005ACF RID: 23247
		[SerializeField]
		private float m_radius = 10f;

		// Token: 0x04005AD0 RID: 23248
		[SerializeField]
		private GameObject m_prefab;

		// Token: 0x04005AD1 RID: 23249
		[SerializeField]
		private DynamicCharacterAvatar m_dcaToUse;

		// Token: 0x04005AD2 RID: 23250
		[SerializeField]
		private int m_seed = 8675309;

		// Token: 0x04005AD3 RID: 23251
		[SerializeField]
		private string m_dbId;

		// Token: 0x04005AD4 RID: 23252
		[SerializeField]
		private NpcProfile m_npcProfile;

		// Token: 0x04005AD5 RID: 23253
		[SerializeField]
		private NpcPopulationVisualsProfile m_npcPopulationVisualsProfile;

		// Token: 0x04005AD6 RID: 23254
		[SerializeField]
		private CharacterSex m_sex = CharacterSex.Male;

		// Token: 0x04005AD7 RID: 23255
		[SerializeField]
		private int m_nToSpawn = 10;

		// Token: 0x04005AD8 RID: 23256
		[SerializeField]
		private WardrobeRecipePairEnsemble m_ensemble;

		// Token: 0x04005AD9 RID: 23257
		[SerializeField]
		private EnsembleProbabilityCollection m_ensembles;

		// Token: 0x04005ADA RID: 23258
		private static float m_pos;

		// Token: 0x02000D9C RID: 3484
		private enum PlacementType
		{
			// Token: 0x04005ADC RID: 23260
			Line,
			// Token: 0x04005ADD RID: 23261
			Circle
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using SoL.Game.Login.Client.Creation.NewCreation;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UMA.CharacterSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Game.NPCs
{
	// Token: 0x0200080D RID: 2061
	[CreateAssetMenu(menuName = "SoL/Profiles/NPC")]
	public class NpcProfile : DialogueSource, INpcVisualProfile
	{
		// Token: 0x17000DAD RID: 3501
		// (get) Token: 0x06003BAB RID: 15275 RVA: 0x000685C2 File Offset: 0x000667C2
		public int ProfileSeed
		{
			get
			{
				if (!this.m_overrideSeed)
				{
					return Animator.StringToHash(this.m_id.Value);
				}
				return this.m_seedValue;
			}
		}

		// Token: 0x17000DAE RID: 3502
		// (get) Token: 0x06003BAC RID: 15276 RVA: 0x000685E3 File Offset: 0x000667E3
		public string NpcName
		{
			get
			{
				return this.m_npcName;
			}
		}

		// Token: 0x17000DAF RID: 3503
		// (get) Token: 0x06003BAD RID: 15277 RVA: 0x000685EB File Offset: 0x000667EB
		public CharacterSex Sex
		{
			get
			{
				return this.m_sex;
			}
		}

		// Token: 0x17000DB0 RID: 3504
		// (get) Token: 0x06003BAE RID: 15278 RVA: 0x000685F3 File Offset: 0x000667F3
		public WardrobeRecipePairEnsemble Ensemble
		{
			get
			{
				return this.m_ensemble;
			}
		}

		// Token: 0x17000DB1 RID: 3505
		// (get) Token: 0x06003BAF RID: 15279 RVA: 0x000685FB File Offset: 0x000667FB
		public bool HasVisualEquipment
		{
			get
			{
				return this.m_equipment != null && this.m_equipment.Length != 0;
			}
		}

		// Token: 0x17000DB2 RID: 3506
		// (get) Token: 0x06003BB0 RID: 15280 RVA: 0x00068611 File Offset: 0x00066811
		public bool HasVisualWeapons
		{
			get
			{
				return this.m_weapons != null && this.m_weapons.Length != 0;
			}
		}

		// Token: 0x17000DB3 RID: 3507
		// (get) Token: 0x06003BB1 RID: 15281 RVA: 0x00068627 File Offset: 0x00066827
		public string[] KnowledgeLabels
		{
			get
			{
				return this.m_knowledgeLabels;
			}
		}

		// Token: 0x06003BB2 RID: 15282 RVA: 0x0006862F File Offset: 0x0006682F
		private void RandomizeSeed()
		{
			this.m_seedValue = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		}

		// Token: 0x17000DB4 RID: 3508
		// (get) Token: 0x06003BB3 RID: 15283 RVA: 0x00068646 File Offset: 0x00066846
		private bool ShowEquipment
		{
			get
			{
				return this.m_ensemble == null;
			}
		}

		// Token: 0x17000DB5 RID: 3509
		// (get) Token: 0x06003BB4 RID: 15284 RVA: 0x00068654 File Offset: 0x00066854
		private bool m_hideBuildType
		{
			get
			{
				return this.m_randomizeBuild || this.m_randomizeBuildType;
			}
		}

		// Token: 0x17000DB6 RID: 3510
		// (get) Token: 0x06003BB5 RID: 15285 RVA: 0x00068666 File Offset: 0x00066866
		private bool m_hideBuildTypeFlags
		{
			get
			{
				return this.m_randomizeBuild || !this.m_randomizeBuildType;
			}
		}

		// Token: 0x17000DB7 RID: 3511
		// (get) Token: 0x06003BB6 RID: 15286 RVA: 0x0006867B File Offset: 0x0006687B
		private bool m_hideSize
		{
			get
			{
				return this.m_randomizeBuild || this.m_randomizeSize;
			}
		}

		// Token: 0x17000DB8 RID: 3512
		// (get) Token: 0x06003BB7 RID: 15287 RVA: 0x0006868D File Offset: 0x0006688D
		private bool m_hideSizeRange
		{
			get
			{
				return this.m_randomizeBuild || !this.m_randomizeSize;
			}
		}

		// Token: 0x17000DB9 RID: 3513
		// (get) Token: 0x06003BB8 RID: 15288 RVA: 0x000686A2 File Offset: 0x000668A2
		private bool m_hideTone
		{
			get
			{
				return this.m_randomizeBuild || this.m_randomizeTone;
			}
		}

		// Token: 0x17000DBA RID: 3514
		// (get) Token: 0x06003BB9 RID: 15289 RVA: 0x000686B4 File Offset: 0x000668B4
		private bool m_hideToneRange
		{
			get
			{
				return this.m_randomizeBuild || !this.m_randomizeTone;
			}
		}

		// Token: 0x17000DBB RID: 3515
		// (get) Token: 0x06003BBA RID: 15290 RVA: 0x000686C9 File Offset: 0x000668C9
		private bool m_hideSkinColor
		{
			get
			{
				return this.m_randomizeColors || this.m_randomizeSkinColor;
			}
		}

		// Token: 0x17000DBC RID: 3516
		// (get) Token: 0x06003BBB RID: 15291 RVA: 0x000686DB File Offset: 0x000668DB
		private bool m_hideHairColor
		{
			get
			{
				return this.m_randomizeColors || this.m_randomizeHairColor;
			}
		}

		// Token: 0x17000DBD RID: 3517
		// (get) Token: 0x06003BBC RID: 15292 RVA: 0x000686ED File Offset: 0x000668ED
		private bool m_hideEyeColor
		{
			get
			{
				return this.m_randomizeColors || this.m_randomizeEyeColor;
			}
		}

		// Token: 0x17000DBE RID: 3518
		// (get) Token: 0x06003BBD RID: 15293 RVA: 0x000686FF File Offset: 0x000668FF
		private bool m_hideFavoriteColor
		{
			get
			{
				return this.m_randomizeColors || this.m_randomizeFavoriteColor;
			}
		}

		// Token: 0x17000DBF RID: 3519
		// (get) Token: 0x06003BBE RID: 15294 RVA: 0x00063BEB File Offset: 0x00061DEB
		private IEnumerable GetEnsembles
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<WardrobeRecipePairEnsemble>();
			}
		}

		// Token: 0x06003BBF RID: 15295 RVA: 0x00068711 File Offset: 0x00066911
		public bool TryGetSex(out CharacterSex sex)
		{
			sex = this.m_sex;
			return this.m_sex > CharacterSex.None;
		}

		// Token: 0x06003BC0 RID: 15296 RVA: 0x00068724 File Offset: 0x00066924
		public CharacterBuildType GetBuildType(System.Random seed)
		{
			if (this.m_randomizeBuild)
			{
				return CharacterBuildTypeExtensions.GetRandomBuildType(seed);
			}
			if (this.m_randomizeBuildType)
			{
				return CharacterBuildTypeExtensions.GetRandomBuildType(this.m_buildTypeFlags, seed);
			}
			return this.m_buildType;
		}

		// Token: 0x06003BC1 RID: 15297 RVA: 0x0017C640 File Offset: 0x0017A840
		public void SetDna(DynamicCharacterAvatar dca, System.Random seed, GameEntity entity)
		{
			if (dca == null)
			{
				throw new ArgumentNullException("dca");
			}
			if (seed == null)
			{
				throw new ArgumentException("seed");
			}
			if (this.m_randomizeBuild)
			{
				UMAManager.RandomizeDnaNew(dca, seed, entity);
				return;
			}
			float size = this.m_randomizeSize ? this.m_sizeRange.RandomWithinRange(seed) : this.m_size;
			float tone = this.m_randomizeTone ? this.m_toneRange.RandomWithinRange(seed) : this.m_tone;
			dca.predefinedDNA = UMAManager.GetPredefinedDna(entity, size, tone);
		}

		// Token: 0x06003BC2 RID: 15298 RVA: 0x0017C6C8 File Offset: 0x0017A8C8
		public void SetColors(DynamicCharacterAvatar dca, System.Random seed)
		{
			if (dca == null)
			{
				throw new ArgumentNullException("dca");
			}
			if (seed == null)
			{
				throw new ArgumentException("seed");
			}
			if (this.m_randomizeColors)
			{
				UMAManager.RandomizeColorsNew(dca, seed);
				return;
			}
			this.SetIndividualColor(this.m_randomizeSkinColor, this.m_skinColor, dca, seed, CharacterColorType.Skin);
			this.SetIndividualColor(this.m_randomizeHairColor, this.m_hairColor, dca, seed, CharacterColorType.Hair);
			this.SetIndividualColor(this.m_randomizeEyeColor, this.m_eyeColor, dca, seed, CharacterColorType.Eyes);
			this.SetIndividualColor(this.m_randomizeFavoriteColor, this.m_favoriteColor, dca, seed, CharacterColorType.Favorite);
		}

		// Token: 0x06003BC3 RID: 15299 RVA: 0x0006859A File Offset: 0x0006679A
		private void SetIndividualColor(bool randomize, Color color, DynamicCharacterAvatar dca, System.Random seed, CharacterColorType colorType)
		{
			if (randomize)
			{
				UMAManager.SetRandomColorNew(dca, seed, colorType);
				return;
			}
			colorType.SetDcaSharedColor(dca, color);
		}

		// Token: 0x06003BC4 RID: 15300 RVA: 0x0017C75C File Offset: 0x0017A95C
		public void SetCustomizations(DynamicCharacterAvatar dca, System.Random seed, CharacterSex sex)
		{
			if (dca == null)
			{
				throw new ArgumentNullException("dca");
			}
			if (this.m_randomizeCustomizations)
			{
				UMAManager.RandomizeCustomizationNew(dca, seed, sex);
				return;
			}
			for (int i = 0; i < this.m_customizations.Length; i++)
			{
				if (this.m_customizations[i] != null && this.m_customizations[i].WardrobeRecipe != null)
				{
					dca.SetSlot(this.m_customizations[i].WardrobeRecipe);
				}
			}
		}

		// Token: 0x06003BC5 RID: 15301 RVA: 0x0017C7DC File Offset: 0x0017A9DC
		public bool TryGetKnowledgeIndexByLabel(string label, out int knowledgeIndex)
		{
			knowledgeIndex = -1;
			for (int i = 0; i < this.m_knowledgeLabels.Length; i++)
			{
				if (this.m_knowledgeLabels[i] == label)
				{
					knowledgeIndex = i;
				}
			}
			return knowledgeIndex != -1;
		}

		// Token: 0x17000DC0 RID: 3520
		// (get) Token: 0x06003BC6 RID: 15302 RVA: 0x00068750 File Offset: 0x00066950
		private bool ShowImport
		{
			get
			{
				return !string.IsNullOrEmpty(this.m_importJson);
			}
		}

		// Token: 0x06003BC7 RID: 15303 RVA: 0x0017C81C File Offset: 0x0017AA1C
		private void Import()
		{
			try
			{
				CustomSerialization.Initialize();
				NpcProfile.CustomizedCharacter customizedCharacter = JsonConvert.DeserializeObject<NpcProfile.CustomizedCharacter>(this.m_importJson);
				if (customizedCharacter == null)
				{
					Debug.LogWarning("nothing?!");
				}
				else
				{
					this.m_ensemble = null;
					this.m_randomizeBuild = false;
					this.m_randomizeBuildType = false;
					this.m_randomizeSize = false;
					this.m_randomizeTone = false;
					this.m_randomizeCustomizations = false;
					this.m_randomizeColors = false;
					this.m_randomizeSkinColor = false;
					this.m_randomizeHairColor = false;
					this.m_randomizeEyeColor = false;
					this.m_randomizeFavoriteColor = false;
					CharacterVisuals visuals = customizedCharacter.Visuals;
					this.m_sex = visuals.Sex;
					this.m_buildType = visuals.BuildType;
					float left;
					visuals.Dna.TryGetValue("sizeSmall", out left);
					float right;
					visuals.Dna.TryGetValue("sizeLarge", out right);
					this.m_size = UMAManager.GetValueForLeftRight(left, right);
					float left2;
					visuals.Dna.TryGetValue("toneFlabby", out left2);
					float right2;
					visuals.Dna.TryGetValue("toneMuscular", out right2);
					this.m_tone = UMAManager.GetValueForLeftRight(left2, right2);
					List<FeatureRecipe> list = new List<FeatureRecipe>(visuals.CustomizedSlots.Count);
					foreach (UniqueId id in visuals.CustomizedSlots)
					{
						FeatureRecipe item;
						if (InternalGameDatabase.Archetypes.TryGetAsType<FeatureRecipe>(id, out item))
						{
							list.Add(item);
						}
					}
					this.m_customizations = list.ToArray();
					this.m_skinColor = this.GetColor(visuals, CharacterColorType.Skin);
					this.m_hairColor = this.GetColor(visuals, CharacterColorType.Hair);
					this.m_eyeColor = this.GetColor(visuals, CharacterColorType.Eyes);
					this.m_favoriteColor = this.GetColor(visuals, CharacterColorType.Favorite);
					List<NpcProfile.EquipableItemData> list2 = new List<NpcProfile.EquipableItemData>();
					List<NpcProfile.EquipableItemData> list3 = new List<NpcProfile.EquipableItemData>();
					foreach (NpcProfile.EquipmentData equipmentData in customizedCharacter.Equipment)
					{
						UniqueId id2 = new UniqueId(equipmentData.ArchetypeId);
						EquipableItem equipableItem;
						if (!id2.IsEmpty && InternalGameDatabase.Archetypes.TryGetAsType<EquipableItem>(id2, out equipableItem))
						{
							(equipableItem.Type.IsWeaponSlot() ? list3 : list2).Add(new NpcProfile.EquipableItemData
							{
								Item = equipableItem,
								Slot = (EquipmentSlot)equipmentData.Index,
								HasVisualIndex = equipmentData.HasVisualIndex,
								VisualIndex = equipmentData.VisualIndex,
								HasColorIndex = equipmentData.HasColorIndex,
								ColorIndex = equipmentData.ColorIndex
							});
						}
					}
					this.m_equipment = list2.ToArray();
					this.m_weapons = list3.ToArray();
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}

		// Token: 0x06003BC8 RID: 15304 RVA: 0x0017CB04 File Offset: 0x0017AD04
		private Color GetColor(CharacterVisuals visuals, CharacterColorType colorType)
		{
			string htmlString;
			Color result;
			if (!visuals.SharedColors.TryGetValue(colorType, out htmlString) || !ColorUtility.TryParseHtmlString(htmlString, out result))
			{
				return Color.white;
			}
			return result;
		}

		// Token: 0x06003BC9 RID: 15305 RVA: 0x0017CB34 File Offset: 0x0017AD34
		public void LoadEquipmentData(DynamicCharacterAvatar dca)
		{
			if (dca == null || !this.HasVisualEquipment)
			{
				return;
			}
			foreach (NpcProfile.EquipableItemData equipableItemData in this.m_equipment)
			{
				if (equipableItemData != null && equipableItemData.Item != null && equipableItemData.Item.Type.IsVisible() && !equipableItemData.Item.Type.IsWeaponSlot())
				{
					byte? visualIndex = equipableItemData.HasVisualIndex ? new byte?(equipableItemData.VisualIndex) : null;
					byte? colorIndex = equipableItemData.HasColorIndex ? new byte?(equipableItemData.ColorIndex) : null;
					equipableItemData.Item.OnEquipVisuals(this.m_sex, dca, (int)equipableItemData.Slot, visualIndex, colorIndex, false);
				}
			}
		}

		// Token: 0x06003BCA RID: 15306 RVA: 0x0017CC0C File Offset: 0x0017AE0C
		public bool TryGetVisibleWeapon(EquipmentSlot slot, out EquipableItem item)
		{
			item = null;
			if (this.m_weapons != null)
			{
				for (int i = 0; i < this.m_weapons.Length; i++)
				{
					if (this.m_weapons[i].Slot == slot && this.m_weapons[i].Item != null && this.m_weapons[i].Item.Type.IsVisible() && this.m_weapons[i].Item.Type.IsWeaponSlot())
					{
						item = this.m_weapons[i].Item;
						break;
					}
				}
			}
			return item != null;
		}

		// Token: 0x04003A47 RID: 14919
		private const string kVisuals = "Visuals";

		// Token: 0x04003A48 RID: 14920
		private const string kSeedGroup = "Seed";

		// Token: 0x04003A49 RID: 14921
		private const string kBuildGroup = "Build";

		// Token: 0x04003A4A RID: 14922
		private const string kCustoGroup = "Customizations";

		// Token: 0x04003A4B RID: 14923
		private const string kColorGroup = "Colors";

		// Token: 0x04003A4C RID: 14924
		private const string kImportGroup = "Import";

		// Token: 0x04003A4D RID: 14925
		[SerializeField]
		private string m_npcName;

		// Token: 0x04003A4E RID: 14926
		[SerializeField]
		private bool m_overrideSeed;

		// Token: 0x04003A4F RID: 14927
		[SerializeField]
		private int m_seedValue;

		// Token: 0x04003A50 RID: 14928
		[SerializeField]
		private CharacterSex m_sex = CharacterSex.Male;

		// Token: 0x04003A51 RID: 14929
		[SerializeField]
		private WardrobeRecipePairEnsemble m_ensemble;

		// Token: 0x04003A52 RID: 14930
		[SerializeField]
		private NpcProfile.EquipmentData[] m_equipmentData;

		// Token: 0x04003A53 RID: 14931
		[SerializeField]
		private NpcProfile.EquipableItemData[] m_equipment;

		// Token: 0x04003A54 RID: 14932
		[SerializeField]
		private NpcProfile.EquipableItemData[] m_weapons;

		// Token: 0x04003A55 RID: 14933
		[SerializeField]
		private bool m_randomizeBuild;

		// Token: 0x04003A56 RID: 14934
		[SerializeField]
		private bool m_randomizeBuildType;

		// Token: 0x04003A57 RID: 14935
		[SerializeField]
		private bool m_randomizeSize;

		// Token: 0x04003A58 RID: 14936
		[SerializeField]
		private bool m_randomizeTone;

		// Token: 0x04003A59 RID: 14937
		[SerializeField]
		private CharacterBuildType m_buildType = CharacterBuildType.Brawny;

		// Token: 0x04003A5A RID: 14938
		[SerializeField]
		private CharacterBuildTypeFlags m_buildTypeFlags = CharacterBuildTypeFlags.All;

		// Token: 0x04003A5B RID: 14939
		[Range(0f, 1f)]
		[SerializeField]
		private float m_size = 0.5f;

		// Token: 0x04003A5C RID: 14940
		[SerializeField]
		private MinMaxFloatRange m_sizeRange = new MinMaxFloatRange(0f, 1f);

		// Token: 0x04003A5D RID: 14941
		[Range(0f, 1f)]
		[SerializeField]
		private float m_tone = 0.5f;

		// Token: 0x04003A5E RID: 14942
		[SerializeField]
		private MinMaxFloatRange m_toneRange = new MinMaxFloatRange(0f, 1f);

		// Token: 0x04003A5F RID: 14943
		[SerializeField]
		private bool m_randomizeCustomizations;

		// Token: 0x04003A60 RID: 14944
		[SerializeField]
		private FeatureRecipe[] m_customizations;

		// Token: 0x04003A61 RID: 14945
		private const string kRandomizeLabel = "Randomize";

		// Token: 0x04003A62 RID: 14946
		private const string kSkinColorGroup = "Colors/Skin";

		// Token: 0x04003A63 RID: 14947
		private const string kHairColorGroup = "Colors/Hair";

		// Token: 0x04003A64 RID: 14948
		private const string kEyeColorGroup = "Colors/Eyes";

		// Token: 0x04003A65 RID: 14949
		private const string kFavoriteColorGroup = "Colors/Favorite";

		// Token: 0x04003A66 RID: 14950
		[SerializeField]
		private bool m_randomizeColors;

		// Token: 0x04003A67 RID: 14951
		[SerializeField]
		private bool m_randomizeSkinColor;

		// Token: 0x04003A68 RID: 14952
		[SerializeField]
		private Color m_skinColor = Color.white;

		// Token: 0x04003A69 RID: 14953
		[SerializeField]
		private bool m_randomizeHairColor;

		// Token: 0x04003A6A RID: 14954
		[FormerlySerializedAs("m_HairColor")]
		[SerializeField]
		private Color m_hairColor = Color.white;

		// Token: 0x04003A6B RID: 14955
		[SerializeField]
		private bool m_randomizeEyeColor;

		// Token: 0x04003A6C RID: 14956
		[SerializeField]
		private Color m_eyeColor = Color.white;

		// Token: 0x04003A6D RID: 14957
		[SerializeField]
		private bool m_randomizeFavoriteColor;

		// Token: 0x04003A6E RID: 14958
		[SerializeField]
		private Color m_favoriteColor = Color.white;

		// Token: 0x04003A6F RID: 14959
		[SerializeField]
		private string[] m_knowledgeLabels;

		// Token: 0x04003A70 RID: 14960
		[SerializeField]
		private string m_importJson;

		// Token: 0x0200080E RID: 2062
		[Serializable]
		public class CustomizedCharacter
		{
			// Token: 0x06003BCC RID: 15308 RVA: 0x0017CD40 File Offset: 0x0017AF40
			public CustomizedCharacter(CharacterRecord record)
			{
				if (record == null)
				{
					return;
				}
				this.Visuals = record.Visuals;
				this.Equipment = new List<NpcProfile.EquipmentData>();
				ContainerRecord containerRecord;
				if (record.Storage != null && record.Storage.TryGetValue(ContainerType.Equipment, out containerRecord))
				{
					foreach (ArchetypeInstance archetypeInstance in containerRecord.Instances)
					{
						EquipableItem equipableItem;
						if (archetypeInstance.Archetype.TryGetAsType(out equipableItem) && equipableItem.Type.IsVisible())
						{
							this.Equipment.Add(new NpcProfile.EquipmentData
							{
								ArchetypeId = archetypeInstance.ArchetypeId.Value,
								Index = archetypeInstance.Index,
								HasVisualIndex = (archetypeInstance.ItemData.VisualIndex != null),
								VisualIndex = ((archetypeInstance.ItemData.VisualIndex != null) ? archetypeInstance.ItemData.VisualIndex.Value : 0),
								HasColorIndex = (archetypeInstance.ItemData.ColorIndex != null),
								ColorIndex = ((archetypeInstance.ItemData.ColorIndex != null) ? archetypeInstance.ItemData.ColorIndex.Value : 0)
							});
						}
					}
				}
			}

			// Token: 0x06003BCD RID: 15309 RVA: 0x00068760 File Offset: 0x00066960
			public void CopyJsonToClipboard()
			{
				GUIUtility.systemCopyBuffer = JsonConvert.SerializeObject(this);
			}

			// Token: 0x04003A71 RID: 14961
			public CharacterVisuals Visuals;

			// Token: 0x04003A72 RID: 14962
			public List<NpcProfile.EquipmentData> Equipment;
		}

		// Token: 0x0200080F RID: 2063
		[Serializable]
		public class EquipmentData
		{
			// Token: 0x04003A73 RID: 14963
			public string ArchetypeId;

			// Token: 0x04003A74 RID: 14964
			public int Index;

			// Token: 0x04003A75 RID: 14965
			public bool HasVisualIndex;

			// Token: 0x04003A76 RID: 14966
			public byte VisualIndex;

			// Token: 0x04003A77 RID: 14967
			public bool HasColorIndex;

			// Token: 0x04003A78 RID: 14968
			public byte ColorIndex;
		}

		// Token: 0x02000810 RID: 2064
		[Serializable]
		public class EquipableItemData
		{
			// Token: 0x04003A79 RID: 14969
			public EquipableItem Item;

			// Token: 0x04003A7A RID: 14970
			public EquipmentSlot Slot;

			// Token: 0x04003A7B RID: 14971
			public bool HasVisualIndex;

			// Token: 0x04003A7C RID: 14972
			public byte VisualIndex;

			// Token: 0x04003A7D RID: 14973
			public bool HasColorIndex;

			// Token: 0x04003A7E RID: 14974
			public byte ColorIndex;
		}
	}
}

using System;
using System.Collections.Generic;
using RootMotion.FinalIK;
using SoL.Game.Animation;
using SoL.Game.Audio;
using SoL.Game.EffectSystem;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Pooling;
using SoL.Game.Settings;
using SoL.Networking.Database;
using SoL.Networking.Replication;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000584 RID: 1412
	public class HandheldMountController : GameEntityComponent
	{
		// Token: 0x1700094B RID: 2379
		// (get) Token: 0x06002C05 RID: 11269 RVA: 0x0005E8F9 File Offset: 0x0005CAF9
		private Dictionary<string, AudioEvent> AudioEvents
		{
			get
			{
				if (this.m_audioEvents == null)
				{
					this.m_audioEvents = StaticDictionaryPool<string, AudioEvent>.GetFromPool();
				}
				return this.m_audioEvents;
			}
		}

		// Token: 0x1700094C RID: 2380
		// (get) Token: 0x06002C06 RID: 11270 RVA: 0x0005E914 File Offset: 0x0005CB14
		private List<HandheldMountController.HandheldMountedItem> MountedItems
		{
			get
			{
				if (this.m_mountedItems == null)
				{
					this.m_mountedItems = StaticListPool<HandheldMountController.HandheldMountedItem>.GetFromPool();
				}
				return this.m_mountedItems;
			}
		}

		// Token: 0x1700094D RID: 2381
		// (get) Token: 0x06002C07 RID: 11271 RVA: 0x0005E92F File Offset: 0x0005CB2F
		internal bool ShadowsOnly
		{
			get
			{
				return this.m_isFirstPerson && this.m_localAttachedState == ItemsAttached.None;
			}
		}

		// Token: 0x1700094E RID: 2382
		// (get) Token: 0x06002C08 RID: 11272 RVA: 0x0005E944 File Offset: 0x0005CB44
		internal PooledHandheldItem EmberStoneItem
		{
			get
			{
				HandheldMountController.HandheldMountedItem emberStoneItem = this.m_emberStoneItem;
				if (emberStoneItem == null)
				{
					return null;
				}
				return emberStoneItem.Item;
			}
		}

		// Token: 0x1700094F RID: 2383
		// (get) Token: 0x06002C09 RID: 11273 RVA: 0x0005E957 File Offset: 0x0005CB57
		// (set) Token: 0x06002C0A RID: 11274 RVA: 0x0005E95F File Offset: 0x0005CB5F
		internal Color EmberStoneFillColor { get; private set; }

		// Token: 0x17000950 RID: 2384
		// (get) Token: 0x06002C0B RID: 11275 RVA: 0x0005E968 File Offset: 0x0005CB68
		// (set) Token: 0x06002C0C RID: 11276 RVA: 0x0005E970 File Offset: 0x0005CB70
		internal bool EmberStoneColorExternallyControlled { get; set; }

		// Token: 0x06002C0D RID: 11277 RVA: 0x00147694 File Offset: 0x00145894
		private void Awake()
		{
			if (base.GameEntity)
			{
				base.GameEntity.HandheldMountController = this;
			}
			bool flag = HandheldMountController.m_mountedItemIndexes == null;
			if (flag)
			{
				HandheldMountController.m_mountedItemIndexes = new HashSet<int>();
			}
			this.MountedItems.Clear();
			for (int i = 0; i < EquipmentExtensions.EquipmentSlots.Length; i++)
			{
				if (EquipmentExtensions.EquipmentSlots[i].IsHandheldMounted())
				{
					HandheldMountController.HandheldMountedItem handheldMountedItem = new HandheldMountController.HandheldMountedItem(this, EquipmentExtensions.EquipmentSlots[i]);
					this.MountedItems.Add(handheldMountedItem);
					if (handheldMountedItem.Slot == EquipmentSlot.EmberStone)
					{
						this.m_emberStoneItem = handheldMountedItem;
					}
					if (flag)
					{
						HandheldMountController.m_mountedItemIndexes.Add((int)EquipmentExtensions.EquipmentSlots[i]);
					}
				}
			}
		}

		// Token: 0x06002C0E RID: 11278 RVA: 0x0005E979 File Offset: 0x0005CB79
		private void Start()
		{
			if (this.m_dcaController != null && !this.m_dcaController.Initialized)
			{
				this.m_dcaController.DcaInitialized += this.DcaControllerOnDcaInitialized;
				return;
			}
			this.Init();
		}

		// Token: 0x06002C0F RID: 11279 RVA: 0x00147740 File Offset: 0x00145940
		private void OnDestroy()
		{
			if (base.GameEntity != null && base.GameEntity.CharacterData != null)
			{
				base.GameEntity.CharacterData.EmberStoneFillLevelChanged -= this.CharacterDataOnEmberStoneFillLevelChanged;
				base.GameEntity.CharacterData.ItemsAttached.Changed -= this.ItemsAttachedOnChanged;
				base.GameEntity.CharacterData.CurrentCombatId.Changed -= this.CurrentCombatIdOnChanged;
				base.GameEntity.CharacterData.VisibleEquipment.Changed -= this.VisibleEquipmentOnChanged;
				base.GameEntity.CharacterData.HandConfigurationChanged -= this.HandConfigurationChanged;
			}
			if (this.m_mountedItems != null)
			{
				for (int i = 0; i < this.m_mountedItems.Count; i++)
				{
					this.m_mountedItems[i].OnDestroy();
				}
				StaticListPool<HandheldMountController.HandheldMountedItem>.ReturnToPool(this.m_mountedItems);
			}
			HandheldMountController.HandheldMountedItem emberStoneItem = this.m_emberStoneItem;
			if (emberStoneItem != null)
			{
				emberStoneItem.OnDestroy();
			}
			this.m_emberStoneItem = null;
			if (this.m_audioEvents != null)
			{
				StaticDictionaryPool<string, AudioEvent>.ReturnToPool(this.m_audioEvents);
			}
		}

		// Token: 0x06002C10 RID: 11280 RVA: 0x00147874 File Offset: 0x00145A74
		public void UpdateIkTargets(FullBodyBipedIK fbbik, float targetWeightFraction)
		{
			if (!base.GameEntity || !base.GameEntity.CharacterData)
			{
				return;
			}
			Transform target = null;
			Transform target2 = null;
			for (int i = 0; i < this.MountedItems.Count; i++)
			{
				HandheldMountController.HandheldMountedItem handheldMountedItem = this.MountedItems[i];
				if (handheldMountedItem.IsVisible && handheldMountedItem.IsAttached && handheldMountedItem.Item && handheldMountedItem.Item.AttachedData != null && handheldMountedItem.Item.AttachedData.HandTargetType != PooledHandheldItem.HandTargetType.None && handheldMountedItem.Item.AttachedData.HandTarget)
				{
					PooledHandheldItem.HandTargetType handTargetType = handheldMountedItem.Item.AttachedData.HandTargetType;
					if (handTargetType != PooledHandheldItem.HandTargetType.Left)
					{
						if (handTargetType == PooledHandheldItem.HandTargetType.Right)
						{
							target2 = handheldMountedItem.Item.AttachedData.HandTarget.transform;
						}
					}
					else
					{
						target = handheldMountedItem.Item.AttachedData.HandTarget.transform;
					}
				}
			}
			IAnimationController animancerController = base.GameEntity.AnimancerController;
			bool flag = ((animancerController != null) ? animancerController.CurrentAnimationSet : null) != null && base.GameEntity.AnimancerController.CurrentAnimationSet.IsCombatStance;
			if (flag && flag != this.m_lastIsCombat)
			{
				this.m_stanceDelay = new float?(Time.time + 0.5f);
			}
			else if (this.m_stanceDelay != null && this.m_stanceDelay.Value <= Time.time)
			{
				this.m_stanceDelay = null;
			}
			bool permitEffectors = (this.m_stanceDelay == null && flag) || this.IsGathering;
			if (this.m_deferredHandIk != null)
			{
				if (this.m_deferredHandIk.Value > Time.time)
				{
					targetWeightFraction = 0f;
				}
				else
				{
					this.m_deferredHandIk = null;
				}
			}
			this.UpdateEffectorTarget(fbbik.solver, FullBodyBipedEffector.LeftHand, target, targetWeightFraction, permitEffectors);
			this.UpdateEffectorTarget(fbbik.solver, FullBodyBipedEffector.RightHand, target2, targetWeightFraction, permitEffectors);
			this.m_lastIsCombat = flag;
		}

		// Token: 0x06002C11 RID: 11281 RVA: 0x00147A80 File Offset: 0x00145C80
		private void UpdateEffectorTarget(IKSolverFullBodyBiped solver, FullBodyBipedEffector effectorType, Transform target, float targetWeightFraction, bool permitEffectors)
		{
			IKEffector effector = solver.GetEffector(effectorType);
			float num = 0f;
			if (permitEffectors && target)
			{
				num = ((base.GameEntity.CharacterData.Sex == CharacterSex.Male) ? 1f : 1f);
			}
			num *= targetWeightFraction;
			float positionWeight = (num <= 0f) ? 0f : Mathf.MoveTowards(effector.positionWeight, num, Time.deltaTime * 2f);
			effector.positionWeight = positionWeight;
			effector.target = target;
			solver.GetLimbMapping(effectorType).maintainRotationWeight = 1f;
		}

		// Token: 0x06002C12 RID: 11282 RVA: 0x0005E9B4 File Offset: 0x0005CBB4
		public void DeferHandIk(float? duration)
		{
			if (duration != null)
			{
				this.DeferHandIk(duration.Value);
			}
		}

		// Token: 0x06002C13 RID: 11283 RVA: 0x0005E9CC File Offset: 0x0005CBCC
		public void DeferHandIk(float duration)
		{
			this.m_deferredHandIk = new float?(Time.time + duration);
		}

		// Token: 0x06002C14 RID: 11284 RVA: 0x0005E9E0 File Offset: 0x0005CBE0
		public void CancelDeferredHandIk()
		{
			this.m_deferredHandIk = null;
		}

		// Token: 0x06002C15 RID: 11285 RVA: 0x0005E9EE File Offset: 0x0005CBEE
		private void DcaControllerOnDcaInitialized()
		{
			this.m_dcaController.DcaInitialized -= this.DcaControllerOnDcaInitialized;
			this.Init();
		}

		// Token: 0x06002C16 RID: 11286 RVA: 0x00147B14 File Offset: 0x00145D14
		private void Init()
		{
			if (base.GameEntity == null)
			{
				Debug.LogError("No GameEntity for HandheldMountController! " + base.gameObject.transform.GetPath());
				return;
			}
			if (base.GameEntity.CharacterData == null)
			{
				Debug.LogError("No Character data on GameEntity for HandheldMountController! " + base.gameObject.transform.GetPath());
				return;
			}
			if (this.m_customReferencePoints && this.m_assignReferencePointsToCharacterData)
			{
				base.GameEntity.CharacterData.ReferencePoints = new HumanoidReferencePoints?(this.m_referencePoints);
			}
			base.GameEntity.CharacterData.EmberStoneFillLevelChanged += this.CharacterDataOnEmberStoneFillLevelChanged;
			base.GameEntity.CharacterData.ItemsAttached.Changed += this.ItemsAttachedOnChanged;
			base.GameEntity.CharacterData.CurrentCombatId.Changed += this.CurrentCombatIdOnChanged;
			base.GameEntity.CharacterData.VisibleEquipment.Changed += this.VisibleEquipmentOnChanged;
			base.GameEntity.CharacterData.HandConfigurationChanged += this.HandConfigurationChanged;
			this.m_localAttachedState = base.GameEntity.CharacterData.ItemsAttached.Value;
			this.CurrentCombatIdOnChanged(base.GameEntity.CharacterData.CurrentCombatId.Value);
			for (int i = 0; i < this.MountedItems.Count; i++)
			{
				this.MountedItems[i].InitializeState();
			}
			this.RefreshAudioEvents();
		}

		// Token: 0x06002C17 RID: 11287 RVA: 0x00147CAC File Offset: 0x00145EAC
		private void CurrentCombatIdOnChanged(UniqueId obj)
		{
			if (this.m_currentCombatSet && this.m_currentCombatSet.Id == obj)
			{
				return;
			}
			InternalGameDatabase.Archetypes.TryGetAsType<AnimancerAnimationSet>(obj, out this.m_currentCombatSet);
			this.RefreshMountPosRot();
			for (int i = 0; i < this.MountedItems.Count; i++)
			{
				this.MountedItems[i].RefreshMountPoint();
			}
		}

		// Token: 0x06002C18 RID: 11288 RVA: 0x0005EA0D File Offset: 0x0005CC0D
		private void HandConfigurationChanged()
		{
			this.RefreshCurrentItems();
		}

		// Token: 0x06002C19 RID: 11289 RVA: 0x0005EA15 File Offset: 0x0005CC15
		private void VisibleEquipmentOnChanged(SynchronizedCollection<int, EquipableItemVisualData>.Operation arg1, int arg2, EquipableItemVisualData arg3, EquipableItemVisualData arg4)
		{
			if (HandheldMountController.m_mountedItemIndexes.Contains(arg2))
			{
				this.RefreshCurrentItems();
			}
		}

		// Token: 0x06002C1A RID: 11290 RVA: 0x0005EA2A File Offset: 0x0005CC2A
		private void ItemsAttachedOnChanged(ItemsAttached attached)
		{
			this.m_localAttachedState = attached;
			this.RefreshMountPosRot();
			this.RefreshAttachedState();
		}

		// Token: 0x06002C1B RID: 11291 RVA: 0x00147D1C File Offset: 0x00145F1C
		private void CharacterDataOnEmberStoneFillLevelChanged(EmberStoneFillLevels level)
		{
			this.EmberStoneFillColor = level.GetEmissiveColor(AlchemyPowerLevel.None);
			if (!this.EmberStoneColorExternallyControlled && this.m_emberStoneItem != null && this.m_emberStoneItem.Item && this.m_emberStoneItem.Item.RendererToAlter && this.m_emberStoneItem.Item.RendererToAlter.material)
			{
				this.m_emberStoneItem.Item.RendererToAlter.material.SetColor(ShaderExtensions.kEmissiveColorId, this.EmberStoneFillColor);
			}
		}

		// Token: 0x06002C1C RID: 11292 RVA: 0x00147DB0 File Offset: 0x00145FB0
		private void RefreshCurrentItems()
		{
			bool flag = false;
			for (int i = 0; i < this.MountedItems.Count; i++)
			{
				flag = (this.MountedItems[i].RefreshState() || flag);
			}
			if (flag)
			{
				this.RefreshAudioEvents();
			}
		}

		// Token: 0x06002C1D RID: 11293 RVA: 0x00147DF4 File Offset: 0x00145FF4
		private void RefreshAttachedState()
		{
			for (int i = 0; i < this.MountedItems.Count; i++)
			{
				this.MountedItems[i].RefreshAttachedState();
			}
		}

		// Token: 0x17000951 RID: 2385
		// (get) Token: 0x06002C1E RID: 11294 RVA: 0x0005EA3F File Offset: 0x0005CC3F
		private bool IsGathering
		{
			get
			{
				return this.m_visibleToolType > CraftingToolType.None;
			}
		}

		// Token: 0x06002C1F RID: 11295 RVA: 0x00147E28 File Offset: 0x00146028
		public void RefreshIsGathering()
		{
			CraftingToolType visibleToolType = this.m_visibleToolType;
			this.m_visibleToolType = ((base.GameEntity.SkillsController && base.GameEntity.SkillsController.Pending != null && base.GameEntity.SkillsController.Pending.Active) ? base.GameEntity.SkillsController.Pending.ActiveToolType : CraftingToolType.None);
			if (visibleToolType != this.m_visibleToolType)
			{
				this.m_visibleTool = null;
				for (int i = 0; i < this.MountedItems.Count; i++)
				{
					this.MountedItems[i].RefreshVisibility();
				}
				this.RefreshMountPosRot();
			}
		}

		// Token: 0x06002C20 RID: 11296 RVA: 0x00147ED0 File Offset: 0x001460D0
		public void RefreshIsLearning()
		{
			this.IsLearning = (base.GameEntity.SkillsController && base.GameEntity.SkillsController.Pending != null && base.GameEntity.SkillsController.Pending.Active && base.GameEntity.SkillsController.Pending.IsLearning);
			if (this.m_book)
			{
				this.m_book.SetActive(this.IsLearning);
				return;
			}
			if (this.IsLearning && GlobalSettings.Values && GlobalSettings.Values.Animation != null && GlobalSettings.Values.Animation.BookPrefab)
			{
				this.m_book = UnityEngine.Object.Instantiate<GameObject>(GlobalSettings.Values.Animation.BookPrefab, base.gameObject.transform);
				if (this.HumanoidReferencePoints != null)
				{
					this.m_book.transform.SetParent(this.HumanoidReferencePoints.Value.LeftMount.transform);
					this.m_book.transform.localPosition = Vector3.zero;
					this.m_book.transform.localRotation = Quaternion.identity;
				}
			}
		}

		// Token: 0x06002C21 RID: 11297 RVA: 0x00148020 File Offset: 0x00146220
		private void RefreshMountPosRot()
		{
			if (this.HumanoidReferencePoints == null)
			{
				return;
			}
			Vector3 handMountPos_L = AnimatorExtensions.HandMountPos_L;
			Quaternion handMountRot_L = AnimatorExtensions.HandMountRot_L;
			Vector3 handMountPos_R = AnimatorExtensions.HandMountPos_R;
			Quaternion handMountRot_R = AnimatorExtensions.HandMountRot_R;
			if (this.m_localAttachedState == ItemsAttached.Weapons && this.m_currentCombatSet && !this.IsGathering)
			{
				this.m_currentCombatSet.UpdateLeftRightMountData(ref handMountPos_L, ref handMountRot_L, ref handMountPos_R, ref handMountRot_R);
				this.m_lastCombatSetId = new UniqueId?(this.m_currentCombatSet.Id);
			}
			this.HumanoidReferencePoints.Value.LeftMount.transform.localPosition = handMountPos_L;
			this.HumanoidReferencePoints.Value.LeftMount.transform.localRotation = handMountRot_L;
			this.HumanoidReferencePoints.Value.RightMount.transform.localPosition = handMountPos_R;
			this.HumanoidReferencePoints.Value.RightMount.transform.localRotation = handMountRot_R;
		}

		// Token: 0x17000952 RID: 2386
		// (get) Token: 0x06002C22 RID: 11298 RVA: 0x0014811C File Offset: 0x0014631C
		private HumanoidReferencePoints? HumanoidReferencePoints
		{
			get
			{
				if (!this.m_cachedHumanoidReferencePoints && this.m_humanoidReferencePoints == null)
				{
					if (this.m_customReferencePoints)
					{
						this.m_humanoidReferencePoints = new HumanoidReferencePoints?(this.m_referencePoints);
					}
					else if (base.GameEntity.CharacterData.ReferencePoints != null)
					{
						this.m_humanoidReferencePoints = new HumanoidReferencePoints?(base.GameEntity.CharacterData.ReferencePoints.Value);
					}
					this.m_cachedHumanoidReferencePoints = true;
				}
				return this.m_humanoidReferencePoints;
			}
		}

		// Token: 0x06002C23 RID: 11299 RVA: 0x001481A4 File Offset: 0x001463A4
		private void RefreshAudioEvents()
		{
			if (base.GameEntity == null || base.GameEntity.AudioEventController == null)
			{
				return;
			}
			foreach (KeyValuePair<string, AudioEvent> keyValuePair in this.AudioEvents)
			{
				base.GameEntity.AudioEventController.UnregisterEvent(keyValuePair.Value);
			}
			this.AudioEvents.Clear();
			for (int i = 0; i < this.MountedItems.Count; i++)
			{
				if (this.MountedItems[i].IsVisible && this.MountedItems[i].Item != null && this.MountedItems[i].Item.AudioEvents != null)
				{
					for (int j = 0; j < this.MountedItems[i].Item.AudioEvents.Length; j++)
					{
						if (!string.IsNullOrEmpty(this.MountedItems[i].Item.AudioEvents[j].EventName) && this.MountedItems[i].Item.AudioEvents[j].GetClipCount() > 0)
						{
							this.AudioEvents.AddOrReplace(this.MountedItems[i].Item.AudioEvents[j].EventName, this.MountedItems[i].Item.AudioEvents[j]);
						}
					}
				}
			}
			foreach (KeyValuePair<string, AudioEvent> keyValuePair2 in this.AudioEvents)
			{
				base.GameEntity.AudioEventController.RegisterEvent(keyValuePair2.Value);
			}
		}

		// Token: 0x06002C24 RID: 11300 RVA: 0x0005EA4A File Offset: 0x0005CC4A
		internal void AttachWeapons()
		{
			if (base.GameEntity.NetworkEntity.IsLocal)
			{
				base.GameEntity.CharacterData.ItemsAttached.Value = ItemsAttached.Weapons;
			}
			this.m_localAttachedState = ItemsAttached.Weapons;
			this.RefreshAttachedState();
		}

		// Token: 0x06002C25 RID: 11301 RVA: 0x001483A0 File Offset: 0x001465A0
		internal void DetachWeapons()
		{
			if (base.GameEntity.NetworkEntity.IsLocal && base.GameEntity.CharacterData.ItemsAttached.Value == ItemsAttached.Weapons)
			{
				base.GameEntity.CharacterData.ItemsAttached.Value = ItemsAttached.None;
			}
			if (this.m_localAttachedState == ItemsAttached.Weapons)
			{
				this.m_localAttachedState = ItemsAttached.None;
			}
			this.RefreshAttachedState();
		}

		// Token: 0x06002C26 RID: 11302 RVA: 0x0005EA81 File Offset: 0x0005CC81
		internal void AttachLight()
		{
			if (base.GameEntity.NetworkEntity.IsLocal)
			{
				base.GameEntity.CharacterData.ItemsAttached.Value = ItemsAttached.Light;
			}
			this.m_localAttachedState = ItemsAttached.Light;
			this.RefreshAttachedState();
		}

		// Token: 0x06002C27 RID: 11303 RVA: 0x00148404 File Offset: 0x00146604
		internal void DetachLight()
		{
			if (base.GameEntity.NetworkEntity.IsLocal && base.GameEntity.CharacterData.ItemsAttached.Value == ItemsAttached.Light)
			{
				base.GameEntity.CharacterData.ItemsAttached.Value = ItemsAttached.None;
			}
			if (this.m_localAttachedState == ItemsAttached.Light)
			{
				this.m_localAttachedState = ItemsAttached.None;
			}
			this.RefreshAttachedState();
		}

		// Token: 0x06002C28 RID: 11304 RVA: 0x00148468 File Offset: 0x00146668
		public void ToggleFirstPerson(bool isFirstPerson)
		{
			bool flag = isFirstPerson != this.m_isFirstPerson;
			this.m_isFirstPerson = isFirstPerson;
			if (flag)
			{
				for (int i = 0; i < this.MountedItems.Count; i++)
				{
					this.MountedItems[i].RefreshShadowsOnly();
				}
			}
		}

		// Token: 0x04002BDE RID: 11230
		private static HashSet<int> m_mountedItemIndexes;

		// Token: 0x04002BDF RID: 11231
		private ItemsAttached m_localAttachedState;

		// Token: 0x04002BE0 RID: 11232
		private AnimancerAnimationSet m_currentCombatSet;

		// Token: 0x04002BE1 RID: 11233
		private Dictionary<string, AudioEvent> m_audioEvents;

		// Token: 0x04002BE2 RID: 11234
		private List<HandheldMountController.HandheldMountedItem> m_mountedItems;

		// Token: 0x04002BE3 RID: 11235
		private HandheldMountController.HandheldMountedItem m_emberStoneItem;

		// Token: 0x04002BE4 RID: 11236
		private bool m_isFirstPerson;

		// Token: 0x04002BE5 RID: 11237
		[SerializeField]
		private DCAController m_dcaController;

		// Token: 0x04002BE6 RID: 11238
		[SerializeField]
		private bool m_customReferencePoints;

		// Token: 0x04002BE7 RID: 11239
		[SerializeField]
		private bool m_assignReferencePointsToCharacterData;

		// Token: 0x04002BE8 RID: 11240
		[SerializeField]
		private HumanoidReferencePoints m_referencePoints;

		// Token: 0x04002BEB RID: 11243
		private const float kMaleWeight = 1f;

		// Token: 0x04002BEC RID: 11244
		private const float kFemaleWeight = 1f;

		// Token: 0x04002BED RID: 11245
		private bool m_lastIsCombat;

		// Token: 0x04002BEE RID: 11246
		private float? m_stanceDelay;

		// Token: 0x04002BEF RID: 11247
		private float? m_deferredHandIk;

		// Token: 0x04002BF0 RID: 11248
		private const float kStanceDelayTransition = 0.5f;

		// Token: 0x04002BF1 RID: 11249
		private CraftingToolType m_visibleToolType;

		// Token: 0x04002BF2 RID: 11250
		private HandheldMountController.HandheldMountedItem m_visibleTool;

		// Token: 0x04002BF3 RID: 11251
		private bool IsLearning;

		// Token: 0x04002BF4 RID: 11252
		private GameObject m_book;

		// Token: 0x04002BF5 RID: 11253
		private UniqueId? m_lastCombatSetId;

		// Token: 0x04002BF6 RID: 11254
		private bool m_cachedHumanoidReferencePoints;

		// Token: 0x04002BF7 RID: 11255
		private HumanoidReferencePoints? m_humanoidReferencePoints;

		// Token: 0x02000585 RID: 1413
		private class HandheldMountedItem
		{
			// Token: 0x17000953 RID: 2387
			// (get) Token: 0x06002C2A RID: 11306 RVA: 0x0005EAB8 File Offset: 0x0005CCB8
			public PooledHandheldItem Item
			{
				get
				{
					return this.m_item;
				}
			}

			// Token: 0x17000954 RID: 2388
			// (get) Token: 0x06002C2B RID: 11307 RVA: 0x0005EAC0 File Offset: 0x0005CCC0
			public EquipmentSlot Slot
			{
				get
				{
					return this.m_slot;
				}
			}

			// Token: 0x17000955 RID: 2389
			// (get) Token: 0x06002C2C RID: 11308 RVA: 0x0005EAC8 File Offset: 0x0005CCC8
			// (set) Token: 0x06002C2D RID: 11309 RVA: 0x001484B4 File Offset: 0x001466B4
			public bool IsAttached
			{
				get
				{
					return this.m_isAttached;
				}
				private set
				{
					if (this.m_initialized && this.m_isAttached == value)
					{
						return;
					}
					this.m_isAttached = value;
					if (this.m_item != null)
					{
						if (this.m_controller.HumanoidReferencePoints != null)
						{
							this.RefreshMountPoint();
							return;
						}
						this.IsVisible = false;
					}
				}
			}

			// Token: 0x17000956 RID: 2390
			// (get) Token: 0x06002C2E RID: 11310 RVA: 0x0005EAD0 File Offset: 0x0005CCD0
			// (set) Token: 0x06002C2F RID: 11311 RVA: 0x0005EAD8 File Offset: 0x0005CCD8
			public bool IsVisible
			{
				get
				{
					return this.m_isVisible;
				}
				private set
				{
					this.m_isVisible = value;
					if (this.m_item)
					{
						if (this.m_isVisible)
						{
							this.RefreshMountPoint();
							return;
						}
						this.m_item.SetItemsActive(this.m_isVisible);
					}
				}
			}

			// Token: 0x17000957 RID: 2391
			// (set) Token: 0x06002C30 RID: 11312 RVA: 0x0014850C File Offset: 0x0014670C
			private EquipableItemVisualData? ItemVisuals
			{
				set
				{
					if (this.m_itemVisuals == value)
					{
						return;
					}
					if (this.m_item != null)
					{
						this.m_item.ReturnToPool();
						this.m_item = null;
					}
					this.m_itemVisuals = value;
					this.m_isEmberStone = false;
					if (this.m_itemVisuals != null)
					{
						PooledHandheldItem pooledHandheldItem = null;
						this.m_toolType = CraftingToolType.None;
						EquipableItem equipableItem;
						EmberStone emberStone;
						if (InternalGameDatabase.Archetypes.TryGetAsType<EquipableItem>(this.m_itemVisuals.Value.ArchetypeId, out equipableItem))
						{
							pooledHandheldItem = equipableItem.GetHandheldItem(this.m_itemVisuals.Value.VisualIndex);
							CraftingToolItem craftingToolItem;
							if (equipableItem.TryGetAsType(out craftingToolItem))
							{
								this.m_toolType = craftingToolItem.ToolType;
							}
						}
						else if (InternalGameDatabase.Archetypes.TryGetAsType<EmberStone>(this.m_itemVisuals.Value.ArchetypeId, out emberStone))
						{
							pooledHandheldItem = emberStone.HandHeldItem;
							this.m_isEmberStone = true;
						}
						if (pooledHandheldItem != null)
						{
							this.m_item = pooledHandheldItem.GetPooledInstance<PooledHandheldItem>();
							this.m_item.Initialize(this.m_controller, this.m_controller.GameEntity.gameObject.transform, Vector3.zero, Quaternion.identity);
							this.RefreshMountPoint();
							if (this.m_isEmberStone)
							{
								this.m_controller.CharacterDataOnEmberStoneFillLevelChanged(this.m_controller.GameEntity.CharacterData.EmberStoneFillLevel);
							}
						}
					}
				}
			}

			// Token: 0x06002C31 RID: 11313 RVA: 0x0005EB0E File Offset: 0x0005CD0E
			public HandheldMountedItem(HandheldMountController controller, EquipmentSlot slot)
			{
				this.m_controller = controller;
				this.m_slot = slot;
			}

			// Token: 0x06002C32 RID: 11314 RVA: 0x0005EB24 File Offset: 0x0005CD24
			public void InitializeState()
			{
				this.RefreshState();
				this.m_initialized = true;
			}

			// Token: 0x06002C33 RID: 11315 RVA: 0x0014868C File Offset: 0x0014688C
			public bool RefreshState()
			{
				EquipableItemVisualData? itemVisuals = this.m_itemVisuals;
				bool isVisible = this.IsVisible;
				EquipableItemVisualData value;
				if (this.m_controller.GameEntity.CharacterData.VisibleEquipment.TryGetValue((int)this.m_slot, out value))
				{
					this.ItemVisuals = new EquipableItemVisualData?(value);
				}
				else
				{
					this.ItemVisuals = null;
				}
				this.RefreshAttached();
				this.RefreshVisibility();
				this.RefreshShadowsOnly();
				return this.m_itemVisuals != itemVisuals || this.IsVisible != isVisible;
			}

			// Token: 0x06002C34 RID: 11316 RVA: 0x0005EB34 File Offset: 0x0005CD34
			public void RefreshAttachedState()
			{
				this.RefreshAttached();
				this.RefreshVisibility();
				this.RefreshShadowsOnly();
			}

			// Token: 0x06002C35 RID: 11317 RVA: 0x00148744 File Offset: 0x00146944
			private void RefreshAttached()
			{
				EquipmentSlot slot = this.m_slot;
				if (slot <= EquipmentSlot.Tool1)
				{
					if (slot <= EquipmentSlot.SecondaryWeapon_MainHand)
					{
						if (slot - EquipmentSlot.PrimaryWeapon_MainHand > 1 && slot != EquipmentSlot.SecondaryWeapon_MainHand)
						{
							goto IL_8C;
						}
					}
					else if (slot != EquipmentSlot.SecondaryWeapon_OffHand)
					{
						if (slot != EquipmentSlot.Tool1)
						{
							goto IL_8C;
						}
						goto IL_84;
					}
					this.IsAttached = (this.m_controller.m_localAttachedState == ItemsAttached.Weapons);
					return;
				}
				if (slot <= EquipmentSlot.Tool3)
				{
					if (slot != EquipmentSlot.Tool2 && slot != EquipmentSlot.Tool3)
					{
						goto IL_8C;
					}
				}
				else if (slot != EquipmentSlot.Tool4)
				{
					if (slot == EquipmentSlot.LightSource)
					{
						this.IsAttached = (this.m_controller.m_localAttachedState == ItemsAttached.Light);
						return;
					}
					if (slot != EquipmentSlot.EmberStone)
					{
						goto IL_8C;
					}
					this.IsAttached = true;
					return;
				}
				IL_84:
				this.IsAttached = true;
				return;
				IL_8C:
				throw new ArgumentException("Invalid SlotType for HandheldMountedItem! m_slot");
			}

			// Token: 0x06002C36 RID: 11318 RVA: 0x001487E8 File Offset: 0x001469E8
			internal void RefreshVisibility()
			{
				if (!this.m_controller.IsGathering)
				{
					EquipmentSlot slot = this.m_slot;
					bool isVisible;
					if (slot <= EquipmentSlot.Tool2)
					{
						if (slot <= EquipmentSlot.SecondaryWeapon_OffHand)
						{
							switch (slot)
							{
							case EquipmentSlot.PrimaryWeapon_MainHand:
								isVisible = !this.m_controller.GameEntity.CharacterData.MainHand_SecondaryActive;
								goto IL_188;
							case EquipmentSlot.PrimaryWeapon_OffHand:
								isVisible = !this.m_controller.GameEntity.CharacterData.OffHand_SecondaryActive;
								goto IL_188;
							case EquipmentSlot.PrimaryWeapon_MainHand | EquipmentSlot.PrimaryWeapon_OffHand:
								goto IL_17D;
							case EquipmentSlot.SecondaryWeapon_MainHand:
								isVisible = this.m_controller.GameEntity.CharacterData.MainHand_SecondaryActive;
								goto IL_188;
							default:
								if (slot != EquipmentSlot.SecondaryWeapon_OffHand)
								{
									goto IL_17D;
								}
								isVisible = this.m_controller.GameEntity.CharacterData.OffHand_SecondaryActive;
								goto IL_188;
							}
						}
						else if (slot != EquipmentSlot.Tool1 && slot != EquipmentSlot.Tool2)
						{
							goto IL_17D;
						}
					}
					else if (slot <= EquipmentSlot.Tool4)
					{
						if (slot != EquipmentSlot.Tool3 && slot != EquipmentSlot.Tool4)
						{
							goto IL_17D;
						}
					}
					else
					{
						if (slot == EquipmentSlot.LightSource)
						{
							isVisible = (this.m_controller.m_localAttachedState == ItemsAttached.Light);
							goto IL_188;
						}
						if (slot != EquipmentSlot.EmberStone)
						{
							goto IL_17D;
						}
						isVisible = true;
						goto IL_188;
					}
					isVisible = false;
					goto IL_188;
					IL_17D:
					throw new ArgumentException("Invalid SlotType for HandheldMountedItem! m_slot");
					IL_188:
					this.IsVisible = isVisible;
					return;
				}
				if (this.m_toolType == CraftingToolType.None)
				{
					this.IsVisible = (this.m_isEmberStone || (!this.IsAttached && this.IsVisible));
					return;
				}
				if (this.m_controller.m_visibleToolType == this.m_toolType)
				{
					if (this.m_controller.m_visibleTool == null)
					{
						this.m_controller.m_visibleTool = this;
					}
					this.IsVisible = (this.m_controller.m_visibleTool == this);
					return;
				}
				this.IsVisible = false;
			}

			// Token: 0x06002C37 RID: 11319 RVA: 0x00148984 File Offset: 0x00146B84
			internal void RefreshShadowsOnly()
			{
				if (!this.m_controller || this.m_controller.GameEntity != LocalPlayer.GameEntity)
				{
					return;
				}
				EquipmentSlot slot = this.m_slot;
				if (slot <= EquipmentSlot.SecondaryWeapon_MainHand)
				{
					if (slot - EquipmentSlot.PrimaryWeapon_MainHand > 1 && slot != EquipmentSlot.SecondaryWeapon_MainHand)
					{
						return;
					}
				}
				else if (slot != EquipmentSlot.SecondaryWeapon_OffHand && slot != EquipmentSlot.EmberStone)
				{
					return;
				}
				if (this.m_item)
				{
					this.m_item.RefreshLocalShadowsOnly();
				}
			}

			// Token: 0x06002C38 RID: 11320 RVA: 0x001489F0 File Offset: 0x00146BF0
			public void RefreshMountPoint()
			{
				if (this.m_item && this.IsVisible)
				{
					this.m_item.MountItem(this.m_controller.HumanoidReferencePoints, this.m_slot, this.IsAttached, this.m_controller.m_currentCombatSet, this.m_controller.GameEntity);
				}
			}

			// Token: 0x06002C39 RID: 11321 RVA: 0x0005EB48 File Offset: 0x0005CD48
			public void OnDestroy()
			{
				if (this.m_item)
				{
					this.m_item.ReturnToPool();
				}
				this.m_controller = null;
			}

			// Token: 0x04002BF8 RID: 11256
			private HandheldMountController m_controller;

			// Token: 0x04002BF9 RID: 11257
			private readonly EquipmentSlot m_slot;

			// Token: 0x04002BFA RID: 11258
			private bool m_initialized;

			// Token: 0x04002BFB RID: 11259
			private bool m_isEmberStone;

			// Token: 0x04002BFC RID: 11260
			private PooledHandheldItem m_item;

			// Token: 0x04002BFD RID: 11261
			private CraftingToolType m_toolType;

			// Token: 0x04002BFE RID: 11262
			private bool m_isAttached;

			// Token: 0x04002BFF RID: 11263
			private bool m_isVisible;

			// Token: 0x04002C00 RID: 11264
			private EquipableItemVisualData? m_itemVisuals;
		}
	}
}

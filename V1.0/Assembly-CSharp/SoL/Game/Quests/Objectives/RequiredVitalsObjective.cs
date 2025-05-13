using System;
using System.Collections.Generic;
using SoL.Game.Objects;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007B4 RID: 1972
	[CreateAssetMenu(menuName = "SoL/Quests/Objectives/RequiredVitalsObjective")]
	public class RequiredVitalsObjective : QuestObjective
	{
		// Token: 0x060039F9 RID: 14841 RVA: 0x000674B2 File Offset: 0x000656B2
		public override bool Validate(GameEntity sourceEntity, ObjectiveIterationCache cache, out string message)
		{
			if (cache.IterationsRequested > 1)
			{
				message = "Too many iterations requested.";
				return false;
			}
			return this.MeetsRequirements(sourceEntity, out message);
		}

		// Token: 0x060039FA RID: 14842 RVA: 0x000674CE File Offset: 0x000656CE
		public override void OnEntityInitializedWhenActive(GameEntity sourceEntity, UniqueId questOrTaskId)
		{
			base.OnEntityInitializedWhenActive(sourceEntity, questOrTaskId);
			if (GameManager.IsServer)
			{
				return;
			}
			if (!this.TryAdvance(questOrTaskId, sourceEntity))
			{
				this.SubscribeToVitals(sourceEntity);
				RequiredVitalsObjective.m_orders.Add(new ValueTuple<UniqueId, RequiredVitalsObjective>(questOrTaskId, this));
			}
		}

		// Token: 0x060039FB RID: 14843 RVA: 0x00174F28 File Offset: 0x00173128
		public override void OnEntityDestroyedWhenActive(GameEntity sourceEntity, UniqueId questOrTaskId)
		{
			base.OnEntityDestroyedWhenActive(sourceEntity, questOrTaskId);
			if (GameManager.IsServer)
			{
				return;
			}
			this.UnsubscribeFromVitals(sourceEntity);
			RequiredVitalsObjective.m_orders.RemoveAll((ValueTuple<UniqueId, RequiredVitalsObjective> x) => x.Item1 == questOrTaskId && x.Item2 == this);
		}

		// Token: 0x060039FC RID: 14844 RVA: 0x00067502 File Offset: 0x00065702
		public override void OnActivate(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			base.OnActivate(cache, sourceEntity);
			if (GameManager.IsServer)
			{
				return;
			}
			if (!this.TryAdvance(cache.QuestId, sourceEntity))
			{
				this.SubscribeToVitals(sourceEntity);
				RequiredVitalsObjective.m_orders.Add(new ValueTuple<UniqueId, RequiredVitalsObjective>(cache.QuestId, this));
			}
		}

		// Token: 0x060039FD RID: 14845 RVA: 0x00174F7C File Offset: 0x0017317C
		public override void OnDeactivate(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			base.OnDeactivate(cache, sourceEntity);
			if (GameManager.IsServer)
			{
				return;
			}
			this.UnsubscribeFromVitals(sourceEntity);
			RequiredVitalsObjective.m_orders.RemoveAll((ValueTuple<UniqueId, RequiredVitalsObjective> x) => x.Item1 == cache.QuestId && x.Item2 == this);
		}

		// Token: 0x060039FE RID: 14846 RVA: 0x00174FD0 File Offset: 0x001731D0
		private void SubscribeToVitals(GameEntity sourceEntity)
		{
			if (sourceEntity == LocalPlayer.GameEntity)
			{
				VitalsReplicatorPlayer vitalsReplicatorPlayer = sourceEntity.VitalsReplicator as VitalsReplicatorPlayer;
				if (vitalsReplicatorPlayer != null)
				{
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					bool flag4 = false;
					VitalsRequirement[] requirements = this.m_requirements;
					for (int i = 0; i < requirements.Length; i++)
					{
						switch (requirements[i].Type)
						{
						case VitalType.Health:
							if (!flag)
							{
								vitalsReplicatorPlayer.Health.Changed += this.OnHealthChanged;
								flag = true;
							}
							break;
						case VitalType.HealthWound:
							if (!flag2)
							{
								vitalsReplicatorPlayer.HealthWound.Changed += this.OnHealthWoundChanged;
								flag2 = true;
							}
							break;
						case VitalType.Stamina:
							if (!flag3)
							{
								vitalsReplicatorPlayer.Stamina.Changed += this.OnStaminaChanged;
								flag3 = true;
							}
							break;
						case VitalType.StaminaWound:
							if (!flag4)
							{
								vitalsReplicatorPlayer.StaminaWound.Changed += this.OnStaminaWoundChanged;
								flag4 = true;
							}
							break;
						}
					}
				}
			}
		}

		// Token: 0x060039FF RID: 14847 RVA: 0x001750C8 File Offset: 0x001732C8
		private void UnsubscribeFromVitals(GameEntity sourceEntity)
		{
			if (sourceEntity == LocalPlayer.GameEntity)
			{
				VitalsReplicatorPlayer vitalsReplicatorPlayer = sourceEntity.VitalsReplicator as VitalsReplicatorPlayer;
				if (vitalsReplicatorPlayer != null)
				{
					vitalsReplicatorPlayer.Health.Changed -= this.OnHealthChanged;
					vitalsReplicatorPlayer.HealthWound.Changed -= this.OnHealthWoundChanged;
					vitalsReplicatorPlayer.Stamina.Changed -= this.OnStaminaChanged;
					vitalsReplicatorPlayer.StaminaWound.Changed -= this.OnStaminaWoundChanged;
				}
			}
		}

		// Token: 0x06003A00 RID: 14848 RVA: 0x00175150 File Offset: 0x00173350
		private void OnHealthChanged(int value)
		{
			foreach (ValueTuple<UniqueId, RequiredVitalsObjective> valueTuple in RequiredVitalsObjective.m_orders)
			{
				if (valueTuple.Item2 == this)
				{
					this.TryAdvance(valueTuple.Item1, LocalPlayer.GameEntity);
				}
			}
		}

		// Token: 0x06003A01 RID: 14849 RVA: 0x00175150 File Offset: 0x00173350
		private void OnHealthWoundChanged(byte value)
		{
			foreach (ValueTuple<UniqueId, RequiredVitalsObjective> valueTuple in RequiredVitalsObjective.m_orders)
			{
				if (valueTuple.Item2 == this)
				{
					this.TryAdvance(valueTuple.Item1, LocalPlayer.GameEntity);
				}
			}
		}

		// Token: 0x06003A02 RID: 14850 RVA: 0x00175150 File Offset: 0x00173350
		private void OnStaminaChanged(byte value)
		{
			foreach (ValueTuple<UniqueId, RequiredVitalsObjective> valueTuple in RequiredVitalsObjective.m_orders)
			{
				if (valueTuple.Item2 == this)
				{
					this.TryAdvance(valueTuple.Item1, LocalPlayer.GameEntity);
				}
			}
		}

		// Token: 0x06003A03 RID: 14851 RVA: 0x00175150 File Offset: 0x00173350
		private void OnStaminaWoundChanged(byte value)
		{
			foreach (ValueTuple<UniqueId, RequiredVitalsObjective> valueTuple in RequiredVitalsObjective.m_orders)
			{
				if (valueTuple.Item2 == this)
				{
					this.TryAdvance(valueTuple.Item1, LocalPlayer.GameEntity);
				}
			}
		}

		// Token: 0x06003A04 RID: 14852 RVA: 0x001751BC File Offset: 0x001733BC
		private bool TryAdvance(UniqueId questId, GameEntity entity)
		{
			string text;
			Quest quest;
			int hash;
			if (this.MeetsRequirements(entity, out text) && InternalGameDatabase.Quests.TryGetItem(questId, out quest) && quest.TryGetObjectiveHashForActiveObjective(base.Id, out hash))
			{
				GameManager.QuestManager.Progress(new ObjectiveIterationCache
				{
					QuestId = questId,
					ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(hash)
				}, null, false);
				return true;
			}
			BBTask bbtask;
			if (this.MeetsRequirements(entity, out text) && InternalGameDatabase.BBTasks.TryGetItem(questId, out bbtask))
			{
				GameManager.QuestManager.ProgressTask(new ObjectiveIterationCache
				{
					QuestId = questId,
					ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(base.CombinedId(questId))
				}, null, false);
				return true;
			}
			return false;
		}

		// Token: 0x06003A05 RID: 14853 RVA: 0x0017526C File Offset: 0x0017346C
		private bool MeetsRequirements(GameEntity sourceEntity, out string message)
		{
			message = string.Empty;
			bool flag = true;
			foreach (VitalsRequirement vitalsRequirement in this.m_requirements)
			{
				switch (vitalsRequirement.Type)
				{
				case VitalType.Health:
					switch (vitalsRequirement.Comparator)
					{
					case NumericComparator.EqualTo:
						flag = (flag && sourceEntity.Vitals.Health == (float)vitalsRequirement.Value);
						break;
					case NumericComparator.GreaterThan:
						flag = (flag && sourceEntity.Vitals.Health > (float)vitalsRequirement.Value);
						break;
					case NumericComparator.GreaterThanOrEqualTo:
						flag = (flag && sourceEntity.Vitals.Health >= (float)vitalsRequirement.Value);
						break;
					case NumericComparator.LessThan:
						flag = (flag && sourceEntity.Vitals.Health < (float)vitalsRequirement.Value);
						break;
					case NumericComparator.LessThanOrEqualTo:
						flag = (flag && sourceEntity.Vitals.Health <= (float)vitalsRequirement.Value);
						break;
					}
					if (message == string.Empty && !flag)
					{
						message = "Health requirements not met.";
					}
					break;
				case VitalType.HealthWound:
					switch (vitalsRequirement.Comparator)
					{
					case NumericComparator.EqualTo:
						flag = (flag && sourceEntity.Vitals.HealthWound == (float)vitalsRequirement.Value);
						break;
					case NumericComparator.GreaterThan:
						flag = (flag && sourceEntity.Vitals.HealthWound > (float)vitalsRequirement.Value);
						break;
					case NumericComparator.GreaterThanOrEqualTo:
						flag = (flag && sourceEntity.Vitals.HealthWound >= (float)vitalsRequirement.Value);
						break;
					case NumericComparator.LessThan:
						flag = (flag && sourceEntity.Vitals.HealthWound < (float)vitalsRequirement.Value);
						break;
					case NumericComparator.LessThanOrEqualTo:
						flag = (flag && sourceEntity.Vitals.HealthWound <= (float)vitalsRequirement.Value);
						break;
					}
					if (message == string.Empty && !flag)
					{
						message = "HealthWound requirements not met.";
					}
					break;
				case VitalType.Stamina:
					switch (vitalsRequirement.Comparator)
					{
					case NumericComparator.EqualTo:
						flag = (flag && sourceEntity.Vitals.Stamina == (float)vitalsRequirement.Value);
						break;
					case NumericComparator.GreaterThan:
						flag = (flag && sourceEntity.Vitals.Stamina > (float)vitalsRequirement.Value);
						break;
					case NumericComparator.GreaterThanOrEqualTo:
						flag = (flag && sourceEntity.Vitals.Stamina >= (float)vitalsRequirement.Value);
						break;
					case NumericComparator.LessThan:
						flag = (flag && sourceEntity.Vitals.Stamina < (float)vitalsRequirement.Value);
						break;
					case NumericComparator.LessThanOrEqualTo:
						flag = (flag && sourceEntity.Vitals.Stamina <= (float)vitalsRequirement.Value);
						break;
					}
					if (message == string.Empty && !flag)
					{
						message = "Stamina requirements not met.";
					}
					break;
				case VitalType.StaminaWound:
					switch (vitalsRequirement.Comparator)
					{
					case NumericComparator.EqualTo:
						flag = (flag && sourceEntity.Vitals.StaminaWound == (float)vitalsRequirement.Value);
						break;
					case NumericComparator.GreaterThan:
						flag = (flag && sourceEntity.Vitals.StaminaWound > (float)vitalsRequirement.Value);
						break;
					case NumericComparator.GreaterThanOrEqualTo:
						flag = (flag && sourceEntity.Vitals.StaminaWound >= (float)vitalsRequirement.Value);
						break;
					case NumericComparator.LessThan:
						flag = (flag && sourceEntity.Vitals.StaminaWound < (float)vitalsRequirement.Value);
						break;
					case NumericComparator.LessThanOrEqualTo:
						flag = (flag && sourceEntity.Vitals.StaminaWound <= (float)vitalsRequirement.Value);
						break;
					}
					if (message == string.Empty && !flag)
					{
						message = "StaminaWound requirements not met.";
					}
					break;
				}
			}
			return flag;
		}

		// Token: 0x04003889 RID: 14473
		[SerializeField]
		private VitalsRequirement[] m_requirements;

		// Token: 0x0400388A RID: 14474
		private static List<ValueTuple<UniqueId, RequiredVitalsObjective>> m_orders = new List<ValueTuple<UniqueId, RequiredVitalsObjective>>();
	}
}

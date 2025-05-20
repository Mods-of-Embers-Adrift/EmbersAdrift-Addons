using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200055B RID: 1371
	public class CampfireEffectApplicator : MonoBehaviour
	{
		// Token: 0x1700087E RID: 2174
		// (get) Token: 0x0600298B RID: 10635 RVA: 0x0005CB6F File Offset: 0x0005AD6F
		// (set) Token: 0x0600298C RID: 10636 RVA: 0x0005CB77 File Offset: 0x0005AD77
		internal InteractiveEmberRing Interactive { get; set; }

		// Token: 0x1700087F RID: 2175
		// (get) Token: 0x0600298D RID: 10637 RVA: 0x0005CB80 File Offset: 0x0005AD80
		internal CampfireEffectApplicator.CampfireType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x0600298E RID: 10638 RVA: 0x00141CEC File Offset: 0x0013FEEC
		private static void CacheSubscriberRegenBonuses()
		{
			if (!CampfireEffectApplicator.m_cachedSubscriberBonuses && GlobalSettings.Values && GlobalSettings.Values.Subscribers != null && GlobalSettings.Values.Subscribers.EmberRingEnhancer)
			{
				CampfireEffectApplicator.m_cachedSubscriberWoundRegenBonus = GlobalSettings.Values.Subscribers.EmberRingEnhancer.WoundRegenBooster;
				CampfireEffectApplicator.m_cachedSubscriberHealthRegenBonus = GlobalSettings.Values.Subscribers.EmberRingEnhancer.HealthRegenBooster;
				CampfireEffectApplicator.m_cachedSubscriberBonuses = true;
			}
		}

		// Token: 0x0600298F RID: 10639 RVA: 0x0005CB88 File Offset: 0x0005AD88
		private static float GetSubscriberWoundRegenBonus()
		{
			CampfireEffectApplicator.CacheSubscriberRegenBonuses();
			return CampfireEffectApplicator.m_cachedSubscriberWoundRegenBonus;
		}

		// Token: 0x06002990 RID: 10640 RVA: 0x0005CB94 File Offset: 0x0005AD94
		private static float GetSubscriberHealthRegenBonus()
		{
			CampfireEffectApplicator.CacheSubscriberRegenBonuses();
			return CampfireEffectApplicator.m_cachedSubscriberHealthRegenBonus;
		}

		// Token: 0x17000880 RID: 2176
		// (get) Token: 0x06002991 RID: 10641 RVA: 0x00141D68 File Offset: 0x0013FF68
		public float WoundRegenRatePerSecond
		{
			get
			{
				float num = this.m_woundRegenRatePerSecond;
				if (this.HasSubscribers)
				{
					num += CampfireEffectApplicator.GetSubscriberWoundRegenBonus();
				}
				return num;
			}
		}

		// Token: 0x17000881 RID: 2177
		// (get) Token: 0x06002992 RID: 10642 RVA: 0x00141D90 File Offset: 0x0013FF90
		public float HealthRegenRatePerSecond
		{
			get
			{
				float num = this.m_healthRegenRatePerSecond;
				if (this.HasSubscribers)
				{
					num += CampfireEffectApplicator.GetSubscriberHealthRegenBonus();
				}
				return num;
			}
		}

		// Token: 0x17000882 RID: 2178
		// (get) Token: 0x06002993 RID: 10643 RVA: 0x0005CBA0 File Offset: 0x0005ADA0
		private List<GameEntity> CurrentOccupants
		{
			get
			{
				if (this.m_currentOccupants == null)
				{
					this.m_currentOccupants = new List<GameEntity>(10);
				}
				return this.m_currentOccupants;
			}
		}

		// Token: 0x17000883 RID: 2179
		// (get) Token: 0x06002994 RID: 10644 RVA: 0x0005CBBD File Offset: 0x0005ADBD
		private List<GameEntity> CurrentSubscribers
		{
			get
			{
				if (this.m_currentSubscribers == null)
				{
					this.m_currentSubscribers = new List<GameEntity>(10);
				}
				return this.m_currentSubscribers;
			}
		}

		// Token: 0x17000884 RID: 2180
		// (get) Token: 0x06002995 RID: 10645 RVA: 0x0005CBDA File Offset: 0x0005ADDA
		private bool HasSubscribers
		{
			get
			{
				return this.m_currentSubscribers != null && this.m_currentSubscribers.Count > 0;
			}
		}

		// Token: 0x17000885 RID: 2181
		// (get) Token: 0x06002996 RID: 10646 RVA: 0x0005CBF4 File Offset: 0x0005ADF4
		public bool LightAtStart
		{
			get
			{
				return this.Lightable == null;
			}
		}

		// Token: 0x17000886 RID: 2182
		// (get) Token: 0x06002997 RID: 10647 RVA: 0x0005CC02 File Offset: 0x0005AE02
		// (set) Token: 0x06002998 RID: 10648 RVA: 0x0005CC0A File Offset: 0x0005AE0A
		public LightableCampfire Lightable { get; set; }

		// Token: 0x17000887 RID: 2183
		// (get) Token: 0x06002999 RID: 10649 RVA: 0x0005CC13 File Offset: 0x0005AE13
		// (set) Token: 0x0600299A RID: 10650 RVA: 0x00141DB8 File Offset: 0x0013FFB8
		public bool IsLit
		{
			get
			{
				return this.m_isLit;
			}
			private set
			{
				bool flag = this.m_isLit != value;
				this.m_isLit = value;
				this.UpdateRange();
				if (GameManager.IsServer)
				{
					this.m_collider.enabled = this.m_isLit;
					if (this.m_areaDamageZone)
					{
						this.m_areaDamageZone.gameObject.SetActive(this.m_isLit);
					}
					this.UpdateRegenRates();
					if (flag && !this.m_isLit)
					{
						this.RemoveFromAllEntities();
						return;
					}
				}
				else
				{
					bool instant = this.m_instant;
					this.ToggleVisuals(this.m_isLit);
				}
			}
		}

		// Token: 0x0600299B RID: 10651 RVA: 0x00141E48 File Offset: 0x00140048
		private void Start()
		{
			this.m_collider = this.m_applicatorCollider;
			if (this.m_collider == null)
			{
				Debug.LogError("No collider on CampfireEffectApplicator! " + base.gameObject.name);
				base.gameObject.SetActive(false);
				return;
			}
			if (!GameManager.IsServer)
			{
				this.m_collider.enabled = false;
				this.ToggleVisuals(this.LightAtStart);
				return;
			}
			if (this.m_profile == null)
			{
				Debug.LogError("No Campfire Profile on CampfireEffectApplicator! " + base.gameObject.name);
				base.gameObject.SetActive(false);
				return;
			}
			this.m_areaDamageZone = base.gameObject.GetComponentInChildren<AreaDamageZone>();
			this.ToggleVisuals(false);
			this.m_instant = true;
			this.IsLit = this.LightAtStart;
			this.m_instant = false;
			this.m_collider.isTrigger = true;
			this.m_collider.gameObject.layer = LayerMap.Detection.Layer;
		}

		// Token: 0x0600299C RID: 10652 RVA: 0x0005CC1B File Offset: 0x0005AE1B
		public float LightFire(int masteryLevel)
		{
			this.m_masteryLevel = masteryLevel;
			this.IsLit = true;
			return this.m_profile.GetDuration(masteryLevel);
		}

		// Token: 0x0600299D RID: 10653 RVA: 0x0005CC37 File Offset: 0x0005AE37
		public void ExtinguishFire()
		{
			this.m_masteryLevel = 1;
			this.IsLit = false;
		}

		// Token: 0x0600299E RID: 10654 RVA: 0x0005CC47 File Offset: 0x0005AE47
		public void RefreshInteractiveState()
		{
			this.UpdateRegenRates();
		}

		// Token: 0x0600299F RID: 10655 RVA: 0x00141F48 File Offset: 0x00140148
		private void UpdateRegenRates()
		{
			if (this.IsLit)
			{
				this.m_woundRegenRatePerSecond = this.m_profile.GetWoundRegenRage(this.m_masteryLevel);
				this.m_healthRegenRatePerSecond = this.m_profile.GetHealthRegenRate(this.m_masteryLevel);
				if (this.Interactive != null && this.Interactive.Data.Value.Item != null)
				{
					this.m_woundRegenRatePerSecond += this.Interactive.Data.Value.Item.WoundRegenBooster;
					this.m_healthRegenRatePerSecond += this.Interactive.Data.Value.Item.HealthRegenBooster;
					return;
				}
			}
			else
			{
				this.m_woundRegenRatePerSecond = 0f;
				this.m_healthRegenRatePerSecond = 0f;
			}
		}

		// Token: 0x060029A0 RID: 10656 RVA: 0x0005CC4F File Offset: 0x0005AE4F
		private void UpdateRange()
		{
			if (this.IsLit)
			{
				this.m_range = this.m_profile.GetRange(this.m_masteryLevel);
				this.m_applicatorCollider.radius = this.m_range;
			}
		}

		// Token: 0x060029A1 RID: 10657 RVA: 0x0014202C File Offset: 0x0014022C
		private void AddToEntity(GameEntity entity)
		{
			if (entity)
			{
				if (entity.Vitals)
				{
					entity.Vitals.AddCampfireEffect(this);
				}
				this.CurrentOccupants.Add(entity);
				if (entity.Subscriber)
				{
					this.CurrentSubscribers.Add(entity);
					if (this.Interactive)
					{
						this.Interactive.SubscriberPresent.Value = true;
					}
				}
			}
		}

		// Token: 0x060029A2 RID: 10658 RVA: 0x00142098 File Offset: 0x00140298
		private bool RemoveFromEntity(GameEntity entity)
		{
			if (entity)
			{
				if (entity.Vitals)
				{
					entity.Vitals.RemoveCampfireEffect(this);
				}
				if (entity.Subscriber)
				{
					this.CurrentSubscribers.Remove(entity);
					this.RefreshSubscriberPresent();
				}
				return this.CurrentOccupants.Remove(entity);
			}
			return false;
		}

		// Token: 0x060029A3 RID: 10659 RVA: 0x001420F0 File Offset: 0x001402F0
		private void RemoveFromAllEntities()
		{
			for (int i = 0; i < this.CurrentOccupants.Count; i++)
			{
				if (this.RemoveFromEntity(this.CurrentOccupants[i]))
				{
					i--;
				}
			}
		}

		// Token: 0x060029A4 RID: 10660 RVA: 0x0005CC81 File Offset: 0x0005AE81
		private void RefreshSubscriberPresent()
		{
			if (this.Interactive && GameManager.IsServer)
			{
				this.Interactive.SubscriberPresent.Value = (this.CurrentSubscribers.Count > 0);
			}
		}

		// Token: 0x060029A5 RID: 10661 RVA: 0x0014212C File Offset: 0x0014032C
		internal void CleanupLists()
		{
			for (int i = 0; i < this.CurrentOccupants.Count; i++)
			{
				if (this.CurrentOccupants[i] == null || this.CurrentOccupants[i].Vitals == null)
				{
					this.CurrentOccupants.RemoveAt(i);
					i--;
				}
			}
			for (int j = 0; j < this.CurrentSubscribers.Count; j++)
			{
				if (this.CurrentSubscribers[j] == null || this.CurrentSubscribers[j].Vitals == null)
				{
					this.CurrentSubscribers.RemoveAt(j);
					j--;
				}
			}
			this.RefreshSubscriberPresent();
		}

		// Token: 0x060029A6 RID: 10662 RVA: 0x0005CCB5 File Offset: 0x0005AEB5
		private void ToggleVisuals(bool isOn)
		{
			if (this.m_visuals)
			{
				this.m_visuals.Toggle(isOn);
			}
		}

		// Token: 0x04002A7F RID: 10879
		[SerializeField]
		private CampfireProfile m_profile;

		// Token: 0x04002A80 RID: 10880
		[SerializeField]
		private SphereCollider m_applicatorCollider;

		// Token: 0x04002A81 RID: 10881
		[SerializeField]
		private ToggleController m_visuals;

		// Token: 0x04002A82 RID: 10882
		[SerializeField]
		private InteractionSettings m_interactionDistance;

		// Token: 0x04002A83 RID: 10883
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04002A84 RID: 10884
		[SerializeField]
		private CampfireEffectApplicator.CampfireType m_type;

		// Token: 0x04002A86 RID: 10886
		private int m_masteryLevel;

		// Token: 0x04002A87 RID: 10887
		private float m_range;

		// Token: 0x04002A88 RID: 10888
		private float m_woundRegenRatePerSecond;

		// Token: 0x04002A89 RID: 10889
		private float m_healthRegenRatePerSecond;

		// Token: 0x04002A8A RID: 10890
		private static bool m_cachedSubscriberBonuses;

		// Token: 0x04002A8B RID: 10891
		private static float m_cachedSubscriberWoundRegenBonus;

		// Token: 0x04002A8C RID: 10892
		private static float m_cachedSubscriberHealthRegenBonus;

		// Token: 0x04002A8D RID: 10893
		private List<GameEntity> m_currentOccupants;

		// Token: 0x04002A8E RID: 10894
		private List<GameEntity> m_currentSubscribers;

		// Token: 0x04002A8F RID: 10895
		private Collider m_collider;

		// Token: 0x04002A90 RID: 10896
		private IEnumerator m_postFadeCo;

		// Token: 0x04002A91 RID: 10897
		private const float kWeightChangeRate = 2f;

		// Token: 0x04002A92 RID: 10898
		private bool m_instant;

		// Token: 0x04002A93 RID: 10899
		private AreaDamageZone m_areaDamageZone;

		// Token: 0x04002A95 RID: 10901
		private bool m_isLit;

		// Token: 0x0200055C RID: 1372
		internal enum CampfireType
		{
			// Token: 0x04002A97 RID: 10903
			EmberRing,
			// Token: 0x04002A98 RID: 10904
			Campfire
		}
	}
}

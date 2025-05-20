using System;
using System.Collections.Generic;
using SoL.Game.Grouping;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Objects;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Targeting
{
	// Token: 0x02000658 RID: 1624
	public class PlayerTargetController : BaseTargetController
	{
		// Token: 0x0600328E RID: 12942 RVA: 0x001609AC File Offset: 0x0015EBAC
		public override void Initialize()
		{
			this.m_offensive = new PlayerTarget(this, this.m_offensiveTargetNetworkId, TargetType.Offensive);
			this.m_defensive = new PlayerTarget(this, this.m_defensiveTargetNetworkId, TargetType.Defensive);
			this.m_offensiveTabTargetGroup = new PlayerTabTargetGroup(this, this.m_offensive);
			this.m_defensiveTabTargetGroup = new PlayerTabTargetGroup(this, this.m_defensive);
		}

		// Token: 0x0600328F RID: 12943 RVA: 0x00062D0A File Offset: 0x00060F0A
		public override void AssignNameplate(TargetType type, NameplateControllerUI nameplate)
		{
			if (type != TargetType.Offensive)
			{
				if (type == TargetType.Defensive)
				{
					this.m_defensive.NameplateUI = nameplate;
					return;
				}
			}
			else
			{
				this.m_offensive.NameplateUI = nameplate;
			}
		}

		// Token: 0x06003290 RID: 12944 RVA: 0x00160A04 File Offset: 0x0015EC04
		public override bool EscapePressed()
		{
			bool result = false;
			if (this.m_offensive.Target != null)
			{
				this.m_offensive.Target = null;
				result = true;
			}
			else if (this.m_defensive.Target != null)
			{
				this.m_defensive.Target = null;
				result = true;
			}
			return result;
		}

		// Token: 0x06003291 RID: 12945 RVA: 0x00160A4C File Offset: 0x0015EC4C
		private void Update()
		{
			this.ProcessTargetChangeKeyPresses();
			if (this.m_offensive == null || this.m_defensive == null)
			{
				return;
			}
			this.m_offensive.ValidateTarget();
			this.m_defensive.ValidateTarget();
			if (!UIManager.EventSystem.IsPointerOverGameObject() && !CursorManager.IsCursorHiddenForDoubleClickThreshold())
			{
				if (Input.GetMouseButtonDown(0))
				{
					this.m_downTarget = this.GetTargetableViaRay(LayerMap.Interaction.LayerMask);
					this.m_downTargetTime = ((this.m_downTarget == null) ? DateTime.MinValue : DateTime.UtcNow);
				}
				if (Input.GetMouseButtonUp(0))
				{
					if (this.m_downTarget != null && (DateTime.UtcNow - this.m_downTargetTime).TotalSeconds <= (double)GlobalSettings.Values.General.InteractiveClickDragTimeThreshold)
					{
						this.m_upTarget = this.GetTargetableViaRay(LayerMap.Interaction.LayerMask);
						if (this.m_downTarget == this.m_upTarget)
						{
							if (this.m_upTarget.IsPlayer)
							{
								TargetType? targetType = null;
								if (this.m_upTarget.Entity == LocalPlayer.GameEntity)
								{
									targetType = new TargetType?(TargetType.Defensive);
								}
								else
								{
									bool flag = false;
									bool flag2 = false;
									if (this.m_upTarget.Entity && this.m_upTarget.Entity.CharacterData)
									{
										flag = this.m_upTarget.Entity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.Pvp);
										flag2 = ClientGroupManager.IsMyGroup(this.m_upTarget.Entity.CharacterData.GroupId);
									}
									bool isPvp = LocalPlayer.IsPvp;
									if (isPvp && flag)
									{
										targetType = new TargetType?(flag2 ? TargetType.Defensive : TargetType.Offensive);
									}
									else if (!isPvp && !flag)
									{
										targetType = new TargetType?(TargetType.Defensive);
									}
								}
								if (targetType != null)
								{
									TargetType targetType2 = targetType.Value;
									if (targetType2 != TargetType.Offensive)
									{
										if (targetType2 == TargetType.Defensive)
										{
											this.m_defensive.Target = this.m_upTarget;
										}
									}
									else
									{
										this.m_offensive.Target = this.m_upTarget;
									}
								}
							}
							else
							{
								TargetType targetType2 = this.m_upTarget.Faction.GetPlayerTargetType();
								if (targetType2 != TargetType.Offensive)
								{
									if (targetType2 == TargetType.Defensive)
									{
										this.m_defensive.Target = this.m_upTarget;
									}
								}
								else
								{
									this.m_offensive.Target = this.m_upTarget;
								}
							}
						}
					}
					this.m_downTarget = null;
					this.m_upTarget = null;
				}
			}
		}

		// Token: 0x06003292 RID: 12946 RVA: 0x00062D2C File Offset: 0x00060F2C
		private void LateUpdate()
		{
			PlayerTarget offensive = this.m_offensive;
			if (offensive != null)
			{
				offensive.RefreshReticle();
			}
			PlayerTarget defensive = this.m_defensive;
			if (defensive == null)
			{
				return;
			}
			defensive.RefreshReticle();
		}

		// Token: 0x06003293 RID: 12947 RVA: 0x00160CB0 File Offset: 0x0015EEB0
		private void ProcessTargetChangeKeyPresses()
		{
			if (ClientGameManager.InputManager == null || ClientGameManager.InputManager.PreventInputForUI)
			{
				return;
			}
			if (SolInput.GetButtonDown(32))
			{
				this.m_defensive.Target = LocalPlayer.GameEntity.Targetable;
				return;
			}
			if (SolInput.GetButtonDown(19))
			{
				this.m_offensive.Target = this.m_offensiveTabTargetGroup.GetTabTarget(false);
				return;
			}
			if (SolInput.GetButtonDown(58))
			{
				this.m_offensive.Target = this.m_offensiveTabTargetGroup.GetTabTarget(true);
				return;
			}
			if (SolInput.GetButtonDown(65))
			{
				this.m_defensive.Target = this.m_defensiveTabTargetGroup.GetTabTarget(false);
				return;
			}
			if (SolInput.GetButtonDown(66))
			{
				this.m_defensive.Target = this.m_defensiveTabTargetGroup.GetTabTarget(true);
				return;
			}
			if (SolInput.GetButtonDown(69))
			{
				this.SetAssist(this.m_defensive);
				return;
			}
			if (SolInput.GetButtonDown(70))
			{
				this.SetAssist(this.m_offensive);
				return;
			}
			if (ClientGameManager.GroupManager.IsGrouped && UIManager.GroupWindowUI)
			{
				int i = 0;
				while (i < PlayerTargetController.kGroupActions.Length)
				{
					if (SolInput.GetButtonDown(PlayerTargetController.kGroupActions[i]))
					{
						GroupMember groupMemberByIndex = UIManager.GroupWindowUI.GetGroupMemberByIndex(i);
						if (groupMemberByIndex != null && groupMemberByIndex.Entity)
						{
							this.m_defensive.Target = groupMemberByIndex.Entity.Targetable;
							return;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
		}

		// Token: 0x06003294 RID: 12948 RVA: 0x00160E10 File Offset: 0x0015F010
		private bool SetAssist(PlayerTarget source)
		{
			if (((source != null) ? source.Target : null) != null && source.Target.Entity && source.Target.Entity.TargetController && source.Target.Entity.TargetController.OffensiveTarget)
			{
				GameEntity offensiveTarget = source.Target.Entity.TargetController.OffensiveTarget;
				((offensiveTarget.CharacterData.Faction.GetPlayerTargetType() == TargetType.Offensive) ? this.m_offensive : this.m_defensive).Target = offensiveTarget.Targetable;
				return true;
			}
			return false;
		}

		// Token: 0x06003295 RID: 12949 RVA: 0x00160EB8 File Offset: 0x0015F0B8
		private int SortRayHitsByDistance(RaycastHit a, RaycastHit b)
		{
			return a.distance.CompareTo(b.distance);
		}

		// Token: 0x06003296 RID: 12950 RVA: 0x00160EDC File Offset: 0x0015F0DC
		private ITargetable GetTargetableViaRay(int mask)
		{
			if (!ClientGameManager.MainCamera)
			{
				return null;
			}
			Ray ray = ClientGameManager.MainCamera.ScreenPointToRay(InteractionManager.GetMouseRaySource());
			mask |= GlobalSettings.Values.General.MouseClickTargetLayersToInclude;
			RaycastHit[] hits = Hits.Hits100;
			int num = Physics.RaycastNonAlloc(ray, hits, base.GameEntity.Vitals.MaxTargetDistance * 2f, mask);
			if (this.m_rayHitList == null)
			{
				this.m_rayHitList = new List<RaycastHit>(100);
				this.m_rayHitDistanceSorter = new Comparison<RaycastHit>(this.SortRayHitsByDistance);
			}
			this.m_rayHitList.Clear();
			float maxTargetDistance = base.GameEntity.Vitals.MaxTargetDistance;
			for (int i = 0; i < num; i++)
			{
				if (hits[i].distance <= maxTargetDistance)
				{
					this.m_rayHitList.Add(hits[i]);
				}
			}
			if (this.m_rayHitList.Count <= 0)
			{
				return null;
			}
			this.m_rayHitList.Sort(this.m_rayHitDistanceSorter);
			int num2 = 0;
			GameEntityComponent gameEntityComponent;
			while (num2 < this.m_rayHitList.Count && this.m_rayHitList[num2].collider.gameObject.TryGetComponent<GameEntityComponent>(out gameEntityComponent))
			{
				if (gameEntityComponent.GameEntity)
				{
					if (gameEntityComponent.GameEntity.Targetable != base.GameEntity.Targetable)
					{
						return gameEntityComponent.GameEntity.Targetable;
					}
					if (Options.GameOptions.AllowSelfTarget.Value)
					{
						return base.GameEntity.Targetable;
					}
				}
				num2++;
			}
			this.m_rayHitList.Clear();
			return null;
		}

		// Token: 0x06003297 RID: 12951 RVA: 0x00062D4F File Offset: 0x00060F4F
		public override void SetTarget(TargetType type, ITargetable targetable)
		{
			base.SetTarget(type, targetable);
			if (type != TargetType.Offensive)
			{
				if (type == TargetType.Defensive)
				{
					this.m_defensive.Target = targetable;
					return;
				}
			}
			else
			{
				this.m_offensive.Target = targetable;
			}
		}

		// Token: 0x06003298 RID: 12952 RVA: 0x00062D79 File Offset: 0x00060F79
		public override void ReplacementNetworkEntitySpawned(uint isReplacingId, NetworkEntity incomingReplacement)
		{
			base.ReplacementNetworkEntitySpawned(isReplacingId, incomingReplacement);
			PlayerTarget offensive = this.m_offensive;
			if (offensive == null)
			{
				return;
			}
			offensive.ReplacementNetworkEntitySpawned(isReplacingId, incomingReplacement);
		}

		// Token: 0x06003299 RID: 12953 RVA: 0x00062D95 File Offset: 0x00060F95
		public override void ConsiderTarget()
		{
			base.ConsiderTarget();
			if (this.m_offensive != null)
			{
				ITargetableExtensions.EchoChallengeTextToChat(this.m_offensive.Target);
			}
		}

		// Token: 0x040030F2 RID: 12530
		private static readonly int[] kGroupActions = new int[]
		{
			33,
			34,
			35,
			36,
			37
		};

		// Token: 0x040030F3 RID: 12531
		private PlayerTarget m_offensive;

		// Token: 0x040030F4 RID: 12532
		private PlayerTarget m_defensive;

		// Token: 0x040030F5 RID: 12533
		private PlayerTabTargetGroup m_offensiveTabTargetGroup;

		// Token: 0x040030F6 RID: 12534
		private PlayerTabTargetGroup m_defensiveTabTargetGroup;

		// Token: 0x040030F7 RID: 12535
		private DateTime m_downTargetTime = DateTime.MinValue;

		// Token: 0x040030F8 RID: 12536
		private ITargetable m_downTarget;

		// Token: 0x040030F9 RID: 12537
		private ITargetable m_upTarget;

		// Token: 0x040030FA RID: 12538
		private List<RaycastHit> m_rayHitList;

		// Token: 0x040030FB RID: 12539
		private Comparison<RaycastHit> m_rayHitDistanceSorter;
	}
}

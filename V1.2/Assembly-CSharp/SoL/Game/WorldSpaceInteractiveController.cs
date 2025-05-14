using System;
using Cysharp.Text;
using SoL.Game.Crafting;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.GameCamera;
using SoL.Managers;
using TMPro;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005A6 RID: 1446
	public class WorldSpaceInteractiveController : MonoBehaviour
	{
		// Token: 0x17000993 RID: 2451
		// (get) Token: 0x06002D57 RID: 11607 RVA: 0x0005F837 File Offset: 0x0005DA37
		public bool IsActive
		{
			get
			{
				return this.m_canvasGroup.alpha > 0f;
			}
		}

		// Token: 0x06002D58 RID: 11608 RVA: 0x0005F84B File Offset: 0x0005DA4B
		private void OnDestroy()
		{
			this.Unsubscribe();
		}

		// Token: 0x06002D59 RID: 11609 RVA: 0x0005F853 File Offset: 0x0005DA53
		private void Unsubscribe()
		{
			this.m_interactiveGatheringNode;
		}

		// Token: 0x06002D5A RID: 11610 RVA: 0x0014D9A8 File Offset: 0x0014BBA8
		public void Init(WorldSpaceOverheadController controller)
		{
			this.m_controller = controller;
			this.m_canvasGroup.alpha = 0f;
			if (this.m_controller.GameEntity)
			{
				if (this.m_controller.GameEntity.Type == GameEntityType.Interactive)
				{
					this.m_name.color = GlobalSettings.Values.Nameplates.InteractiveColor;
					if (this.m_controller.GameEntity.Interactive != null)
					{
						this.m_interactiveGatheringNode = (this.m_controller.GameEntity.Interactive as InteractiveGatheringNode);
						if (this.m_interactiveGatheringNode)
						{
							this.RefreshDisplayName();
							this.AbilitiesOnContentsChanged();
							return;
						}
					}
				}
				else
				{
					this.m_name.color = this.m_defaultNameColor;
				}
			}
		}

		// Token: 0x06002D5B RID: 11611 RVA: 0x0014DA68 File Offset: 0x0014BC68
		internal void AbilitiesOnContentsChanged()
		{
			ArchetypeInstance archetypeInstance;
			this.m_playerHasProfession = (this.m_interactiveGatheringNode && !this.m_interactiveGatheringNode.GatheringParams.AbilityId.IsEmpty && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Abilities != null && LocalPlayer.GameEntity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.m_interactiveGatheringNode.GatheringParams.AbilityId, out archetypeInstance));
		}

		// Token: 0x06002D5C RID: 11612 RVA: 0x0005F861 File Offset: 0x0005DA61
		public void ResetData()
		{
			this.Unsubscribe();
			this.m_controller = null;
			this.m_interactiveGatheringNode = null;
			this.m_maxAlpha = 1f;
			this.m_showNameplate = true;
			this.Unsubscribe();
		}

		// Token: 0x06002D5D RID: 11613 RVA: 0x0005F88F File Offset: 0x0005DA8F
		private void RefreshDisplayName()
		{
			if (this.m_interactiveGatheringNode)
			{
				this.m_name.ZStringSetText(this.m_interactiveGatheringNode.Description);
			}
		}

		// Token: 0x06002D5E RID: 11614 RVA: 0x0014DAF4 File Offset: 0x0014BCF4
		private float GetTargetAlphaForDistance()
		{
			if (this.m_controller && this.m_controller.OffScreen)
			{
				return 0f;
			}
			if (!this.m_controller || !this.m_controller.GameEntity)
			{
				return 0f;
			}
			float cachedSqrDistanceFromLocalPlayer = this.m_controller.GameEntity.GetCachedSqrDistanceFromLocalPlayer();
			if (cachedSqrDistanceFromLocalPlayer <= LocalPlayer.GameEntity.Vitals.MaxTargetDistanceSqr)
			{
				float distanceFraction = Mathf.Sqrt(cachedSqrDistanceFromLocalPlayer / LocalPlayer.GameEntity.Vitals.MaxTargetDistanceSqr);
				return GlobalSettings.Values.Nameplates.GetOverheadAlpha(this.m_maxAlpha, distanceFraction, this.m_controller.Mode);
			}
			return 0f;
		}

		// Token: 0x06002D5F RID: 11615 RVA: 0x0014DBA8 File Offset: 0x0014BDA8
		internal void UpdateScale()
		{
			if (!this.m_controller || !this.m_controller.GameEntity)
			{
				return;
			}
			float cachedSqrDistanceFromCamera = this.m_controller.GameEntity.GetCachedSqrDistanceFromCamera();
			float distanceFraction = (cachedSqrDistanceFromCamera <= LocalPlayer.GameEntity.Vitals.MaxTargetDistanceSqr) ? Mathf.Sqrt(cachedSqrDistanceFromCamera / LocalPlayer.GameEntity.Vitals.MaxTargetDistanceSqr) : 1f;
			float overheadScale = GlobalSettings.Values.Nameplates.GetOverheadScale(this.m_defaultScale, distanceFraction, this.m_controller.Mode);
			this.m_rectTransform.localScale = Vector3.one * overheadScale;
		}

		// Token: 0x06002D60 RID: 11616 RVA: 0x0014DC50 File Offset: 0x0014BE50
		public void LateUpdateExternal()
		{
			if (!LocalPlayer.GameEntity || !this.m_controller || !this.m_controller.GameEntity || !ClientGameManager.MainCamera)
			{
				return;
			}
			this.UpdateShowVariables();
			float num = 0f;
			if (!this.m_controller.OffScreen && this.m_showNameplate && this.m_playerHasProfession)
			{
				Vector3 start = this.m_controller ? this.m_controller.WorldPos : base.gameObject.transform.position;
				Transform transform = ClientGameManager.MainCamera.transform;
				Vector3 vector = transform.position;
				if (CameraManager.ActiveType == ActiveCameraTypes.FirstPerson && this.m_controller && this.m_controller.Mode == OverheadNameplateMode.UISpace)
				{
					Vector3 b = transform.forward * 0.25f;
					b.y = 0f;
					vector += b;
				}
				if (!Physics.Linecast(start, vector, this.m_losLayerMask, QueryTriggerInteraction.Ignore))
				{
					num = this.GetTargetAlphaForDistance();
					this.UpdateScale();
				}
			}
			if (this.m_canvasGroup.alpha != num)
			{
				this.m_canvasGroup.alpha = Mathf.MoveTowards(this.m_canvasGroup.alpha, num, Time.deltaTime * 2f);
			}
		}

		// Token: 0x06002D61 RID: 11617 RVA: 0x0014DDA4 File Offset: 0x0014BFA4
		private void UpdateShowVariables()
		{
			if (!Options.GameOptions.ShowOverheadNameplates.Value || !this.m_controller || !this.m_controller.GameEntity || !this.m_controller.GameEntity.NetworkEntity)
			{
				this.m_showNameplate = false;
				return;
			}
			this.m_showNameplate = Options.GameOptions.ShowOverheadNameplate_ResourceNode.Value;
		}

		// Token: 0x06002D62 RID: 11618 RVA: 0x0005F8B4 File Offset: 0x0005DAB4
		private void SyncInteractiveFlagsOnChanged(InteractiveFlags obj)
		{
			this.m_maxAlpha = ((!obj.HasBitFlag(InteractiveFlags.Interactive)) ? 0.5f : 1f);
		}

		// Token: 0x06002D63 RID: 11619 RVA: 0x0004475B File Offset: 0x0004295B
		private void LooterStringOnChanged(string obj)
		{
		}

		// Token: 0x04002CE6 RID: 11494
		[SerializeField]
		private TextMeshProUGUI m_name;

		// Token: 0x04002CE7 RID: 11495
		[SerializeField]
		private CanvasGroup m_canvasGroup;

		// Token: 0x04002CE8 RID: 11496
		[SerializeField]
		private RectTransform m_rectTransform;

		// Token: 0x04002CE9 RID: 11497
		[SerializeField]
		private LayerMask m_losLayerMask;

		// Token: 0x04002CEA RID: 11498
		[SerializeField]
		private float m_defaultScale = 1f;

		// Token: 0x04002CEB RID: 11499
		[SerializeField]
		private Color m_defaultNameColor = Color.white;

		// Token: 0x04002CEC RID: 11500
		private float m_maxAlpha = 1f;

		// Token: 0x04002CED RID: 11501
		private WorldSpaceOverheadController m_controller;

		// Token: 0x04002CEE RID: 11502
		private InteractiveGatheringNode m_interactiveGatheringNode;

		// Token: 0x04002CEF RID: 11503
		private bool m_showNameplate = true;

		// Token: 0x04002CF0 RID: 11504
		private bool m_playerHasProfession;
	}
}

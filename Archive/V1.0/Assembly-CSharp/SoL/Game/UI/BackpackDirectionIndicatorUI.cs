using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000855 RID: 2133
	public class BackpackDirectionIndicatorUI : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06003D9F RID: 15775 RVA: 0x00069BD3 File Offset: 0x00067DD3
		private void Awake()
		{
			this.m_sqrDistanceThreshold = GlobalSettings.Values.Player.BackpackIndicatorDistance * GlobalSettings.Values.Player.BackpackIndicatorDistance;
		}

		// Token: 0x06003DA0 RID: 15776 RVA: 0x001831A4 File Offset: 0x001813A4
		private void Update()
		{
			if (LocalPlayer.GameEntity)
			{
				if (LocalPlayer.Corpse)
				{
					if (!GlobalSettings.Values.Player.LimitBackpackIndicatorByDistance)
					{
						this.m_directionIndicatorUi.DisableDynamics = this.DisableDueToBackpackVolumes();
						if (this.m_toggle.State != ToggleController.ToggleState.ON || this.m_directionIndicatorUi.Target != LocalPlayer.Corpse.gameObject)
						{
							this.m_directionIndicatorUi.Target = LocalPlayer.Corpse.gameObject;
							this.m_toggle.State = ToggleController.ToggleState.ON;
						}
						return;
					}
					this.m_sqrDistance = new float?((LocalPlayer.Corpse.gameObject.transform.position - LocalPlayer.GameEntity.gameObject.transform.position).sqrMagnitude);
					if (this.m_sqrDistance.Value <= this.m_sqrDistanceThreshold)
					{
						this.m_directionIndicatorUi.DisableDynamics = this.DisableDueToBackpackVolumes();
						this.m_directionIndicatorUi.Target = LocalPlayer.Corpse.gameObject;
						this.m_toggle.State = ToggleController.ToggleState.ON;
						return;
					}
				}
				else if (LocalPlayer.GameEntity.CharacterData && LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.MissingBag))
				{
					this.m_sqrDistance = null;
					this.m_directionIndicatorUi.DisableDynamics = true;
					this.m_directionIndicatorUi.Target = null;
					this.m_toggle.State = ToggleController.ToggleState.ON;
					return;
				}
			}
			this.m_sqrDistance = null;
			this.m_directionIndicatorUi.DisableDynamics = false;
			this.m_directionIndicatorUi.Target = null;
			this.m_toggle.State = ToggleController.ToggleState.OFF;
		}

		// Token: 0x06003DA1 RID: 15777 RVA: 0x00183358 File Offset: 0x00181558
		private bool DisableDueToBackpackVolumes()
		{
			BackpackVolumeOverride x;
			BackpackVolumeOverride y;
			return LocalZoneManager.TryGetBackpackVolumeOverride(LocalPlayer.Corpse.gameObject.transform.position, out x) && (!LocalZoneManager.TryGetBackpackVolumeOverride(LocalPlayer.GameEntity.gameObject.transform.position, out y) || x != y);
		}

		// Token: 0x06003DA2 RID: 15778 RVA: 0x001833AC File Offset: 0x001815AC
		private ITooltipParameter GetTooltipParameter()
		{
			if (LocalPlayer.Corpse)
			{
				int arg = Mathf.FloorToInt(Vector3.Distance(LocalPlayer.Corpse.gameObject.transform.position, LocalPlayer.GameEntity.transform.position));
				string txt = ZString.Format<int>("Your bag is {0}m away!", arg);
				return new ObjectTextTooltipParameter(this, txt, false);
			}
			if (LocalZoneManager.ZoneRecord == null || !LocalPlayer.GameEntity || !(LocalPlayer.GameEntity.CharacterData != null) || !LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.MissingBag) || LocalPlayer.GameEntity.CollectionController == null || LocalPlayer.GameEntity.CollectionController.Record == null || LocalPlayer.GameEntity.CollectionController.Record.CorpseZoneId == 0 || LocalPlayer.GameEntity.CollectionController.Record.CorpseZoneId == LocalZoneManager.ZoneRecord.ZoneId)
			{
				return null;
			}
			ZoneRecord zoneRecord = SessionData.GetZoneRecord((ZoneId)LocalPlayer.GameEntity.CollectionController.Record.CorpseZoneId);
			if (zoneRecord == null)
			{
				return null;
			}
			string txt2 = ZString.Format<string>("Your bag is in {0}!", zoneRecord.DisplayName);
			return new ObjectTextTooltipParameter(this, txt2, false);
		}

		// Token: 0x17000E40 RID: 3648
		// (get) Token: 0x06003DA3 RID: 15779 RVA: 0x00069BFA File Offset: 0x00067DFA
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000E41 RID: 3649
		// (get) Token: 0x06003DA4 RID: 15780 RVA: 0x00069C08 File Offset: 0x00067E08
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000E42 RID: 3650
		// (get) Token: 0x06003DA5 RID: 15781 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003DA7 RID: 15783 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003C36 RID: 15414
		private float m_sqrDistanceThreshold = float.MaxValue;

		// Token: 0x04003C37 RID: 15415
		private float? m_sqrDistance;

		// Token: 0x04003C38 RID: 15416
		[SerializeField]
		private DirectionIndicatorUI m_directionIndicatorUi;

		// Token: 0x04003C39 RID: 15417
		[SerializeField]
		private ToggleController m_toggle;

		// Token: 0x04003C3A RID: 15418
		[SerializeField]
		private TooltipSettings m_tooltipSettings;
	}
}

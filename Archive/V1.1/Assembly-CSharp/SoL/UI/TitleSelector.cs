using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.UI
{
	// Token: 0x02000376 RID: 886
	public class TitleSelector : MonoBehaviour
	{
		// Token: 0x0600184E RID: 6222 RVA: 0x00103E94 File Offset: 0x00102094
		private void Start()
		{
			TitleManager.TitlesChanged += this.TitleManagerOnTitlesChanged;
			LocalPlayer.HighestMasteryLevelChanged += this.TitleManagerOnTitlesChanged;
			LocalPlayer.SpecializationMaxLevelChanged += this.TitleManagerOnTitlesChanged;
			if (LocalPlayer.IsInitialized)
			{
				this.Init();
			}
			else
			{
				LocalPlayer.LocalPlayerInitialized += this.OnLocalPlayerInitialized;
			}
			this.m_dropdown.onValueChanged.AddListener(new UnityAction<int>(this.DropdownChanged));
		}

		// Token: 0x0600184F RID: 6223 RVA: 0x00103F10 File Offset: 0x00102110
		private void OnDestroy()
		{
			TitleManager.TitlesChanged -= this.TitleManagerOnTitlesChanged;
			LocalPlayer.HighestMasteryLevelChanged -= this.TitleManagerOnTitlesChanged;
			LocalPlayer.SpecializationMaxLevelChanged -= this.TitleManagerOnTitlesChanged;
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData)
			{
				LocalPlayer.GameEntity.CharacterData.RoleChanged -= this.TitleManagerOnTitlesChanged;
			}
			this.m_dropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.DropdownChanged));
		}

		// Token: 0x06001850 RID: 6224 RVA: 0x0005311B File Offset: 0x0005131B
		private void OnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.OnLocalPlayerInitialized;
			this.Init();
		}

		// Token: 0x06001851 RID: 6225 RVA: 0x00053134 File Offset: 0x00051334
		private void Init()
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData)
			{
				LocalPlayer.GameEntity.CharacterData.RoleChanged += this.TitleManagerOnTitlesChanged;
			}
			this.RebuildTitleList();
		}

		// Token: 0x06001852 RID: 6226 RVA: 0x00053174 File Offset: 0x00051374
		private void TitleManagerOnTitlesChanged()
		{
			this.RebuildTitleList();
		}

		// Token: 0x06001853 RID: 6227 RVA: 0x00103FA4 File Offset: 0x001021A4
		private void RebuildTitleList()
		{
			List<string> availableTitles = TitleManager.GetAvailableTitles(LocalPlayer.GameEntity, SessionData.User.Flags, SessionData.User.Rewards);
			availableTitles.Insert(0, "None");
			this.m_dropdown.ClearOptions();
			this.m_dropdown.AddOptions(availableTitles);
			string value = LocalPlayer.GameEntity.CharacterData.Title.Value;
			if (string.IsNullOrEmpty(value))
			{
				this.m_dropdown.value = 0;
				return;
			}
			bool flag = false;
			for (int i = 0; i < this.m_dropdown.options.Count; i++)
			{
				if (this.m_dropdown.options[i].text == value)
				{
					this.m_dropdown.value = i;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.m_dropdown.value = 0;
				this.DropdownChanged(0);
			}
		}

		// Token: 0x06001854 RID: 6228 RVA: 0x00104080 File Offset: 0x00102280
		private void DropdownChanged(int index)
		{
			string text = "";
			if (index != 0)
			{
				text = this.m_dropdown.options[index].text;
			}
			if (text != LocalPlayer.GameEntity.CharacterData.Title.Value)
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.RequestTitleChange(text);
			}
		}

		// Token: 0x04001F96 RID: 8086
		[SerializeField]
		private TMP_Dropdown m_dropdown;
	}
}

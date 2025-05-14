using System;
using System.Linq;
using System.Text;
using SoL.Game;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200000B RID: 11
public class ServerLoginUI : MonoBehaviour
{
	// Token: 0x06000022 RID: 34 RVA: 0x00087A94 File Offset: 0x00085C94
	private void Start()
	{
		this.m_zoneIds = (ZoneId[])Enum.GetValues(typeof(ZoneId));
		this.m_description.text = "Select Zone";
		this.m_host.enabled = false;
		this.m_host.onClick.AddListener(new UnityAction(this.OnHostClicked));
		this.m_dropdown.ClearOptions();
		this.m_dropdown.AddOptions(Enum.GetNames(typeof(ZoneId)).ToList<string>());
		this.m_dropdown.onValueChanged.AddListener(new UnityAction<int>(this.DropdownChanged));
	}

	// Token: 0x06000023 RID: 35 RVA: 0x000447FC File Offset: 0x000429FC
	private void OnDestroy()
	{
		this.m_dropdown.onValueChanged.RemoveAllListeners();
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00087B3C File Offset: 0x00085D3C
	private void DropdownChanged(int index)
	{
		ZoneId zoneId = this.m_zoneIds[index];
		if (zoneId == ZoneId.None)
		{
			this.m_description.text = "Select Zone";
			this.m_host.enabled = false;
			this.m_selectedZoneRecord = null;
			return;
		}
		ZoneRecord zoneRecord = ZoneRecord.LoadZoneId(ExternalGameDatabase.Database, zoneId);
		if (zoneRecord == null)
		{
			this.m_description.text = string.Format("Unable to locate ZoneRecord for ZoneId: {0}", zoneId);
			this.m_host.enabled = false;
			this.m_selectedZoneRecord = null;
			return;
		}
		this.m_selectedZoneRecord = zoneRecord;
		StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
		fromPool.AppendLine(string.Format("Ready to host ZoneId: {0}", zoneId));
		fromPool.AppendLine(string.Format("@ {0}:{1}", zoneRecord.Address, zoneRecord.Port));
		fromPool.AppendLine("");
		fromPool.AppendLine("SceneName: <b>" + zoneRecord.SceneName + "</b>");
		fromPool.AppendLine("DisplayName: <b>" + zoneRecord.DisplayName + "</b>");
		this.m_description.text = fromPool.ToString_ReturnToPool();
		this.m_host.enabled = true;
	}

	// Token: 0x06000025 RID: 37 RVA: 0x0004480E File Offset: 0x00042A0E
	private void OnHostClicked()
	{
		this.m_dropdown.interactable = false;
		this.m_host.interactable = false;
		GameManager.SceneCompositionManager.LoadZoneId((ZoneId)this.m_selectedZoneRecord.ZoneId);
	}

	// Token: 0x04000014 RID: 20
	[SerializeField]
	private TMP_Dropdown m_dropdown;

	// Token: 0x04000015 RID: 21
	[SerializeField]
	private TextMeshProUGUI m_description;

	// Token: 0x04000016 RID: 22
	[SerializeField]
	private Button m_host;

	// Token: 0x04000017 RID: 23
	private ZoneId[] m_zoneIds;

	// Token: 0x04000018 RID: 24
	private ZoneRecord m_selectedZoneRecord;
}

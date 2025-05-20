using System;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x020008A2 RID: 2210
	public class KeybindsUI : MonoBehaviour
	{
		// Token: 0x06004068 RID: 16488 RVA: 0x0006B988 File Offset: 0x00069B88
		public bool IsBinding()
		{
			return this.m_bindingPrompt.activeInHierarchy;
		}

		// Token: 0x06004069 RID: 16489 RVA: 0x0006B995 File Offset: 0x00069B95
		public void StopBinding()
		{
			if (this.m_bindingPrompt != null)
			{
				this.m_bindingPrompt.SetActive(false);
			}
			SolInput.Mapper.StopMapping();
		}

		// Token: 0x0600406A RID: 16490 RVA: 0x0018C418 File Offset: 0x0018A618
		private void Awake()
		{
			this.m_resetAllButton.onClick.AddListener(new UnityAction(this.ResetAllClicked));
			SolInput.Mapper.ConflictFoundEvent += this.OnConflict;
			SolInput.Mapper.InputMappedEvent += this.OnMapped;
		}

		// Token: 0x0600406B RID: 16491 RVA: 0x0006B9BB File Offset: 0x00069BBB
		private void Start()
		{
			this.PopulateKeybindList(SolInput.Mapper.Bindings);
		}

		// Token: 0x0600406C RID: 16492 RVA: 0x0006B9CD File Offset: 0x00069BCD
		private void OnDestroy()
		{
			this.m_resetAllButton.onClick.RemoveListener(new UnityAction(this.ResetAllClicked));
		}

		// Token: 0x0600406D RID: 16493 RVA: 0x0018C470 File Offset: 0x0018A670
		private void PopulateKeybindList(IDictionary<int, Binding> actions)
		{
			RectTransform rectTransform = (RectTransform)this.m_scrollViewContent.transform;
			RectTransform rectTransform2 = (RectTransform)this.m_keybindPrefab.transform;
			rectTransform.sizeDelta = new Vector2(0f, rectTransform2.rect.height * (float)actions.Count);
			for (int i = 0; i < actions.Count; i++)
			{
				int num = actions.Keys.ElementAt(i);
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_keybindPrefab, this.m_scrollViewContent.transform);
				gameObject.transform.localPosition = new Vector3(0f, (float)i * rectTransform2.rect.height * -1f, 0f);
				KeybindItemUI component = gameObject.GetComponent<KeybindItemUI>();
				component.ActionId = num;
				component.BindActivated += this.InitiateBind;
				component.ResetToDefaults += this.ResetToDefaults;
				component.UnbindAction += this.UnbindAction;
				this.m_bindingItems.Add(num, component);
				component.Label = ReInput.mapping.GetAction(num).descriptiveName;
				ActionElementMap[] mapsForBinding = SolInput.Mapper.GetMapsForBinding(num);
				if (mapsForBinding.Length > 2)
				{
					Debug.LogError("Too many mappings found for action, will not be able to rebind all of them with the UI.");
				}
				component.SetElementMaps(mapsForBinding);
			}
			rectTransform.sizeDelta = new Vector2(0f, rectTransform.rect.height + 10f);
		}

		// Token: 0x0600406E RID: 16494 RVA: 0x0018C5F0 File Offset: 0x0018A7F0
		private void InitiateBind(int actionId, int bindIndex)
		{
			if (this.m_bindingPrompt != null)
			{
				SolInput.Mapper.StartMapping(actionId, bindIndex);
				this.m_bindingPrompt.SetActive(true);
				this.m_bindingPromptText.text = string.Format("<color=#CAC5C4>Press key to bind to:\n<color=\"white\">{0}\n<color=#CAC5C4>(Press Escape to cancel)", ReInput.mapping.GetAction(actionId).descriptiveName);
			}
		}

		// Token: 0x0600406F RID: 16495 RVA: 0x0018C648 File Offset: 0x0018A848
		private void ResetAllClicked()
		{
			SolInput.Mapper.ResetAll();
			foreach (KeyValuePair<int, Binding> keyValuePair in SolInput.Mapper.Bindings)
			{
				this.m_bindingItems[keyValuePair.Key].SetElementMaps(SolInput.Mapper.GetMapsForBinding(keyValuePair.Key));
			}
		}

		// Token: 0x06004070 RID: 16496 RVA: 0x0018C6C4 File Offset: 0x0018A8C4
		private void ResetToDefaults(int actionId)
		{
			SolInput.Mapper.ResetToDefaults(actionId);
			ActionElementMap[] mapsForBinding = SolInput.Mapper.GetMapsForBinding(actionId);
			this.m_bindingItems[actionId].SetElementMaps(mapsForBinding);
		}

		// Token: 0x06004071 RID: 16497 RVA: 0x0006B9EB File Offset: 0x00069BEB
		private void UnbindAction(int actionId)
		{
			SolInput.Mapper.UnbindAction(actionId);
			this.m_bindingItems[actionId].Unbind();
		}

		// Token: 0x06004072 RID: 16498 RVA: 0x0018C6FC File Offset: 0x0018A8FC
		private void ClearConflictingAssignments(IEnumerable<BindingEventData> conflicts)
		{
			foreach (BindingEventData bindingEventData in conflicts)
			{
				if (this.m_bindingItems.ContainsKey(bindingEventData.ActionId))
				{
					this.m_bindingItems[bindingEventData.ActionId].Unbind(bindingEventData.BindingIndex);
				}
			}
		}

		// Token: 0x06004073 RID: 16499 RVA: 0x0006BA09 File Offset: 0x00069C09
		private void OnConflict(Mapper.ConflictFoundEventData eventData)
		{
			if (eventData.IsProtected)
			{
				if (eventData.Assignment.KeyCode != KeyCode.Escape)
				{
					this.m_bindingPrompt.SetActive(false);
				}
				return;
			}
			this.ClearConflictingAssignments(eventData.Conflicts);
		}

		// Token: 0x06004074 RID: 16500 RVA: 0x0018C76C File Offset: 0x0018A96C
		private void OnMapped(Mapper.InputMappedEventData eventData)
		{
			this.m_bindingItems[eventData.Assignment.ActionId].Bind(eventData.Assignment.BindingIndex, eventData.Assignment.RuntimeMap.elementIdentifierName);
			if (eventData.Assignment.KeyCode != KeyCode.Escape)
			{
				this.m_bindingPrompt.SetActive(false);
			}
		}

		// Token: 0x04003E31 RID: 15921
		[SerializeField]
		private GameObject m_keybindPrefab;

		// Token: 0x04003E32 RID: 15922
		[SerializeField]
		private GameObject m_scrollViewContent;

		// Token: 0x04003E33 RID: 15923
		[SerializeField]
		private GameObject m_bindingPrompt;

		// Token: 0x04003E34 RID: 15924
		[SerializeField]
		private TextMeshProUGUI m_bindingPromptText;

		// Token: 0x04003E35 RID: 15925
		[SerializeField]
		private SolButton m_resetAllButton;

		// Token: 0x04003E36 RID: 15926
		private IDictionary<int, KeybindItemUI> m_bindingItems = new DictionaryList<int, KeybindItemUI>(false);

		// Token: 0x04003E37 RID: 15927
		private const string kBindingPromptTextString = "<color=#CAC5C4>Press key to bind to:\n<color=\"white\">{0}\n<color=#CAC5C4>(Press Escape to cancel)";
	}
}

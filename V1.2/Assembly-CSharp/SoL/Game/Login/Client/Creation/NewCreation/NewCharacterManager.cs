using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.UI;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UMA.CharacterSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.Login.Client.Creation.NewCreation
{
	// Token: 0x02000B6E RID: 2926
	public class NewCharacterManager : MonoBehaviour
	{
		// Token: 0x1400011D RID: 285
		// (add) Token: 0x060059E2 RID: 23010 RVA: 0x001EADE0 File Offset: 0x001E8FE0
		// (remove) Token: 0x060059E3 RID: 23011 RVA: 0x001EAE18 File Offset: 0x001E9018
		public event Action RoleChanged;

		// Token: 0x1400011E RID: 286
		// (add) Token: 0x060059E4 RID: 23012 RVA: 0x001EAE50 File Offset: 0x001E9050
		// (remove) Token: 0x060059E5 RID: 23013 RVA: 0x001EAE88 File Offset: 0x001E9088
		public event Action StageEnterInitializeEvent;

		// Token: 0x170014F2 RID: 5362
		// (get) Token: 0x060059E6 RID: 23014 RVA: 0x0007C48A File Offset: 0x0007A68A
		// (set) Token: 0x060059E7 RID: 23015 RVA: 0x0007C492 File Offset: 0x0007A692
		private CharacterSex Sex
		{
			get
			{
				return this.m_sex;
			}
			set
			{
				if (this.m_sex == value)
				{
					return;
				}
				this.m_sex = value;
				this.RefreshSexOptions();
				this.ChangeCharacter();
			}
		}

		// Token: 0x060059E8 RID: 23016 RVA: 0x001EAEC0 File Offset: 0x001E90C0
		private void RefreshSexOptions()
		{
			CharacterSex sex = this.m_sex;
			if (sex != CharacterSex.Male)
			{
				if (sex == CharacterSex.Female)
				{
					this.m_maleButton.interactable = true;
					this.m_femaleButton.interactable = false;
					this.m_maleHighlight.enabled = false;
					this.m_femaleHighlight.enabled = true;
					this.m_maleFeaturePanel.SetActive(false);
					this.m_femaleFeaturePanel.SetActive(true);
				}
			}
			else
			{
				this.m_maleButton.interactable = false;
				this.m_femaleButton.interactable = true;
				this.m_maleHighlight.enabled = true;
				this.m_femaleHighlight.enabled = false;
				this.m_maleFeaturePanel.SetActive(true);
				this.m_femaleFeaturePanel.SetActive(false);
			}
			for (int i = 0; i < this.m_bodyTypeButtons.Length; i++)
			{
				this.m_bodyTypeButtons[i].RefreshImage();
			}
		}

		// Token: 0x170014F3 RID: 5363
		// (get) Token: 0x060059E9 RID: 23017 RVA: 0x0007C4B1 File Offset: 0x0007A6B1
		// (set) Token: 0x060059EA RID: 23018 RVA: 0x0007C4B9 File Offset: 0x0007A6B9
		private CharacterBuildType BuildType
		{
			get
			{
				return this.m_buildType;
			}
			set
			{
				if (this.m_buildType == value)
				{
					return;
				}
				this.m_buildType = value;
				this.RefreshBuildTypeOptions();
				this.ChangeCharacter();
			}
		}

		// Token: 0x060059EB RID: 23019 RVA: 0x001EAF94 File Offset: 0x001E9194
		private void RefreshBuildTypeOptions()
		{
			for (int i = 0; i < this.m_bodyTypeButtons.Length; i++)
			{
				this.m_bodyTypeButtons[i].Button.interactable = (this.m_bodyTypeButtons[i].BuildType != this.m_buildType);
			}
		}

		// Token: 0x170014F4 RID: 5364
		// (get) Token: 0x060059EC RID: 23020 RVA: 0x0007C4D8 File Offset: 0x0007A6D8
		// (set) Token: 0x060059ED RID: 23021 RVA: 0x0007C4E0 File Offset: 0x0007A6E0
		private NewCharacterManager.NewCharacterMode CurrentMode
		{
			get
			{
				return this.m_currentMode;
			}
			set
			{
				this.m_currentMode = value;
				this.RefreshUiForMode();
			}
		}

		// Token: 0x170014F5 RID: 5365
		// (get) Token: 0x060059EE RID: 23022 RVA: 0x0007C4EF File Offset: 0x0007A6EF
		public float SizeSliderValue
		{
			get
			{
				return this.m_sizeSlider.value;
			}
		}

		// Token: 0x170014F6 RID: 5366
		// (get) Token: 0x060059EF RID: 23023 RVA: 0x0007C4FC File Offset: 0x0007A6FC
		public NewCharacter CurrentCharacter
		{
			get
			{
				return this.m_currentCharacter;
			}
		}

		// Token: 0x170014F7 RID: 5367
		// (get) Token: 0x060059F0 RID: 23024 RVA: 0x0007C504 File Offset: 0x0007A704
		public TMP_InputField CharacterName
		{
			get
			{
				return this.m_characterName;
			}
		}

		// Token: 0x170014F8 RID: 5368
		// (get) Token: 0x060059F1 RID: 23025 RVA: 0x0007C50C File Offset: 0x0007A70C
		public SolButton AcceptButton
		{
			get
			{
				return this.m_acceptButton;
			}
		}

		// Token: 0x170014F9 RID: 5369
		// (get) Token: 0x060059F2 RID: 23026 RVA: 0x0007C514 File Offset: 0x0007A714
		public TextMeshProUGUI AcceptDescription
		{
			get
			{
				return this.m_acceptDescription;
			}
		}

		// Token: 0x060059F3 RID: 23027 RVA: 0x001EAFE0 File Offset: 0x001E91E0
		private void Awake()
		{
			this.CurrentMode = (this.m_isCharacterCreation ? NewCharacterManager.NewCharacterMode.Create : NewCharacterManager.NewCharacterMode.Edit);
			this.SetupNewCharacters();
			for (int i = 0; i < this.m_bodyTypeButtons.Length; i++)
			{
				this.m_bodyTypeButtons[i].Init(this);
			}
			this.m_colorPaletteDict = new Dictionary<CharacterColorType, ColorPalette>(default(CharacterColorTypeComparer));
			for (int j = 0; j < this.m_colorPalettes.Length; j++)
			{
				this.m_colorPalettes[j].Init(this);
				this.m_colorPaletteDict.Add(this.m_colorPalettes[j].CharacterColorType, this.m_colorPalettes[j]);
			}
			for (int k = 0; k < this.m_features.Length; k++)
			{
				this.m_features[k].Init(this);
			}
			for (int l = 0; l < this.m_roleSelectionButtons.Length; l++)
			{
				this.m_roleSelectionButtons[l].Init(this);
			}
			this.m_specPanel.SetActive(false);
			if (this.m_clothedToggle != null)
			{
				this.m_clothedToggle.isOn = false;
				this.m_clothedToggle.onValueChanged.AddListener(new UnityAction<bool>(this.ClothedToggleChanged));
			}
			if (this.m_armorToggle != null)
			{
				this.m_armorToggle.isOn = false;
				this.m_armorToggle.onValueChanged.AddListener(new UnityAction<bool>(this.ArmorToggleChanged));
			}
			if (this.m_faceCamToggle != null)
			{
				this.m_faceCamObj.SetActive(false);
				this.m_faceCamToggle.isOn = false;
				this.m_faceCamToggle.onValueChanged.AddListener(new UnityAction<bool>(this.FaceCamToggleChanged));
			}
			this.m_maleButton.onClick.AddListener(new UnityAction(this.MaleButtonClicked));
			this.m_femaleButton.onClick.AddListener(new UnityAction(this.FemaleButtonClicked));
			this.SizeSliderResetClicked();
			this.ToneSliderResetClicked();
			this.m_sizeSlider.onValueChanged.AddListener(new UnityAction<float>(this.SizeSliderChanged));
			this.m_toneSlider.onValueChanged.AddListener(new UnityAction<float>(this.ToneSliderChanged));
			this.m_sizeSliderReset.onClick.AddListener(new UnityAction(this.SizeSliderResetClicked));
			this.m_toneSliderReset.onClick.AddListener(new UnityAction(this.ToneSliderResetClicked));
			this.m_featureRandomize.onClick.AddListener(new UnityAction(this.FeatureRandomizeClicked));
			this.m_featureReset.onClick.AddListener(new UnityAction(this.FeatureResetClicked));
			this.m_colorRandomize.onClick.AddListener(new UnityAction(this.ColorRandomizeClicked));
			this.m_colorReset.onClick.AddListener(new UnityAction(this.ColorResetClicked));
			this.m_editButton.onClick.AddListener(new UnityAction(this.EditButtonClicked));
			TMP_InputField characterName = this.m_characterName;
			characterName.onValidateInput = (TMP_InputField.OnValidateInput)Delegate.Combine(characterName.onValidateInput, new TMP_InputField.OnValidateInput(NewCharacterManager.OnValidateNameInput));
		}

		// Token: 0x060059F4 RID: 23028 RVA: 0x0007C51C File Offset: 0x0007A71C
		private void Start()
		{
			this.Sex = CharacterSex.Male;
			this.BodyTypeButtonClicked(this.m_bodyTypeButtons[1]);
		}

		// Token: 0x060059F5 RID: 23029 RVA: 0x001EB2E8 File Offset: 0x001E94E8
		private void OnDestroy()
		{
			for (int i = 0; i < this.m_bodyTypeButtons.Length; i++)
			{
				this.m_bodyTypeButtons[i].OnDestroy();
			}
			if (this.m_clothedToggle != null)
			{
				this.m_clothedToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.ClothedToggleChanged));
			}
			if (this.m_armorToggle != null)
			{
				this.m_armorToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.ArmorToggleChanged));
			}
			if (this.m_faceCamToggle != null)
			{
				this.m_faceCamToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.FaceCamToggleChanged));
			}
			this.m_maleButton.onClick.RemoveListener(new UnityAction(this.MaleButtonClicked));
			this.m_femaleButton.onClick.RemoveListener(new UnityAction(this.FemaleButtonClicked));
			this.m_sizeSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.SizeSliderChanged));
			this.m_toneSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.ToneSliderChanged));
			this.m_sizeSliderReset.onClick.RemoveListener(new UnityAction(this.SizeSliderResetClicked));
			this.m_toneSliderReset.onClick.RemoveListener(new UnityAction(this.ToneSliderResetClicked));
			this.m_featureRandomize.onClick.RemoveListener(new UnityAction(this.FeatureRandomizeClicked));
			this.m_featureReset.onClick.RemoveListener(new UnityAction(this.FeatureResetClicked));
			this.m_colorRandomize.onClick.RemoveListener(new UnityAction(this.ColorRandomizeClicked));
			this.m_colorReset.onClick.RemoveListener(new UnityAction(this.ColorResetClicked));
			this.m_editButton.onClick.RemoveListener(new UnityAction(this.EditButtonClicked));
			TMP_InputField characterName = this.m_characterName;
			characterName.onValidateInput = (TMP_InputField.OnValidateInput)Delegate.Remove(characterName.onValidateInput, new TMP_InputField.OnValidateInput(NewCharacterManager.OnValidateNameInput));
		}

		// Token: 0x060059F6 RID: 23030 RVA: 0x0007C533 File Offset: 0x0007A733
		public static char OnValidateNameInput(string text, int pos, char ch)
		{
			if (pos >= 15 || !char.IsLetter(ch) || !ch.IsAscii())
			{
				return '\0';
			}
			if (pos != 0)
			{
				return char.ToLowerInvariant(ch);
			}
			return char.ToUpperInvariant(ch);
		}

		// Token: 0x060059F7 RID: 23031 RVA: 0x0007C55C File Offset: 0x0007A75C
		private void ToneSliderResetClicked()
		{
			this.m_toneSlider.value = 0.5f;
		}

		// Token: 0x060059F8 RID: 23032 RVA: 0x0007C56E File Offset: 0x0007A76E
		private void SizeSliderResetClicked()
		{
			this.m_sizeSlider.value = 0.5f;
		}

		// Token: 0x060059F9 RID: 23033 RVA: 0x001EB4F0 File Offset: 0x001E96F0
		private void ColorResetClicked()
		{
			for (int i = 0; i < this.m_colorPalettes.Length; i++)
			{
				this.m_colorPalettes[i].ResetColor();
			}
			this.RefreshCharacterColors(this.m_currentCharacter);
			this.RefreshDCA(this.m_currentCharacter);
		}

		// Token: 0x060059FA RID: 23034 RVA: 0x001EB538 File Offset: 0x001E9738
		private void FeatureResetClicked()
		{
			bool flag = false;
			for (int i = 0; i < this.m_features.Length; i++)
			{
				if (this.m_features[i].Sex == this.Sex)
				{
					this.m_features[i].ResetFeature();
					flag = true;
				}
			}
			if (flag)
			{
				this.RefreshFeatures(this.m_currentCharacter);
				this.RefreshDCA(this.m_currentCharacter);
			}
		}

		// Token: 0x060059FB RID: 23035 RVA: 0x001EB59C File Offset: 0x001E979C
		private void ColorRandomizeClicked()
		{
			for (int i = 0; i < this.m_colorPalettes.Length; i++)
			{
				this.m_colorPalettes[i].RandomizeColor();
			}
			this.RefreshCharacterColors(this.m_currentCharacter);
			this.RefreshDCA(this.m_currentCharacter);
		}

		// Token: 0x060059FC RID: 23036 RVA: 0x001EB5E4 File Offset: 0x001E97E4
		private void FeatureRandomizeClicked()
		{
			bool flag = false;
			for (int i = 0; i < this.m_features.Length; i++)
			{
				if (this.m_features[i].Sex == this.Sex)
				{
					this.m_features[i].RandomizeFeature();
					flag = true;
				}
			}
			if (flag)
			{
				this.RefreshFeatures(this.m_currentCharacter);
				this.RefreshDCA(this.m_currentCharacter);
			}
		}

		// Token: 0x060059FD RID: 23037 RVA: 0x001EB648 File Offset: 0x001E9848
		private void FaceCamToggleChanged(bool arg0)
		{
			this.m_faceCamObj.SetActive(arg0);
			for (int i = 0; i < this.m_newCharacters.Length; i++)
			{
				this.m_newCharacters[i].ToggleFaceCam(arg0);
			}
		}

		// Token: 0x060059FE RID: 23038 RVA: 0x0007C580 File Offset: 0x0007A780
		private void ClothedToggleChanged(bool arg0)
		{
			this.UpdateSelectedClothing();
		}

		// Token: 0x060059FF RID: 23039 RVA: 0x0007C588 File Offset: 0x0007A788
		private void ArmorToggleChanged(bool arg0)
		{
			this.UpdateSelectedArmor();
		}

		// Token: 0x06005A00 RID: 23040 RVA: 0x0007C590 File Offset: 0x0007A790
		private void FemaleButtonClicked()
		{
			this.Sex = CharacterSex.Female;
		}

		// Token: 0x06005A01 RID: 23041 RVA: 0x0007C599 File Offset: 0x0007A799
		private void MaleButtonClicked()
		{
			this.Sex = CharacterSex.Male;
		}

		// Token: 0x06005A02 RID: 23042 RVA: 0x0007C5A2 File Offset: 0x0007A7A2
		private void BodyTypeButtonClicked(NewCharacterManager.BodyTypeButton bodyTypeButton)
		{
			this.BuildType = bodyTypeButton.BuildType;
		}

		// Token: 0x06005A03 RID: 23043 RVA: 0x0007C5B0 File Offset: 0x0007A7B0
		private void SizeSliderChanged(float arg0)
		{
			this.UpdateSelectedSlider("sizeSmall", "sizeLarge", arg0);
		}

		// Token: 0x06005A04 RID: 23044 RVA: 0x0007C5C3 File Offset: 0x0007A7C3
		private void ToneSliderChanged(float arg0)
		{
			this.UpdateSelectedSlider("toneFlabby", "toneMuscular", arg0);
		}

		// Token: 0x06005A05 RID: 23045 RVA: 0x001EB684 File Offset: 0x001E9884
		private void EditButtonClicked()
		{
			if (this.m_recordToEdit != null)
			{
				this.m_editButton.interactable = false;
				CharacterVisuals visuals = this.m_recordToEdit.Visuals;
				CharacterVisuals characterVisuals = this.GetCharacterVisuals();
				if (this.VisualsAreDifferent(visuals, characterVisuals))
				{
					this.m_recordToEdit.Visuals = characterVisuals;
					LoginApiManager.UpdateCharacterVisuals(this.m_recordToEdit);
					return;
				}
				this.m_editButton.interactable = true;
				this.m_acceptDescription.text = "Visuals are the same!";
			}
		}

		// Token: 0x06005A06 RID: 23046 RVA: 0x0007C5D6 File Offset: 0x0007A7D6
		private void ResetRotation()
		{
			this.m_newCharacterParent.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
		}

		// Token: 0x06005A07 RID: 23047 RVA: 0x001EB6F8 File Offset: 0x001E98F8
		private bool VisualsAreDifferent(CharacterVisuals prev, CharacterVisuals curr)
		{
			if (prev.Sex != curr.Sex || prev.BuildType != curr.BuildType)
			{
				return true;
			}
			int num = (prev.Dna != null) ? prev.Dna.Count : 0;
			int num2 = (curr.Dna != null) ? curr.Dna.Count : 0;
			if (num != num2)
			{
				return true;
			}
			if (prev.Dna != null && curr.Dna != null)
			{
				foreach (KeyValuePair<string, float> keyValuePair in curr.Dna)
				{
					float num3;
					if (!prev.Dna.TryGetValue(keyValuePair.Key, out num3) || num3 != keyValuePair.Value)
					{
						return true;
					}
				}
			}
			int num4 = (prev.SharedColors != null) ? prev.SharedColors.Count : 0;
			int num5 = (curr.SharedColors != null) ? curr.SharedColors.Count : 0;
			if (num4 != num5)
			{
				return true;
			}
			if (prev.SharedColors != null && curr.SharedColors != null)
			{
				foreach (KeyValuePair<CharacterColorType, string> keyValuePair2 in curr.SharedColors)
				{
					string a;
					if (!prev.SharedColors.TryGetValue(keyValuePair2.Key, out a) || a != keyValuePair2.Value)
					{
						return true;
					}
				}
			}
			int num6 = (prev.CustomizedSlots != null) ? prev.CustomizedSlots.Count : 0;
			int num7 = (curr.CustomizedSlots != null) ? curr.CustomizedSlots.Count : 0;
			if (num6 != num7)
			{
				return true;
			}
			if (prev.CustomizedSlots != null && curr.CustomizedSlots != null)
			{
				for (int i = 0; i < curr.CustomizedSlots.Count; i++)
				{
					if (!prev.CustomizedSlots.Contains(curr.CustomizedSlots[i]))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06005A08 RID: 23048 RVA: 0x001EB8F8 File Offset: 0x001E9AF8
		private void RefreshUiForMode()
		{
			NewCharacterManager.NewCharacterMode currentMode = this.CurrentMode;
			if (currentMode == NewCharacterManager.NewCharacterMode.Create)
			{
				this.m_characterPanel.gameObject.SetActive(true);
				this.m_acceptButton.gameObject.SetActive(true);
				this.m_editButton.gameObject.SetActive(false);
				this.m_editButton.interactable = false;
				return;
			}
			if (currentMode != NewCharacterManager.NewCharacterMode.Edit)
			{
				return;
			}
			this.m_characterPanel.gameObject.SetActive(false);
			this.m_acceptButton.gameObject.SetActive(false);
			this.m_acceptDescription.text = string.Empty;
			this.m_editButton.gameObject.SetActive(true);
			this.m_editButton.interactable = true;
		}

		// Token: 0x06005A09 RID: 23049 RVA: 0x001EB9A4 File Offset: 0x001E9BA4
		private void SetupNewCharacters()
		{
			List<NewCharacter> list = new List<NewCharacter>(this.m_buildTypes.Length * 2);
			this.SetupNewCharacterSex(list, CharacterSex.Male);
			this.SetupNewCharacterSex(list, CharacterSex.Female);
			this.m_newCharacters = list.ToArray();
		}

		// Token: 0x06005A0A RID: 23050 RVA: 0x001EB9E0 File Offset: 0x001E9BE0
		private void SetupNewCharacterSex(List<NewCharacter> newChars, CharacterSex sex)
		{
			for (int i = 0; i < this.m_buildTypes.Length; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_newCharacterPrefab, this.m_newCharacterParent);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				gameObject.name = "NewCharacter_" + sex.ToString() + "_" + this.m_buildTypes[i].ToString();
				NewCharacter component = gameObject.GetComponent<NewCharacter>();
				component.Init(this, sex, this.m_buildTypes[i]);
				newChars.Add(component);
			}
		}

		// Token: 0x06005A0B RID: 23051 RVA: 0x0007C5FC File Offset: 0x0007A7FC
		private bool IsSelected(NewCharacter character)
		{
			return character.Sex == this.Sex && character.BuildType == this.BuildType;
		}

		// Token: 0x06005A0C RID: 23052 RVA: 0x0007C61C File Offset: 0x0007A81C
		private void RefreshDCA(NewCharacter character)
		{
			if (character && !this.m_preventDcaRefresh)
			{
				character.Dca.LoadDefaultWardrobe();
				character.Dca.Refresh(true, true, true);
			}
		}

		// Token: 0x06005A0D RID: 23053 RVA: 0x001EBAA0 File Offset: 0x001E9CA0
		private void ChangeCharacter()
		{
			for (int i = 0; i < this.m_newCharacters.Length; i++)
			{
				bool flag = this.IsSelected(this.m_newCharacters[i]);
				if (flag)
				{
					this.m_currentCharacter = this.m_newCharacters[i];
					this.m_currentCharacter.Dca.hide = true;
					this.RefreshCharacterDna(this.m_currentCharacter);
					this.RefreshCharacterColors(this.m_currentCharacter);
					this.RefreshClothing(this.m_currentCharacter);
					this.RefreshArmor(this.m_currentCharacter);
					this.RefreshFeatures(this.m_currentCharacter);
					this.RefreshDCA(this.m_currentCharacter);
				}
				this.m_newCharacters[i].SelectCharacter(flag);
			}
		}

		// Token: 0x06005A0E RID: 23054 RVA: 0x0007C647 File Offset: 0x0007A847
		private void RefreshCharacterDna(NewCharacter character)
		{
			character.UpdateDna("sizeSmall", "sizeLarge", this.m_sizeSlider.value);
			character.UpdateDna("toneFlabby", "toneMuscular", this.m_toneSlider.value);
		}

		// Token: 0x06005A0F RID: 23055 RVA: 0x001EBB50 File Offset: 0x001E9D50
		private void RefreshCharacterColors(NewCharacter character)
		{
			if (this.m_colorPaletteDict == null)
			{
				return;
			}
			foreach (KeyValuePair<CharacterColorType, ColorPalette> keyValuePair in this.m_colorPaletteDict)
			{
				this.SetSharedColor(character, keyValuePair.Key, keyValuePair.Value.SelectedColor.SwatchColor);
			}
		}

		// Token: 0x06005A10 RID: 23056 RVA: 0x0007C67F File Offset: 0x0007A87F
		private void SetSharedColor(NewCharacter character, CharacterColorType colorType, Color color)
		{
			if (character == null)
			{
				return;
			}
			if (this.m_colorReferenceType == colorType && this.m_colorReference)
			{
				this.m_colorReference.color = color;
			}
			colorType.SetDcaSharedColor(this.m_currentCharacter.Dca, color);
		}

		// Token: 0x06005A11 RID: 23057 RVA: 0x0007C6BF File Offset: 0x0007A8BF
		private void RefreshClothing(NewCharacter character)
		{
			if (this.m_clothedToggle == null)
			{
				return;
			}
			if (this.m_recordToEdit != null)
			{
				this.RefreshClothingArmorFromRecord(character, false);
				return;
			}
			this.RefreshClothingArmorForNew(character, false);
		}

		// Token: 0x06005A12 RID: 23058 RVA: 0x0007C6E9 File Offset: 0x0007A8E9
		private void RefreshArmor(NewCharacter character)
		{
			if (this.m_armorToggle == null)
			{
				return;
			}
			if (this.m_recordToEdit != null)
			{
				this.RefreshClothingArmorFromRecord(character, true);
				return;
			}
			this.RefreshClothingArmorForNew(character, true);
		}

		// Token: 0x06005A13 RID: 23059 RVA: 0x001EBBC4 File Offset: 0x001E9DC4
		private void RefreshClothingArmorForNew(NewCharacter character, bool isArmor)
		{
			SolToggle solToggle = isArmor ? this.m_armorToggle : this.m_clothedToggle;
			UMAWardrobeRecipe[] array = isArmor ? this.m_maleArmorRecipes : this.m_maleClothingRecipes;
			if (character.Sex == CharacterSex.Female)
			{
				array = (isArmor ? this.m_femaleArmorRecipes : this.m_femaleClothingRecipes);
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (solToggle.isOn)
				{
					character.Dca.SetSlot(array[i]);
				}
				else
				{
					character.Dca.ClearSlot(array[i].wardrobeSlot);
				}
			}
		}

		// Token: 0x06005A14 RID: 23060 RVA: 0x001EBC4C File Offset: 0x001E9E4C
		private void RefreshClothingArmorFromRecord(NewCharacter character, bool isArmor)
		{
			ContainerRecord containerRecord;
			if ((isArmor ? this.m_armorToggle : this.m_clothedToggle).isOn && this.m_recordToEdit != null && this.m_recordToEdit.Storage != null && this.m_recordToEdit.Storage.TryGetValue(ContainerType.Equipment, out containerRecord))
			{
				ArchetypeInstance archetypeInstance = null;
				EquipableItem equipableItem = null;
				for (int i = 0; i < containerRecord.Instances.Count; i++)
				{
					EquipableItem equipableItem2;
					if (containerRecord.Instances[i].Index == 65536 && containerRecord.Instances[i].Archetype != null && InternalGameDatabase.Archetypes.TryGetAsType<EquipableItem>(containerRecord.Instances[i].ArchetypeId, out equipableItem2))
					{
						archetypeInstance = containerRecord.Instances[i];
						equipableItem = equipableItem2;
						break;
					}
				}
				for (int j = 0; j < containerRecord.Instances.Count; j++)
				{
					EquipableItem equipableItem3;
					if (containerRecord.Instances[j].Archetype != null && containerRecord.Instances[j].Archetype.TryGetAsType(out equipableItem3) && equipableItem3.Type.HasArmorCost() == isArmor && (!equipableItem || equipableItem.Type != equipableItem3.Type))
					{
						equipableItem3.OnEquipVisuals(this.m_recordToEdit.Visuals.Sex, character.Dca, containerRecord.Instances[j].Index, containerRecord.Instances[j].ItemData.VisualIndex, containerRecord.Instances[j].ItemData.ColorIndex, false);
					}
					if (equipableItem && archetypeInstance != null && archetypeInstance.ItemData != null && equipableItem.Type.HasArmorCost() == isArmor)
					{
						foreach (EquipmentSlot index in equipableItem.Type.GetCachedCompatibleSlots())
						{
							equipableItem.OnEquipVisuals(this.m_recordToEdit.Visuals.Sex, character.Dca, (int)index, archetypeInstance.ItemData.VisualIndex, archetypeInstance.ItemData.ColorIndex, false);
						}
					}
				}
				return;
			}
			string[] array = isArmor ? this.m_armorSlotsToClear : this.m_clothingSlotsToClear;
			for (int k = 0; k < array.Length; k++)
			{
				character.Dca.ClearSlot(array[k]);
			}
		}

		// Token: 0x06005A15 RID: 23061 RVA: 0x001EBED8 File Offset: 0x001EA0D8
		private void RefreshFeatures(NewCharacter character)
		{
			for (int i = 0; i < this.m_features.Length; i++)
			{
				if (this.m_features[i].Sex == character.Sex)
				{
					FeatureRecipe currentFeatureRecipe = this.m_features[i].GetCurrentFeatureRecipe();
					if (currentFeatureRecipe == null || currentFeatureRecipe.WardrobeRecipe == null)
					{
						character.Dca.ClearSlot(this.m_features[i].SlotName);
					}
					else
					{
						character.Dca.SetSlot(currentFeatureRecipe.WardrobeRecipe);
					}
				}
			}
		}

		// Token: 0x06005A16 RID: 23062 RVA: 0x0007C713 File Offset: 0x0007A913
		private void UpdateSelectedSlider(string dnaLeft, string dnaRight, float value)
		{
			if (this.m_currentCharacter == null)
			{
				return;
			}
			this.m_currentCharacter.UpdateDna(dnaLeft, dnaRight, value);
			this.RefreshDCA(this.m_currentCharacter);
		}

		// Token: 0x06005A17 RID: 23063 RVA: 0x0007C73E File Offset: 0x0007A93E
		private void UpdateSelectedClothing()
		{
			if (this.m_currentCharacter == null)
			{
				return;
			}
			this.RefreshClothing(this.m_currentCharacter);
			this.RefreshDCA(this.m_currentCharacter);
		}

		// Token: 0x06005A18 RID: 23064 RVA: 0x0007C767 File Offset: 0x0007A967
		private void UpdateSelectedArmor()
		{
			if (this.m_currentCharacter == null)
			{
				return;
			}
			this.RefreshArmor(this.m_currentCharacter);
			this.RefreshDCA(this.m_currentCharacter);
		}

		// Token: 0x06005A19 RID: 23065 RVA: 0x001EBF60 File Offset: 0x001EA160
		public void RoleSelected(RoleSelectionButton roleSelectionButton)
		{
			this.m_roleDescription.text = string.Empty;
			for (int i = 0; i < this.m_roleSelectionButtons.Length; i++)
			{
				bool flag = this.m_roleSelectionButtons[i] == roleSelectionButton;
				this.m_roleSelectionButtons[i].SetSelected(flag);
				if (flag)
				{
					this.m_currentRoleSelection = this.m_roleSelectionButtons[i];
					this.m_roleDescription.text = this.m_currentRoleSelection.Role.CreationDescription;
				}
			}
			if (this.m_currentRoleSelection && this.m_currentRoleSelection.Role && this.m_currentRoleSelection.Role.HasSpecializations)
			{
				for (int j = 0; j < this.m_specIcons.Length; j++)
				{
					if (j < this.m_currentRoleSelection.Role.Specializations.Length)
					{
						this.m_specIcons[j].SpecIcon.SetIcon(this.m_currentRoleSelection.Role.Specializations[j], new Color?(this.m_currentRoleSelection.Role.Specializations[j].IconTint));
						this.m_specIcons[j].Tooltip.Text = this.m_currentRoleSelection.Role.Specializations[j].CreationDescription;
					}
				}
				this.m_specPanel.SetActive(true);
			}
			else
			{
				this.m_specPanel.SetActive(false);
			}
			Action roleChanged = this.RoleChanged;
			if (roleChanged == null)
			{
				return;
			}
			roleChanged();
		}

		// Token: 0x06005A1A RID: 23066 RVA: 0x0007C790 File Offset: 0x0007A990
		public void ColorSelected(CharacterColorType colorType, Color color)
		{
			this.SetSharedColor(this.m_currentCharacter, colorType, color);
			this.RefreshDCA(this.m_currentCharacter);
		}

		// Token: 0x06005A1B RID: 23067 RVA: 0x001EC0D4 File Offset: 0x001EA2D4
		public void FeatureCarouselChanged(FeatureCarousel carousel)
		{
			if (this.m_currentCharacter == null)
			{
				return;
			}
			if (carousel.Sex != this.m_currentCharacter.Sex)
			{
				return;
			}
			FeatureRecipe currentFeatureRecipe = carousel.GetCurrentFeatureRecipe();
			if (currentFeatureRecipe == null || currentFeatureRecipe.WardrobeRecipe == null)
			{
				this.m_currentCharacter.Dca.ClearSlot(carousel.SlotName);
			}
			else
			{
				this.m_currentCharacter.Dca.SetSlot(currentFeatureRecipe.WardrobeRecipe);
			}
			this.RefreshDCA(this.m_currentCharacter);
		}

		// Token: 0x06005A1C RID: 23068 RVA: 0x001EC160 File Offset: 0x001EA360
		public void StageEnterInitialize()
		{
			this.m_recordToEdit = null;
			this.CurrentMode = (this.m_isCharacterCreation ? NewCharacterManager.NewCharacterMode.Create : NewCharacterManager.NewCharacterMode.Edit);
			this.ResetRotation();
			this.m_characterName.text = string.Empty;
			this.RoleSelected(null);
			this.m_preventDcaRefresh = true;
			this.m_faceCamToggle.isOn = false;
			this.m_armorToggle.isOn = false;
			this.m_clothedToggle.isOn = false;
			for (int i = 0; i < this.m_features.Length; i++)
			{
				this.m_features[i].ResetFeature();
			}
			this.ColorResetClicked();
			this.SizeSliderResetClicked();
			this.ToneSliderResetClicked();
			this.m_sex = CharacterSex.Male;
			this.RefreshSexOptions();
			this.m_buildType = CharacterBuildType.Brawny;
			this.RefreshBuildTypeOptions();
			this.m_preventDcaRefresh = false;
			this.ChangeCharacter();
			Action stageEnterInitializeEvent = this.StageEnterInitializeEvent;
			if (stageEnterInitializeEvent == null)
			{
				return;
			}
			stageEnterInitializeEvent();
		}

		// Token: 0x06005A1D RID: 23069 RVA: 0x0007C7AC File Offset: 0x0007A9AC
		public BaseRole GetSelectedRole()
		{
			if (!(this.m_currentRoleSelection == null))
			{
				return this.m_currentRoleSelection.Role;
			}
			return null;
		}

		// Token: 0x06005A1E RID: 23070 RVA: 0x001EC238 File Offset: 0x001EA438
		public CharacterVisuals GetCharacterVisuals()
		{
			Dictionary<string, float> dictionary = new Dictionary<string, float>();
			this.AddDnaToDictFromSlider(dictionary, "sizeSmall", "sizeLarge", this.m_sizeSlider.value);
			this.AddDnaToDictFromSlider(dictionary, "toneFlabby", "toneMuscular", this.m_toneSlider.value);
			Dictionary<CharacterColorType, string> dictionary2 = new Dictionary<CharacterColorType, string>();
			this.AddColorToDict(dictionary2, CharacterColorType.Skin);
			this.AddColorToDict(dictionary2, CharacterColorType.Hair);
			this.AddColorToDict(dictionary2, CharacterColorType.Eyes);
			this.AddColorToDict(dictionary2, CharacterColorType.Favorite);
			List<UniqueId> list = new List<UniqueId>();
			for (int i = 0; i < this.m_features.Length; i++)
			{
				if (this.m_features[i].Sex == this.Sex)
				{
					FeatureRecipe currentFeatureRecipe = this.m_features[i].GetCurrentFeatureRecipe();
					if (currentFeatureRecipe != null)
					{
						list.Add(currentFeatureRecipe.Id);
					}
				}
			}
			return new CharacterVisuals
			{
				Sex = this.Sex,
				BuildType = this.BuildType,
				Dna = dictionary,
				SharedColors = dictionary2,
				CustomizedSlots = list
			};
		}

		// Token: 0x06005A1F RID: 23071 RVA: 0x001EC330 File Offset: 0x001EA530
		private void AddDnaToDictFromSlider(IDictionary<string, float> dict, string leftDna, string rightDna, float dnaValue)
		{
			float num;
			float num2;
			UMAManager.GetLeftRightDna(dnaValue, out num, out num2);
			if (num != 0f)
			{
				dict.Add(leftDna, num);
			}
			if (num2 != 0f)
			{
				dict.Add(rightDna, num2);
			}
		}

		// Token: 0x06005A20 RID: 23072 RVA: 0x001EC368 File Offset: 0x001EA568
		private void AddColorToDict(IDictionary<CharacterColorType, string> dict, CharacterColorType colorType)
		{
			ColorPalette colorPalette;
			if (this.m_colorPaletteDict.TryGetValue(colorType, out colorPalette))
			{
				string value = "#" + ColorUtility.ToHtmlStringRGB(colorPalette.SelectedColor.SwatchColor);
				dict.Add(colorType, value);
			}
		}

		// Token: 0x06005A21 RID: 23073 RVA: 0x001EC3A8 File Offset: 0x001EA5A8
		public void LoadCharacter(CharacterRecord record)
		{
			if (record == null)
			{
				return;
			}
			this.m_recordToEdit = record;
			this.CurrentMode = NewCharacterManager.NewCharacterMode.Edit;
			this.ResetRotation();
			CharacterVisuals visuals = record.Visuals;
			this.m_preventDcaRefresh = true;
			this.m_faceCamToggle.isOn = false;
			this.m_armorToggle.isOn = false;
			this.m_clothedToggle.isOn = false;
			this.m_sex = visuals.Sex;
			this.RefreshSexOptions();
			this.m_buildType = visuals.BuildType;
			this.RefreshBuildTypeOptions();
			this.SetColorsFromDict(visuals.SharedColors);
			this.SetFeatures(visuals.CustomizedSlots);
			this.RefreshCharacterColors(this.m_currentCharacter);
			this.RefreshFeatures(this.m_currentCharacter);
			this.RefreshDCA(this.m_currentCharacter);
			if (visuals.Dna != null)
			{
				this.SetDnaSlidersFromDict(visuals.Dna, "sizeSmall", "sizeLarge", this.m_sizeSlider);
				this.SetDnaSlidersFromDict(visuals.Dna, "toneFlabby", "toneMuscular", this.m_toneSlider);
			}
			else
			{
				this.m_sizeSlider.value = 0.5f;
				this.m_toneSlider.value = 0.5f;
			}
			this.m_preventDcaRefresh = false;
			this.ChangeCharacter();
			Action stageEnterInitializeEvent = this.StageEnterInitializeEvent;
			if (stageEnterInitializeEvent == null)
			{
				return;
			}
			stageEnterInitializeEvent();
		}

		// Token: 0x06005A22 RID: 23074 RVA: 0x001EC4E0 File Offset: 0x001EA6E0
		private void SetDnaSlidersFromDict(IDictionary<string, float> dict, string leftDna, string rightDna, Slider slider)
		{
			float left;
			if (!dict.TryGetValue(leftDna, out left))
			{
				left = 0f;
			}
			float right;
			if (!dict.TryGetValue(rightDna, out right))
			{
				right = 0f;
			}
			slider.value = UMAManager.GetValueForLeftRight(left, right);
		}

		// Token: 0x06005A23 RID: 23075 RVA: 0x001EC520 File Offset: 0x001EA720
		private void SetColorsFromDict(IDictionary<CharacterColorType, string> dict)
		{
			foreach (KeyValuePair<CharacterColorType, string> keyValuePair in dict)
			{
				ColorPalette colorPalette;
				if (this.m_colorPaletteDict.TryGetValue(keyValuePair.Key, out colorPalette))
				{
					colorPalette.SelectColor(keyValuePair.Value);
				}
			}
		}

		// Token: 0x06005A24 RID: 23076 RVA: 0x001EC584 File Offset: 0x001EA784
		private void SetFeatures(List<UniqueId> featureIds)
		{
			for (int i = 0; i < this.m_features.Length; i++)
			{
				if (this.m_features[i].Sex == this.Sex)
				{
					this.m_features[i].SelectRecipeIfPresent(featureIds);
				}
			}
		}

		// Token: 0x06005A25 RID: 23077 RVA: 0x0007C7C9 File Offset: 0x0007A9C9
		private void PrintVisuals()
		{
			Debug.Log(JsonConvert.SerializeObject(this.GetCharacterVisuals()));
		}

		// Token: 0x04004F08 RID: 20232
		public const string kSizeDnaLeft = "sizeSmall";

		// Token: 0x04004F09 RID: 20233
		public const string kSizeDnaRight = "sizeLarge";

		// Token: 0x04004F0A RID: 20234
		public const string kToneDnaLeft = "toneFlabby";

		// Token: 0x04004F0B RID: 20235
		public const string kToneDnaRight = "toneMuscular";

		// Token: 0x04004F0C RID: 20236
		private CharacterSex m_sex;

		// Token: 0x04004F0D RID: 20237
		private CharacterBuildType m_buildType;

		// Token: 0x04004F0E RID: 20238
		private NewCharacterManager.NewCharacterMode m_currentMode;

		// Token: 0x04004F0F RID: 20239
		private readonly CharacterBuildType[] m_buildTypes = new CharacterBuildType[]
		{
			CharacterBuildType.Stoic,
			CharacterBuildType.Brawny,
			CharacterBuildType.Lean,
			CharacterBuildType.Heavyset,
			CharacterBuildType.Rotund
		};

		// Token: 0x04004F10 RID: 20240
		private readonly string[] m_clothingSlotsToClear = new string[]
		{
			"Clothing_Head",
			"Clothing_Chest",
			"Clothing_Hands",
			"Clothing_Waist",
			"Clothing_Legs",
			"Clothing_Feet"
		};

		// Token: 0x04004F11 RID: 20241
		private readonly string[] m_armorSlotsToClear = new string[]
		{
			"Armor_Head",
			"Armor_Chest",
			"Armor_Shoulders_L",
			"Armor_Shoulders_R",
			"Armor_Hands_L",
			"Armor_Hands_R",
			"Armor_Legs",
			"Armor_Feet_L",
			"Armor_Feet_R"
		};

		// Token: 0x04004F12 RID: 20242
		private const string kMaleGroup = "Male";

		// Token: 0x04004F13 RID: 20243
		private const string kFemaleGroup = "Female";

		// Token: 0x04004F14 RID: 20244
		private const string kColorGroup = "Colors";

		// Token: 0x04004F15 RID: 20245
		private const string kFeatureGroup = "Features";

		// Token: 0x04004F16 RID: 20246
		private const string kCharacterGroup = "Character";

		// Token: 0x04004F17 RID: 20247
		private const string kAcceptGroup = "Accept";

		// Token: 0x04004F18 RID: 20248
		[SerializeField]
		private bool m_isCharacterCreation;

		// Token: 0x04004F19 RID: 20249
		[SerializeField]
		private GameObject m_newCharacterPrefab;

		// Token: 0x04004F1A RID: 20250
		[SerializeField]
		private Transform m_newCharacterParent;

		// Token: 0x04004F1B RID: 20251
		[SerializeField]
		private NewCharacterManager.BodyTypeButton[] m_bodyTypeButtons;

		// Token: 0x04004F1C RID: 20252
		[SerializeField]
		private FeatureCarousel[] m_features;

		// Token: 0x04004F1D RID: 20253
		[SerializeField]
		private GameObject m_maleFeaturePanel;

		// Token: 0x04004F1E RID: 20254
		[SerializeField]
		private GameObject m_femaleFeaturePanel;

		// Token: 0x04004F1F RID: 20255
		[SerializeField]
		private SolButton m_featureRandomize;

		// Token: 0x04004F20 RID: 20256
		[SerializeField]
		private SolButton m_featureReset;

		// Token: 0x04004F21 RID: 20257
		[SerializeField]
		private SolButton m_colorRandomize;

		// Token: 0x04004F22 RID: 20258
		[SerializeField]
		private SolButton m_colorReset;

		// Token: 0x04004F23 RID: 20259
		[SerializeField]
		private ColorPalette[] m_colorPalettes;

		// Token: 0x04004F24 RID: 20260
		[SerializeField]
		private CharacterColorType m_colorReferenceType = CharacterColorType.Skin;

		// Token: 0x04004F25 RID: 20261
		[SerializeField]
		private Image m_colorReference;

		// Token: 0x04004F26 RID: 20262
		[SerializeField]
		private SolButton m_maleButton;

		// Token: 0x04004F27 RID: 20263
		[SerializeField]
		private Image m_maleHighlight;

		// Token: 0x04004F28 RID: 20264
		[SerializeField]
		private SolButton m_femaleButton;

		// Token: 0x04004F29 RID: 20265
		[SerializeField]
		private Image m_femaleHighlight;

		// Token: 0x04004F2A RID: 20266
		[SerializeField]
		private UMAWardrobeRecipe[] m_maleClothingRecipes;

		// Token: 0x04004F2B RID: 20267
		[SerializeField]
		private UMAWardrobeRecipe[] m_maleArmorRecipes;

		// Token: 0x04004F2C RID: 20268
		[SerializeField]
		private UMAWardrobeRecipe[] m_femaleClothingRecipes;

		// Token: 0x04004F2D RID: 20269
		[SerializeField]
		private UMAWardrobeRecipe[] m_femaleArmorRecipes;

		// Token: 0x04004F2E RID: 20270
		[SerializeField]
		private RectTransform m_characterPanel;

		// Token: 0x04004F2F RID: 20271
		[SerializeField]
		private TextMeshProUGUI m_roleDescription;

		// Token: 0x04004F30 RID: 20272
		[SerializeField]
		private RoleSelectionButton[] m_roleSelectionButtons;

		// Token: 0x04004F31 RID: 20273
		[SerializeField]
		private GameObject m_specPanel;

		// Token: 0x04004F32 RID: 20274
		[SerializeField]
		private NewCharacterManager.SpecIconData[] m_specIcons;

		// Token: 0x04004F33 RID: 20275
		[SerializeField]
		private TMP_InputField m_characterName;

		// Token: 0x04004F34 RID: 20276
		[SerializeField]
		private SolButton m_acceptButton;

		// Token: 0x04004F35 RID: 20277
		[SerializeField]
		private TextMeshProUGUI m_acceptDescription;

		// Token: 0x04004F36 RID: 20278
		[SerializeField]
		private SolButton m_editButton;

		// Token: 0x04004F37 RID: 20279
		[SerializeField]
		private SolToggle m_clothedToggle;

		// Token: 0x04004F38 RID: 20280
		[SerializeField]
		private SolToggle m_armorToggle;

		// Token: 0x04004F39 RID: 20281
		[SerializeField]
		private SolToggle m_faceCamToggle;

		// Token: 0x04004F3A RID: 20282
		[SerializeField]
		private GameObject m_faceCamObj;

		// Token: 0x04004F3B RID: 20283
		[SerializeField]
		private Slider m_sizeSlider;

		// Token: 0x04004F3C RID: 20284
		[SerializeField]
		private Slider m_toneSlider;

		// Token: 0x04004F3D RID: 20285
		[SerializeField]
		private SolButton m_sizeSliderReset;

		// Token: 0x04004F3E RID: 20286
		[SerializeField]
		private SolButton m_toneSliderReset;

		// Token: 0x04004F3F RID: 20287
		private Dictionary<CharacterColorType, ColorPalette> m_colorPaletteDict;

		// Token: 0x04004F40 RID: 20288
		private NewCharacter[] m_newCharacters;

		// Token: 0x04004F41 RID: 20289
		private NewCharacter m_currentCharacter;

		// Token: 0x04004F42 RID: 20290
		private RoleSelectionButton m_currentRoleSelection;

		// Token: 0x04004F43 RID: 20291
		private CharacterRecord m_recordToEdit;

		// Token: 0x04004F44 RID: 20292
		private bool m_preventDcaRefresh;

		// Token: 0x02000B6F RID: 2927
		[Serializable]
		private class BodyTypeButton
		{
			// Token: 0x170014FA RID: 5370
			// (get) Token: 0x06005A27 RID: 23079 RVA: 0x0007C7DB File Offset: 0x0007A9DB
			public Button Button
			{
				get
				{
					return this.m_button;
				}
			}

			// Token: 0x170014FB RID: 5371
			// (get) Token: 0x06005A28 RID: 23080 RVA: 0x0007C7E3 File Offset: 0x0007A9E3
			public CharacterBuildType BuildType
			{
				get
				{
					return this.m_buildType;
				}
			}

			// Token: 0x06005A29 RID: 23081 RVA: 0x0007C7EB File Offset: 0x0007A9EB
			public void Init(NewCharacterManager manager)
			{
				this.m_manager = manager;
				this.m_button.onClick.AddListener(new UnityAction(this.OnButtonClicked));
			}

			// Token: 0x06005A2A RID: 23082 RVA: 0x0007C810 File Offset: 0x0007AA10
			public void OnDestroy()
			{
				this.m_button.onClick.RemoveListener(new UnityAction(this.OnButtonClicked));
			}

			// Token: 0x06005A2B RID: 23083 RVA: 0x0007C82E File Offset: 0x0007AA2E
			private void OnButtonClicked()
			{
				this.m_manager.BodyTypeButtonClicked(this);
			}

			// Token: 0x06005A2C RID: 23084 RVA: 0x0007C83C File Offset: 0x0007AA3C
			public void RefreshImage()
			{
				if (this.m_separateFemaleImage)
				{
					this.m_button.image.overrideSprite = ((this.m_manager.Sex == CharacterSex.Female) ? this.m_femaleImage : null);
				}
			}

			// Token: 0x04004F45 RID: 20293
			[SerializeField]
			private SolButton m_button;

			// Token: 0x04004F46 RID: 20294
			[SerializeField]
			private CharacterBuildType m_buildType;

			// Token: 0x04004F47 RID: 20295
			[SerializeField]
			private bool m_separateFemaleImage;

			// Token: 0x04004F48 RID: 20296
			[SerializeField]
			private Sprite m_femaleImage;

			// Token: 0x04004F49 RID: 20297
			private NewCharacterManager m_manager;
		}

		// Token: 0x02000B70 RID: 2928
		private enum NewCharacterMode
		{
			// Token: 0x04004F4B RID: 20299
			None,
			// Token: 0x04004F4C RID: 20300
			Create,
			// Token: 0x04004F4D RID: 20301
			Edit
		}

		// Token: 0x02000B71 RID: 2929
		[Serializable]
		private class SpecIconData
		{
			// Token: 0x04004F4E RID: 20302
			public ArchetypeIconUI SpecIcon;

			// Token: 0x04004F4F RID: 20303
			public TextTooltipTrigger Tooltip;
		}
	}
}

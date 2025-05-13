using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using SoL.Game.Login.Client.Creation.NewCreation;
using SoL.Game.Login.Client.Selection;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.Login.Client.Creation
{
	// Token: 0x02000B4A RID: 2890
	public class CreationDirector : MonoBehaviour
	{
		// Token: 0x170014CC RID: 5324
		// (get) Token: 0x060058FE RID: 22782 RVA: 0x0007B86D File Offset: 0x00079A6D
		public CreationDirector.CharacterToCreate CurrentCharacter
		{
			get
			{
				return this.m_currentCharacter;
			}
		}

		// Token: 0x170014CD RID: 5325
		// (get) Token: 0x060058FF RID: 22783 RVA: 0x0007B875 File Offset: 0x00079A75
		public GameObject HeaderPrefab
		{
			get
			{
				return this.m_headerPrefab;
			}
		}

		// Token: 0x170014CE RID: 5326
		// (get) Token: 0x06005900 RID: 22784 RVA: 0x0007B87D File Offset: 0x00079A7D
		public GameObject SliderPrefab
		{
			get
			{
				return this.m_sliderPrefab;
			}
		}

		// Token: 0x170014CF RID: 5327
		// (get) Token: 0x06005901 RID: 22785 RVA: 0x0007B885 File Offset: 0x00079A85
		private SolButton m_create
		{
			get
			{
				return this.m_newCharacterManager.AcceptButton;
			}
		}

		// Token: 0x170014D0 RID: 5328
		// (get) Token: 0x06005902 RID: 22786 RVA: 0x0007B892 File Offset: 0x00079A92
		private TMP_InputField m_characterName
		{
			get
			{
				return this.m_newCharacterManager.CharacterName;
			}
		}

		// Token: 0x170014D1 RID: 5329
		// (get) Token: 0x06005903 RID: 22787 RVA: 0x0007B89F File Offset: 0x00079A9F
		// (set) Token: 0x06005904 RID: 22788 RVA: 0x001E8B5C File Offset: 0x001E6D5C
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
				CreationDirector.CharacterToCreate currentCharacter = this.m_currentCharacter;
				if (currentCharacter != null)
				{
					currentCharacter.OnDeselected();
				}
				this.m_sex = value;
				CharacterSex sex = this.m_sex;
				if (sex != CharacterSex.Male)
				{
					if (sex == CharacterSex.Female)
					{
						this.m_currentCharacter = this.m_femaleCharacter;
					}
				}
				else
				{
					this.m_currentCharacter = this.m_maleCharacter;
				}
				this.m_maleButton.interactable = (this.m_sex != CharacterSex.Male);
				this.m_femaleButton.interactable = (this.m_sex != CharacterSex.Female);
				CreationDirector.CharacterToCreate currentCharacter2 = this.m_currentCharacter;
				if (currentCharacter2 != null)
				{
					currentCharacter2.OnSelected();
				}
				for (int i = 0; i < this.m_uniqueTabs.Length; i++)
				{
					this.m_uniqueTabs[i].ToggleFeaturesForSex();
				}
			}
		}

		// Token: 0x06005905 RID: 22789 RVA: 0x001E8C18 File Offset: 0x001E6E18
		private void Awake()
		{
			this.m_creationStage.OnStageEnter += this.OnStageEnter;
			this.m_creationStage.OnStageError += this.OnErrorRaised;
			this.m_creationStage.OnStageExit += this.CreationStageOnOnStageExit;
			this.m_characterName.onValueChanged.AddListener(new UnityAction<string>(this.NameChanged));
			this.m_create.onClick.AddListener(new UnityAction(this.CreatePressed));
			this.m_newCharacterManager.RoleChanged += this.RefreshCreateInteractivity;
			this.RefreshCreateInteractivity();
		}

		// Token: 0x06005906 RID: 22790 RVA: 0x001E8CC0 File Offset: 0x001E6EC0
		private void OnDestroy()
		{
			this.m_creationStage.OnStageEnter -= this.OnStageEnter;
			this.m_creationStage.OnStageError -= this.OnErrorRaised;
			this.m_creationStage.OnStageExit -= this.CreationStageOnOnStageExit;
			this.m_characterName.onValueChanged.RemoveListener(new UnityAction<string>(this.NameChanged));
			this.m_create.onClick.RemoveListener(new UnityAction(this.CreatePressed));
			this.m_newCharacterManager.RoleChanged -= this.RefreshCreateInteractivity;
		}

		// Token: 0x06005907 RID: 22791 RVA: 0x001E8D64 File Offset: 0x001E6F64
		private void CameraButtonClicked(int index)
		{
			for (int i = 0; i < this.m_cameraButtons.Length; i++)
			{
				this.m_cameraButtons[i].Toggle(i == index);
			}
		}

		// Token: 0x06005908 RID: 22792 RVA: 0x0007B8A7 File Offset: 0x00079AA7
		private void NameChanged(string txt)
		{
			this.RefreshCreateInteractivity();
		}

		// Token: 0x06005909 RID: 22793 RVA: 0x001E8D98 File Offset: 0x001E6F98
		private void RefreshCreateInteractivity()
		{
			bool interactable = true;
			string text = string.Empty;
			if (!this.m_interactable)
			{
				interactable = false;
			}
			else if (string.IsNullOrEmpty(this.m_characterName.text))
			{
				interactable = false;
				text = "Enter a character name!";
			}
			else if (this.m_newCharacterManager.GetSelectedRole() == null)
			{
				interactable = false;
				text = "Select a role!";
			}
			this.m_create.interactable = interactable;
			this.m_newCharacterManager.AcceptDescription.SetText(text);
			this.m_newCharacterManager.AcceptDescription.gameObject.SetActive(!string.IsNullOrEmpty(text));
		}

		// Token: 0x0600590A RID: 22794 RVA: 0x0007B8AF File Offset: 0x00079AAF
		private void OnErrorRaised()
		{
			this.ToggleInteraction(true);
		}

		// Token: 0x0600590B RID: 22795 RVA: 0x001E8E2C File Offset: 0x001E702C
		private void ToggleInteraction(bool enabled)
		{
			this.m_create.interactable = (enabled && !string.IsNullOrEmpty(this.m_characterName.text));
			this.m_characterName.interactable = enabled;
			for (int i = 0; i < this.m_customizations.Length; i++)
			{
				if (this.m_customizations[i])
				{
					if (enabled)
					{
						this.m_customizations[i].Show(false);
					}
					else
					{
						this.m_customizations[i].Hide(false);
					}
				}
			}
			this.m_interactable = enabled;
		}

		// Token: 0x0600590C RID: 22796 RVA: 0x001E8EB4 File Offset: 0x001E70B4
		private void CreatePressed()
		{
			this.ToggleInteraction(false);
			string text = this.m_characterName.text.Substring(0, 1);
			string text2 = this.m_characterName.text.Substring(1);
			string name = text.ToUpper() + text2.ToLower();
			CharacterRecord characterRecord = new CharacterRecord
			{
				Name = name,
				Visuals = this.m_newCharacterManager.GetCharacterVisuals(),
				Location = new CharacterLocation
				{
					x = CharacterRecord.kStartingPosition,
					y = CharacterRecord.kStartingPosition,
					z = CharacterRecord.kStartingPosition,
					h = 0f,
					ZoneId = 100
				},
				SelectionPositionIndex = -1
			};
			characterRecord.InitializeCharacterStorage();
			BaseRole selectedRole = this.m_newCharacterManager.GetSelectedRole();
			if (selectedRole != null)
			{
				selectedRole.AddStartingEquipment(characterRecord);
			}
			HashSet<int> hashSet = new HashSet<int>();
			for (int i = 0; i < SessionData.Characters.Length; i++)
			{
				hashSet.Add(SessionData.Characters[i].SelectionPositionIndex);
			}
			for (int j = 0; j < SelectionDirector.MaxCharactersDisplayed; j++)
			{
				if (!hashSet.Contains(j))
				{
					characterRecord.SelectionPositionIndex = j;
					break;
				}
			}
			if (characterRecord.SelectionPositionIndex < 0)
			{
				Debug.Log("Could not find an appropriate position index! Will not be shown.");
			}
			LoginApiManager.CreateCharacter(characterRecord);
		}

		// Token: 0x0600590D RID: 22797 RVA: 0x001E8FF8 File Offset: 0x001E71F8
		private void OnStageEnter()
		{
			this.ToggleInteraction(true);
			if (this.m_recordToLoad != null)
			{
				this.m_newCharacterManager.LoadCharacter(this.m_recordToLoad);
			}
			else if (this.m_initialized)
			{
				this.m_newCharacterManager.StageEnterInitialize();
			}
			this.m_recordToLoad = null;
			this.m_initialized = true;
		}

		// Token: 0x0600590E RID: 22798 RVA: 0x0004475B File Offset: 0x0004295B
		private void CreationStageOnOnStageExit()
		{
		}

		// Token: 0x0600590F RID: 22799 RVA: 0x0007B8B8 File Offset: 0x00079AB8
		private IEnumerator InitializeUMAs()
		{
			while (this.m_maleCharacter == null || this.m_femaleCharacter == null)
			{
				yield return null;
			}
			this.RandomizeSex();
			this.InitializeSharedTabs();
			yield break;
		}

		// Token: 0x06005910 RID: 22800 RVA: 0x001E9048 File Offset: 0x001E7248
		private void InitializeSharedTabs()
		{
			for (int i = 0; i < this.m_sharedTabs.Length; i++)
			{
				this.m_sharedTabs[i].Initialize(this, null);
			}
		}

		// Token: 0x06005911 RID: 22801 RVA: 0x0007B8C7 File Offset: 0x00079AC7
		private void OnMaleCreated(UMAData obj)
		{
			this.m_maleDca.umaData.OnCharacterCreated -= this.OnMaleCreated;
			this.m_maleHead = this.GetHeadTransform(this.m_maleDca);
			this.m_maleCharacter = new CreationDirector.CharacterToCreate(this, CharacterSex.Male);
		}

		// Token: 0x06005912 RID: 22802 RVA: 0x0007B904 File Offset: 0x00079B04
		private void OnFemaleCreated(UMAData obj)
		{
			this.m_femaleDca.umaData.OnCharacterCreated -= this.OnFemaleCreated;
			this.m_femaleHead = this.GetHeadTransform(this.m_femaleDca);
			this.m_femaleCharacter = new CreationDirector.CharacterToCreate(this, CharacterSex.Female);
		}

		// Token: 0x06005913 RID: 22803 RVA: 0x001E9078 File Offset: 0x001E7278
		private Transform GetHeadTransform(DynamicCharacterAvatar dca)
		{
			Animator component = dca.gameObject.GetComponent<Animator>();
			if (component != null)
			{
				return component.GetBoneTransform(HumanBodyBones.Head);
			}
			return null;
		}

		// Token: 0x06005914 RID: 22804 RVA: 0x0007B941 File Offset: 0x00079B41
		private void OnMalePressed()
		{
			this.Sex = CharacterSex.Male;
		}

		// Token: 0x06005915 RID: 22805 RVA: 0x0007B94A File Offset: 0x00079B4A
		private void OnFemalePressed()
		{
			this.Sex = CharacterSex.Female;
		}

		// Token: 0x06005916 RID: 22806 RVA: 0x001E90A4 File Offset: 0x001E72A4
		private void RandomizeAll()
		{
			for (int i = 0; i < this.m_sharedTabs.Length; i++)
			{
				this.m_sharedTabs[i].RandomizeFeatures();
			}
			for (int j = 0; j < this.m_uniqueTabs.Length; j++)
			{
				this.m_uniqueTabs[j].RandomizeFeatures(this.m_currentCharacter);
			}
		}

		// Token: 0x06005917 RID: 22807 RVA: 0x0007B953 File Offset: 0x00079B53
		private void ResetAllButtonClicked()
		{
			this.ResetAll(false);
		}

		// Token: 0x06005918 RID: 22808 RVA: 0x001E90F8 File Offset: 0x001E72F8
		private void ResetAll(bool force)
		{
			this.m_characterName.text = null;
			for (int i = 0; i < this.m_sharedTabs.Length; i++)
			{
				this.m_sharedTabs[i].ResetFeatures(force);
			}
			for (int j = 0; j < this.m_uniqueTabs.Length; j++)
			{
				if (force)
				{
					this.m_uniqueTabs[j].ResetFeatures(force);
				}
				else
				{
					this.m_uniqueTabs[j].ResetFeatures(this.m_currentCharacter);
				}
			}
		}

		// Token: 0x06005919 RID: 22809 RVA: 0x001E916C File Offset: 0x001E736C
		private void UnlockAll()
		{
			for (int i = 0; i < this.m_sharedTabs.Length; i++)
			{
				this.m_sharedTabs[i].UnlockFeatures();
			}
			for (int j = 0; j < this.m_uniqueTabs.Length; j++)
			{
				this.m_uniqueTabs[j].UnlockFeatures();
			}
		}

		// Token: 0x0600591A RID: 22810 RVA: 0x001E91BC File Offset: 0x001E73BC
		private void RandomizeSex()
		{
			float num = UnityEngine.Random.Range(0f, 1f);
			this.Sex = ((num > 0.5f) ? CharacterSex.Male : CharacterSex.Female);
		}

		// Token: 0x0600591B RID: 22811 RVA: 0x0007B95C File Offset: 0x00079B5C
		public void SetSharedColor(CharacterColorType type, Color color)
		{
			this.SetColorForDca(this.m_maleDca, type, color);
			this.SetColorForDca(this.m_femaleDca, type, color);
			this.RefreshAll();
		}

		// Token: 0x0600591C RID: 22812 RVA: 0x0007B980 File Offset: 0x00079B80
		private void SetColorForDca(DynamicCharacterAvatar dca, CharacterColorType type, Color color)
		{
			dca.SetSharedColorForDca(type, color);
		}

		// Token: 0x0600591D RID: 22813 RVA: 0x0007B98A File Offset: 0x00079B8A
		public void SetSharedSlider(string type, float value)
		{
			this.SetDnaForDca(this.m_maleCharacter, type, value);
			this.SetDnaForDca(this.m_femaleCharacter, type, value);
		}

		// Token: 0x0600591E RID: 22814 RVA: 0x0007B9A8 File Offset: 0x00079BA8
		private void SetDnaForDca(CreationDirector.CharacterToCreate toCreate, string type, float value)
		{
			if (toCreate.DNASetters.ContainsKey(type))
			{
				toCreate.DNASetters[type].Set(value);
				return;
			}
			Debug.Log("Unable to find key: " + type);
		}

		// Token: 0x0600591F RID: 22815 RVA: 0x0007B9DB File Offset: 0x00079BDB
		public void RefreshAll()
		{
			if (!this.m_maleDca.hide)
			{
				this.m_maleDca.Refresh(true, true, true);
			}
			if (!this.m_femaleDca.hide)
			{
				this.m_femaleDca.Refresh(true, true, true);
			}
		}

		// Token: 0x06005920 RID: 22816 RVA: 0x0007BA13 File Offset: 0x00079C13
		public void LoadCharacter(CharacterRecord record)
		{
			this.m_recordToLoad = record;
		}

		// Token: 0x04004E46 RID: 20038
		[SerializeField]
		private NewCharacterManager m_newCharacterManager;

		// Token: 0x04004E47 RID: 20039
		[Header("References")]
		[SerializeField]
		private LoginStageCharacterCreation m_creationStage;

		// Token: 0x04004E48 RID: 20040
		[SerializeField]
		private Button m_maleButton;

		// Token: 0x04004E49 RID: 20041
		[SerializeField]
		private Button m_femaleButton;

		// Token: 0x04004E4A RID: 20042
		[SerializeField]
		private UIWindow[] m_customizations;

		// Token: 0x04004E4B RID: 20043
		[SerializeField]
		private DynamicCharacterAvatar m_maleDca;

		// Token: 0x04004E4C RID: 20044
		[SerializeField]
		private DynamicCharacterAvatar m_femaleDca;

		// Token: 0x04004E4D RID: 20045
		[Header("UI Prefabs")]
		[SerializeField]
		private GameObject m_selectorPrefab;

		// Token: 0x04004E4E RID: 20046
		[SerializeField]
		private GameObject m_sliderPrefab;

		// Token: 0x04004E4F RID: 20047
		[SerializeField]
		private GameObject m_sliderclusterPrefab;

		// Token: 0x04004E50 RID: 20048
		[SerializeField]
		private GameObject m_colorSelectorPrefab;

		// Token: 0x04004E51 RID: 20049
		[SerializeField]
		private GameObject m_headerPrefab;

		// Token: 0x04004E52 RID: 20050
		[Header("UI Settings")]
		[SerializeField]
		private CreationDirector.Tab[] m_sharedTabs;

		// Token: 0x04004E53 RID: 20051
		[SerializeField]
		private CreationDirector.Tab[] m_uniqueTabs;

		// Token: 0x04004E54 RID: 20052
		[Header("Cameras")]
		[SerializeField]
		private CreationDirector.CameraPosButton[] m_cameraButtons;

		// Token: 0x04004E55 RID: 20053
		private bool m_initialized;

		// Token: 0x04004E56 RID: 20054
		private CreationDirector.CharacterToCreate m_maleCharacter;

		// Token: 0x04004E57 RID: 20055
		private CreationDirector.CharacterToCreate m_femaleCharacter;

		// Token: 0x04004E58 RID: 20056
		private CreationDirector.CharacterToCreate m_currentCharacter;

		// Token: 0x04004E59 RID: 20057
		private Transform m_maleHead;

		// Token: 0x04004E5A RID: 20058
		private Transform m_femaleHead;

		// Token: 0x04004E5B RID: 20059
		private bool m_interactable = true;

		// Token: 0x04004E5C RID: 20060
		private CharacterSex m_sex;

		// Token: 0x04004E5D RID: 20061
		private CharacterRecord m_recordToLoad;

		// Token: 0x02000B4B RID: 2891
		[Serializable]
		private class CameraPosButton
		{
			// Token: 0x06005922 RID: 22818 RVA: 0x0007BA2B File Offset: 0x00079C2B
			public void Init(CreationDirector director, int index)
			{
				this.m_director = director;
				this.m_index = index;
				this.m_button.onClick.AddListener(new UnityAction(this.ButtonPressed));
				this.Toggle(index == 0);
			}

			// Token: 0x06005923 RID: 22819 RVA: 0x0007BA61 File Offset: 0x00079C61
			public void Destroy()
			{
				this.m_button.onClick.RemoveListener(new UnityAction(this.ButtonPressed));
			}

			// Token: 0x06005924 RID: 22820 RVA: 0x0007BA7F File Offset: 0x00079C7F
			private void ButtonPressed()
			{
				this.m_director.CameraButtonClicked(this.m_index);
			}

			// Token: 0x06005925 RID: 22821 RVA: 0x0007BA92 File Offset: 0x00079C92
			public void Toggle(bool enabled)
			{
				this.m_button.interactable = !enabled;
				if (this.m_camera != null)
				{
					this.m_camera.gameObject.SetActive(enabled);
				}
			}

			// Token: 0x06005926 RID: 22822 RVA: 0x001E91EC File Offset: 0x001E73EC
			public void UpdatePos()
			{
				if (this.m_camera != null && this.m_camera.gameObject.activeInHierarchy)
				{
					Transform transform = (this.m_director.m_currentCharacter.Sex == CharacterSex.Male) ? this.m_director.m_maleHead : this.m_director.m_femaleHead;
					if (transform != null)
					{
						Vector3 target = new Vector3(this.m_camera.gameObject.transform.position.x, transform.position.y, this.m_camera.gameObject.transform.position.z);
						this.m_camera.gameObject.transform.position = Vector3.MoveTowards(this.m_camera.gameObject.transform.position, target, Time.deltaTime * 2f);
						this.m_focus.gameObject.transform.position = transform.position;
					}
				}
			}

			// Token: 0x04004E5E RID: 20062
			[SerializeField]
			private CinemachineVirtualCamera m_camera;

			// Token: 0x04004E5F RID: 20063
			[SerializeField]
			private SolButton m_button;

			// Token: 0x04004E60 RID: 20064
			[SerializeField]
			private GameObject m_focus;

			// Token: 0x04004E61 RID: 20065
			private int m_index = -1;

			// Token: 0x04004E62 RID: 20066
			private CreationDirector m_director;
		}

		// Token: 0x02000B4C RID: 2892
		public class CustomizationCategory
		{
			// Token: 0x06005928 RID: 22824 RVA: 0x0007BAD1 File Offset: 0x00079CD1
			public CustomizationCategory(CharacterCustomization first)
			{
				this.Customizations = new List<CharacterCustomization>
				{
					null,
					first
				};
			}

			// Token: 0x06005929 RID: 22825 RVA: 0x0007BAF2 File Offset: 0x00079CF2
			public void AddValue(CharacterCustomization cust)
			{
				if (!this.Customizations.Contains(cust))
				{
					this.Customizations.Add(cust);
				}
			}

			// Token: 0x0600592A RID: 22826 RVA: 0x0007BB0E File Offset: 0x00079D0E
			public CharacterCustomization IterateForward()
			{
				this.Index++;
				if (this.Index >= this.Customizations.Count)
				{
					this.Index = 0;
				}
				return this.Customizations[this.Index];
			}

			// Token: 0x0600592B RID: 22827 RVA: 0x0007BB49 File Offset: 0x00079D49
			public CharacterCustomization IterateBackward()
			{
				this.Index--;
				if (this.Index < 0)
				{
					this.Index = this.Customizations.Count - 1;
				}
				return this.Customizations[this.Index];
			}

			// Token: 0x04004E63 RID: 20067
			public int Index;

			// Token: 0x04004E64 RID: 20068
			public readonly List<CharacterCustomization> Customizations;
		}

		// Token: 0x02000B4D RID: 2893
		[Serializable]
		public class FeatureSetting
		{
			// Token: 0x170014D2 RID: 5330
			// (get) Token: 0x0600592C RID: 22828 RVA: 0x0007BB86 File Offset: 0x00079D86
			private bool m_showSliders
			{
				get
				{
					return this.Type == FeatureUIType.Slider || this.Type == FeatureUIType.ClusterSlider;
				}
			}

			// Token: 0x04004E65 RID: 20069
			public string Label;

			// Token: 0x04004E66 RID: 20070
			public FeatureUIType Type;

			// Token: 0x04004E67 RID: 20071
			public CharacterColorType ColorType;

			// Token: 0x04004E68 RID: 20072
			public ColorCollection ColorCollection;

			// Token: 0x04004E69 RID: 20073
			public CharacterCustomizationType Slot;

			// Token: 0x04004E6A RID: 20074
			public UMADnaType[] Sliders;
		}

		// Token: 0x02000B4E RID: 2894
		[Serializable]
		private class Tab
		{
			// Token: 0x0600592E RID: 22830 RVA: 0x001E92F4 File Offset: 0x001E74F4
			public void Initialize(CreationDirector director, CreationDirector.CharacterToCreate charToCreate)
			{
				if (!this.m_enabled)
				{
					return;
				}
				this.m_director = director;
				int i = 0;
				while (i < this.m_features.Length)
				{
					GameObject gameObject = null;
					switch (this.m_features[i].Type)
					{
					case FeatureUIType.Header:
						gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_director.m_headerPrefab);
						goto IL_BF;
					case FeatureUIType.Slider:
						gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_director.m_sliderPrefab);
						goto IL_BF;
					case FeatureUIType.ClusterSlider:
						gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_director.m_sliderclusterPrefab);
						goto IL_BF;
					case FeatureUIType.Selector:
						if (charToCreate.Categories.ContainsKey(this.m_features[i].Slot))
						{
							gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_director.m_selectorPrefab);
							goto IL_BF;
						}
						break;
					case FeatureUIType.ColorSelector:
						gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_director.m_colorSelectorPrefab);
						goto IL_BF;
					default:
						goto IL_BF;
					}
					IL_145:
					i++;
					continue;
					IL_BF:
					if (gameObject == null)
					{
						Debug.LogError("Failed to initialize feature! " + this.m_features[i].Label);
						goto IL_145;
					}
					Feature component = gameObject.GetComponent<Feature>();
					if (component == null)
					{
						Debug.LogError("Unable to locate feature on " + gameObject.name + "!");
						goto IL_145;
					}
					gameObject.transform.SetParent(this.m_content, false);
					component.Initialize(this.m_director, charToCreate, this.m_features[i]);
					this.m_activeFeatures.Add(component);
					goto IL_145;
				}
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.m_content);
			}

			// Token: 0x0600592F RID: 22831 RVA: 0x001E9464 File Offset: 0x001E7664
			public void ToggleFeaturesForSex()
			{
				if (!this.m_enabled)
				{
					return;
				}
				for (int i = 0; i < this.m_activeFeatures.Count; i++)
				{
					this.m_activeFeatures[i].Toggle(this.m_director.Sex);
				}
			}

			// Token: 0x06005930 RID: 22832 RVA: 0x001E94AC File Offset: 0x001E76AC
			public void UnlockFeatures()
			{
				for (int i = 0; i < this.m_activeFeatures.Count; i++)
				{
					this.m_activeFeatures[i].Unlock();
				}
			}

			// Token: 0x06005931 RID: 22833 RVA: 0x001E94E0 File Offset: 0x001E76E0
			public void ResetFeatures(bool force)
			{
				if (!this.m_enabled && !force)
				{
					return;
				}
				for (int i = 0; i < this.m_activeFeatures.Count; i++)
				{
					this.m_activeFeatures[i].Reset();
				}
			}

			// Token: 0x06005932 RID: 22834 RVA: 0x001E9520 File Offset: 0x001E7720
			public void RandomizeFeatures()
			{
				if (!this.m_enabled)
				{
					return;
				}
				for (int i = 0; i < this.m_activeFeatures.Count; i++)
				{
					this.m_activeFeatures[i].Randomize();
				}
			}

			// Token: 0x06005933 RID: 22835 RVA: 0x001E9560 File Offset: 0x001E7760
			public void ResetFeatures(CreationDirector.CharacterToCreate charToCreate)
			{
				if (!this.m_enabled)
				{
					return;
				}
				for (int i = 0; i < this.m_activeFeatures.Count; i++)
				{
					if (this.m_activeFeatures[i].Sex == charToCreate.Sex)
					{
						this.m_activeFeatures[i].Reset();
					}
				}
			}

			// Token: 0x06005934 RID: 22836 RVA: 0x001E95B8 File Offset: 0x001E77B8
			public void RandomizeFeatures(CreationDirector.CharacterToCreate charToCreate)
			{
				if (!this.m_enabled)
				{
					return;
				}
				for (int i = 0; i < this.m_activeFeatures.Count; i++)
				{
					if (this.m_activeFeatures[i].Sex == charToCreate.Sex)
					{
						this.m_activeFeatures[i].Randomize();
					}
				}
			}

			// Token: 0x04004E6B RID: 20075
			[SerializeField]
			private bool m_enabled;

			// Token: 0x04004E6C RID: 20076
			[SerializeField]
			private RectTransform m_content;

			// Token: 0x04004E6D RID: 20077
			[SerializeField]
			private CreationDirector.FeatureSetting[] m_features;

			// Token: 0x04004E6E RID: 20078
			private readonly List<Feature> m_activeFeatures = new List<Feature>();

			// Token: 0x04004E6F RID: 20079
			private CreationDirector m_director;
		}

		// Token: 0x02000B4F RID: 2895
		public class CharacterToCreate
		{
			// Token: 0x170014D3 RID: 5331
			// (get) Token: 0x06005936 RID: 22838 RVA: 0x0007BBAF File Offset: 0x00079DAF
			public CharacterSex Sex
			{
				get
				{
					return this.m_sex;
				}
			}

			// Token: 0x170014D4 RID: 5332
			// (get) Token: 0x06005937 RID: 22839 RVA: 0x0007BBB7 File Offset: 0x00079DB7
			public DynamicCharacterAvatar DCA
			{
				get
				{
					return this.m_dca;
				}
			}

			// Token: 0x170014D5 RID: 5333
			// (get) Token: 0x06005938 RID: 22840 RVA: 0x0007BBBF File Offset: 0x00079DBF
			public Dictionary<string, DnaSetter> DNASetters
			{
				get
				{
					if (this.m_dnaSetters == null)
					{
						this.m_dnaSetters = this.m_dca.GetDNA(null);
					}
					return this.m_dnaSetters;
				}
			}

			// Token: 0x06005939 RID: 22841 RVA: 0x001E9610 File Offset: 0x001E7810
			public CharacterToCreate(CreationDirector director, CharacterSex sex)
			{
				this.m_director = director;
				this.m_sex = sex;
				this.m_dca = ((sex == CharacterSex.Male) ? this.m_director.m_maleDca : this.m_director.m_femaleDca);
				this.Categories = new Dictionary<CharacterCustomizationType, CreationDirector.CustomizationCategory>(default(CharacterCustomizationTypeComparer));
				this.InitializeCustomizations();
				this.InitializeUniqueTabs();
			}

			// Token: 0x0600593A RID: 22842 RVA: 0x001E9678 File Offset: 0x001E7878
			private void InitializeCustomizations()
			{
				foreach (CharacterCustomization characterCustomization in UMAManager.Customizations.GetCustomizations())
				{
					if (!(characterCustomization.Id == UniqueId.Empty) && !(characterCustomization.Recipe == null) && characterCustomization.Sex == this.m_sex)
					{
						if (this.Categories.ContainsKey(characterCustomization.Type))
						{
							this.Categories[characterCustomization.Type].AddValue(characterCustomization);
						}
						else
						{
							CreationDirector.CustomizationCategory value = new CreationDirector.CustomizationCategory(characterCustomization);
							this.Categories.Add(characterCustomization.Type, value);
						}
					}
				}
			}

			// Token: 0x0600593B RID: 22843 RVA: 0x001E9738 File Offset: 0x001E7938
			private void InitializeUniqueTabs()
			{
				for (int i = 0; i < this.m_director.m_uniqueTabs.Length; i++)
				{
					this.m_director.m_uniqueTabs[i].Initialize(this.m_director, this);
				}
			}

			// Token: 0x0600593C RID: 22844 RVA: 0x0007BBE1 File Offset: 0x00079DE1
			public void OnSelected()
			{
				this.m_dca.BuildCharacterEnabled = true;
				this.m_dca.Refresh(true, true, true);
				this.m_dca.hide = false;
			}

			// Token: 0x0600593D RID: 22845 RVA: 0x0007BC09 File Offset: 0x00079E09
			public void OnDeselected()
			{
				this.m_dca.hide = true;
			}

			// Token: 0x04004E70 RID: 20080
			private readonly CreationDirector m_director;

			// Token: 0x04004E71 RID: 20081
			private readonly DynamicCharacterAvatar m_dca;

			// Token: 0x04004E72 RID: 20082
			private readonly CharacterSex m_sex;

			// Token: 0x04004E73 RID: 20083
			public readonly Dictionary<CharacterCustomizationType, CreationDirector.CustomizationCategory> Categories;

			// Token: 0x04004E74 RID: 20084
			private Dictionary<string, DnaSetter> m_dnaSetters;
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006BB RID: 1723
	[CreateAssetMenu(menuName = "SoL/Spawning/Remote Spawnable Profile")]
	public class RemoteSpawnProfile : ScriptableObject
	{
		// Token: 0x06003473 RID: 13427 RVA: 0x00063EA3 File Offset: 0x000620A3
		public static string GetNextColor(bool reset)
		{
			if (reset)
			{
				RemoteSpawnProfile.m_colorIndex = 0;
			}
			string result = RemoteSpawnProfile.m_colors[RemoteSpawnProfile.m_colorIndex];
			RemoteSpawnProfile.m_colorIndex++;
			if (RemoteSpawnProfile.m_colorIndex >= RemoteSpawnProfile.m_colors.Length)
			{
				RemoteSpawnProfile.m_colorIndex = 0;
			}
			return result;
		}

		// Token: 0x17000B72 RID: 2930
		// (get) Token: 0x06003474 RID: 13428 RVA: 0x00063ED9 File Offset: 0x000620D9
		public string BaseName
		{
			get
			{
				return this.m_baseName;
			}
		}

		// Token: 0x17000B73 RID: 2931
		// (get) Token: 0x06003475 RID: 13429 RVA: 0x00063EE1 File Offset: 0x000620E1
		public RemoteSpawnProfile.RemoteSpawnProfileType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x06003476 RID: 13430 RVA: 0x00164C24 File Offset: 0x00162E24
		public bool GetRemoteNames(string baseName, string category, out string result)
		{
			result = string.Empty;
			if (!string.Equals(baseName, this.m_baseName, StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}
			if (RemoteSpawnProfile.m_tempList == null)
			{
				RemoteSpawnProfile.m_tempList = new List<string>(10);
				RemoteSpawnProfile.m_categorySet = new HashSet<int>();
			}
			if (string.IsNullOrEmpty(category))
			{
				RemoteSpawnProfile.m_tempList.Clear();
				RemoteSpawnProfile.m_categorySet.Clear();
				int num = 0;
				for (int i = 0; i < this.m_profiles.Length; i++)
				{
					if (this.m_profiles[i].CategoryIndex < 0)
					{
						RemoteSpawnProfile.m_tempList.Add(string.Concat(new string[]
						{
							"<color=",
							RemoteSpawnProfile.GetNextColor(num == 0),
							">",
							this.m_profiles[i].Aliases,
							"</color>"
						}));
						num++;
					}
					else
					{
						RemoteSpawnProfile.m_categorySet.Add(this.m_profiles[i].CategoryIndex);
					}
				}
				foreach (int num2 in RemoteSpawnProfile.m_categorySet)
				{
					string text = (num2 < this.m_categories.Length && num2 >= 0) ? this.m_categories[num2] : "INVALID";
					RemoteSpawnProfile.m_tempList.Add(string.Concat(new string[]
					{
						"<color=",
						RemoteSpawnProfile.GetNextColor(num == 0),
						">[C] ",
						text,
						"...</color>"
					}));
					num++;
				}
				result = string.Join(" | ", RemoteSpawnProfile.m_tempList);
				return true;
			}
			this.BuildCategoryDict();
			List<RemoteSpawnProfile.ProfileEntry> list;
			if (this.m_categoryDict.TryGetValue(category, out list))
			{
				RemoteSpawnProfile.m_tempList.Clear();
				for (int j = 0; j < list.Count; j++)
				{
					RemoteSpawnProfile.m_tempList.Add(string.Concat(new string[]
					{
						"<color=",
						RemoteSpawnProfile.GetNextColor(j == 0),
						">",
						list[j].Aliases,
						"</color>"
					}));
				}
				result = string.Concat(new string[]
				{
					baseName,
					".",
					category,
					": ",
					string.Join(" | ", RemoteSpawnProfile.m_tempList)
				});
				return true;
			}
			return false;
		}

		// Token: 0x06003477 RID: 13431 RVA: 0x00164E84 File Offset: 0x00163084
		private void BuildCategoryDict()
		{
			if (this.m_categoryDict == null)
			{
				this.m_categoryDict = new Dictionary<string, List<RemoteSpawnProfile.ProfileEntry>>();
				for (int i = 0; i < this.m_profiles.Length; i++)
				{
					if (this.m_profiles[i].CategoryIndex >= 0 && this.m_profiles[i].CategoryIndex < this.m_categories.Length)
					{
						string key = this.m_categories[this.m_profiles[i].CategoryIndex].ToLowerInvariant();
						List<RemoteSpawnProfile.ProfileEntry> list;
						if (this.m_categoryDict.TryGetValue(key, out list))
						{
							list.Add(this.m_profiles[i]);
						}
						else
						{
							List<RemoteSpawnProfile.ProfileEntry> value = new List<RemoteSpawnProfile.ProfileEntry>(10)
							{
								this.m_profiles[i]
							};
							this.m_categoryDict.Add(key, value);
						}
					}
				}
			}
		}

		// Token: 0x06003478 RID: 13432 RVA: 0x00164F44 File Offset: 0x00163144
		public bool TryGetSpawnProfile(string baseName, string category, string alias, out SpawnProfile profile)
		{
			profile = null;
			if (!string.Equals(baseName, this.m_baseName, StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(category))
			{
				this.BuildCategoryDict();
				List<RemoteSpawnProfile.ProfileEntry> list;
				if (this.m_categoryDict.TryGetValue(category, out list))
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].HasAlias(alias))
						{
							profile = list[i].Profile;
							break;
						}
					}
				}
			}
			else if (!string.IsNullOrEmpty(alias))
			{
				for (int j = 0; j < this.m_profiles.Length; j++)
				{
					if (this.m_profiles[j].CategoryIndex < 0 && this.m_profiles[j].HasAlias(alias))
					{
						profile = this.m_profiles[j].Profile;
						break;
					}
				}
			}
			else
			{
				profile = this.m_defaultProfile;
			}
			return profile != null;
		}

		// Token: 0x06003479 RID: 13433 RVA: 0x0016501C File Offset: 0x0016321C
		private void OnValidate()
		{
			if (this.m_profiles != null)
			{
				for (int i = 0; i < this.m_profiles.Length; i++)
				{
					this.m_profiles[i].SetController(this);
				}
			}
		}

		// Token: 0x0600347A RID: 13434 RVA: 0x00063EE9 File Offset: 0x000620E9
		private IEnumerable GetSpawnProfiles()
		{
			return SolOdinUtilities.GetDropdownItems<SpawnProfile>();
		}

		// Token: 0x04003273 RID: 12915
		public const string kResultSeparator = " | ";

		// Token: 0x04003274 RID: 12916
		private static List<string> m_tempList = null;

		// Token: 0x04003275 RID: 12917
		private static HashSet<int> m_categorySet = null;

		// Token: 0x04003276 RID: 12918
		private static int m_colorIndex = 0;

		// Token: 0x04003277 RID: 12919
		private static string[] m_colors = new string[]
		{
			"#9FB7BF",
			"#F2EBC5",
			"#BF9C91"
		};

		// Token: 0x04003278 RID: 12920
		private Dictionary<string, List<RemoteSpawnProfile.ProfileEntry>> m_categoryDict;

		// Token: 0x04003279 RID: 12921
		[SerializeField]
		private RemoteSpawnProfile.RemoteSpawnProfileType m_type;

		// Token: 0x0400327A RID: 12922
		[SerializeField]
		private string m_baseName;

		// Token: 0x0400327B RID: 12923
		[SerializeField]
		private SpawnProfile m_defaultProfile;

		// Token: 0x0400327C RID: 12924
		[SerializeField]
		private string[] m_categories;

		// Token: 0x0400327D RID: 12925
		[SerializeField]
		private RemoteSpawnProfile.ProfileEntry[] m_profiles;

		// Token: 0x020006BC RID: 1724
		public enum RemoteSpawnProfileType
		{
			// Token: 0x0400327F RID: 12927
			Npc,
			// Token: 0x04003280 RID: 12928
			Node
		}

		// Token: 0x020006BD RID: 1725
		[Serializable]
		private class ProfileEntry
		{
			// Token: 0x0600347D RID: 13437 RVA: 0x00063F27 File Offset: 0x00062127
			private IEnumerable GetSpawnProfiles()
			{
				if (!(this.m_controller != null))
				{
					return null;
				}
				return this.m_controller.GetSpawnProfiles();
			}

			// Token: 0x0600347E RID: 13438 RVA: 0x00165054 File Offset: 0x00163254
			private IEnumerable GetCategories()
			{
				List<ValueDropdownItem> list = new List<ValueDropdownItem>((this.m_controller == null || this.m_controller.m_categories == null) ? 1 : (this.m_controller.m_categories.Length + 1))
				{
					new ValueDropdownItem("None", -1)
				};
				if (this.m_controller != null && this.m_controller.m_categories != null)
				{
					for (int i = 0; i < this.m_controller.m_categories.Length; i++)
					{
						list.Add(new ValueDropdownItem(this.m_controller.m_categories[i], i));
					}
				}
				return list;
			}

			// Token: 0x0600347F RID: 13439 RVA: 0x00063F44 File Offset: 0x00062144
			internal void SetController(RemoteSpawnProfile controller)
			{
				this.m_controller = controller;
				if (this.m_controller == null || this.m_categoryIndex >= this.m_controller.m_categories.Length)
				{
					this.m_categoryIndex = -1;
				}
			}

			// Token: 0x17000B74 RID: 2932
			// (get) Token: 0x06003480 RID: 13440 RVA: 0x00063F77 File Offset: 0x00062177
			public int CategoryIndex
			{
				get
				{
					return this.m_categoryIndex;
				}
			}

			// Token: 0x17000B75 RID: 2933
			// (get) Token: 0x06003481 RID: 13441 RVA: 0x00063F7F File Offset: 0x0006217F
			public string Aliases
			{
				get
				{
					return this.m_aliases;
				}
			}

			// Token: 0x17000B76 RID: 2934
			// (get) Token: 0x06003482 RID: 13442 RVA: 0x00063F87 File Offset: 0x00062187
			public SpawnProfile Profile
			{
				get
				{
					return this.m_profile;
				}
			}

			// Token: 0x06003483 RID: 13443 RVA: 0x001650FC File Offset: 0x001632FC
			public bool HasAlias(string alias)
			{
				if (this.m_aliasList == null)
				{
					char[] trimChars = new char[]
					{
						' '
					};
					this.m_aliasList = this.m_aliases.Split(',', StringSplitOptions.None).ToList<string>();
					for (int i = 0; i < this.m_aliasList.Count; i++)
					{
						this.m_aliasList[i] = this.m_aliasList[i].TrimStart(trimChars).TrimEnd(trimChars);
					}
				}
				return this.m_aliasList.Contains(alias);
			}

			// Token: 0x04003281 RID: 12929
			[HideInInspector]
			[SerializeField]
			private RemoteSpawnProfile m_controller;

			// Token: 0x04003282 RID: 12930
			[SerializeField]
			private int m_categoryIndex = -1;

			// Token: 0x04003283 RID: 12931
			[SerializeField]
			private SpawnProfile m_profile;

			// Token: 0x04003284 RID: 12932
			[Tooltip("Comma separated list")]
			[SerializeField]
			private string m_aliases;

			// Token: 0x04003285 RID: 12933
			[NonSerialized]
			private List<string> m_aliasList;
		}
	}
}

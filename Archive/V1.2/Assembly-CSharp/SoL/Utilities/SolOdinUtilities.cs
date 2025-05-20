using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002E1 RID: 737
	public static class SolOdinUtilities
	{
		// Token: 0x0600152F RID: 5423 RVA: 0x00049FFA File Offset: 0x000481FA
		public static IEnumerable GetDropdownItems<T>(string filter, string[] searchInFolders, Func<T, bool> condition) where T : UnityEngine.Object
		{
			return null;
		}

		// Token: 0x06001530 RID: 5424 RVA: 0x00050DE1 File Offset: 0x0004EFE1
		public static IEnumerable GetDropdownItems<T>() where T : UnityEngine.Object
		{
			return SolOdinUtilities.GetDropdownItems<T>(null);
		}

		// Token: 0x06001531 RID: 5425 RVA: 0x00049FFA File Offset: 0x000481FA
		public static IEnumerable GetDropdownItems<T>(Func<T, bool> condition) where T : UnityEngine.Object
		{
			return null;
		}

		// Token: 0x06001532 RID: 5426 RVA: 0x000FC180 File Offset: 0x000FA380
		public static IEnumerable GetColorValues()
		{
			List<ValueDropdownItem> list = new List<ValueDropdownItem>();
			foreach (object obj in Enum.GetValues(typeof(Colors.ColorName)))
			{
				Colors.ColorName color = (Colors.ColorName)obj;
				list.Add(new ValueDropdownItem(color.ToString(), color.GetColor()));
			}
			return list;
		}
	}
}

using System;
using SoL.Game.Objects.Archetypes;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000598 RID: 1432
	[CreateAssetMenu(menuName = "SoL/Collections/Male Female Sprites", order = 5)]
	public class MaleFemaleSpriteCollection : BaseArchetype
	{
		// Token: 0x06002C9E RID: 11422 RVA: 0x0005EF05 File Offset: 0x0005D105
		public int GetCount(CharacterSex sex)
		{
			if (sex == CharacterSex.Male)
			{
				return this.m_male.Length;
			}
			if (sex != CharacterSex.Female)
			{
				return 0;
			}
			return this.m_female.Length;
		}

		// Token: 0x06002C9F RID: 11423 RVA: 0x0014A8F8 File Offset: 0x00148AF8
		public Sprite GetIndex(CharacterSex sex, int index)
		{
			Sprite[] array;
			if (sex != CharacterSex.Male)
			{
				if (sex != CharacterSex.Female)
				{
					return null;
				}
				array = this.m_female;
			}
			else
			{
				array = this.m_male;
			}
			if (index >= array.Length)
			{
				throw new IndexOutOfRangeException(string.Concat(new string[]
				{
					sex.ToString(),
					" array is of length ",
					array.Length.ToString(),
					" but asking for index ",
					index.ToString(),
					"!"
				}));
			}
			return array[index];
		}

		// Token: 0x04002C5C RID: 11356
		[SerializeField]
		private Sprite[] m_male;

		// Token: 0x04002C5D RID: 11357
		[SerializeField]
		private Sprite[] m_female;
	}
}

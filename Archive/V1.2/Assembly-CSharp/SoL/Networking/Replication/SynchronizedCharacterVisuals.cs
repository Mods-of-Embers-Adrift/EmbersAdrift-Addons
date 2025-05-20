using System;
using System.Collections.Generic;
using NetStack.Serialization;
using SoL.Networking.Database;

namespace SoL.Networking.Replication
{
	// Token: 0x020004A6 RID: 1190
	public class SynchronizedCharacterVisuals : SynchronizedVariable<CharacterVisuals>
	{
		// Token: 0x0600212C RID: 8492 RVA: 0x00122A2C File Offset: 0x00120C2C
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			buffer.AddEnum(base.Value.Sex);
			buffer.AddEnum(base.Value.BuildType);
			bool flag = base.Value.Dna != null && base.Value.Dna.Count > 0;
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddInt(base.Value.Dna.Count);
				foreach (KeyValuePair<string, float> keyValuePair in base.Value.Dna)
				{
					buffer.AddString(keyValuePair.Key);
					buffer.AddFloat(keyValuePair.Value);
				}
			}
			buffer.AddInt(base.Value.SharedColors.Count);
			foreach (KeyValuePair<CharacterColorType, string> keyValuePair2 in base.Value.SharedColors)
			{
				buffer.AddEnum(keyValuePair2.Key);
				buffer.AddString(keyValuePair2.Value);
			}
			buffer.AddInt(base.Value.CustomizedSlots.Count);
			for (int i = 0; i < base.Value.CustomizedSlots.Count; i++)
			{
				buffer.AddUniqueId(base.Value.CustomizedSlots[i]);
			}
			return buffer;
		}

		// Token: 0x0600212D RID: 8493 RVA: 0x00122BC8 File Offset: 0x00120DC8
		protected override CharacterVisuals ReadDataInternal(BitBuffer buffer)
		{
			CharacterVisuals characterVisuals = new CharacterVisuals();
			characterVisuals.Sex = buffer.ReadEnum<CharacterSex>();
			characterVisuals.BuildType = buffer.ReadEnum<CharacterBuildType>();
			if (buffer.ReadBool())
			{
				int num = buffer.ReadInt();
				characterVisuals.Dna = new Dictionary<string, float>(num);
				for (int i = 0; i < num; i++)
				{
					string key = buffer.ReadString();
					float value = buffer.ReadFloat();
					characterVisuals.Dna.Add(key, value);
				}
			}
			int num2 = buffer.ReadInt();
			if (characterVisuals.SharedColors == null)
			{
				characterVisuals.SharedColors = new Dictionary<CharacterColorType, string>(num2, default(CharacterColorTypeComparer));
			}
			else
			{
				characterVisuals.SharedColors.Clear();
			}
			for (int j = 0; j < num2; j++)
			{
				CharacterColorType key2 = buffer.ReadEnum<CharacterColorType>();
				string value2 = buffer.ReadString();
				characterVisuals.SharedColors.Add(key2, value2);
			}
			int num3 = buffer.ReadInt();
			if (characterVisuals.CustomizedSlots == null)
			{
				characterVisuals.CustomizedSlots = new List<UniqueId>(num3);
			}
			else
			{
				characterVisuals.CustomizedSlots.Clear();
			}
			for (int k = 0; k < num3; k++)
			{
				characterVisuals.CustomizedSlots.Add(buffer.ReadUniqueId());
			}
			return characterVisuals;
		}
	}
}

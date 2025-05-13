using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace SoL.Game.Player
{
	// Token: 0x020007EC RID: 2028
	public struct DebugLocation
	{
		// Token: 0x17000D80 RID: 3456
		// (get) Token: 0x06003AF8 RID: 15096 RVA: 0x00067F41 File Offset: 0x00066141
		// (set) Token: 0x06003AF9 RID: 15097 RVA: 0x00067F49 File Offset: 0x00066149
		public bool Valid { readonly get; private set; }

		// Token: 0x17000D81 RID: 3457
		// (get) Token: 0x06003AFA RID: 15098 RVA: 0x00067F52 File Offset: 0x00066152
		// (set) Token: 0x06003AFB RID: 15099 RVA: 0x00067F5A File Offset: 0x0006615A
		public int ZoneId { readonly get; private set; }

		// Token: 0x17000D82 RID: 3458
		// (get) Token: 0x06003AFC RID: 15100 RVA: 0x00067F63 File Offset: 0x00066163
		// (set) Token: 0x06003AFD RID: 15101 RVA: 0x00067F6B File Offset: 0x0006616B
		public Vector3 Position { readonly get; private set; }

		// Token: 0x17000D83 RID: 3459
		// (get) Token: 0x06003AFE RID: 15102 RVA: 0x00067F74 File Offset: 0x00066174
		// (set) Token: 0x06003AFF RID: 15103 RVA: 0x00067F7C File Offset: 0x0006617C
		public Quaternion Rotation { readonly get; private set; }

		// Token: 0x17000D84 RID: 3460
		// (get) Token: 0x06003B00 RID: 15104 RVA: 0x00067F85 File Offset: 0x00066185
		// (set) Token: 0x06003B01 RID: 15105 RVA: 0x00067F8D File Offset: 0x0006618D
		public string DebugString { readonly get; private set; }

		// Token: 0x06003B02 RID: 15106 RVA: 0x00179B38 File Offset: 0x00177D38
		public DebugLocation(int zoneId, GameObject obj)
		{
			this.ZoneId = zoneId;
			this.Position = obj.transform.position;
			Vector3 eulerAngles = obj.transform.eulerAngles;
			this.Rotation = Quaternion.Euler(new Vector3(0f, eulerAngles.y, 0f));
			this.m_fourVector = new Vector4(this.Position.x, this.Position.y, this.Position.z, eulerAngles.y);
			string s = this.ZoneId.ToString() + '|'.ToString() + this.m_fourVector.ToString("0.###");
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			this.DebugString = Convert.ToBase64String(bytes);
			this.Valid = true;
		}

		// Token: 0x06003B03 RID: 15107 RVA: 0x00179C0C File Offset: 0x00177E0C
		public DebugLocation(int zoneId, Vector3 pos, Quaternion rot)
		{
			this.ZoneId = zoneId;
			this.Position = pos;
			this.Rotation = rot;
			this.m_fourVector = new Vector4(this.Position.x, this.Position.y, this.Position.z, rot.eulerAngles.y);
			string s = this.ZoneId.ToString() + '|'.ToString() + this.m_fourVector.ToString("0.###");
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			this.DebugString = Convert.ToBase64String(bytes);
			this.Valid = true;
		}

		// Token: 0x06003B04 RID: 15108 RVA: 0x00179CB4 File Offset: 0x00177EB4
		public DebugLocation(string value)
		{
			this.ZoneId = 0;
			this.Position = Vector3.zero;
			this.Rotation = Quaternion.identity;
			this.m_fourVector = Vector4.zero;
			this.Valid = false;
			this.DebugString = value;
			string[] array = null;
			try
			{
				byte[] bytes = Convert.FromBase64String(value);
				array = Encoding.UTF8.GetString(bytes).Split('|', StringSplitOptions.None);
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Invalid debug string!  " + ex.ToString());
				return;
			}
			if (array.Length != 2)
			{
				Debug.LogWarning("Invalid debug string parts!  Has " + array.Length.ToString() + " expected 2!");
				return;
			}
			string text = array[0];
			ZoneId zoneId;
			if (!Enum.TryParse<ZoneId>(text, out zoneId))
			{
				Debug.LogWarning("Invalid debug zoneId! " + text);
				return;
			}
			this.ZoneId = (int)zoneId;
			string text2 = array[1];
			string[] array2 = text2.Substring(1, text2.Length - 2).Split(',', StringSplitOptions.None);
			this.m_fourVector = new Vector4(float.Parse(array2[0], CultureInfo.InvariantCulture), float.Parse(array2[1], CultureInfo.InvariantCulture), float.Parse(array2[2], CultureInfo.InvariantCulture), float.Parse(array2[3], CultureInfo.InvariantCulture));
			this.Position = new Vector3(this.m_fourVector.x, this.m_fourVector.y, this.m_fourVector.z);
			this.Rotation = Quaternion.Euler(new Vector3(0f, this.m_fourVector.w, 0f));
			this.Valid = true;
		}

		// Token: 0x0400397C RID: 14716
		private const char kSeparator = '|';

		// Token: 0x04003982 RID: 14722
		private Vector4 m_fourVector;
	}
}

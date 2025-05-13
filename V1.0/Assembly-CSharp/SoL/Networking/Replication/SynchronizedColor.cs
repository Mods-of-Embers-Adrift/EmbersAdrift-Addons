using System;
using NetStack.Serialization;
using UnityEngine;

namespace SoL.Networking.Replication
{
	// Token: 0x020004A2 RID: 1186
	public class SynchronizedColor : SynchronizedVariable<Color>
	{
		// Token: 0x0600211C RID: 8476 RVA: 0x0005806E File Offset: 0x0005626E
		public SynchronizedColor()
		{
		}

		// Token: 0x0600211D RID: 8477 RVA: 0x00058076 File Offset: 0x00056276
		public SynchronizedColor(Color initial) : base(initial)
		{
		}

		// Token: 0x0600211E RID: 8478 RVA: 0x001228E8 File Offset: 0x00120AE8
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			buffer.AddFloat(base.Value.r);
			buffer.AddFloat(base.Value.g);
			buffer.AddFloat(base.Value.b);
			buffer.AddFloat(base.Value.a);
			return buffer;
		}

		// Token: 0x0600211F RID: 8479 RVA: 0x00122940 File Offset: 0x00120B40
		protected override Color ReadDataInternal(BitBuffer buffer)
		{
			float r = buffer.ReadFloat();
			float g = buffer.ReadFloat();
			float b = buffer.ReadFloat();
			float a = buffer.ReadFloat();
			return new Color(r, g, b, a);
		}
	}
}

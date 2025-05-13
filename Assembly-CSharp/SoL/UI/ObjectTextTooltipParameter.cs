using System;

namespace SoL.UI
{
	// Token: 0x02000388 RID: 904
	public struct ObjectTextTooltipParameter : ITooltipParameter, IEquatable<ObjectTextTooltipParameter>
	{
		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x060018C3 RID: 6339 RVA: 0x00053503 File Offset: 0x00051703
		public TooltipType Type
		{
			get
			{
				if (!this.IsOptionsMenu)
				{
					return TooltipType.ObjectText;
				}
				return TooltipType.ObjectTextOptionsMenu;
			}
		}

		// Token: 0x060018C4 RID: 6340 RVA: 0x00053510 File Offset: 0x00051710
		public ObjectTextTooltipParameter(object obj, string txt, bool isOptionsMenu = false)
		{
			this.Obj = obj;
			this.Text = txt;
			this.IsOptionsMenu = isOptionsMenu;
		}

		// Token: 0x060018C5 RID: 6341 RVA: 0x00053527 File Offset: 0x00051727
		public bool Equals(ObjectTextTooltipParameter other)
		{
			return object.Equals(this.Obj, other.Obj);
		}

		// Token: 0x060018C6 RID: 6342 RVA: 0x001053C4 File Offset: 0x001035C4
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is ObjectTextTooltipParameter)
			{
				ObjectTextTooltipParameter other = (ObjectTextTooltipParameter)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x060018C7 RID: 6343 RVA: 0x0005353A File Offset: 0x0005173A
		public override int GetHashCode()
		{
			if (this.Obj == null)
			{
				return 0;
			}
			return this.Obj.GetHashCode();
		}

		// Token: 0x04001FDA RID: 8154
		private readonly object Obj;

		// Token: 0x04001FDB RID: 8155
		public string Text;

		// Token: 0x04001FDC RID: 8156
		public readonly bool IsOptionsMenu;
	}
}

using System;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001CB RID: 459
	public abstract class BGCcWithCursorObject : BGCcWithCursor
	{
		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x0600107F RID: 4223 RVA: 0x0004DC9E File Offset: 0x0004BE9E
		// (set) Token: 0x06001080 RID: 4224 RVA: 0x0004DCA6 File Offset: 0x0004BEA6
		public Transform ObjectToManipulate
		{
			get
			{
				return this.objectToManipulate;
			}
			set
			{
				base.ParamChanged<Transform>(ref this.objectToManipulate, value);
			}
		}

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x06001081 RID: 4225 RVA: 0x0004DCB6 File Offset: 0x0004BEB6
		public override string Error
		{
			get
			{
				if (!(this.objectToManipulate == null))
				{
					return null;
				}
				return "Object To Manipulate is not set.";
			}
		}

		// Token: 0x04000DCB RID: 3531
		private const string ErrorObjectNotSet = "Object To Manipulate is not set.";

		// Token: 0x04000DCC RID: 3532
		[SerializeField]
		[Tooltip("Object to manipulate.\r\n")]
		private Transform objectToManipulate;
	}
}

using System;
using Rewired;
using UnityEngine;

namespace SoL
{
	// Token: 0x02000215 RID: 533
	public class BindingEventData
	{
		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x060011E6 RID: 4582 RVA: 0x0004EC38 File Offset: 0x0004CE38
		// (set) Token: 0x060011E7 RID: 4583 RVA: 0x0004EC40 File Offset: 0x0004CE40
		public int ActionId { get; set; }

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x060011E8 RID: 4584 RVA: 0x0004EC49 File Offset: 0x0004CE49
		// (set) Token: 0x060011E9 RID: 4585 RVA: 0x0004EC51 File Offset: 0x0004CE51
		public Binding Binding { get; set; }

		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x060011EA RID: 4586 RVA: 0x0004EC5A File Offset: 0x0004CE5A
		// (set) Token: 0x060011EB RID: 4587 RVA: 0x0004EC62 File Offset: 0x0004CE62
		public int BindingIndex { get; set; }

		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x060011EC RID: 4588 RVA: 0x0004EC6B File Offset: 0x0004CE6B
		// (set) Token: 0x060011ED RID: 4589 RVA: 0x0004EC73 File Offset: 0x0004CE73
		public ActionElementMap RuntimeMap { get; set; }

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x060011EE RID: 4590 RVA: 0x0004EC7C File Offset: 0x0004CE7C
		// (set) Token: 0x060011EF RID: 4591 RVA: 0x0004EC84 File Offset: 0x0004CE84
		public KeyCode KeyCode { get; set; }

		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x060011F0 RID: 4592 RVA: 0x0004EC8D File Offset: 0x0004CE8D
		// (set) Token: 0x060011F1 RID: 4593 RVA: 0x0004EC95 File Offset: 0x0004CE95
		public ModifierKeyFlags ModifierKeyFlags { get; set; }
	}
}

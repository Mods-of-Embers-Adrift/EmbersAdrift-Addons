using System;
using System.Text;
using Rewired.UI;
using UnityEngine.EventSystems;

namespace Rewired.Integration.UnityUI
{
	// Token: 0x0200006F RID: 111
	public class PlayerPointerEventData : PointerEventData
	{
		// Token: 0x17000239 RID: 569
		// (get) Token: 0x06000466 RID: 1126 RVA: 0x00046117 File Offset: 0x00044317
		// (set) Token: 0x06000467 RID: 1127 RVA: 0x0004611F File Offset: 0x0004431F
		public int playerId { get; set; }

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x06000468 RID: 1128 RVA: 0x00046128 File Offset: 0x00044328
		// (set) Token: 0x06000469 RID: 1129 RVA: 0x00046130 File Offset: 0x00044330
		public int inputSourceIndex { get; set; }

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x0600046A RID: 1130 RVA: 0x00046139 File Offset: 0x00044339
		// (set) Token: 0x0600046B RID: 1131 RVA: 0x00046141 File Offset: 0x00044341
		public IMouseInputSource mouseSource { get; set; }

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x0600046C RID: 1132 RVA: 0x0004614A File Offset: 0x0004434A
		// (set) Token: 0x0600046D RID: 1133 RVA: 0x00046152 File Offset: 0x00044352
		public ITouchInputSource touchSource { get; set; }

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x0600046E RID: 1134 RVA: 0x0004615B File Offset: 0x0004435B
		// (set) Token: 0x0600046F RID: 1135 RVA: 0x00046163 File Offset: 0x00044363
		public PointerEventType sourceType { get; set; }

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x06000470 RID: 1136 RVA: 0x0004616C File Offset: 0x0004436C
		// (set) Token: 0x06000471 RID: 1137 RVA: 0x00046174 File Offset: 0x00044374
		public int buttonIndex { get; set; }

		// Token: 0x06000472 RID: 1138 RVA: 0x0004617D File Offset: 0x0004437D
		public PlayerPointerEventData(EventSystem eventSystem) : base(eventSystem)
		{
			this.playerId = -1;
			this.inputSourceIndex = -1;
			this.buttonIndex = -1;
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x0009C6C0 File Offset: 0x0009A8C0
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<b>Player Id</b>: " + this.playerId.ToString());
			string str = "<b>Mouse Source</b>: ";
			IMouseInputSource mouseSource = this.mouseSource;
			stringBuilder.AppendLine(str + ((mouseSource != null) ? mouseSource.ToString() : null));
			stringBuilder.AppendLine("<b>Input Source Index</b>: " + this.inputSourceIndex.ToString());
			string str2 = "<b>Touch Source/b>: ";
			ITouchInputSource touchSource = this.touchSource;
			stringBuilder.AppendLine(str2 + ((touchSource != null) ? touchSource.ToString() : null));
			stringBuilder.AppendLine("<b>Source Type</b>: " + this.sourceType.ToString());
			stringBuilder.AppendLine("<b>Button Index</b>: " + this.buttonIndex.ToString());
			stringBuilder.Append(base.ToString());
			return stringBuilder.ToString();
		}
	}
}

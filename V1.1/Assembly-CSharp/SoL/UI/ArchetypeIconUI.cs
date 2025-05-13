using System;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x02000344 RID: 836
	public class ArchetypeIconUI : MonoBehaviour
	{
		// Token: 0x060016D8 RID: 5848 RVA: 0x00101534 File Offset: 0x000FF734
		public void SetIcon(BaseArchetype archetype, Color? color = null)
		{
			if (archetype == null)
			{
				this.m_rectangular.Image.overrideSprite = null;
				this.m_circular.Image.overrideSprite = null;
				this.ToggleIconRefs(true);
				return;
			}
			bool isRectangular = true;
			ArchetypeTooltip.ArchetypeIconReference archetypeIconReference = this.m_rectangular;
			ArchetypeTooltip.ArchetypeIconReference archetypeIconReference2 = this.m_circular;
			if (archetype.IconShape == ArchetypeIconType.Circle)
			{
				isRectangular = false;
				archetypeIconReference = this.m_circular;
				archetypeIconReference2 = this.m_rectangular;
			}
			WindowComponentStylizer stylizer = archetypeIconReference.Stylizer;
			archetypeIconReference.Image.overrideSprite = archetype.Icon;
			archetypeIconReference.Image.color = ((color != null) ? color.Value : Color.white);
			archetypeIconReference2.Image.overrideSprite = null;
			this.ToggleIconRefs(isRectangular);
			if (stylizer != null)
			{
				if (archetype == null || !archetype.ChangeFrameColor)
				{
					stylizer.ResetFrameColor();
					return;
				}
				if (archetype != null && archetype.ChangeFrameColor)
				{
					stylizer.SetFrameColor(archetype.FrameColor);
				}
			}
		}

		// Token: 0x060016D9 RID: 5849 RVA: 0x00101628 File Offset: 0x000FF828
		private void ToggleIconRefs(bool isRectangular)
		{
			if (isRectangular)
			{
				if (this.m_circular.Stylizer.gameObject.activeSelf)
				{
					this.m_circular.Stylizer.gameObject.SetActive(false);
				}
				if (!this.m_rectangular.Stylizer.gameObject.activeSelf)
				{
					this.m_rectangular.Stylizer.gameObject.SetActive(true);
					return;
				}
			}
			else
			{
				if (!this.m_circular.Stylizer.gameObject.activeSelf)
				{
					this.m_circular.Stylizer.gameObject.SetActive(true);
				}
				if (this.m_rectangular.Stylizer.gameObject.activeSelf)
				{
					this.m_rectangular.Stylizer.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x04001EAA RID: 7850
		[SerializeField]
		private ArchetypeTooltip.ArchetypeIconReference m_rectangular;

		// Token: 0x04001EAB RID: 7851
		[SerializeField]
		private ArchetypeTooltip.ArchetypeIconReference m_circular;
	}
}

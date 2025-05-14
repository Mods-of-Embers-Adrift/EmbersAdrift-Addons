using System;
using SoL.Game.Animation;
using SoL.Game.Objects.Archetypes;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Game.NPCs.Others
{
	// Token: 0x02000836 RID: 2102
	public class OtherVisibilityTransition : GameEntityComponent
	{
		// Token: 0x06003CD3 RID: 15571 RVA: 0x00181248 File Offset: 0x0017F448
		private void Start()
		{
			if (base.GameEntity == null)
			{
				base.enabled = false;
				return;
			}
			this.m_visibility = base.GameEntity.gameObject.GetComponent<OtherVisibility>();
			if (this.m_visibility == null)
			{
				base.enabled = false;
				return;
			}
			this.PlayHideTransition(this.m_visibility.IsHidden.Value, false);
			this.m_visibility.IsHidden.Changed += this.IsHiddenOnChanged;
		}

		// Token: 0x06003CD4 RID: 15572 RVA: 0x00069375 File Offset: 0x00067575
		private void OnDestroy()
		{
			if (this.m_visibility)
			{
				this.m_visibility.IsHidden.Changed -= this.IsHiddenOnChanged;
			}
		}

		// Token: 0x06003CD5 RID: 15573 RVA: 0x000693A0 File Offset: 0x000675A0
		private void Update()
		{
			if (this.m_hideTime != null && Time.time >= this.m_hideTime.Value)
			{
				this.m_hideTime = null;
				this.ToggleVisuals(false);
				this.ToggleParticleSystems(false);
			}
		}

		// Token: 0x06003CD6 RID: 15574 RVA: 0x000693DB File Offset: 0x000675DB
		private void IsHiddenOnChanged(bool obj)
		{
			this.PlayHideTransition(obj, true);
		}

		// Token: 0x06003CD7 RID: 15575 RVA: 0x001812CC File Offset: 0x0017F4CC
		private void PlayHideTransition(bool isHidden, bool playTransition)
		{
			if (playTransition && isHidden && this.m_hideEmote && base.GameEntity.AnimancerController != null)
			{
				AnimationSequenceWithOverride animationSequence = this.m_hideEmote.GetAnimationSequence(CharacterSex.Male);
				base.GameEntity.AnimancerController.StartSequence(animationSequence, null);
				this.ToggleParticleSystems(true);
				this.m_hideTime = new float?(Time.time + this.m_hideDelay);
				return;
			}
			if (isHidden)
			{
				this.m_hideTime = null;
				this.ToggleVisuals(false);
				this.ToggleParticleSystems(false);
				return;
			}
			this.m_hideTime = null;
			this.ToggleVisuals(true);
			this.ToggleParticleSystems(false);
		}

		// Token: 0x06003CD8 RID: 15576 RVA: 0x000693E5 File Offset: 0x000675E5
		private void ToggleVisuals(bool isVisible)
		{
			base.gameObject.SetActive(isVisible);
		}

		// Token: 0x06003CD9 RID: 15577 RVA: 0x00181370 File Offset: 0x0017F570
		private void ToggleParticleSystems(bool isVisible)
		{
			for (int i = 0; i < this.m_castParticleSystems.Length; i++)
			{
				if (!isVisible)
				{
					this.m_castParticleSystems[i].Stop();
				}
				this.m_castParticleSystems[i].gameObject.SetActive(isVisible);
				if (isVisible)
				{
					this.m_castParticleSystems[i].Play();
				}
			}
		}

		// Token: 0x04003BAA RID: 15274
		private OtherVisibility m_visibility;

		// Token: 0x04003BAB RID: 15275
		private float? m_hideTime;

		// Token: 0x04003BAC RID: 15276
		[SerializeField]
		private float m_hideDelay;

		// Token: 0x04003BAD RID: 15277
		[SerializeField]
		private Emote m_hideEmote;

		// Token: 0x04003BAE RID: 15278
		[SerializeField]
		private ParticleSystem[] m_castParticleSystems;
	}
}

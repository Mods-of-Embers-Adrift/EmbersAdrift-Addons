using System;
using Cysharp.Text;
using SoL.Game.UI;
using TMPro;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200059C RID: 1436
	public class NameplateControllerStatBarUI : MonoBehaviour
	{
		// Token: 0x17000978 RID: 2424
		// (get) Token: 0x06002CC2 RID: 11458 RVA: 0x0005F07E File Offset: 0x0005D27E
		public StatBarType BarType
		{
			get
			{
				return this.m_statBarType;
			}
		}

		// Token: 0x17000979 RID: 2425
		// (get) Token: 0x06002CC3 RID: 11459 RVA: 0x0005F086 File Offset: 0x0005D286
		public TextMeshProUGUI LeftText
		{
			get
			{
				return this.m_leftText;
			}
		}

		// Token: 0x1700097A RID: 2426
		// (get) Token: 0x06002CC4 RID: 11460 RVA: 0x0005F08E File Offset: 0x0005D28E
		public TextMeshProUGUI CenterText
		{
			get
			{
				return this.m_centerText;
			}
		}

		// Token: 0x1700097B RID: 2427
		// (get) Token: 0x06002CC5 RID: 11461 RVA: 0x0005F096 File Offset: 0x0005D296
		public TextMeshProUGUI RightText
		{
			get
			{
				return this.m_rightText;
			}
		}

		// Token: 0x06002CC6 RID: 11462 RVA: 0x0005F09E File Offset: 0x0005D29E
		private void Awake()
		{
			this.NullifyText(this.m_leftText);
			this.NullifyText(this.m_centerText);
			this.NullifyText(this.m_rightText);
		}

		// Token: 0x06002CC7 RID: 11463 RVA: 0x0005F0C4 File Offset: 0x0005D2C4
		private void NullifyText(TMP_Text tmp)
		{
			if (tmp != null)
			{
				tmp.SetText(NameplateControllerStatBarUI.kEmptyCharArray);
			}
		}

		// Token: 0x06002CC8 RID: 11464 RVA: 0x0014B15C File Offset: 0x0014935C
		private void UpdateFills(float valueFill, float woundFill)
		{
			if (this.m_valueFill != null)
			{
				float num = (this.m_valueFill.fillAmount > valueFill) ? 4f : 2f;
				this.m_valueFill.fillAmount = Mathf.Lerp(this.m_valueFill.fillAmount, valueFill, Time.deltaTime * num);
			}
			if (this.m_woundFill != null)
			{
				float num2 = (this.m_woundFill.fillAmount > valueFill) ? 4f : 2f;
				this.m_woundFill.fillAmount = Mathf.Lerp(this.m_woundFill.fillAmount, woundFill, Time.deltaTime * num2);
			}
		}

		// Token: 0x06002CC9 RID: 11465 RVA: 0x0005F0DA File Offset: 0x0005D2DA
		public void SetFill(float valueFill, float woundFill)
		{
			if (this.m_valueFill != null)
			{
				this.m_valueFill.fillAmount = valueFill;
			}
			if (this.m_woundFill != null)
			{
				this.m_woundFill.fillAmount = woundFill;
			}
		}

		// Token: 0x06002CCA RID: 11466 RVA: 0x0005F104 File Offset: 0x0005D304
		public void ResetLastValues()
		{
			this.m_lastHealth = -100;
			this.m_lastMaxHealth = -100;
			this.m_lastHealthWound = -100;
			this.m_lastStamina = -100;
			this.m_lastStaminaWound = -100;
			this.m_lastArmorClass = -100;
		}

		// Token: 0x06002CCB RID: 11467 RVA: 0x0014B1F8 File Offset: 0x001493F8
		public void UpdateStatBar(GameEntity entity, bool immediate, bool force)
		{
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			if (force)
			{
				this.NullifyText(this.m_leftText);
				this.NullifyText(this.m_centerText);
				this.NullifyText(this.m_rightText);
			}
			float valueFill = 0f;
			float woundFill = 0f;
			GameEntityType type = entity.Type;
			if (type != GameEntityType.Player)
			{
				if (type == GameEntityType.Npc)
				{
					this.UpdateNpcStatBar(entity, force, out valueFill, out woundFill);
				}
			}
			else
			{
				this.UpdatePlayerStatBar(entity, force, out valueFill, out woundFill);
			}
			if (immediate)
			{
				this.SetFill(valueFill, woundFill);
				return;
			}
			this.UpdateFills(valueFill, woundFill);
		}

		// Token: 0x06002CCC RID: 11468 RVA: 0x0014B288 File Offset: 0x00149488
		private void UpdatePlayerStatBar(GameEntity entity, bool force, out float valueFill, out float woundFill)
		{
			valueFill = 0f;
			woundFill = 0f;
			switch (this.m_statBarType)
			{
			case StatBarType.Health:
			{
				valueFill = entity.Vitals.Health / (float)entity.Vitals.MaxHealth;
				woundFill = entity.Vitals.HealthWound / 100f;
				int num = Mathf.FloorToInt(entity.Vitals.Health);
				int maxHealth = entity.Vitals.MaxHealth;
				int num2 = Mathf.Clamp(Mathf.FloorToInt(entity.Vitals.HealthWound), 0, 100);
				if (this.m_leftText && (force || num != this.m_lastHealth || maxHealth != this.m_lastMaxHealth))
				{
					this.m_leftText.SetTextFormat("{0}/{1}", num, maxHealth);
					this.m_lastHealth = num;
					this.m_lastMaxHealth = maxHealth;
				}
				if (this.m_rightText && (force || num2 != this.m_lastHealthWound))
				{
					if (num2 <= 0)
					{
						this.m_rightText.SetText(NameplateControllerStatBarUI.kEmptyCharArray);
					}
					else
					{
						this.m_rightText.SetTextFormat("{0}%", num2);
					}
					this.m_lastHealthWound = num2;
					return;
				}
				break;
			}
			case StatBarType.Stamina:
			{
				valueFill = entity.Vitals.Stamina / 100f;
				woundFill = entity.Vitals.StaminaWound / 100f;
				int num3 = Mathf.FloorToInt(Mathf.Clamp(entity.Vitals.Stamina, 0f, 100f));
				int num4 = Mathf.Clamp(Mathf.FloorToInt(entity.Vitals.StaminaWound), 0, 100);
				if (this.m_leftText && (force || num3 != this.m_lastStamina))
				{
					this.m_leftText.SetTextFormat("{0}%", num3);
					this.m_lastStamina = num3;
				}
				if (this.m_rightText && (force || num4 != this.m_lastStaminaWound))
				{
					if (num4 <= 0)
					{
						this.m_rightText.SetText(NameplateControllerStatBarUI.kEmptyCharArray);
					}
					else
					{
						this.m_rightText.SetTextFormat("{0}%", num4);
					}
					this.m_lastStaminaWound = num4;
					return;
				}
				break;
			}
			case StatBarType.ArmorClass:
			{
				valueFill = entity.Vitals.GetArmorClassPercent();
				int num5 = Mathf.Clamp(Mathf.FloorToInt(valueFill * 100f), 0, 100);
				if (this.m_centerText && (force || num5 != this.m_lastArmorClass))
				{
					if (num5 >= 100)
					{
						this.m_centerText.text = string.Empty;
					}
					else
					{
						this.m_centerText.SetTextFormat("{0}%", num5);
					}
					this.m_lastArmorClass = num5;
				}
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x06002CCD RID: 11469 RVA: 0x0014B508 File Offset: 0x00149708
		private void UpdateNpcStatBar(GameEntity entity, bool force, out float valueFill, out float woundFill)
		{
			valueFill = 0f;
			woundFill = 0f;
			switch (this.m_statBarType)
			{
			case StatBarType.Health:
			{
				valueFill = entity.Vitals.GetHealthPercent();
				int num = Mathf.FloorToInt(entity.Vitals.Health);
				if (this.m_centerText && (force || num != this.m_lastHealth))
				{
					this.m_centerText.SetTextFormat("{0}%", num);
					this.m_lastHealth = num;
					return;
				}
				break;
			}
			case StatBarType.Stamina:
				break;
			case StatBarType.ArmorClass:
			{
				valueFill = entity.Vitals.GetArmorClassPercent();
				int num2 = Mathf.FloorToInt((float)entity.Vitals.ArmorClass);
				if (this.m_centerText && (force || num2 != this.m_lastArmorClass))
				{
					if (num2 >= 100)
					{
						this.m_centerText.text = string.Empty;
					}
					else
					{
						this.m_centerText.SetTextFormat("{0}%", num2);
					}
					this.m_lastArmorClass = num2;
				}
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x04002C78 RID: 11384
		private const float kFillUpRate = 2f;

		// Token: 0x04002C79 RID: 11385
		private const float kFillDownRate = 4f;

		// Token: 0x04002C7A RID: 11386
		private static readonly char[] kEmptyCharArray = " ".ToCharArray();

		// Token: 0x04002C7B RID: 11387
		[SerializeField]
		private StatBarType m_statBarType;

		// Token: 0x04002C7C RID: 11388
		[SerializeField]
		private TextMeshProUGUI m_leftText;

		// Token: 0x04002C7D RID: 11389
		[SerializeField]
		private TextMeshProUGUI m_centerText;

		// Token: 0x04002C7E RID: 11390
		[SerializeField]
		private TextMeshProUGUI m_rightText;

		// Token: 0x04002C7F RID: 11391
		[SerializeField]
		private FilledImage m_valueFill;

		// Token: 0x04002C80 RID: 11392
		[SerializeField]
		private FilledWound m_woundFill;

		// Token: 0x04002C81 RID: 11393
		private int m_lastHealth = -100;

		// Token: 0x04002C82 RID: 11394
		private int m_lastMaxHealth = -100;

		// Token: 0x04002C83 RID: 11395
		private int m_lastHealthWound = -100;

		// Token: 0x04002C84 RID: 11396
		private int m_lastStamina = -100;

		// Token: 0x04002C85 RID: 11397
		private int m_lastStaminaWound = -100;

		// Token: 0x04002C86 RID: 11398
		private int m_lastArmorClass = -100;
	}
}

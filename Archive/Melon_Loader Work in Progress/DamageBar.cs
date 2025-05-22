// DamageBar.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageBar : MonoBehaviour
{
    [Header("UI References")]
    public Image roleIcon;
    public TMP_Text nameText;
    public TMP_Text damageText;
    public Image fillBar;

    [Header("Config")]
    public Color dpsColor = new Color(0.8f, 0.2f, 0.2f);
    public Color healColor = new Color(0.2f, 0.8f, 0.2f);
    public Color tankColor = new Color(0.2f, 0.4f, 0.8f);

    public void SetEntry(string name, float dps, float max, Sprite icon = null, string role = "dps")
    {
        nameText.text = name;
        damageText.text = dps.ToString("F1");
        fillBar.fillAmount = Mathf.Clamp01(dps / max);

        switch (role.ToLower())
        {
            case "tank":
                fillBar.color = tankColor;
                break;
            case "heal":
            case "healer":
                fillBar.color = healColor;
                break;
            default:
                fillBar.color = dpsColor;
                break;
        }

        if (icon != null && roleIcon != null)
        {
            roleIcon.sprite = icon;
            roleIcon.enabled = true;
        }
        else if (roleIcon != null)
        {
            roleIcon.enabled = false;
        }
    }
}

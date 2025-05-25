# ShowVendorItemPrice

A MelonLoader mod for **Embers Adrift**  
**Always see the sale price for items in all tooltips, not just at the merchant!**

---

## What Does It Do?

By default, Embers Adrift only shows the sale price of items in tooltips **when you're at a merchant**.  
This mod makes the sale price row always visible in item tooltips, anywhere in the game.

---

## How it works

- **Without this mod:**  
  - The tooltip only displays the sale price when the merchant window is open and you're hovering a merchant item.
  - When the merchant window is closed, regular tooltips don't show the sale price.

- **With this mod:**  
  - The sale price is always visible in all item tooltips, even outside the merchant window.

---

## Flow Diagram

```mermaid
flowchart TD
    A[Merchant Window Open] --> B[MerchantForSaleListItem Created]
    B --> C{User Hovers Item}
    C --> D[GetTooltipParameter() called]
    D --> E[Tooltip receives AtMerchant = true]
    E --> F[Tooltip displays Sale Price]
    G[Merchant Window Closed] --> H[MerchantForSaleListItem Destroyed]
    H --> I[No Sale Price in regular tooltips]
```

With this mod:  
**The tooltip always displays the sale price, regardless of merchant state!**

---
## Credits

- Mod by MrJambix



```
    A[Merchant Window Open] --> B[MerchantForSaleListItem Created]
    B --> C{User Hovers Item}
    C --> D[GetTooltipParameter() called]
    D --> E[Tooltip receives AtMerchant = true]
    E --> F[Tooltip displays Sale Price]
    G[Merchant Window Closed] --> H[MerchantForSaleListItem Destroyed]
    H --> I[No Sale Price in regular tooltips]
```
# Rival silhouettes (presentation)

No portraits or real logos. Each lab gets:

1. **`ui-rival-badge.obj`** instance
2. **`RivalPresentationPalette.AccentFor(labId)`** tint
3. **`CampusSilhouetteCatalog.PathFor(labId)`** → `silhouette-{labId}.svg` (4 curated labs in manifest)

| Lab id | Shape (v1) | Notes |
| --- | --- | --- |
| `openaye` | Circle badge | Curated blue |
| `anthoptics` | Diamond | Coral accent |
| `xay` | Tall hex | Rocket-orange |
| `envydia` | Green slab | Vendor green |

Hash fallback for arrivals (`macrosoft`, `palantear`, …): HSV from `labId.GetHashCode()`.
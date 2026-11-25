# URP material — `CampusFallback.mat` (Unity thread creates file)

Create at `unity/Assets/Art/Materials/CampusFallback.mat` on the Unity editor / GameCI project.

| Property | Value |
| --- | --- |
| Shader | Universal Render Pipeline/Lit |
| Surface | Opaque |
| Base color | `#F4F0E8` (`CampusUiTokens.UiPaper`) |
| Smoothness | `0.2` (`CampusUiTokens.DefaultSmoothness`) |
| Metallic | 0 |
| Emission | off by default; room compute enables HDR via `HqVisualPalette.GlowEmission` |

Apply to all imported `Generated/campus-kit/*.obj` until per-purpose materials land.
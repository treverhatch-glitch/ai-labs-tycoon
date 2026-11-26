using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>Runtime 32×32 swatches matching generated PNG checker specs.</summary>
    public static class ProceduralSwatch
    {
        public static Texture2D ForPath(string assetPath)
        {
            Color baseCol = Color.gray;
            Color altCol = Color.white;
            if (assetPath != null)
            {
                if (assetPath.Contains("trim-orange") || assetPath.Contains("cash-era0") || assetPath.Contains("era-bar"))
                {
                    baseCol = EarlyGameHudArt.TrimOrange;
                    altCol = CampusUiTokens.Hex("#C87830");
                }
                else if (assetPath.Contains("carpet") || assetPath.Contains("concrete") || assetPath.Contains("date-era0"))
                {
                    baseCol = EarlyGameHudArt.CarpetBeige;
                    altCol = CampusUiTokens.Hex("#B8A890");
                }
                else if (assetPath.Contains("phosphor") || assetPath.Contains("wisp-happy"))
                {
                    baseCol = CampusUiTokens.Hex("#66BF73");
                    altCol = CampusUiTokens.Hex("#4A9A58");
                }
                else if (assetPath.Contains("wisp-angry"))
                {
                    baseCol = CampusUiTokens.UiDanger;
                    altCol = CampusUiTokens.Hex("#C84838");
                }
                else if (assetPath.Contains("wisp"))
                {
                    baseCol = CampusUiTokens.Hex("#8C66F2");
                    altCol = CampusUiTokens.Hex("#6A46D0");
                }
                else if (assetPath.Contains("build"))
                {
                    baseCol = CampusUiTokens.Hex("#F2BF5A");
                    altCol = CampusUiTokens.Hex("#D2A040");
                }
                else if (assetPath.Contains("hire") || assetPath.Contains("staff"))
                {
                    baseCol = CampusUiTokens.Hex("#66BF73");
                    altCol = CampusUiTokens.Hex("#4A9A58");
                }
                else if (assetPath.Contains("train"))
                {
                    baseCol = CampusUiTokens.Hex("#8C66F2");
                    altCol = CampusUiTokens.Hex("#6A46D0");
                }
            }

            return Checker(32, 32, baseCol, altCol, 8);
        }

        public static Texture2D Checker(int w, int h, Color a, Color b, int period)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                tex.SetPixel(x, y, ((x / period) + (y / period)) % 2 == 0 ? a : b);
            tex.Apply();
            return tex;
        }
    }
}
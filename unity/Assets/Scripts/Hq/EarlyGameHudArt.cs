using UnityEngine;
using UnityEngine.UI;

namespace AiLabs.Unity.Hq
{
    /// <summary>Procedural dial-up HUD chrome for block-out (matches committed PNG/SVG specs).</summary>
    public static class EarlyGameHudArt
    {
        public static readonly Color TrimOrange = CampusUiTokens.Hex("#A85828");
        public static readonly Color CarpetBeige = CampusUiTokens.Hex("#C8B8A0");

        public static void ApplyTopBar(Image topBarImage)
        {
            if (topBarImage == null) return;
            topBarImage.color = CampusUiTokens.UiSlate;
        }

        public static void ApplyWeekButton(Image buttonImage)
        {
            if (buttonImage == null) return;
            buttonImage.color = CampusUiTokens.Hex("#4088D9");
        }

        public static void DecorateCanvas(Transform canvasRoot, HqHudView hud)
        {
            if (canvasRoot == null) return;
            Image topBar = hud != null ? hud.TopBarImage : null;
            ApplyTopBar(topBar);

            AddCornerIcon(canvasRoot, "EarlyHudCash", EarlyGameHudCatalog.PathFor("icon-cash-era0"),
                new Vector2(8f, -8f), TrimOrange);
            AddCornerIcon(canvasRoot, "EarlyHudDate", EarlyGameHudCatalog.PathFor("icon-date-era0"),
                new Vector2(48f, -8f), CarpetBeige);
            AddCornerIcon(canvasRoot, "EarlyHudEra", EarlyGameHudCatalog.PathFor("icon-era-bar"),
                new Vector2(-8f, -8f), TrimOrange, TextAnchor.UpperRight);

            foreach (Transform child in canvasRoot)
            {
                if (child.name == "AdvanceWeek")
                {
                    var img = child.GetComponent<Image>();
                    ApplyWeekButton(img);
                    AddChildBadge(child, "icon-week-advance");
                }
            }
        }

        static void AddCornerIcon(Transform parent, string name, string catalogPath, Vector2 offset, Color tint,
            TextAnchor align = TextAnchor.UpperLeft)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            bool right = align == TextAnchor.UpperRight;
            rect.anchorMin = right ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
            rect.anchorMax = right ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
            rect.pivot = right ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
            rect.sizeDelta = new Vector2(32f, 32f);
            rect.anchoredPosition = offset;
            var image = go.AddComponent<Image>();
            image.color = tint;
            image.sprite = SpriteFromCatalog(catalogPath);
            EarlyGameArtPaths.LogResolve(catalogPath);
        }

        static void AddChildBadge(Transform parent, string iconId)
        {
            string path = EarlyGameHudCatalog.PathFor(iconId);
            var badge = new GameObject("WeekIcon");
            badge.transform.SetParent(parent, false);
            var rect = badge.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.sizeDelta = new Vector2(24f, 24f);
            rect.anchoredPosition = new Vector2(8f, 0f);
            var image = badge.AddComponent<Image>();
            image.color = Color.white;
            image.sprite = SpriteFromCatalog(path);
            EarlyGameArtPaths.LogResolve(path);
        }

        public static Sprite SpriteFromCatalog(string assetPath)
        {
            if (assetPath != null && assetPath.Contains("textures/"))
            {
                string id = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                var tex = EarlyGameTextureLoader.LoadAlbedo(id);
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 64f);
            }
            Texture2D flat = EarlyGameTextureLoader.LoadAlbedo("tex-hud-paper-warm");
            return Sprite.Create(flat, new Rect(0, 0, flat.width, flat.height), new Vector2(0.5f, 0.5f), 64f);
        }
    }
}
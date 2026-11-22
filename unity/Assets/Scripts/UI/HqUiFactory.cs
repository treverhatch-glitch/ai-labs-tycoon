using AiLabs.Unity.Hq;
using UnityEngine;
using UnityEngine.UI;

namespace AiLabs.Unity.UI
{
    static class HqUiFactory
    {
        public static Font DefaultFont => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        public static GameObject Panel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 size, Color? bg = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(anchorMin.x < 0.5f ? 0f : 1f, 1f);
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = bg ?? CampusUiTokens.UiSlate;
            return go;
        }

        public static Text Label(Transform parent, string name, string text, int fontSize, TextAnchor align)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(8f, 4f);
            rect.offsetMax = new Vector2(-8f, -4f);
            var label = go.AddComponent<Text>();
            label.font = DefaultFont;
            label.fontSize = fontSize;
            label.color = CampusUiTokens.UiPaper;
            label.alignment = align;
            label.text = text;
            return label;
        }

        public static Button SmallButton(Transform parent, string label, UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject(label);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(180f, 32f);
            var img = go.AddComponent<Image>();
            img.color = CampusUiTokens.Hex("#38588C");
            var button = go.AddComponent<Button>();
            button.targetGraphic = img;
            button.onClick.AddListener(onClick);
            Label(go.transform, "Text", label, 15, TextAnchor.MiddleCenter);
            return button;
        }
    }
}
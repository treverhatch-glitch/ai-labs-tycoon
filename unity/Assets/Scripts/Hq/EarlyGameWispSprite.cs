using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace AiLabs.Unity.Hq
{
    /// <summary>Animated wisp mood sprite from generated sheet (4 frames).</summary>
    public sealed class EarlyGameWispSprite : MonoBehaviour
    {
        const int FrameCount = 4;
        const float FrameSeconds = 0.22f;

        static readonly Dictionary<string, Sprite[]> Cache = new Dictionary<string, Sprite[]>();

        Image _image;
        Sprite[] _frames;
        float _t;

        public static EarlyGameWispSprite Attach(Transform canvasRoot, string moodId, Vector2 anchoredPos)
        {
            var go = new GameObject("Wisp_" + moodId);
            go.transform.SetParent(canvasRoot, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.sizeDelta = new Vector2(96f, 96f);
            rect.anchoredPosition = anchoredPos;
            var img = go.AddComponent<Image>();
            var comp = go.AddComponent<EarlyGameWispSprite>();
            comp._image = img;
            comp._frames = LoadFrames(moodId);
            if (comp._frames != null && comp._frames.Length > 0)
                img.sprite = comp._frames[0];
            img.color = Color.white;
            EarlyGameArtPaths.LogResolve(EarlyGameWispCatalog.PathFor(moodId));
            return comp;
        }

        void Update()
        {
            if (_frames == null || _frames.Length == 0) return;
            _t += Time.deltaTime;
            int frame = Mathf.FloorToInt(_t / FrameSeconds) % FrameCount;
            _image.sprite = _frames[frame];
        }

        static Sprite[] LoadFrames(string moodId)
        {
            if (Cache.TryGetValue(moodId, out Sprite[] cached))
                return cached;

            string rel = $"EarlyGame/wisp-moods/{moodId}-sheet.png";
            string full = Path.Combine(Application.streamingAssetsPath, rel);
            if (!File.Exists(full))
                return Cache[moodId] = new Sprite[0];

            byte[] bytes = File.ReadAllBytes(full);
            var sheet = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            sheet.LoadImage(bytes);
            int fw = sheet.width / FrameCount;
            int fh = sheet.height;
            var frames = new Sprite[FrameCount];
            for (int i = 0; i < FrameCount; i++)
            {
                var slice = new Texture2D(fw, fh, TextureFormat.RGBA32, false);
                slice.SetPixels(sheet.GetPixels(i * fw, 0, fw, fh));
                slice.Apply();
                frames[i] = Sprite.Create(slice, new Rect(0, 0, fw, fh), new Vector2(0.5f, 0.5f), 128f);
            }
            Cache[moodId] = frames;
            return frames;
        }
    }
}
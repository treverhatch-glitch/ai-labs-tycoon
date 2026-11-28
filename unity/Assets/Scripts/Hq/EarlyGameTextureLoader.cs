using System.IO;
using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>Loads committed PNG swatches from StreamingAssets (packet-2 tileable art).</summary>
    public static class EarlyGameTextureLoader
    {
        const string Root = "EarlyGame/textures";

        public static Texture2D LoadAlbedo(string textureId)
        {
            return LoadPng($"{Root}/{textureId}.png");
        }

        public static Texture2D LoadNormal(string textureId)
        {
            return LoadPng($"{Root}/{textureId}_normal.png");
        }

        public static Texture2D LoadPng(string relativePath)
        {
            string full = Path.Combine(Application.streamingAssetsPath, relativePath);
            if (!File.Exists(full))
            {
                Debug.LogWarning("EarlyGameTextureLoader: missing " + full);
                return Texture2D.whiteTexture;
            }

            byte[] bytes = File.ReadAllBytes(full);
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, true);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Repeat;
            if (!tex.LoadImage(bytes))
            {
                Debug.LogWarning("EarlyGameTextureLoader: decode failed " + relativePath);
                return Texture2D.whiteTexture;
            }
            return tex;
        }
    }
}
using System.IO;
using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>Blender PNG HUD icons in StreamingAssets; SVG paths remain catalog fallbacks for editors.</summary>
    public static class EarlyGameHudIconLoader
    {
        const string Root = "EarlyGame/hud-icons";

        public static Texture2D Load(string iconId)
        {
            string rel = $"{Root}/{iconId}.png";
            string full = Path.Combine(Application.streamingAssetsPath, rel);
            if (!File.Exists(full))
                return null;
            return EarlyGameTextureLoader.LoadPng(rel);
        }
    }
}
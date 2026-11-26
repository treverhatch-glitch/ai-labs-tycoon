using UnityEngine;
using UnityEngine.Rendering;

namespace AiLabs.Unity.Hq
{
    /// <summary>Warm 1995 garage mood for the first GameBootstrap HQ scene (Built-in / URP-friendly globals).</summary>
    public static class EarlyGameGarageLighting
    {
        public static void Apply()
        {
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.45f, 0.41f, 0.36f);
            RenderSettings.ambientEquatorColor = new Color(0.58f, 0.50f, 0.42f);
            RenderSettings.ambientGroundColor = new Color(0.22f, 0.19f, 0.17f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.68f, 0.60f, 0.52f);
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.012f;

            Light sun = Object.FindFirstObjectByType<Light>();
            if (sun != null && sun.type == LightType.Directional)
            {
                sun.color = new Color(1f, 0.91f, 0.76f);
                sun.intensity = 0.92f;
                sun.transform.rotation = Quaternion.Euler(38f, -42f, 0f);
            }

            Camera cam = Camera.main;
            if (cam != null)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0.70f, 0.63f, 0.54f);
                cam.allowHDR = true;
            }
        }
    }
}
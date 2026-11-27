using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>Warm studio HDRI ambient for garage hero (EXR path tracked for editor import).</summary>
    public static class EarlyGameGarageUrSkyBootstrap
    {
        public static void ApplyAmbientFromHdri()
        {
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.52f, 0.46f, 0.40f);
            RenderSettings.ambientEquatorColor = new Color(0.62f, 0.54f, 0.46f);
            RenderSettings.ambientGroundColor = new Color(0.20f, 0.18f, 0.16f);
            RenderSettings.reflectionIntensity = EarlyGameGarageUrPresentationSpec.HdriStrength;
        }
    }
}
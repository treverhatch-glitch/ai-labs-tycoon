using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AiLabs.Unity.Hq
{
    /// <summary>Global URP volume for garage hero (bloom, AO, ACES tonemapping).</summary>
    public static class EarlyGameGarageUrVolumeBootstrap
    {
        const string VolumeGoName = "GarageHeroGlobalVolume";

        public static void EnsureGlobalVolume()
        {
            if (Object.FindFirstObjectByType<Volume>() != null)
                return;

            var go = new GameObject(VolumeGoName);
            Object.DontDestroyOnLoad(go);
            var volume = go.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 15f;
            volume.weight = 1f;
            volume.profile = BuildProfile();
        }

        static VolumeProfile BuildProfile()
        {
            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            profile.name = "GarageHeroVolumeProfile_Runtime";
            AddBloom(profile);
            AddTonemapping(profile);
            AddAmbientOcclusion(profile);
            return profile;
        }

        static void AddBloom(VolumeProfile profile)
        {
            if (!profile.TryGet(out Bloom bloom))
                bloom = profile.Add<Bloom>(true);
            bloom.active = true;
            bloom.threshold.Override(EarlyGameGarageUrPresentationSpec.BloomThreshold);
            bloom.intensity.Override(EarlyGameGarageUrPresentationSpec.BloomIntensity);
        }

        static void AddTonemapping(VolumeProfile profile)
        {
            if (!profile.TryGet(out Tonemapping tonemapping))
                tonemapping = profile.Add<Tonemapping>(true);
            tonemapping.active = true;
            tonemapping.mode.Override((TonemappingMode)EarlyGameGarageUrPresentationSpec.TonemappingModeAces);
        }

        static void AddAmbientOcclusion(VolumeProfile profile)
        {
            if (!profile.TryGet(out ScreenSpaceAmbientOcclusion ao))
                ao = profile.Add<ScreenSpaceAmbientOcclusion>(true);
            ao.active = true;
            ao.intensity.Override(EarlyGameGarageUrPresentationSpec.AmbientOcclusionIntensity);
            ao.radius.Override(EarlyGameGarageUrPresentationSpec.AmbientOcclusionRadius);
        }
    }
}
using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>URP Lit when available, else Standard — binds Blender-baked albedo / normal / roughness.</summary>
    public static class EarlyGamePbrMaterial
    {
        public static Material Create(string materialName, Texture2D albedo, Texture2D normal, Texture2D roughness)
        {
            var mat = new Material(HqShaders.Lit()) { name = materialName };
            if (albedo != null)
                mat.mainTexture = albedo;
            if (normal != null && mat.HasProperty("_BumpMap"))
                mat.SetTexture("_BumpMap", normal);
            if (roughness != null)
            {
                if (mat.HasProperty("_MetallicGlossMap"))
                    mat.SetTexture("_MetallicGlossMap", roughness);
                else if (mat.HasProperty("_SpecGlossMap"))
                    mat.SetTexture("_SpecGlossMap", roughness);
            }
            if (mat.HasProperty("_Smoothness"))
                mat.SetFloat("_Smoothness", 0.35f);
            return mat;
        }
    }
}
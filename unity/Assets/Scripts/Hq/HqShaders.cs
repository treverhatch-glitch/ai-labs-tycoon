using UnityEngine;

namespace AiLabs.Unity.Hq
{
    static class HqShaders
    {
        public static Shader Lit()
        {
            Shader urp = Shader.Find("Universal Render Pipeline/Lit");
            if (urp != null) return urp;
            Shader standard = Shader.Find("Standard");
            if (standard != null) return standard;
            return Shader.Find("Diffuse");
        }
    }
}
using AiLabs.Rules.Config;

namespace AiLabs.Rules.Systems
{
    public static class ShadyFreezeAudit
    {
        public static bool Enabled(GameConfig c) { return c.ShadyFreezeAudit.Enabled > 0; }
    }
}
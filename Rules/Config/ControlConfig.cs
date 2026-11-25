using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Control goes stale (bible ch. 10, "what erodes it": "each new era: control techniques go stale as models change", "firing or ignoring the
    /// safety team"). The lab's safety techniques count for less for every era that has begun since it last did any new safety research, and
    /// letting safety people go costs culture. Only the player's labs go stale; rivals keep their techniques at full strength.
    /// </summary>
    public sealed class ControlConfig : ISync
    {
        /// <summary>Eras the lab can go without new safety research before its techniques start to count for less.</summary>
        public int GraceEras;
        /// <summary>For each era beyond the grace, this much of the safety techniques' bonus is lost, in permille of the bonus.</summary>
        public int LossPerEraPermille;
        /// <summary>The most the techniques can lose, in permille of their bonus. Old techniques never become worth nothing.</summary>
        public int MaxLossPermille;
        /// <summary>Culture lost for each person let go from a role that listens to the lab's own concerns (see culture.listeningRoles).</summary>
        public int FiredListenerPoints;

        public void Sync(Syncer s)
        {
            s.Int("graceEras", ref GraceEras);
            s.Int("lossPerEraPermille", ref LossPerEraPermille);
            s.Int("maxLossPermille", ref MaxLossPermille);
            s.Int("firedListenerPoints", ref FiredListenerPoints);
        }
    }
}

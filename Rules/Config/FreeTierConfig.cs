using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// A free tier for the lab's best released model. Its reach grows each week toward a ceiling set by the level; the more reach the more of it converts into extra revenue, and the more it costs to serve. Shutting it upsets the people who built on it. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class FreeTierConfig : ISync
    {
        public int Enabled;
        /// <summary>Most extra revenue a level 1 free tier brings (permille of a week).</summary>
        public int LiftA;
        /// <summary>Same for level 2.</summary>
        public int LiftB;
        /// <summary>Same for level 3.</summary>
        public int LiftC;
        /// <summary>Reach (of 1000) a level 1 tier gains each week.</summary>
        public int StepA;
        /// <summary>Same for level 2.</summary>
        public int StepB;
        /// <summary>Same for level 3.</summary>
        public int StepC;
        /// <summary>Weekly serving cost of a level 1 tier at no reach.</summary>
        public int CostA;
        /// <summary>Same for level 2.</summary>
        public int CostB;
        /// <summary>Same for level 3.</summary>
        public int CostC;
        /// <summary>Reputation lost per 1000 reach when the tier is shut (damped).</summary>
        public int CloseLoss;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("liftA", ref LiftA);
            s.Int("liftB", ref LiftB);
            s.Int("liftC", ref LiftC);
            s.Int("stepA", ref StepA);
            s.Int("stepB", ref StepB);
            s.Int("stepC", ref StepC);
            s.Int("costA", ref CostA);
            s.Int("costB", ref CostB);
            s.Int("costC", ref CostC);
            s.Int("closeLoss", ref CloseLoss);
        }
    }
}

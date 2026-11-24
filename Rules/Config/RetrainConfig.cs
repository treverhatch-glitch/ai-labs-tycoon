using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Retraining: the player moves one person from their role to another instead of letting them go and hiring someone new. The person keeps
    /// their trait and most of their morale, the lab pays a share of the new role's hire fee, and the new role needs a free seat. Moving to
    /// a role of the same function is cheaper and easier than changing function. Only the player's lab retrains.
    /// </summary>
    public sealed class RetrainConfig : ISync
    {
        public bool Enabled;
        /// <summary>The share of the new role's hire fee a change of function costs, in permille.</summary>
        public int FeePermille;
        /// <summary>The share of the new role's hire fee a move inside a function costs, in permille.</summary>
        public int SameFunctionFeePermille;
        /// <summary>The morale a change of function costs the person.</summary>
        public int MoraleDrop;
        /// <summary>The morale a move inside a function costs the person.</summary>
        public int SameFunctionMoraleDrop;
        /// <summary>The lowest morale a person can have and still agree to learn a new job.</summary>
        public int MinMorale;

        public void Sync(Syncer s)
        {
            s.Bool("enabled", ref Enabled);
            s.Int("feePermille", ref FeePermille);
            s.Int("sameFunctionFeePermille", ref SameFunctionFeePermille);
            s.Int("moraleDrop", ref MoraleDrop);
            s.Int("sameFunctionMoraleDrop", ref SameFunctionMoraleDrop);
            s.Int("minMorale", ref MinMorale);
        }
    }
}

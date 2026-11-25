using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>A claim against the lab. Stage 0 waits for the hearing; stage 1 is a lost hearing waiting to be paid or appealed.</summary>
    public sealed class LawCase : ISync
    {
        public int Id;
        /// <summary>0 an incident, 1 a layoff round, 2 a breach of security, 3 a change of charter.</summary>
        public int Kind;
        public long Claim;
        /// <summary>How strong the claim is, from 1 to 1000, fixed on the day it was filed.</summary>
        public int Strength;
        public int Stage;
        /// <summary>0 the lab's own counsel, 1 an outside firm.</summary>
        public int Counsel;
        public int Appealed;
        /// <summary>The week of the hearing, or of the payment once the hearing is lost.</summary>
        public int Due;
        public string Ref = "";

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Int("kind", ref Kind);
            s.Long("claim", ref Claim);
            s.Int("strength", ref Strength);
            s.Int("stage", ref Stage);
            s.Int("counsel", ref Counsel);
            s.Int("appealed", ref Appealed);
            s.Int("due", ref Due);
            s.Str("ref", ref Ref);
        }
    }
}

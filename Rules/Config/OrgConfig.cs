using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// How the lab is organised: flat, by function or in cross-functional squads. Each shape costs and gains something that depends on how many people the lab has, how many different roles they hold and how many seniors there are to lead teams. Changing the shape disrupts the lab for a while and cannot be repeated soon. A lab that never chooses has no structure and none of this applies. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class OrgConfig : ISync
    {
        public int Enabled;
        /// <summary>Headcount a flat lab runs without any coordination loss.</summary>
        public int FlatCap;
        /// <summary>Loss for each head beyond the cap in a flat lab (permille).</summary>
        public int FlatStep;
        /// <summary>Heads each function team handles before coordination costs, in a functional lab.</summary>
        public int SpanSize;
        /// <summary>Loss for each head beyond the spans (permille).</summary>
        public int SpanStep;
        /// <summary>Loss for each function with people but no senior to lead it (permille).</summary>
        public int LeadlessLoss;
        /// <summary>Research gain of a functional lab whose teams are all led (permille).</summary>
        public int FuncResearchBonus;
        /// <summary>Heads in a cross-functional squad.</summary>
        public int SquadSize;
        /// <summary>Different roles the lab needs for squads to work.</summary>
        public int SquadMinRoles;
        /// <summary>Loss for each squad with no senior to lead it (permille).</summary>
        public int SquadLeadLoss;
        /// <summary>Training speed gain when every squad is led (permille).</summary>
        public int SquadTrainingBonus;
        /// <summary>Loss for each squad, for the meetings between them (permille).</summary>
        public int CrossTeam;
        /// <summary>Most loss a structure can cost (permille).</summary>
        public int LossCap;
        /// <summary>Weeks of disruption after a reorganisation.</summary>
        public int ReorgWeeks;
        /// <summary>Loss while a reorganisation is settling (permille).</summary>
        public int DisruptLoss;
        /// <summary>Weeks before the structure can be changed again.</summary>
        public int CooldownWeeks;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("flatCap", ref FlatCap);
            s.Int("flatStep", ref FlatStep);
            s.Int("spanSize", ref SpanSize);
            s.Int("spanStep", ref SpanStep);
            s.Int("leadlessLoss", ref LeadlessLoss);
            s.Int("funcResearchBonus", ref FuncResearchBonus);
            s.Int("squadSize", ref SquadSize);
            s.Int("squadMinRoles", ref SquadMinRoles);
            s.Int("squadLeadLoss", ref SquadLeadLoss);
            s.Int("squadTrainingBonus", ref SquadTrainingBonus);
            s.Int("crossTeam", ref CrossTeam);
            s.Int("lossCap", ref LossCap);
            s.Int("reorgWeeks", ref ReorgWeeks);
            s.Int("disruptLoss", ref DisruptLoss);
            s.Int("cooldownWeeks", ref CooldownWeeks);
        }
    }
}

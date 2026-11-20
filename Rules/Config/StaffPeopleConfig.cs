using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public sealed class StaffPeopleConfig : ISync
    {
        public int MoraleBaseline;
        public int MoraleDriftPerWeek;
        public int PoachChancePermille;
        /// <summary>Chance each hire is a star trait, in permille.</summary>
        public int StarHirePermille;
        public List<StaffTraitConfig> Traits = new List<StaffTraitConfig>();

        public StaffTraitConfig Trait(string id)
        {
            for (int i = 0; i < Traits.Count; i++) if (Traits[i].Id == id) return Traits[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Int("moraleBaseline", ref MoraleBaseline);
            s.Int("moraleDriftPerWeek", ref MoraleDriftPerWeek);
            s.Int("poachChancePermille", ref PoachChancePermille);
            s.Int("starHirePermille", ref StarHirePermille);
            s.ObjList("traits", ref Traits);
        }
    }

    public sealed class StaffTraitConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        public int ResearchPermille;
        public bool Star;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Int("researchPermille", ref ResearchPermille);
            s.Bool("star", ref Star);
        }
    }
}
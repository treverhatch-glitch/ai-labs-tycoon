using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Industry standards. See <see cref="StandardsConfig"/>. Only the player's lab has a part in them.</summary>
    public static class Standards
    {
        public const int Observer = 0;
        public const int Member = 1;
        public const int Sponsor = 2;
        public const int Obstructor = 3;

        public static bool Enabled(GameConfig c) { return c.Standards.Enabled > 0; }

        public static bool Drafting(LabState lab) { return lab.StdDraft > 0; }

        /// <summary>Support the draft needs to be ratified.</summary>
        public static int Needed(GameConfig c, LabState lab) { return c.Standards.SupportNeeded * lab.StdDraft; }

        /// <summary>Support the draft gains each week with the lab's part in it.</summary>
        public static int Rate(GameConfig c, LabState lab)
        {
            int rate = c.Standards.IndustryRate;
            if (lab.StdRole == Member) rate += c.Standards.MemberRate;
            else if (lab.StdRole == Sponsor) rate += c.Standards.SponsorRate;
            else if (lab.StdRole == Obstructor) rate -= c.Standards.ObstructRate;
            return (int)MathI.Max(0, rate);
        }

        public static long Dues(GameConfig c, LabState lab)
        {
            return lab.StdRole == Member ? c.Standards.MemberDues : lab.StdRole == Sponsor ? c.Standards.SponsorDues : lab.StdRole == Obstructor ? c.Standards.ObstructDues : 0;
        }

        public static long AdoptPrice(GameConfig c, LabState lab) { return c.Standards.AdoptCost * (lab.StdLevel - lab.StdAdopted); }

        public static long Upkeep(GameConfig c, LabState lab) { return c.Standards.UpkeepPerLevel * lab.StdAdopted; }

        /// <summary>Levels of the standards a capable first release would lack.</summary>
        public static int Missing(GameConfig c, LabState lab, ModelState model)
        {
            if (!Enabled(c) || model.Capability < c.Standards.BindingCapability) return 0;
            return lab.StdLevel - lab.StdAdopted;
        }

        public static int ClaimStrength(GameConfig c, LabState lab) { return Enabled(c) ? lab.StdAdopted * c.Standards.ClaimStrength : 0; }

        public static int SafetyRelief(GameConfig c, LabState lab) { return Enabled(c) ? lab.StdAdopted * c.Standards.SafetyRelief : 0; }

        public static void TakeRole(GameConfig c, LabState lab, int role)
        {
            if (role == Sponsor) Finance.Spend(lab, LedgerLine.Fines, c.Standards.SponsorFee);
            lab.StdRole = role;
        }

        public static void Adopt(GameConfig c, GameState state, LabState lab)
        {
            Finance.Spend(lab, LedgerLine.Insurance, AdoptPrice(c, lab));
            lab.StdAdopted = lab.StdLevel;
            Events.Add(state, EventKind.StandardAdopted, lab.Id, -1, lab.StdAdopted, "");
        }

        public static void OnFirstRelease(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            if (lab.Id != state.PlayerLabId) return;
            int missing = Missing(c, lab, model);
            if (missing <= 0) return;
            long fine = c.Standards.LapseFine * missing;
            Finance.Spend(lab, LedgerLine.Fines, fine);
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - PressSystem.Damp(c, lab, c.Standards.LapseReputation * missing));
            lab.StdLapses++;
            Events.Add(state, EventKind.StandardLapsed, lab.Id, model.Id, fine, model.Name).Pick = missing;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            if (lab.StdAdopted > 0) Finance.Spend(lab, LedgerLine.Insurance, Upkeep(c, lab));
            if (Drafting(lab))
            {
                long dues = Dues(c, lab);
                if (dues > 0) Finance.Spend(lab, LedgerLine.Fines, dues);
                if (lab.StdRole == Obstructor && (state.Tick + 1) % c.Standards.ObstructEvery == 0)
                    lab.Reputation = (int)MathI.Max(0, lab.Reputation - PressSystem.Damp(c, lab, c.Standards.ObstructReputation));
                lab.StdSupport += Rate(c, lab);
                if (lab.StdSupport >= Needed(c, lab))
                {
                    lab.StdLevel = lab.StdDraft;
                    Events.Add(state, EventKind.StandardRatified, lab.Id, -1, lab.StdLevel, "").Pick = lab.StdRole;
                    lab.StdDraft = 0;
                    lab.StdSupport = 0;
                    lab.StdRole = Observer;
                    lab.StdNext = state.Tick + c.Standards.EveryWeeks;
                }
            }
            else if (lab.StdLevel < c.Standards.MaxLevel && state.Tick + 1 >= (lab.StdNext == 0 ? c.Standards.FirstWeeks : lab.StdNext))
            {
                lab.StdDraft = lab.StdLevel + 1;
                lab.StdSupport = 0;
                Events.Add(state, EventKind.StandardDrafted, lab.Id, -1, lab.StdDraft, "");
            }
        }
    }
}

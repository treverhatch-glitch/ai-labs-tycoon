using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Marketing campaigns. See <see cref="MarketingConfig"/>. Only the player's lab runs them.</summary>
    public static class MarketingCampaign
    {
        public static bool Enabled(GameConfig c) { return c.Marketing.MaxLevel > 0 && c.Marketing.Weeks > 0; }

        public static bool Running(LabState lab) { return lab.CampaignWeeks > 0; }

        public static bool Hungover(LabState lab) { return lab.CampaignWeeks <= 0 && lab.HangoverWeeks > 0; }

        public static long Cost(GameConfig c, int level) { return level * c.Marketing.CostPerLevel; }

        public static int Sales(GameConfig c, LabState lab) { return LabQueries.StaffByFunction(c, lab, RoleFunction.Sales); }

        /// <summary>The lab's share of a sector, in permille of it (0 when it does not sell there).</summary>
        public static int SharePermille(GameState state, LabState lab, string sectorId)
        {
            SectorState sector = state.Sector(sectorId);
            SectorEntry entry = sector == null ? null : sector.Entry(lab.Id);
            return entry == null ? 0 : entry.SharePpm / 1000;
        }

        /// <summary>The appeal the campaign adds in a sector right now, in permille: the budget's boost, falling to nothing as the lab's share nears the ceiling.</summary>
        public static int BoostPermille(GameConfig c, GameState state, LabState lab, string sectorId)
        {
            if (!Running(lab) || lab.CampaignSector != sectorId) return 0;
            long full = lab.CampaignLevel * (long)c.Marketing.AppealPermillePerLevel;
            int ceiling = c.Marketing.ShareCeilingPermille;
            if (ceiling <= 0) return 0;
            long room = MathI.Max(0, ceiling - SharePermille(state, lab, sectorId));
            return (int)(full * room / ceiling);
        }

        public static int HangoverPermille(GameConfig c, LabState lab, string sectorId)
        {
            if (!Hungover(lab) || lab.CampaignSector != sectorId) return 0;
            return (int)MathI.Min(900, lab.CampaignLevel * (long)c.Marketing.HangoverPermillePerLevel);
        }

        /// <summary>The multiplier on the lab's appeal in a sector, in permille: above 1000 while a campaign runs, below it in the hangover, 1000 otherwise.</summary>
        public static int AppealPermille(GameConfig c, GameState state, LabState lab, string sectorId)
        {
            return MathI.Permille + BoostPermille(c, state, lab, sectorId) - HangoverPermille(c, lab, sectorId);
        }

        public static void Start(GameConfig c, GameState state, LabState lab, string sectorId, int level, long cost)
        {
            lab.CampaignSector = sectorId;
            lab.CampaignLevel = level;
            lab.CampaignWeeks = c.Marketing.Weeks;
            lab.HangoverWeeks = 0;
            lab.CampaignLockedUntil = state.Tick + c.Marketing.CooldownWeeks;
            Finance.Spend(lab, LedgerLine.Hiring, cost);
            GameEvent e = Events.Add(state, EventKind.CampaignRun, lab.Id, level, cost, sectorId);
            e.Pick = 1;
        }

        static void EndRun(GameConfig c, GameState state, LabState lab, int pick)
        {
            string sector = lab.CampaignSector;
            int level = lab.CampaignLevel;
            lab.CampaignWeeks = 0;
            lab.HangoverWeeks = c.Marketing.HangoverWeeks;
            if (lab.HangoverWeeks <= 0) Clear(lab);
            GameEvent e = Events.Add(state, EventKind.CampaignRun, lab.Id, level, 0, sector);
            e.Pick = pick;
        }

        static void Clear(LabState lab)
        {
            lab.CampaignSector = "";
            lab.CampaignLevel = 0;
            lab.HangoverWeeks = 0;
        }

        /// <summary>An incident in the campaign's sector while it runs ends it at once and costs reputation.</summary>
        public static void OnIncident(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            if (lab.Id != state.PlayerLabId || !Running(lab) || !model.Sectors.Contains(lab.CampaignSector)) return;
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - c.Marketing.BacklashReputation);
            EndRun(c, state, lab, 2);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return;
            if (Running(lab))
            {
                lab.CampaignWeeks--;
                if (lab.CampaignWeeks == 0) EndRun(c, state, lab, 0);
            }
            else if (lab.HangoverWeeks > 0)
            {
                lab.HangoverWeeks--;
                if (lab.HangoverWeeks == 0) Clear(lab);
            }
        }
    }
}

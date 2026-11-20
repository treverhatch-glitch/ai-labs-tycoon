using System;
using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    /// <summary>Builds the world of a new game: the player's empty lab, the rivals and the sectors.</summary>
    public static class WorldSetup
    {
        public static GameState NewGame(GameConfig c, NewGameOptions options)
        {
            var state = new GameState();
            state.Schema = GameState.CurrentSchema;
            state.ConfigVersion = c.Version;
            state.Seed = options.Seed;
            EraConfig era = StartEra(c, options);
            int startTick = EraSystem.StartTick(c, era);
            state.Tick = startTick;
            state.NextId = 1;
            state.Rng = RngSet.FromSeed(options.Seed);
            state.Labs = new List<LabState>();
            state.Sectors = new List<SectorState>();
            state.Sites = new List<SiteState>();
            state.Gauntlets = new List<GauntletRecord>();
            state.Events = new List<GameEvent>();

            PlayerStartConfig start = c.PlayerStart;
            var player = new LabState();
            player.Id = state.TakeId();
            player.Name = Or(options.LabName, start.LabName);
            player.Leader = Or(options.Leader, start.Leader);
            player.FamilyName = Or(options.FamilyName, start.FamilyName);
            player.Controller = LabController.Player;
            player.PlanStartTick = startTick;
            player.PlanPausedUntilTick = -1;
            player.ChipAmbushUntilTick = -1;
            player.RivalId = "";
            player.Cash = MathI.MulPermille(start.Cash, era.StartCashPermille);
            player.Ledger.StartCash = player.Cash;
            player.Reputation = start.Reputation;
            player.GrantPerWeek = MathI.MulPermille(start.GrantPerWeek, era.StartGrantPermille);
            player.GrantEndsTick = startTick + start.GrantWeeks;
            player.HomeBlocId = start.HomeBlocId;
            player.LotId = IsFirst(c, era) ? start.LotId : era.StartLotId;
            player.Techs.AddRange(era.StartTechs);
            player.TrainingSharePermille = start.TrainingSharePermille;
            player.FounderSharePermille = c.Funding.FounderStartPermille;
            player.BoardInvestorSeats = 0;
            state.Labs.Add(player);
            state.PlayerLabId = player.Id;
            state.Endings = new List<EndingRecord>();

            int startYear = GameDate.Year(c.Calendar, startTick);
            for (int i = 0; i < c.Rivals.Count; i++)
            {
                RivalConfig rival = c.Rivals[i];
                if (options.RivalIds != null && !options.RivalIds.Contains(rival.Id)) continue;
                if (rival.JoinsYear > startYear)
                {
                    if (rival.JoinsYear < RivalConfig.NeverJoins) state.PendingRivals.Add(rival.Id);
                    continue;
                }
                AddRival(c, state, rival, era, false);
            }

            if (options.RivalIds != null)
                for (int i = 0; i < options.RivalIds.Count; i++)
                    if (c.Rival(options.RivalIds[i]) == null) throw new ArgumentException("There is no rival \"" + options.RivalIds[i] + "\" in the config.");

            for (int i = 0; i < c.Market.Sectors.Count; i++) state.Sectors.Add(NewSector(c.Market.Sectors[i], GameDate.Year(c.Calendar, startTick)));
            for (int i = 0; i < c.World.Sites.Count; i++) state.Sites.Add(NewSite(c.World.Sites[i].Id));
            for (int i = 0; i < state.Labs.Count; i++)
                for (int k = 0; k < c.World.Blocs.Count; k++) state.Labs[i].Blocs.Add(NewStanding(c.World.Blocs[k].Id));
            VendorSystem.NewState(c, state);
            return state;
        }

        /// <summary>
        /// Makes a rival's lab. At the start of a game it gets the era's share of the starting money and the era's start techs. A lab that
        /// joins later (<paramref name="arrival"/>) comes with its full starting cash, the era's lot and every tech of the eras before.
        /// </summary>
        public static LabState AddRival(GameConfig c, GameState state, RivalConfig rival, EraConfig era, bool arrival)
        {
            var lab = new LabState();
            lab.Id = state.TakeId();
            lab.Name = rival.Name;
            lab.Leader = rival.Leader;
            lab.FamilyName = rival.FamilyName;
            lab.Controller = LabController.Plan;
            lab.RivalId = rival.Id;
            lab.PlanStartTick = state.Tick + rival.Plan.StartDelayWeeks
                + (rival.Plan.StartJitterWeeks > 0 ? state.Rng.Rivals.Range(0, rival.Plan.StartJitterWeeks) : 0);
            lab.Cash = arrival ? rival.StartCash : MathI.MulPermille(rival.StartCash, era.StartCashPermille);
            lab.Ledger.StartCash = lab.Cash;
            lab.Reputation = rival.Reputation;
            lab.GrantPerWeek = arrival ? rival.BackerPerWeek : MathI.MulPermille(rival.BackerPerWeek, era.StartGrantPermille);
            lab.GrantEndsTick = -1;
            lab.HomeBlocId = rival.HomeBlocId;
            lab.LotId = !arrival && IsFirst(c, era) ? rival.LotId : era.StartLotId;
            lab.Techs.AddRange(arrival ? RivalSystem.CatchUpTechs(c, era) : era.StartTechs);
            lab.TrainingSharePermille = rival.Plan.TrainingSharePermille;
            lab.FounderSharePermille = c.Funding.FounderStartPermille;
            lab.BoardInvestorSeats = 0;
            lab.PlanPausedUntilTick = -1;
            lab.ChipAmbushUntilTick = -1;
            state.Labs.Add(lab);
            if (arrival)
            {
                for (int k = 0; k < c.World.Blocs.Count; k++) lab.Blocs.Add(NewStanding(c.World.Blocs[k].Id));
                VendorSystem.EnsureStanding(c, state, lab.Id);
            }
            return lab;
        }

        static bool IsFirst(GameConfig c, EraConfig era) { return c.Eras[0] == era; }

        /// <summary>The era the options ask for. An unknown era, or one the picker does not offer, is the caller's mistake.</summary>
        static EraConfig StartEra(GameConfig c, NewGameOptions options)
        {
            if (string.IsNullOrEmpty(options.StartEraId)) return c.Eras[0];
            EraConfig era = c.Era(options.StartEraId);
            if (era == null) throw new ArgumentException("There is no era \"" + options.StartEraId + "\" in the config.");
            if (!era.Startable) throw new ArgumentException(era.Name + " is not a starting era.");
            return era;
        }

        /// <summary>A sector as it stands in a given year: its opening pool, grown for every year it has been open.</summary>
        public static SectorState NewSector(SectorConfig sector, int year)
        {
            var s = new SectorState();
            s.Id = sector.Id;
            s.PoolPerWeek = sector.PoolPerWeek;
            for (int y = sector.OpensYear; y < year; y++)
                s.PoolPerWeek = MathI.MulPermille(s.PoolPerWeek, MathI.Permille + EraSystem.SectorGrowthPermille(sector, y));
            s.ChampionLabId = -1;
            return s;
        }

        public static SiteState NewSite(string siteId)
        {
            var site = new SiteState();
            site.SiteId = siteId;
            site.HeatwaveUntilTick = -1;
            return site;
        }

        public static BlocStanding NewStanding(string blocId)
        {
            var standing = new BlocStanding();
            standing.BlocId = blocId;
            return standing;
        }

        static string Or(string value, string fallback) { return string.IsNullOrEmpty(value) ? fallback : value; }

        /// <summary>A new player lab after selling the old one: garage start, era techs, vendor standing, sale cash on the ledger.</summary>
        public static LabState FoundAgain(GameConfig c, GameState state, string name, string leader, string family, long price)
        {
            PlayerStartConfig start = c.PlayerStart;
            EraConfig era = EraSystem.AtTick(c, state.Tick);
            var fresh = new LabState();
            fresh.Id = state.TakeId();
            fresh.Name = name;
            fresh.Leader = leader;
            fresh.FamilyName = family;
            fresh.Controller = LabController.Player;
            fresh.PlanStartTick = state.Tick;
            fresh.RivalId = "";
            fresh.Cash = 0;
            fresh.Ledger.StartCash = 0;
            Finance.Earn(fresh, LedgerLine.CapitalIn, price);
            fresh.Reputation = start.Reputation;
            fresh.GrantPerWeek = start.GrantWeeks > 0 ? MathI.MulPermille(start.GrantPerWeek, era.StartGrantPermille) : 0;
            fresh.GrantEndsTick = start.GrantWeeks > 0 ? state.Tick + start.GrantWeeks : -1;
            fresh.HomeBlocId = start.HomeBlocId;
            fresh.LotId = IsFirst(c, era) ? start.LotId : era.StartLotId;
            fresh.Techs.AddRange(RivalSystem.CatchUpTechs(c, era));
            fresh.Techs.AddRange(era.StartTechs);
            fresh.TrainingSharePermille = start.TrainingSharePermille;
            fresh.FounderSharePermille = c.Funding.FounderStartPermille;
            fresh.BoardInvestorSeats = 0;
            fresh.FundingRounds = new List<string>();
            fresh.PlanPausedUntilTick = -1;
            fresh.ChipAmbushUntilTick = -1;
            for (int k = 0; k < c.World.Blocs.Count; k++) fresh.Blocs.Add(NewStanding(c.World.Blocs[k].Id));
            VendorSystem.EnsureStanding(c, state, fresh.Id);
            return fresh;
        }
    }
}

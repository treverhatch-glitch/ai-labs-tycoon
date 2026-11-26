using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Read-only decompositions of numbers the UI shows. Each part sums to the total.</summary>
    public static class DisplayQueries
    {
        public struct QualityBreakdown
        {
            public long FromCapability;
            public long FromAlignment;
            public long SandbagBoost;
            public long GlitchAdjustment;
            public long MoodAdjustment;
            public long OpenWeightsBoost;

            public long Total { get { return FromCapability + FromAlignment + SandbagBoost + GlitchAdjustment + MoodAdjustment + OpenWeightsBoost; } }
        }

        public struct AppealBreakdown
        {
            public long Core;
            public long ReputationBoost;
            public long SalesBoost;
            public long ChampionBoost;
            public long ExpansionBoost;
            public long WorkshopBoost;
            public long CampaignBoost;
            public long DomainLeaderBoost;
            public long DomainChallengerBoost;
            public long BenchmarkBoost;
            public long MoodBoost;

            public long Total
            {
                get
                {
                    return Core + ReputationBoost + SalesBoost + ChampionBoost + ExpansionBoost + WorkshopBoost
                        + CampaignBoost + DomainLeaderBoost + DomainChallengerBoost + BenchmarkBoost + MoodBoost;
                }
            }
        }

        public struct IncidentBreakdown
        {
            public int FromConfig;
            public int FromAlignment;
            public int FromCorruption;
            public int FromSafetyStaff;

            public int Total { get { return FromConfig + FromAlignment + FromCorruption + FromSafetyStaff; } }
        }

        public struct RevenueBreakdown
        {
            public List<RevenuePart> Parts;
            public long Total;
        }

        public struct RevenuePart
        {
            public string SectorId;
            public long Amount;
        }

        public static QualityBreakdown QualityParts(GameConfig c, SectorConfig sector, ModelState model, int tick, LabState lab = null)
        {
            int effective = SafetySystem.EffectiveAlignment(c, model);
            int displayed = SafetySystem.DisplayedAlignment(model);
            int sandbagGap = (int)MathI.Max(0, displayed - model.Alignment);
            var b = new QualityBreakdown();
            b.FromCapability = MathI.MulPermille(model.Capability, sector.CapabilityWeightPermille);
            int specialist = SpecialistSystem.QualityBonusPermille(c, model.LineId, sector.Id);
            if (specialist > 0)
                b.FromCapability = MathI.MulPermille(b.FromCapability, MathI.Permille + specialist);
            long effectiveAlign = MathI.MulPermille(model.Capability, MathI.MulPermille(sector.SafetyWeightPermille, effective));
            long displayBoost = MathI.MulPermille(model.Capability, MathI.MulPermille(sector.SafetyWeightPermille, sandbagGap));
            b.FromAlignment = effectiveAlign;
            b.SandbagBoost = displayBoost;
            if (LabQueries.IsGlitching(c, model, tick))
            {
                long before = b.FromCapability + effectiveAlign + displayBoost;
                long after = MathI.MulPermille(before, c.Safety.GlitchQualityPermille);
                b.GlitchAdjustment = after - before;
            }
            if (lab != null)
            {
                ModelMood mood = LabQueries.Mood(c, lab, model, tick);
                int moodPermille = MathI.Permille;
                if (mood == ModelMood.Overworked && c.Safety.OverworkedQualityPermille > 0)
                    moodPermille = c.Safety.OverworkedQualityPermille;
                else if (mood == ModelMood.Corrupted && c.Safety.CorruptedMoodQualityPermille > 0)
                    moodPermille = c.Safety.CorruptedMoodQualityPermille;
                if (moodPermille < MathI.Permille)
                {
                    long before = b.Total;
                    long after = MathI.MulPermille(before, moodPermille);
                    b.MoodAdjustment = after - before;
                }
            }
            if (model.OpenWeights)
            {
                long before = b.Total;
                long after = MathI.MulPermille(before, MathI.Permille + c.Market.OpenWeightsAppealBonusPermille);
                b.OpenWeightsBoost = after - before;
            }
            return b;
        }

        public static long Quality(GameConfig c, SectorConfig sector, ModelState model, int tick, LabState lab = null)
        {
            return QualityParts(c, sector, model, tick, lab).Total;
        }

        public static AppealBreakdown AppealParts(GameConfig c, GameState state, SectorConfig sector, SectorState sectorState, LabState lab, ModelState model)
        {
            long quality = Quality(c, sector, model, state.Tick, lab);
            var b = new AppealBreakdown();
            b.Core = quality * quality;
            ModelMood mood = LabQueries.Mood(c, lab, model, state.Tick);
            int moodPermille = MathI.Permille;
            if (mood == ModelMood.Glitching && c.Safety.GlitchingAppealPermille > 0)
                moodPermille = c.Safety.GlitchingAppealPermille;
            else if (mood == ModelMood.Overworked && c.Safety.OverworkedAppealPermille > 0)
                moodPermille = c.Safety.OverworkedAppealPermille;
            else if (mood == ModelMood.Corrupted && c.Safety.CorruptedMoodAppealPermille > 0)
                moodPermille = c.Safety.CorruptedMoodAppealPermille;
            if (moodPermille < MathI.Permille)
            {
                long afterMood = MathI.MulPermille(b.Core, moodPermille);
                b.MoodBoost = afterMood - b.Core;
                b.Core = afterMood;
            }
            long afterRep = MathI.MulPermille(b.Core, c.Safety.ReputationBaseline + lab.Reputation);
            b.ReputationBoost = afterRep - b.Core;
            long afterSales = MathI.MulPermille(afterRep, MarketSystem.SalesPermille(c, lab));
            b.SalesBoost = afterSales - afterRep;
            long afterCrown = afterSales;
            if (sectorState.ChampionLabId == lab.Id)
            {
                afterCrown = MathI.MulPermille(afterSales, MathI.Permille + c.Gauntlet.BracketWinnerAppealPermille);
                b.ChampionBoost = afterCrown - afterSales;
            }
            int expansion = ExpansionSystem.AppealBonusPermille(c, state, lab, sector.Id);
            long afterExpansion = ExpansionSystem.ScaleAppeal(afterCrown, expansion);
            b.ExpansionBoost = afterExpansion - afterCrown;
            int workshop = DeviceWorkshopSystem.AppealBonusPermille(c, state, lab, sector.Id);
            long afterWorkshop = ExpansionSystem.ScaleAppeal(afterExpansion, workshop);
            b.WorkshopBoost = afterWorkshop - afterExpansion;
            int campaign = ProductStudioSystem.CampaignAppealPermille(c, state, lab, sector.Id);
            long afterCampaign = ExpansionSystem.ScaleAppeal(afterWorkshop, campaign);
            b.CampaignBoost = afterCampaign - afterWorkshop;
            int domainLeader = CapabilityDomainSystem.LeaderAppealPermille(c, state, lab, sector.Id);
            long afterDomain = ExpansionSystem.ScaleAppeal(afterCampaign, domainLeader);
            b.DomainLeaderBoost = afterDomain - afterCampaign;
            int challenger = CapabilityDomainSystem.ChallengerAppealPermille(c, state, lab, sector.Id);
            long afterChallenger = ExpansionSystem.ScaleAppeal(afterDomain, challenger);
            b.DomainChallengerBoost = afterChallenger - afterDomain;
            int benchmark = BenchmarkLeagueSystem.LiveAppealPermille(c, state, lab, sector.Id);
            long afterBench = ExpansionSystem.ScaleAppeal(afterChallenger, benchmark);
            b.BenchmarkBoost = afterBench - afterChallenger;
            return b;
        }

        public static long Appeal(GameConfig c, GameState state, SectorConfig sector, SectorState sectorState, LabState lab, ModelState model)
        {
            return AppealParts(c, state, sector, sectorState, lab, model).Total;
        }

        public static IncidentBreakdown IncidentParts(GameConfig c, ModelState model, LabState lab = null)
        {
            var b = new IncidentBreakdown();
            b.FromConfig = c.Safety.IncidentChancePermille;
            long baseChance = MathI.MulPermille(c.Safety.IncidentChancePermille, MathI.Permille - model.Alignment);
            b.FromAlignment = (int)(baseChance - b.FromConfig);
            int afterCorruption = (int)MathI.MulPermille(baseChance, MathI.Permille + model.Corruption * c.Shady.CorruptionIncidentWeight / MathI.Permille);
            b.FromCorruption = afterCorruption - (int)baseChance;
            int afterStaff = SafetySystem.IncidentChancePermille(c, model, lab);
            b.FromSafetyStaff = afterStaff - afterCorruption;
            return b;
        }

        public static RevenueBreakdown WeeklyRevenueParts(GameState state, LabState lab)
        {
            var b = new RevenueBreakdown();
            b.Parts = new List<RevenuePart>();
            for (int s = 0; s < state.Sectors.Count; s++)
            {
                SectorState sector = state.Sectors[s];
                long sectorTotal = 0;
                for (int i = 0; i < sector.Entries.Count; i++)
                {
                    SectorEntry entry = sector.Entries[i];
                    if (entry.LabId == lab.Id) sectorTotal += entry.LastRevenue;
                }
                if (sectorTotal != 0)
                {
                    b.Parts.Add(new RevenuePart { SectorId = sector.Id, Amount = sectorTotal });
                    b.Total += sectorTotal;
                }
            }
            return b;
        }

        /// <summary>Parts of a trained model's capability after a run (including data-source scaling).</summary>
        public struct TrainedCapabilityBreakdown
        {
            public int FromBase;
            public int FromTech;
            public int FromCarryover;
            public int FromLuck;
            public int FromDataSource;
            public int FromExpansion;

            public int Total { get { return FromBase + FromTech + FromCarryover + FromLuck + FromDataSource + FromExpansion; } }
        }

        public static TrainedCapabilityBreakdown TrainedCapabilityParts(ModelState model)
        {
            var b = new TrainedCapabilityBreakdown();
            if (model.Status != ModelStatus.Trained && model.Status != ModelStatus.Released) return b;
            b.FromBase = model.CapBase;
            b.FromTech = model.CapTech;
            b.FromCarryover = model.CapCarryover;
            b.FromLuck = model.CapLuck;
            b.FromExpansion = model.CapExpansion;
            int beforeSource = b.FromBase + b.FromTech + b.FromCarryover + b.FromLuck + b.FromExpansion;
            b.FromDataSource = model.Capability - beforeSource;
            return b;
        }

        /// <summary>Every holding's weekly power bill; parts within each bill sum to that bill's total.</summary>
        public static System.Collections.Generic.List<PowerBill> WeeklyPowerBillParts(GameConfig c, GameState state, LabState lab)
        {
            var list = new System.Collections.Generic.List<PowerBill>();
            for (int i = 0; i < lab.Holdings.Count; i++)
            {
                PowerBill bill = WorldQueries.BillFor(c, state, lab, lab.Holdings[i]);
                if (bill.Total != 0) list.Add(bill);
            }
            return list;
        }

        /// <summary>Attention gain parts for each bloc this month (before decay is applied).</summary>
        public static EndingSystem.FrontierCostBreakdown FrontierCostParts(GameConfig c, LabState lab)
        {
            return EndingSystem.FrontierCostParts(c, lab);
        }

        public static System.Collections.Generic.List<AttentionGain> AttentionGainParts(GameConfig c, GameState state, LabState lab, int revenueSharePpm)
        {
            var list = new System.Collections.Generic.List<AttentionGain>();
            for (int i = 0; i < c.World.Blocs.Count; i++)
            {
                string blocId = c.World.Blocs[i].Id;
                list.Add(WorldQueries.GainFor(c, state, lab, blocId, revenueSharePpm));
            }
            return list;
        }
    }
}
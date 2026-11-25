using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
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

            public long Total { get { return FromCapability + FromAlignment + SandbagBoost + GlitchAdjustment; } }
        }

        public struct AppealBreakdown
        {
            public long Core;
            public long ReputationBoost;
            public long SalesBoost;
            public long ChampionBoost;

            public long Total { get { return Core + ReputationBoost + SalesBoost + ChampionBoost; } }
        }

        public struct IncidentBreakdown
        {
            public int FromConfig;
            public int FromAlignment;
            public int FromCorruption;

            public int Total { get { return FromConfig + FromAlignment + FromCorruption; } }
        }

        public struct ExitPriceBreakdown
        {
            public long RevenueYears;
            public long FleetResale;
            public long CashOnHand;
            public long Total;
        }

        public struct BubbleMeterBreakdown
        {
            public int Permille;
            public int GraceMonths;
            public int GraceMonthsNeeded;
            public int CrashMonthsRemaining;
            public long CapitalRaised;
            public long MarketRevenueLastMonth;
            /// <summary>0 calm, 1 warm, 2 hot, 3 crash.</summary>
            public int WarningLevel;
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

        public struct GpuRentBreakdown
        {
            public long SpotRentedGpus;
            public long ReservedDiscounted;
            public long CloudChips;

            public long Total { get { return SpotRentedGpus + ReservedDiscounted + CloudChips; } }
        }

        public struct FundingRoundQuote
        {
            public string RoundId;
            public long Cash;
            public int DilutionPermille;
            public int FounderShareAfterPermille;
            public bool DownRound;
        }

        public static QualityBreakdown QualityParts(GameConfig c, SectorConfig sector, ModelState model, int tick)
        {
            int effective = SafetySystem.EffectiveAlignment(c, model);
            int displayed = SafetySystem.DisplayedAlignment(model);
            int sandbagGap = (int)MathI.Max(0, displayed - model.Alignment);
            var b = new QualityBreakdown();
            b.FromCapability = MathI.MulPermille(model.Capability, sector.CapabilityWeightPermille);
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
            return b;
        }

        public static long Quality(GameConfig c, SectorConfig sector, ModelState model, int tick)
        {
            return QualityParts(c, sector, model, tick).Total;
        }

        public static AppealBreakdown AppealParts(GameConfig c, GameState state, SectorConfig sector, SectorState sectorState, LabState lab, ModelState model)
        {
            long quality = Quality(c, sector, model, state.Tick);
            var b = new AppealBreakdown();
            b.Core = quality * quality;
            long afterRep = MathI.MulPermille(b.Core, c.Safety.ReputationBaseline + lab.Reputation);
            b.ReputationBoost = afterRep - b.Core;
            long afterSales = MathI.MulPermille(afterRep, MarketSystem.SalesPermille(c, lab));
            b.SalesBoost = afterSales - afterRep;
            if (sectorState.ChampionLabId == lab.Id)
            {
                long afterCrown = MathI.MulPermille(afterSales, MathI.Permille + c.Gauntlet.BracketWinnerAppealPermille);
                b.ChampionBoost = afterCrown - afterSales;
            }
            return b;
        }

        public static long Appeal(GameConfig c, GameState state, SectorConfig sector, SectorState sectorState, LabState lab, ModelState model)
        {
            return AppealParts(c, state, sector, sectorState, lab, model).Total;
        }

        public static IncidentBreakdown IncidentParts(GameConfig c, int tick, ModelState model)
        {
            int eraLoad = EraSystem.AtTick(c, tick).IncidentLoadPermille;
            if (eraLoad < 1) eraLoad = MathI.Permille;
            int scaledConfig = (int)MathI.MulPermille(c.Safety.IncidentChancePermille, eraLoad);
            var b = new IncidentBreakdown();
            b.FromConfig = scaledConfig;
            long baseChance = MathI.MulPermille(scaledConfig, MathI.Permille - model.Alignment);
            b.FromAlignment = (int)(baseChance - scaledConfig);
            int total = SafetySystem.IncidentChancePermille(c, tick, model);
            b.FromCorruption = total - (int)baseChance;
            return b;
        }

        public static AcquisitionSystem.AcquisitionQuote AcquisitionQuoteParts(GameConfig c, GameState state, LabState buyer, LabState target)
        {
            return AcquisitionSystem.Quote(c, state, buyer, target);
        }

        public static ModelSalesSystem.ModelSaleQuote ModelSaleParts(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            return ModelSalesSystem.Quote(c, state, lab, model);
        }

        public static BubbleMeterBreakdown BubbleMeterParts(GameConfig c, GameState state)
        {
            BubbleConfig b = c.Bubble;
            var p = new BubbleMeterBreakdown();
            p.Permille = state.BubblePermille;
            p.GraceMonths = state.BubbleGraceMonths;
            p.CrashMonthsRemaining = state.BubbleCrashMonthsRemaining;
            p.CapitalRaised = state.BubbleCapitalRaised;
            p.MarketRevenueLastMonth = BubbleSystem.MarketRevenueLastClosedMonth(state);
            p.GraceMonthsNeeded = b == null ? 0 : b.GraceMonthsBeforeCrash;
            int popWarn = b == null ? 50 : b.PopWarningMarginPermille;
            if (popWarn < 1) popWarn = 50;
            if (BubbleSystem.InCrash(state)) p.WarningLevel = 3;
            else if (b != null && state.BubblePermille >= b.PopAtPermille - popWarn) p.WarningLevel = 2;
            else if (b != null && state.BubblePermille >= b.HotPermille) p.WarningLevel = 1;
            return p;
        }

        public static ExitPriceBreakdown ExitPriceParts(GameConfig c, GameState state, LabState lab)
        {
            var b = new ExitPriceBreakdown();
            b.RevenueYears = EndingSystem.RevenueLast12Months(lab) * c.Endings.ExitRevenueYears;
            b.FleetResale = MathI.MulPermille(LabQueries.FleetResaleValue(c, state, lab), c.Endings.ExitFleetPermille);
            b.CashOnHand = lab.Cash;
            b.Total = b.RevenueYears + b.FleetResale + b.CashOnHand;
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

        public static GpuRentBreakdown GpuRentParts(GameConfig c, GameState state, LabState lab)
        {
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            var b = new GpuRentBreakdown();
            b.SpotRentedGpus = gen.RentPerGpuWeek * lab.RentedGpus;
            b.ReservedDiscounted = ComputeContractSystem.WeeklyReservedRent(c, state, lab);
            b.CloudChips = ChipQueries.WeeklyCloudChipRent(c, state, lab);
            return b;
        }

        public static bool TryNextFundingQuote(GameConfig c, GameState state, LabState lab, out FundingRoundQuote quote)
        {
            quote = new FundingRoundQuote();
            if (lab == null || lab.Id != state.PlayerLabId) return false;
            for (int i = 0; i < c.Funding.Rounds.Count; i++)
            {
                FundingRoundConfig round = c.Funding.Rounds[i];
                if (FundingSystem.HasRound(lab, round.Id)) continue;
                if (!FundingSystem.PriorRoundsDone(c, lab, round)) continue;
                int year = GameDate.Year(c.Calendar, state.Tick);
                if (year < round.OpensYear) continue;
                if (lab.Reputation < round.MinReputation) continue;
                if (WorldQueries.IsFrozen(lab, lab.HomeBlocId)) continue;
                quote.RoundId = round.Id;
                quote.Cash = FundingSystem.EffectiveCash(c, state, round);
                quote.DilutionPermille = FundingSystem.EffectiveDilution(c, state, round);
                quote.DownRound = FundingSystem.IsDownRound(c, state);
                quote.FounderShareAfterPermille = (int)MathI.MulPermille(lab.FounderSharePermille, MathI.Permille - quote.DilutionPermille);
                return true;
            }
            return false;
        }

        /// <summary>Recent rival decision lines for the host UI. Empty for the player's lab or bots.</summary>
        public static List<string> RivalLogLines(GameConfig c, GameState state, LabState lab, int maxLines)
        {
            var lines = new List<string>();
            if (lab == null || lab.Controller != LabController.Plan || maxLines < 1) return lines;
            int start = lab.Log.Count - maxLines;
            if (start < 0) start = 0;
            for (int i = start; i < lab.Log.Count; i++)
                lines.Add(DecisionText.Line(c, state, lab, lab.Log[i]));
            return lines;
        }
    }
}